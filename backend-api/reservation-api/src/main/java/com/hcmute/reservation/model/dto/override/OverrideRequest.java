package com.hcmute.reservation.model.dto.override;

import jakarta.validation.constraints.NotBlank;
import lombok.Data;

@Data
public class OverrideRequest {
    @NotBlank
    private String reason;
}
