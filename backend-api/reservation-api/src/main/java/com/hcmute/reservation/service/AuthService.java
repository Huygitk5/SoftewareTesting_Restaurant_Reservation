package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.auth.*;

public interface AuthService {
    String register(RegisterRequest req);

    void resendVerification(String email);

    String verifyEmail(String token);

    void approveVerification(String token);

    boolean isVerificationApproved(String token);

    void removeVerificationApproval(String token);

    LoginResponse login(LoginRequest req);

    String forgotPassword(ForgotPasswordRequest req);

    void resetPassword(ResetPasswordRequest req);

    boolean validateResetToken(String token);

    void approveReset(String token);

    boolean isResetApproved(String token);

    void removeResetApproval(String token);

    String getLatestVerificationToken(String email);

    String getBaseUrl();

    LoginResponse getCustomerInfo(Long customerId);

    LoginResponse updateProfile(Long customerId, CustomerProfileUpdateRequest req);

    void changePassword(Long customerId, ChangePasswordRequest req);

    String handleEmailVerification(String token);

    String handleResetPasswordPage(String token);
}