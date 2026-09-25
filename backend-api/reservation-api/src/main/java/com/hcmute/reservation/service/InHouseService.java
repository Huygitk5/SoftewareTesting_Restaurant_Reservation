package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.reservation.ReservationResponse;

public interface InHouseService {
    ReservationResponse checkIn(Long id);
    ReservationResponse checkOut(Long id);
}
