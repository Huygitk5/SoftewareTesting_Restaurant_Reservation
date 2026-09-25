package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.service.AssignmentService;
import com.hcmute.reservation.service.ReservationMappingService;
import com.hcmute.reservation.service.TableAvailabilityService;
import com.hcmute.reservation.strategy.TableAllocationStrategy;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
@RequiredArgsConstructor
public class AssignmentServiceImpl implements AssignmentService {

    private final TableAvailabilityService availabilityService;
    private final ReservationMappingService mappingService;
    private final List<TableAllocationStrategy> allocationStrategies;

    @Override
    @Transactional
    public boolean findAlternativeTables(Reservation reservation) {
        // Lấy pool bàn trống
        List<TableInfo> freeTables = availabilityService.getFreeTables(reservation);

        if (freeTables.isEmpty()) return false;

        List<TableInfo> selectedTables = null;

        // Thử lần lượt các chiến lược xếp bàn (Tìm bàn đơn -> Ghép bàn)
        for (TableAllocationStrategy strategy : allocationStrategies) {
            selectedTables = strategy.allocate(reservation.getGuestCount(), freeTables);
            if (selectedTables != null && !selectedTables.isEmpty()) {
                break; // Tìm thấy phương án tối ưu thì dừng lại
            }
        }

        // Nếu thất bại ở mọi chiến lược -> Trả về false
        if (selectedTables == null || selectedTables.isEmpty()) {
            return false;
        }

        // Nếu thành công -> Cập nhật Database
        mappingService.updateTableMappings(reservation, selectedTables);
        return true;
    }
}