package com.hcmute.reservation.model.dto.override;

import lombok.Builder;
import lombok.Data;

import java.time.LocalDateTime;

@Data
@Builder
public class OverrideLogResponse {
    private Long logId;
    private Long reservationId;
    private Long accountId;
    private String accountUsername;
    private String reason;
    private LocalDateTime createdAt;
}
