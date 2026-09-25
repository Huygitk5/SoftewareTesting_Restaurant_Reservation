package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.reservation.*;

import java.util.List;

public interface ReservationService {
    ReservationResponse getById(Long id);
    List<ReservationResponse> getReservationsByCustomer(Long customerId);
    List<ReservationResponse> getActiveReservations();
    List<ReservationResponse> getUpcomingReservations(int minutes);
}