package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.override.OverrideLogResponse;
import com.hcmute.reservation.model.dto.override.OverrideRequest;

import java.time.LocalDateTime;
import java.util.List;

public interface OverrideService {

    /**
     * Ghi đè trạng thái của một đơn đặt bàn (Bắt buộc checkout khẩn cấp).
     */
    OverrideLogResponse override(Long reservationId, OverrideRequest req, Long accountId);

    /**
     * Lấy danh sách lịch sử ghi đè theo bộ lọc.
     */
    List<OverrideLogResponse> getLogs(LocalDateTime from, LocalDateTime to, Long accountId);
}