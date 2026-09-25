package com.hcmute.reservation.service;

public interface CancellationService {
    void cancelReservation(Long id, Long customerId);
}
