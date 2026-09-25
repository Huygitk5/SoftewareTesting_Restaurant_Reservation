package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.table.AvailableWindowResponse;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.List;
import java.util.Map;

public interface AvailabilityApiService {

    /**
     * GET /api/reservations/availability
     * Trả về danh sách bàn khả dụng và gợi ý giờ thay thế cho luồng Booking Online.
     */
    Map<String, Object> checkAvailability(LocalDate date, LocalTime time, int guests);

    /**
     * GET /api/reservations/availability/slots
     * Kiểm tra trạng thái khả dụng cho một danh sách các khung giờ (dùng cho UI Grid).
     */
    Map<String, Boolean> checkSlotsAvailability(LocalDate date, int guests, List<LocalTime> slots);

    /**
     * GET /api/tables/available-windows
     * Phân tích real-time các khoảng thời gian trống của bàn để hiển thị cho màn hình POS.
     */
    List<AvailableWindowResponse> getAvailableWindows(int guests, LocalDateTime time);
}