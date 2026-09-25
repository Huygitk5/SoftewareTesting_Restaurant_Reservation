package com.hcmute.reservation.controller;

import com.hcmute.reservation.model.dto.auth.LoginRequest;
import com.hcmute.reservation.model.dto.auth.LoginResponse;
import com.hcmute.reservation.model.dto.auth.RegisterRequest;
import com.hcmute.reservation.model.dto.auth.ForgotPasswordRequest;
import com.hcmute.reservation.model.dto.auth.ResetPasswordRequest;
import com.hcmute.reservation.model.dto.auth.CustomerProfileUpdateRequest;
import com.hcmute.reservation.model.dto.auth.ChangePasswordRequest;
import com.hcmute.reservation.security.AppSessionManager;
import com.hcmute.reservation.service.AuthService;
import jakarta.servlet.http.HttpSession;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.net.URI;
import java.util.Map;

@RestController
@RequestMapping("/api/auth")
@RequiredArgsConstructor
public class AuthController {
    private final AuthService authService;
    private final AppSessionManager sessionManager;

    /** POST /api/auth/register */
    @PostMapping("/register")
    public ResponseEntity<Map<String, String>> register(@Valid @RequestBody RegisterRequest req) {
        String token = authService.register(req);
        return ResponseEntity.ok(Map.of(
                "message", "Đăng ký thành công. Vui lòng kiểm tra email để xác minh tài khoản.",
                "token", token));
    }

    /** POST /api/auth/resend-verification */
    @PostMapping("/resend-verification")
    public ResponseEntity<Map<String, String>> resendVerification(@RequestBody Map<String, String> req) {
        String email = req.get("email");
        authService.resendVerification(email);
        String newToken = authService.getLatestVerificationToken(email);
        return ResponseEntity.ok(Map.of(
                "message", "Mã xác minh mới đã được gửi tới email của bạn.",
                "token", newToken));
    }

    /**
     * GET /api/auth/verify-email?token= — Xác minh xong Redirect về Trang Thành công/Thất bại
     */
    @GetMapping("/verify-email")
    public ResponseEntity<Void> verifyEmail(@RequestParam String token) {
        String redirectUrl = authService.handleEmailVerification(token);
        return ResponseEntity.status(HttpStatus.FOUND)
                .location(URI.create(redirectUrl))
                .build();
    }

    /** POST /api/auth/login */
    @PostMapping("/login")
    public ResponseEntity<LoginResponse> login(@Valid @RequestBody LoginRequest req, HttpSession session) {
        LoginResponse response = authService.login(req);
        sessionManager.createSession(session, response.getCustomerId(), response.getEmail());
        return ResponseEntity.ok(response);
    }

    /** POST /api/auth/logout */
    @PostMapping("/logout")
    public ResponseEntity<Map<String, String>> logout(HttpSession session) {
        sessionManager.invalidateSession(session);
        return ResponseEntity.ok(Map.of("message", "Đã đăng xuất thành công."));
    }

    /** POST /api/auth/forgot-password */
    @PostMapping("/forgot-password")
    public ResponseEntity<Map<String, String>> forgotPassword(@Valid @RequestBody ForgotPasswordRequest req) {
        String token = authService.forgotPassword(req);
        return ResponseEntity.ok(Map.of(
                "message", "Mã đặt lại mật khẩu đã được gửi.",
                "token", token));
    }

    /** POST /api/auth/reset-password */
    @PostMapping("/reset-password")
    public ResponseEntity<Map<String, String>> resetPassword(@Valid @RequestBody ResetPasswordRequest req) {
        authService.resetPassword(req);
        authService.removeResetApproval(req.getToken());
        return ResponseEntity.ok(Map.of("message", "Đặt lại mật khẩu thành công."));
    }

    /**
     * GET /api/auth/reset-password-page?token= — Redirect về trang thành công/lỗi
     */
    @GetMapping("/reset-password-page")
    public ResponseEntity<Void> resetPasswordPage(@RequestParam String token) {
        String redirectUrl = authService.handleResetPasswordPage(token);
        return ResponseEntity.status(HttpStatus.FOUND)
                .location(URI.create(redirectUrl))
                .build();
    }

    /**
     * GET /api/auth/check-reset-status?token= — Frontend polling gọi để check link đã bấm chưa
     */
    @GetMapping("/check-reset-status")
    public ResponseEntity<Map<String, Boolean>> checkResetStatus(@RequestParam String token) {
        boolean approved = authService.isResetApproved(token);
        return ResponseEntity.ok(Map.of("approved", approved));
    }

    /**
     * GET /api/auth/check-verify-status?token= — Frontend polling gọi để check link verify đã bấm chưa
     */
    @GetMapping("/check-verify-status")
    public ResponseEntity<Map<String, Boolean>> checkVerifyStatus(@RequestParam String token) {
        boolean approved = authService.isVerificationApproved(token);
        if (approved) {
            authService.removeVerificationApproval(token);
        }
        return ResponseEntity.ok(Map.of("approved", approved));
    }

    /** GET /api/auth/me — Lấy thông tin session hiện tại */
    @GetMapping("/me")
    public ResponseEntity<LoginResponse> getMe(HttpSession session) {
        Long customerId = sessionManager.getRequiredCustomerId(session);
        return ResponseEntity.ok(authService.getCustomerInfo(customerId));
    }

    /** PUT /api/auth/profile — Cập nhật thông tin cá nhân */
    @PutMapping("/profile")
    public ResponseEntity<LoginResponse> updateProfile(
            @Valid @RequestBody CustomerProfileUpdateRequest req,
            HttpSession session) {
        Long customerId = sessionManager.getRequiredCustomerId(session);
        return ResponseEntity.ok(authService.updateProfile(customerId, req));
    }

    /** PUT /api/auth/password — Đổi mật khẩu */
    @PutMapping("/password")
    public ResponseEntity<Map<String, String>> changePassword(
            @Valid @RequestBody ChangePasswordRequest req,
            HttpSession session) {
        Long customerId = sessionManager.getRequiredCustomerId(session);
        authService.changePassword(customerId, req);
        return ResponseEntity.ok(Map.of("message", "Đổi mật khẩu thành công."));
    }
}
