package com.hcmute.reservation.model.dto.table;

import com.hcmute.reservation.model.enums.TableStatus;
import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class TableResponse {
    private Long tableId;
    private int capacity;
    private TableStatus status;
    private boolean isActive;
    private int version;
}
