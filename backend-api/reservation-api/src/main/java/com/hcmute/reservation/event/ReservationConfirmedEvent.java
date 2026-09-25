package com.hcmute.reservation.event;

import lombok.Getter;
import org.springframework.context.ApplicationEvent;

import java.time.LocalDateTime;

@Getter
public class ReservationConfirmedEvent extends ApplicationEvent {
    private final String customerEmail;
    private final String customerName;
    private final Long reservationId;
    private final LocalDateTime startTime;

    public ReservationConfirmedEvent(Object source, String customerEmail, String customerName, Long reservationId, LocalDateTime startTime) {
        super(source);
        this.customerEmail = customerEmail;
        this.customerName = customerName;
        this.reservationId = reservationId;
        this.startTime = startTime;
    }
}