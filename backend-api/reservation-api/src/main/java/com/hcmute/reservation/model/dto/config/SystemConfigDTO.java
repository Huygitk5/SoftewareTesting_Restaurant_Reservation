package com.hcmute.reservation.model.dto.config;

import lombok.Data;
import jakarta.validation.constraints.NotBlank;

@Data
public class SystemConfigDTO {
    @NotBlank(message = "Config key không được để trống")
    private String configKey;

    @NotBlank(message = "Config value không được để trống")
    private String configValue;

    private String description;
}