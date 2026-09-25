package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.event.TableStatusChangedEvent;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.mapper.ReservationMapper;
import com.hcmute.reservation.model.dto.reservation.ChangeTableRequest;
import com.hcmute.reservation.model.dto.reservation.ReservationResponse;
import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.ReservationTableMapping;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.ReservationTableMappingRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import com.hcmute.reservation.service.ChangeTableService;
import com.hcmute.reservation.service.ConfigProviderService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.orm.ObjectOptimisticLockingFailureException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;

import static com.hcmute.reservation.model.enums.ReservationStatus.RESERVED;
import static com.hcmute.reservation.model.enums.ReservationStatus.SEATED;

@Slf4j
@Service
@RequiredArgsConstructor
public class ChangeTableServiceImpl implements ChangeTableService {

    private final ReservationRepository reservationRepository;
    private final TableInfoRepository tableInfoRepository;
    private final ReservationTableMappingRepository mappingRepository;
    private final ConfigProviderService configProvider;
    private final ReservationMapper mapper;
    private final ApplicationEventPublisher eventPublisher;

    @Override
    @Transactional
    public ReservationResponse changeTable(Long reservationId, ChangeTableRequest req) {
        int bufferMinutes = configProvider.getBufferMinutes();

        Reservation reservation = reservationRepository.findById(reservationId)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn #" + reservationId + " không tồn tại."));

        if (reservation.getStatus() != SEATED && reservation.getStatus() != RESERVED) {
            throw new BadRequestException(
                    "Chỉ có thể đổi bàn khi đơn ở trạng thái RESERVED hoặc SEATED. " +
                            "Trạng thái hiện tại: " + reservation.getStatus());
        }

        // Tổng sức chứa của bàn cũ
        int oldTotalCapacity = reservation.getTableMappings().stream()
                .mapToInt(m -> m.getTableInfo().getCapacity())
                .sum();

        int newTotalCapacity = 0;

        // Lấy ID bàn cũ của đơn này để loại trừ khi kiểm tra conflict
        Set<Long> currentTableIds = reservation.getTableMappings().stream()
                .map(m -> m.getTableInfo().getTableId())
                .collect(Collectors.toSet());

        // Validate danh sách bàn mới
        List<TableInfo> newTables = new ArrayList<>();
        for (Long tableId : req.getTableIds()) {
            TableInfo table = tableInfoRepository.findById(tableId)
                    .orElseThrow(() -> new ResourceNotFoundException("Bàn #" + tableId + " không tồn tại."));

            if (!table.getIsActive()) {
                throw new BadRequestException("Bàn #" + tableId + " đang bị vô hiệu hóa.");
            }

            // Tính sức chứa của bàn mới
            newTotalCapacity += table.getCapacity();

            // Nếu bàn mới không phải bàn cũ → kiểm tra trạng thái
            if (!currentTableIds.contains(tableId)) {
                if (table.isSoftLocked()) {
                    throw new ConflictException(
                            "Bàn #" + tableId + " đang được giữ tạm cho giao dịch thanh toán khác.");
                }
                if (table.getStatus() != TableStatus.AVAILABLE) {
                    throw new ConflictException(
                            "Bàn #" + tableId + " hiện không trống (Trạng thái: " + table.getStatus() + ").");
                }

                // Kiểm tra overlap với booking khác (trừ chính đơn này)
                LocalDateTime start = reservation.getStartTime();
                LocalDateTime end = reservation.getEndTime().plusMinutes(bufferMinutes);
                List<Long> overlappingReservationTableIds = reservationRepository.findOccupiedTableIds(start, end);
                // Loại bỏ chính các bàn của đơn này khỏi danh sách "đang bận"
                overlappingReservationTableIds.removeAll(currentTableIds);

                if (overlappingReservationTableIds.contains(tableId)) {
                    throw new ConflictException("Bàn #" + tableId + " đã có lịch đặt trùng trong khung giờ này.");
                }
            }
            newTables.add(table);
        }

        if (newTotalCapacity >= reservation.getGuestCount() + 2) {
            throw new BadRequestException(
                    "Không thể đổi bàn! Chỉ được phép chuyển sang bàn lớn hơn tối đa 2 chỗ so với sức chứa hiện tại. (Sức chứa cũ: "
                            + oldTotalCapacity + " chỗ, yêu cầu mới: " + newTotalCapacity + " chỗ).");
        }

        // Giải phóng bàn cũ (trừ những bàn vẫn được giữ lại)
        if (reservation.getTableMappings() != null) {
            for (ReservationTableMapping mapping : reservation.getTableMappings()) {
                TableInfo oldTable = mapping.getTableInfo();
                boolean isKeptTable = req.getTableIds().contains(oldTable.getTableId());
                if (!isKeptTable) {
                    oldTable.setStatus(TableStatus.AVAILABLE);
                    tableInfoRepository.save(oldTable);

                    eventPublisher.publishEvent(
                            new TableStatusChangedEvent(this, oldTable.getTableId(), "AVAILABLE"));
                }
            }
            mappingRepository.deleteAll(reservation.getTableMappings());
            reservation.getTableMappings().clear();
        }

        // Gán bàn mới và tạo mapping
        List<ReservationTableMapping> newMappings = new ArrayList<>();
        try {
            for (TableInfo table : newTables) {
                // Chỉ set OCCUPIED nếu khách đang ngồi thực tế
                if (reservation.getStatus() == SEATED) {
                    table.setStatus(TableStatus.OCCUPIED);
                    eventPublisher.publishEvent(
                            new TableStatusChangedEvent(this, table.getTableId(), "OCCUPIED"));
                }
                // Nếu RESERVED thì giữ nguyên AVAILABLE — bàn chỉ bị "chiếm" khi check-in
                tableInfoRepository.saveAndFlush(table);

                ReservationTableMapping mapping = ReservationTableMapping.builder()
                        .reservation(reservation)
                        .tableInfo(table)
                        .build();

                newMappings.add(mappingRepository.save(mapping));
            }
        } catch (ObjectOptimisticLockingFailureException e) {
            throw new ConflictException(
                "Một trong các bàn vừa bị thay đổi bởi giao dịch khác. Vui lòng  tải lại và thử lại.");
        }

        reservation.setTableMappings(newMappings);

        return mapper.toResponse(reservationRepository.save(reservation));
    }
}
