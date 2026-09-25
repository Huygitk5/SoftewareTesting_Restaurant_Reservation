package com.hcmute.reservation.model.dto.table;

import lombok.Builder;
import lombok.Data;

import java.time.LocalDateTime;
import java.util.List;

@Data
@Builder
public class AvailableWindowResponse {

    public enum Availability { FULL_AVAILABLE, PARTIAL_AVAILABLE }

    private Long tableId;
    private int capacity;
    private Availability availability;
    // non-null nếu PARTIAL_AVAILABLE — thời điểm bàn sẽ trống
    private LocalDateTime availableUntil;
    // Các bàn có thể ghép để đủ chỗ
    private List<Long> mergeCandidateIds;
}
