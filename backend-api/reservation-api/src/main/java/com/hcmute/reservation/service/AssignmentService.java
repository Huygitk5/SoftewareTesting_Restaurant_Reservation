package com.hcmute.reservation.service;

import com.hcmute.reservation.model.entity.Reservation;

public interface AssignmentService {
    /**
     * Tìm và gán bàn thay thế cho đơn đặt bàn.
     * @param reservation Đơn đặt bàn cần xếp chỗ.
     * @return true nếu tìm được bàn và gán thành công, false nếu hết bàn phù hợp.
     */
    boolean findAlternativeTables(Reservation reservation);
}