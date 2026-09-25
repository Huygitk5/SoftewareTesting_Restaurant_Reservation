package com.hcmute.reservation.service;

import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.TableInfo;
import java.util.List;

public interface ReservationMappingService {

    /**
     * Cập nhật liên kết (Mapping) giữa Đơn đặt bàn và danh sách Bàn mới.
     * Lưu ý: Hàm này yêu cầu phải được gọi bên trong một Transaction đã có sẵn (MANDATORY).
     *
     * @param reservation Đơn đặt bàn cần cập nhật
     * @param newTables Danh sách các bàn mới sẽ được gán cho đơn đặt bàn này
     */
    void updateTableMappings(Reservation reservation, List<TableInfo> newTables);
}