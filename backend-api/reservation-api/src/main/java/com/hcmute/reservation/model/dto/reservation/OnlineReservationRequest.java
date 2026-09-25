package com.hcmute.reservation.model.dto.reservation;

import jakarta.validation.constraints.Future;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;
import lombok.Data;

import java.time.LocalDateTime;

@Data
public class OnlineReservationRequest {
    @NotNull @Min(1)
    private Integer guestCount;

    @NotNull @Future
    private LocalDateTime startTime;

    private String note;
}
