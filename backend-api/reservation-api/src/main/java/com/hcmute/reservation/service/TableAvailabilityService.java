package com.hcmute.reservation.service;

import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.TableInfo;

import java.time.LocalDateTime;
import java.util.List;

public interface TableAvailabilityService {

    /**
     * Lấy danh sách bàn trống phục vụ cho việc gán bàn (Assignment).
     * @param reservation Đơn đặt bàn cần lấy bàn trống.
     */
    List<TableInfo> getFreeTables(Reservation reservation);

    /**
     * Lấy danh sách bàn trống phục vụ cho việc tra cứu API (Availability).
     * @param startTime Thời gian bắt đầu.
     * @param endTime Thời gian kết thúc dự kiến.
     */
    List<TableInfo> getFreeTables(LocalDateTime startTime, LocalDateTime endTime);

    /**
     * Lấy danh sách các bàn đang rảnh ngay tại thời điểm hiện tại (Không bị soft-lock).
     */
    List<TableInfo> getCurrentlyAvailableTables();

    /**
     * Tìm thời điểm bắt đầu của đơn đặt bàn tiếp theo cho một bàn cụ thể.
     * @return Thời gian booking tiếp theo, hoặc null nếu bàn không có ai đặt.
     */
    LocalDateTime getNextBookingTime(Long tableId, LocalDateTime fromTime);
}