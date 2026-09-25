package com.hcmute.reservation.service;

public interface TableReleaseService {
    void releaseLockedTable(Long reservationId);
}
