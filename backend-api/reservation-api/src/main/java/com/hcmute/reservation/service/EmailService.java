package com.hcmute.reservation.service;

import java.time.LocalDateTime;

public interface EmailService {

    /**
     * Gửi email chứa link xác minh tài khoản khi đăng ký mới.
     * @param toEmail Email người nhận
     * @param token Token xác minh
     */
    void sendVerificationEmail(String toEmail, String token);

    /**
     * Gửi email chứa link đặt lại mật khẩu khi quên mật khẩu.
     * @param toEmail Email người nhận
     * @param token Token khôi phục
     */
    void sendResetPasswordEmail(String toEmail, String token);

    /**
     * Gửi một email tùy chỉnh bất kỳ (Phục vụ cho các module khác nếu cần).
     * @param to Email người nhận
     * @param subject Tiêu đề email
     * @param body Nội dung email
     */
    void sendCustomEmail(String to, String subject, String body);

    /**
     * Gửi email thông báo xác nhận đặt bàn thành công.
     */
    void sendReservationConfirmationEmail(String toEmail, String customerName, Long reservationId, LocalDateTime startTime);

    /**
     * Gửi email cảnh báo bảo mật khi người dùng yêu cầu thay đổi email.
     */
    void sendEmailChangeAlert(String oldEmail, String newEmail);
}