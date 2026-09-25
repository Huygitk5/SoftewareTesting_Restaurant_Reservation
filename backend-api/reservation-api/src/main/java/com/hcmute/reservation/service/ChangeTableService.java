package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.reservation.ChangeTableRequest;
import com.hcmute.reservation.model.dto.reservation.ReservationResponse;

public interface ChangeTableService {
    ReservationResponse changeTable(Long reservationId, ChangeTableRequest req);
}
