package com.hcmute.reservation.strategy;

import com.hcmute.reservation.model.entity.TableInfo;
import org.springframework.core.Ordered;
import java.util.List;

public interface TableAllocationStrategy extends Ordered {

    // Trả về danh sách bàn được chọn. Nếu không tìm được, trả về list rỗng.
    List<TableInfo> allocate(int guestCount, List<TableInfo> freeTables);

}