package com.hcmute.reservation.scheduler;

import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.service.ConfigProviderService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.Map;

@Slf4j
@Service
@RequiredArgsConstructor
public class NoShowDetectionJob implements ScheduledJob {

    private final ReservationRepository reservationRepository;
    private final ConfigProviderService configProvider;
    private final BatchProcessor batchProcessor;

    @Override
    public Map<String, Object> execute() {
        int gracePeriodMinutes = configProvider.getGracePeriodMinutes();

        var toNoShow = reservationRepository
                .findNoShows(LocalDateTime.now().minusMinutes(gracePeriodMinutes));

        int count = batchProcessor.processBatch(toNoShow, ReservationStatus.RESERVED, fresh -> {
            fresh.markNoShow();
            reservationRepository.save(fresh);
        });

        if (count > 0) log.info("[Job] NoShowDetectionJob: {} đơn no-show.", count);
        return batchProcessor.buildResult("noShow", count);
    }
}
