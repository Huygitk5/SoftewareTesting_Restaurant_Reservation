package com.hcmute.reservation.scheduler;

import com.hcmute.reservation.event.TableStatusChangedEvent;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.Map;

@Slf4j
@Service
@RequiredArgsConstructor
public class OverstayDetectionJob implements ScheduledJob {

    private final ReservationRepository reservationRepository;
    private final TableInfoRepository tableInfoRepository;
    private final ApplicationEventPublisher eventPublisher;
    private final BatchProcessor batchProcessor;

    @Override
    public Map<String, Object> execute() {
        var overstayReservations = reservationRepository
                .findByStatusAndEndTimeBefore(ReservationStatus.SEATED, LocalDateTime.now());

        int count = batchProcessor.processBatch(overstayReservations, ReservationStatus.SEATED, fresh -> {
            if (fresh.getTableMappings() == null) return;

            fresh.getTableMappings().forEach(mapping -> {
                TableInfo table = mapping.getTableInfo();
                if (table.getStatus() != TableStatus.OVERSTAY) {
                    table.setStatus(TableStatus.OVERSTAY);
                    tableInfoRepository.save(table);

                    eventPublisher.publishEvent(
                            new TableStatusChangedEvent(this, table.getTableId(), "OVERSTAY"));
                }
            });
        });

        if (count > 0) log.info("[Job] OverstayDetectionJob: {} đơn bị đánh dấu OVERSTAY.", count);
        return batchProcessor.buildResult("overstayDetected", count);
    }
}
