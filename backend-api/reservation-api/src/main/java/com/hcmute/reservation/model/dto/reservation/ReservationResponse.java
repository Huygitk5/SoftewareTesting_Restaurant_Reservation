package com.hcmute.reservation.model.dto.reservation;

import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.ReservationType;
import lombok.Builder;
import lombok.Data;

import java.time.LocalDateTime;
import java.util.List;

@Data
@Builder
public class ReservationResponse {
    private Long reservationId;
    private ReservationStatus status;
    private ReservationType type;
    private int guestCount;
    private LocalDateTime startTime;
    private LocalDateTime endTime;
    private Double depositAmount;
    private String note;
    private LocalDateTime createdAt;
    private Long customerId;
    private String customerName;
    private String customerPhone;
    private List<Long> tableIds;
}
