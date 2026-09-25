package com.hcmute.reservation.model.dto.reservation;

import lombok.Builder;
import lombok.Data;

import java.time.LocalDateTime;
import java.util.List;

/**
 * Trả về sau bước "Gợi ý bàn" (suggest) cho Walk-in.
 * Lễ tân xem thông tin này, rồi bấm Xác nhận hoặc Hủy.
 */
@Data
@Builder
public class WalkInSuggestionResponse {

    /** ID tạm dùng để confirm hoặc cancel ở bước 2 */
    private Long suggestionId;

    /** Danh sách bàn hệ thống gợi ý (đã được soft-lock) */
    private List<SuggestedTable> suggestedTables;

    /** Thời điểm soft-lock hết hạn — FE đếm ngược countdown */
    private LocalDateTime lockExpiresAt;

    private int guestCount;
    private LocalDateTime startTime;
    private LocalDateTime endTime;

    /** Loại gợi ý: FULL_AVAILABLE hoặc PARTIAL_AVAILABLE */
    private String availabilityType;

    @Data
    @Builder
    public static class SuggestedTable {
        private Long tableId;
        private int capacity;
        private String availabilityType; // FULL_AVAILABLE | PARTIAL_AVAILABLE
    }
}