package com.hcmute.reservation.model.dto.report;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class DailyReservationReport {
    private String date;
    private long totalOnline;
    private long totalWalkIn;
    private long total;
    private long noShowCount;
}
