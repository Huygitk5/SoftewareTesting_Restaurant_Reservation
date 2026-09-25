package com.hcmute.reservation.strategy.impl;

import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.strategy.TableAllocationStrategy;
import org.springframework.core.annotation.Order;
import org.springframework.stereotype.Component;

import java.util.Collections;
import java.util.List;

@Component
@Order(1)
public class SingleTableStrategy implements TableAllocationStrategy {

    @Override
    public List<TableInfo> allocate(int guestCount, List<TableInfo> freeTables) {
        return freeTables.stream()
                .filter(t -> t.getCapacity() >= guestCount)
                .findFirst() // Lấy bàn đầu tiên đủ sức chứa
                .map(List::of)
                .orElse(Collections.emptyList());
    }

    @Override
    public int getOrder() { return 1; }
}