package com.hcmute.reservation.controller;

import com.hcmute.reservation.model.dto.config.SystemConfigDTO;
import com.hcmute.reservation.model.entity.Account;
import com.hcmute.reservation.service.SysConfigService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/configs")
@RequiredArgsConstructor
public class SystemConfigController {

    private final SysConfigService configManager;

    @GetMapping
    public ResponseEntity<List<SystemConfigDTO>> getAllConfigs() {
        return ResponseEntity.ok(configManager.getAllConfigs());
    }

    @PutMapping("/{key}")
    public ResponseEntity<SystemConfigDTO> updateConfig(
            @PathVariable String key,
            @RequestBody SystemConfigDTO request,
            Authentication authentication) {

        Account account = (Account) authentication.getPrincipal();

        String actualUsername = account.getUsername();

        return ResponseEntity.ok(configManager.updateConfig(key, request.getConfigValue(), actualUsername));
    }
}
