package com.hcmute.reservation.scheduler;

import com.hcmute.reservation.event.TableAlertEvent;
import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;

@Slf4j
@Service
@RequiredArgsConstructor
public class ConflictAlertJob implements ScheduledJob {

    private final ReservationRepository reservationRepository;
    private final ApplicationEventPublisher eventPublisher;

    /** Trạng thái nội bộ: danh sách bàn đang nhấp nháy (in-memory, không persist). */
    private final Set<Long> currentlyBlinkingTables = ConcurrentHashMap.newKeySet();

    @Override
    public Map<String, Object> execute() {
        LocalDateTime now = LocalDateTime.now();
        LocalDateTime windowEnd = now.plusMinutes(15);

        List<Reservation> upcoming = reservationRepository.findUpcomingReservationsForAlert(now, windowEnd);

        Set<Long> conflictedTableIdsInThisScan = new HashSet<>();

        // 1. Quét tìm các bàn có nguy cơ conflict
        for (Reservation r : upcoming) {
            if (r.getTableMappings() == null) continue;

            for (var mapping : r.getTableMappings()) {
                TableInfo table = mapping.getTableInfo();

                // Nếu bàn CHƯA TRỐNG (đang bị Occupied/Overstay/SoftLocked)
                if (table.getStatus() != TableStatus.AVAILABLE) {
                    conflictedTableIdsInThisScan.add(table.getTableId());

                    // Nếu bàn này chưa nhấp nháy → phát sự kiện BẬT nhấp nháy
                    if (!currentlyBlinkingTables.contains(table.getTableId())) {
                        currentlyBlinkingTables.add(table.getTableId());

                        eventPublisher.publishEvent(
                                new TableAlertEvent(this, table.getTableId(), "START_BLINK"));
                        log.info("[Job] ConflictAlertJob: Bật cảnh báo nhấp nháy cho bàn {}", table.getTableId());
                    }
                }
            }
        }

        // 2. Tắt cảnh báo cho những bàn đã "an toàn"
        Iterator<Long> iterator = currentlyBlinkingTables.iterator();
        while (iterator.hasNext()) {
            Long tableId = iterator.next();
            if (!conflictedTableIdsInThisScan.contains(tableId)) {
                eventPublisher.publishEvent(
                        new TableAlertEvent(this, tableId, "STOP_BLINK"));
                log.info("[Job] ConflictAlertJob: Tắt cảnh báo nhấp nháy cho bàn {}", tableId);
                iterator.remove();
            }
        }

        Map<String, Object> result = new HashMap<>();
        result.put("conflictedTables", conflictedTableIdsInThisScan.size());
        result.put("executedAt", now.toString());
        return result;
    }
}
