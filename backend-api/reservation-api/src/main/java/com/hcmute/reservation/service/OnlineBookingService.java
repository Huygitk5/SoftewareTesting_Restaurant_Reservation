package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.reservation.OnlineReservationRequest;
import com.hcmute.reservation.model.dto.reservation.ReservationResponse;

public interface OnlineBookingService {

    ReservationResponse createOnlineReservation(OnlineReservationRequest req, Long customerId);
    ReservationResponse confirmPayment(Long id);
    ReservationResponse cancelPayment(Long id);
}