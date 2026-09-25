package com.hcmute.reservation.scheduler;

import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.repository.ReservationRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Component;
import org.springframework.transaction.support.TransactionTemplate;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.function.Consumer;

@Slf4j
@Component
@RequiredArgsConstructor
public class BatchProcessor {

    private final ReservationRepository reservationRepository;
    private final TransactionTemplate transactionTemplate;

    public int processBatch(List<Reservation> reservations,
                            ReservationStatus expectedStatus,
                            Consumer<Reservation> action) {
        int successCount = 0;
        for (Reservation r : reservations) {
            try {
                boolean processed = Boolean.TRUE.equals(transactionTemplate.execute(status -> {
                    Reservation fresh = reservationRepository.findById(r.getReservationId()).orElse(null);
                    // Double check để tránh thao tác đè lên dữ liệu cũ (Stale State)
                    if (fresh == null || fresh.getStatus() != expectedStatus) {
                        return false;
                    }
                    action.accept(fresh);
                    return true;
                }));
                if (processed) successCount++;
            } catch (Exception e) {
                log.error("[Scheduler] Lỗi khi xử lý đơn {}: {}", r.getReservationId(), e.getMessage());
            }
        }
        return successCount;
    }

    public Map<String, Object> buildResult(String key, int count) {
        Map<String, Object> result = new HashMap<>();
        result.put(key, count);
        result.put("executedAt", java.time.LocalDateTime.now().toString());
        return result;
    }
}
