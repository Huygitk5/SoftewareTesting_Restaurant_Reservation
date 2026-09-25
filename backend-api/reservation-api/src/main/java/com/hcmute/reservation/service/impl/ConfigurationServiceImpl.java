package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.dto.config.SystemConfigDTO;
import com.hcmute.reservation.model.entity.SystemConfig;
import com.hcmute.reservation.repository.SystemConfigRepository;
import com.hcmute.reservation.service.ConfigProviderService;
import com.hcmute.reservation.service.SysConfigService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.cache.annotation.CacheEvict;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.context.annotation.Lazy;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.stream.Collectors;

@Slf4j
@Service
@RequiredArgsConstructor
public class ConfigurationServiceImpl implements ConfigProviderService, SysConfigService {

    private final SystemConfigRepository repository;

    @Lazy
    @Autowired
    private ConfigProviderService self;

    @Override
    @Cacheable(value = "system_configs", key = "#key")
    public String getConfigValue(String key, String defaultValue) {
        return repository.findById(key)
                .map(SystemConfig::getConfigValue)
                .orElse(defaultValue);
    }

    @Override
    public int getDurationMinutes() {
        return Integer.parseInt(self.getConfigValue("reservation.duration-minutes", "120"));
    }

    @Override
    public int getBufferMinutes() {
        return Integer.parseInt(self.getConfigValue("reservation.buffer-minutes", "10"));
    }

    @Override
    public int getSoftLockMinutes() {
        return Integer.parseInt(self.getConfigValue("reservation.soft-lock-minutes", "5"));
    }

    @Override
    public double getDepositPerGuest() {
        return Double.parseDouble(self.getConfigValue("reservation.deposit-per-guest", "50000"));
    }

    @Override
    public int getGracePeriodMinutes() {
        return Integer.parseInt(self.getConfigValue("reservation.grace-period-minutes", "15"));
    }

    @Override
    public int getMaxCapacityOverflow() {
        return Integer.parseInt(self.getConfigValue("reservation.max-capacity-overflow", "2"));
    }

    @Override
    public int getMaxMergeTables() {
        return Integer.parseInt(self.getConfigValue("reservation.max-merge-tables", "4"));
    }

    @Override
    public String getOpeningTime() {
        return self.getConfigValue("restaurant.opening-time", "10:00");
    }

    @Override
    public String getClosingTime() {
        return self.getConfigValue("restaurant.closing-time", "22:30");
    }

    @Override
    public List<SystemConfigDTO> getAllConfigs() {
        return repository.findAll().stream().map(config -> {
            SystemConfigDTO dto = new SystemConfigDTO();
            dto.setConfigKey(config.getConfigKey());
            dto.setConfigValue(config.getConfigValue());
            dto.setDescription(config.getDescription());
            return dto;
        }).collect(Collectors.toList());
    }

    @Override
    @Transactional
    @CacheEvict(value = "system_configs", key = "#key") // Tự động xóa Cache của key này khi có update
    public SystemConfigDTO updateConfig(String key, String value, String username) {
        SystemConfig config = repository.findById(key)
                .orElseThrow(() -> new ResourceNotFoundException("Không tìm thấy cấu hình: " + key));

        config.setConfigValue(value);
        config.setUpdatedBy(username);
        repository.save(config); // Hibernate sẽ tự động gọi @PreUpdate để set updated_at

        log.info("[Config] Manager {} đã cập nhật {} = {}", username, key, value);

        SystemConfigDTO dto = new SystemConfigDTO();
        dto.setConfigKey(config.getConfigKey());
        dto.setConfigValue(config.getConfigValue());
        dto.setDescription(config.getDescription());
        return dto;
    }
}
