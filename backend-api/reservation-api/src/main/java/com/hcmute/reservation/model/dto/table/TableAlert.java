package com.hcmute.reservation.model.dto.table;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class TableAlert {
    private Long tableId;
    private String alertType; // "START_BLINK" hoặc "STOP_BLINK"
}