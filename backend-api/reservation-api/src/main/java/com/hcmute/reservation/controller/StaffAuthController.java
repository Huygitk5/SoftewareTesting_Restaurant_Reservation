package com.hcmute.reservation.controller;

import com.hcmute.reservation.model.dto.auth.StaffLoginRequest;
import com.hcmute.reservation.model.dto.auth.StaffLoginResponse;
import com.hcmute.reservation.model.dto.auth.StaffRegisterRequest;
import com.hcmute.reservation.service.StaffAuthService;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.Map;

@RestController
@RequestMapping("/api/staff/auth")
@RequiredArgsConstructor
public class StaffAuthController {

    private final StaffAuthService staffAuthService;

    /** POST /api/staff/auth/login */
    @PostMapping("/login")
    public ResponseEntity<StaffLoginResponse> login(@Valid @RequestBody StaffLoginRequest req) {
        return ResponseEntity.ok(staffAuthService.login(req));
    }

    /** POST /api/staff/auth/logout */
    @PostMapping("/logout")
    public ResponseEntity<Void> logout(HttpServletRequest request) {
        staffAuthService.logout(extractToken(request));
        return ResponseEntity.noContent().build();
    }

    /** POST /api/staff/auth/register */
    @PostMapping("/register")
    public ResponseEntity<Map<String, String>> registerStaff(@Valid @RequestBody StaffRegisterRequest req) {
        String message = staffAuthService.register(req);
        return ResponseEntity.ok(Map.of("message", message));
    }

    // Hàm bổ trợ để trích xuất token từ Header tùy chỉnh
    private String extractToken(HttpServletRequest request) {
        return request.getHeader("X-Staff-Token");
    }
}
