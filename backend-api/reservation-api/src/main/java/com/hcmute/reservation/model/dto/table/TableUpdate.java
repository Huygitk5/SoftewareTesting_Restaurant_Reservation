package com.hcmute.reservation.model.dto.table;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class TableUpdate {
    private Long tableId;
    private String status; // "AVAILABLE", "OCCUPIED", "OVERSTAY"
}
