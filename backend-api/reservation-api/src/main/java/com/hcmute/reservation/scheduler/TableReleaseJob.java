package com.hcmute.reservation.scheduler;

import com.hcmute.reservation.event.TableStatusChangedEvent;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import com.hcmute.reservation.service.ConfigProviderService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.Map;
import java.util.concurrent.atomic.AtomicInteger;

@Slf4j
@Service
@RequiredArgsConstructor
public class TableReleaseJob implements ScheduledJob {

    private final ReservationRepository reservationRepository;
    private final TableInfoRepository tableInfoRepository;
    private final ConfigProviderService configProvider;
    private final ApplicationEventPublisher eventPublisher;
    private final BatchProcessor batchProcessor;

    @Override
    public Map<String, Object> execute() {
        int bufferMinutes = configProvider.getBufferMinutes();

        // Chỉ lấy COMPLETED mà bàn vẫn còn OCCUPIED/OVERSTAY (tránh xử lý thừa)
        var completed = reservationRepository
                .findCompletedWithReleasableTables(LocalDateTime.now().minusMinutes(bufferMinutes));

        AtomicInteger tablesReleasedCount = new AtomicInteger(0);

        batchProcessor.processBatch(completed, ReservationStatus.COMPLETED, fresh -> {
            if (fresh.getTableMappings() == null) return;

            for (var m : fresh.getTableMappings()) {
                TableInfo t = m.getTableInfo();
                int seatedCount = tableInfoRepository.countOtherSeated(t.getTableId(), fresh.getReservationId());
                if (seatedCount == 0 && (t.getStatus() == TableStatus.OCCUPIED || t.getStatus() == TableStatus.OVERSTAY)) {
                    t.setStatus(TableStatus.AVAILABLE);
                    tableInfoRepository.save(t);
                    tablesReleasedCount.incrementAndGet();

                    eventPublisher.publishEvent(
                            new TableStatusChangedEvent(this, t.getTableId(), "AVAILABLE"));
                }
            }
        });

        int count = tablesReleasedCount.get();
        if (count > 0) log.info("[Job] TableReleaseJob: {} bàn được giải phóng.", count);
        return batchProcessor.buildResult("tablesReleased", count);
    }
}
