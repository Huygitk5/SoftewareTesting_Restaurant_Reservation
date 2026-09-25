package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.model.dto.override.OverrideLogResponse;
import com.hcmute.reservation.model.dto.override.OverrideRequest;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.entity.Account;
import com.hcmute.reservation.model.entity.OverrideLog;
import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.AccountRepository;
import com.hcmute.reservation.repository.OverrideLogRepository;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import com.hcmute.reservation.service.OverrideService;
import lombok.RequiredArgsConstructor;
import org.springframework.orm.ObjectOptimisticLockingFailureException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class OverrideServiceImpl implements OverrideService {

    private final ReservationRepository reservationRepository;
    private final AccountRepository accountRepository;
    private final OverrideLogRepository overrideLogRepository;
    private final TableInfoRepository tableInfoRepository;

    @Override
    @Transactional
    public OverrideLogResponse override(Long reservationId, OverrideRequest req, Long accountId) {
        Reservation reservation = getReservationOrThrow(reservationId);
        validateOverrideConditions(reservation);

        Account account = getAccountOrThrow(accountId);

        // Gọi checkOut của Reservation: status = COMPLETED, endTime = now
        reservation.checkOut();
        reservationRepository.save(reservation);

        // Giải phóng bàn (bắt lỗi đa luồng)
        releaseTables(reservation);

        // Ghi log
        OverrideLog log = OverrideLog.builder()
                .reservation(reservation)
                .account(account)
                .reason(req.getReason())
                .build();
        return toResponse(overrideLogRepository.save(log));
    }

    @Override
    public List<OverrideLogResponse> getLogs(LocalDateTime from, LocalDateTime to, Long accountId) {
        return overrideLogRepository.findByFilters(from, to, accountId)
                .stream()
                .map(this::toResponse)
                .collect(Collectors.toList());
    }

    private Reservation getReservationOrThrow(Long reservationId) {
        return reservationRepository.findById(reservationId)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn #" + reservationId + " không tồn tại."));
    }

    private Account getAccountOrThrow(Long accountId) {
        return accountRepository.findById(accountId)
                .orElseThrow(() -> new ResourceNotFoundException("Nhân viên #" + accountId + " không tồn tại."));
    }

    private void validateOverrideConditions(Reservation reservation) {
        if (reservation.getStatus() != ReservationStatus.SEATED) {
            throw new BadRequestException("Chỉ có thể ghi đè các đơn đang ở trạng thái SEATED (Đang ngồi).");
        }

        boolean isAnyTableOverstay = reservation.getTableMappings() != null &&
                reservation.getTableMappings().stream()
                        .anyMatch(m -> m.getTableInfo().getStatus() == TableStatus.OVERSTAY);

        if (!isAnyTableOverstay) {
            throw new BadRequestException("Chỉ có thể ghi đè khi bàn đã chuyển sang trạng thái cảnh báo OVERSTAY.");
        }
    }

    private void releaseTables(Reservation reservation) {
        try {
            if (reservation.getTableMappings() != null) {
                reservation.getTableMappings().forEach(m -> {
                    TableInfo t = m.getTableInfo();
                    t.setStatus(TableStatus.AVAILABLE);
                    tableInfoRepository.saveAndFlush(t);
                });
            }
        } catch (ObjectOptimisticLockingFailureException e) {
            throw new ConflictException("Bàn này đang được hệ thống hoặc nhân viên khác xử lý. Vui lòng tải lại trang và thử lại.");
        }
    }

    private OverrideLogResponse toResponse(OverrideLog log) {
        return OverrideLogResponse.builder()
                .logId(log.getLogId())
                .reservationId(log.getReservation().getReservationId())
                .accountId(log.getAccount().getAccountId())
                .accountUsername(log.getAccount().getUsername())
                .reason(log.getReason())
                .createdAt(log.getCreatedAt())
                .build();
    }
}
