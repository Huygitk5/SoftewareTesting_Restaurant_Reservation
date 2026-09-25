package com.hcmute.reservation.model.dto.report;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class NoShowRateResponse {
    private long totalOnline;
    private long totalWalkIn;
    private long totalAll;
    private long noShowCount;
    private double noShowRate;
}
