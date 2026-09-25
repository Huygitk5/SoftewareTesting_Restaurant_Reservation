package com.hcmute.reservation.model.dto.reservation;

import lombok.Builder;
import lombok.Data;

import java.time.LocalDateTime;
import java.util.List;

@Data
@Builder
public class WalkInOptionResponse {
    private List<OptionGroup> groups;

    @Data
    @Builder
    public static class OptionGroup {
        // "Ưu tiên (Trống hoàn toàn)" hoặc "Dự phòng (Vướng lịch sau)"
        private String groupName;
        private List<TableOption> options;
    }

    @Data
    @Builder
    public static class TableOption {
        private List<Long> tableIds;
        private int totalCapacity;
        // FULL_AVAILABLE, MERGE_AVAILABLE, PARTIAL_AVAILABLE, PARTIAL_MERGED_AVAILABLE
        private String type;
        // null nếu là FULL_AVAILABLE hoặc MERGE_AVAILABLE
        private LocalDateTime availableUntil;
    }
}
