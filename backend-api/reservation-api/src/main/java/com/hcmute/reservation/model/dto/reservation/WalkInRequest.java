package com.hcmute.reservation.model.dto.reservation;

import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;
import lombok.Data;

import java.time.LocalDateTime;
import java.util.List;

@Data
public class WalkInRequest {
    @NotNull @Min(1)
    private Integer guestCount;

    private String customerName;

    private String customerPhone;

    // null = auto-detect now
    private LocalDateTime startTime;

    // Thời gian kết thúc dự kiến do Frontend tính toán truyền xuống
    private LocalDateTime endTime;

    // Cho phép chỉ định bàn cụ thể (optional, null = auto-assign)
    private List<Long> tableId;

    // Ghép bàn: nếu 1 bàn không đủ
    private boolean mergeTables;

    private String note;
}
