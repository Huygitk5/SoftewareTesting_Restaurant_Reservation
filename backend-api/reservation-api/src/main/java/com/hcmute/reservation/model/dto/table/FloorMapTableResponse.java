package com.hcmute.reservation.model.dto.table;

import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.TableStatus;
import lombok.Builder;
import lombok.Data;

import java.time.LocalDateTime;

@Data
@Builder
public class FloorMapTableResponse {
    private Long tableId;
    private int capacity;
    private TableStatus status;
    private boolean isActive;
    // null nếu không có đơn
    private Long currentReservationId;
    private String currentCustomerName;
    private ReservationStatus currentReservationStatus;
    private LocalDateTime currentReservationTime;
    private LocalDateTime nextReservationTime;
}
