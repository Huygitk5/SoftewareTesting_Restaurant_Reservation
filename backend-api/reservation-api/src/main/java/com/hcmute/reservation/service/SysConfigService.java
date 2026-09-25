package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.config.SystemConfigDTO;

import java.util.List;

public interface SysConfigService {
    List<SystemConfigDTO> getAllConfigs();
    SystemConfigDTO updateConfig(String key, String value, String username);
}
