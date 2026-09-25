package com.hcmute.reservation.scheduler;

import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.service.ConfigProviderService;
import com.hcmute.reservation.service.TableReleaseService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.Map;

@Slf4j
@Service
@RequiredArgsConstructor
public class ExpireReservationJob implements ScheduledJob {

    private final ReservationRepository reservationRepository;
    private final ConfigProviderService configProvider;
    private final TableReleaseService tableReleaseService;
    private final BatchProcessor batchProcessor;

    @Override
    public Map<String, Object> execute() {
        int softLockMinutes = configProvider.getSoftLockMinutes();

        var toExpire = reservationRepository
                .findExpiredPendingPayments(LocalDateTime.now().minusMinutes(softLockMinutes));

        int count = batchProcessor.processBatch(toExpire, ReservationStatus.PENDING_PAYMENT, fresh -> {
            fresh.markExpired();
            reservationRepository.save(fresh);
            tableReleaseService.releaseLockedTable(fresh.getReservationId());
        });

        if (count > 0) log.info("[Job] ExpireReservationJob: {} đơn hết hạn.", count);
        return batchProcessor.buildResult("expired", count);
    }
}
