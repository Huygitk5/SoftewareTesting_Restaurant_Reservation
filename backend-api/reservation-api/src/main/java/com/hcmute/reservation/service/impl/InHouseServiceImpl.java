package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.event.TableStatusChangedEvent;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.mapper.ReservationMapper;
import com.hcmute.reservation.model.dto.reservation.ReservationResponse;
import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.ReservationTableMapping;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import com.hcmute.reservation.service.AssignmentService;
import com.hcmute.reservation.service.ConfigProviderService;
import com.hcmute.reservation.service.InHouseService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

import static com.hcmute.reservation.model.enums.ReservationStatus.RESERVED;
import static com.hcmute.reservation.model.enums.ReservationStatus.SEATED;

@Slf4j
@Service
@RequiredArgsConstructor
public class InHouseServiceImpl implements InHouseService {

    private final ReservationRepository reservationRepository;
    private final TableInfoRepository tableInfoRepository;
    private final AssignmentService assignmentService;
    private final ReservationMapper mapper;
    private final ConfigProviderService configProvider;
    private final ApplicationEventPublisher eventPublisher;

    @Override
    @Transactional
    public ReservationResponse checkIn(Long id) {
        int durationMinutes = configProvider.getDurationMinutes();
        int gracePeriodMinutes = configProvider.getGracePeriodMinutes();

        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));

        if (reservation.getStatus() != RESERVED) {
            throw new BadRequestException(
                    "Đơn không ở trạng thái RESERVED. (Lưu ý: Đơn Walk-in hoặc đơn khách đã vào bàn sẽ không thể check-in lại).");
        }

        LocalDateTime now = LocalDateTime.now();
        LocalDateTime startTime = reservation.getStartTime();

        // Lấy danh sách bàn đang được mapping với đơn đặt bàn này
        List<TableInfo> assignedTables = reservation.getTableMappings().stream()
                .map(ReservationTableMapping::getTableInfo)
                .toList();

        // ────── Kịch bản A: Quá grace period (No-Show) ──────
        if (now.isAfter(startTime.plusMinutes(gracePeriodMinutes))) {
            reservation.markNoShow();
            reservationRepository.save(reservation);

            throw new BadRequestException("Đơn đặt bàn đã quá giờ giữ chỗ (" + gracePeriodMinutes
                    + " phút). Đơn đã bị hủy theo chính sách No-Show.");
        }

        // ────── Kịch bản B & C: Kiểm tra trạng thái thực tế của bàn ──────
        boolean isOriginalTablesAvailable = assignedTables.stream()
                .allMatch(t -> t.getStatus() == TableStatus.AVAILABLE && !t.isSoftLocked());

        if (!isOriginalTablesAvailable) {
            // Cố gắng nhờ AssignmentService tìm và gán tạm bàn thay thế
            boolean hasAlternativeTable = assignmentService.findAlternativeTables(reservation);

            if (!hasAlternativeTable) {
                if (now.isBefore(startTime)) {
                    throw new ConflictException(
                            "Bàn đặt trước hiện chưa trống và không có bàn thay thế phù hợp. Mời quý khách ngồi chờ ở khu vực Waitlist.");
                } else {
                    throw new ConflictException(
                            "OVERSTAY_CONFLICT: Bàn gốc đang bị khách ca trước ngồi quá giờ và không có bàn thay thế trống. Vui lòng sử dụng chức năng Override để xử lý.");
                }
            } else {
                reservationRepository.flush();
                // Nếu tìm được bàn thay thế thành công, load lại danh sách bàn mới từ Mapping
                assignedTables = new ArrayList<>(reservation.getTableMappings().stream()
                        .map(ReservationTableMapping::getTableInfo)
                        .toList());
            }
        }

        // ────── Check-in Thành công ──────
        reservation.checkIn();
        if (now.isBefore(startTime)) {
            reservation.setEndTime(now.plusMinutes(durationMinutes));
        } else {
            reservation.setEndTime(startTime.plusMinutes(durationMinutes));
        }

        // Cập nhật Table_Info.status = OCCUPIED cho TẤT CẢ bàn trong Mapping
        for (TableInfo t : assignedTables) {
            t.setStatus(TableStatus.OCCUPIED);
            tableInfoRepository.save(t);
            eventPublisher.publishEvent(
                    new TableStatusChangedEvent(this, t.getTableId(), "OCCUPIED"));
        }
        return mapper.toResponse(reservationRepository.save(reservation));
    }

    @Override
    @Transactional
    public ReservationResponse checkOut(Long id) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));
        if (reservation.getStatus() != SEATED) {
            throw new BadRequestException("Đơn không ở trạng thái SEATED.");
        }
        reservation.checkOut();
        reservation.setEndTime(LocalDateTime.now());
        reservationRepository.save(reservation);
        return mapper.toResponse(reservation);
    }
}
