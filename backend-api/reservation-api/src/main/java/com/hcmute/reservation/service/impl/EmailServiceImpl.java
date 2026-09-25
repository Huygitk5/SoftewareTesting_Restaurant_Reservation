package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.service.EmailService;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.stereotype.Service;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

@Service
@RequiredArgsConstructor
public class EmailServiceImpl implements EmailService{

    private final JavaMailSender mailSender;

    @Value("${spring.mail.username}")
    private String from;

    @Value("${server.port:8081}")
    private String serverPort;

    @Override
    public void sendVerificationEmail(String toEmail, String token) {
        String subject = "Xác minh email - Nhà Hàng Đặt Bàn";
        String link = buildUrl("/api/auth/verify-email", token);
        String body = String.format(
                "Chào bạn,\n\nVui lòng click vào link sau để xác minh tài khoản:\n%s\n\nLink có hiệu lực trong 30 phút.\n\nTrân trọng.",
                link
        );
        sendCustomEmail(toEmail, subject, body);
    }

    @Override
    public void sendResetPasswordEmail(String toEmail, String token) {
        String subject = "Đặt lại mật khẩu - Nhà Hàng Đặt Bàn";
        String link = buildUrl("/api/auth/reset-password-page", token);
        String body = String.format(
                "Chào bạn,\n\nVui lòng click vào link sau để đặt lại mật khẩu:\n%s\n\nLink có hiệu lực trong 10 phút. Nếu bạn không yêu cầu, hãy bỏ qua email này.\n\nTrân trọng.",
                link
        );

        sendCustomEmail(toEmail, subject, body);
    }

    @Override
    public void sendCustomEmail(String to, String subject, String body) {
        // Tách biệt hoàn toàn logic thao tác hạ tầng (JavaMailSender) vào một hàm duy nhất
        SimpleMailMessage message = new SimpleMailMessage();
        message.setFrom(from);
        message.setTo(to);
        message.setSubject(subject);
        message.setText(body);

        mailSender.send(message);
    }

    @Override
    public void sendReservationConfirmationEmail(String toEmail, String customerName, Long reservationId, LocalDateTime startTime) {
        String subject = "Xác nhận đặt bàn thành công - San-Lorenzo Restaurant";

        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("HH:mm dd/MM/yyyy");
        String startTimeFormatted = startTime.format(formatter);

        String body = String.format("Chào %s,\n\nChúc mừng bạn đã đặt bàn thành công tại San-Lorenzo!\n\n" +
                "Mã đặt bàn của bạn: SL-%d\n" +
                "Thời gian: %s\n\n" +
                "Hẹn gặp lại bạn sớm.\nTrân trọng.", customerName, reservationId, startTimeFormatted);

        sendCustomEmail(toEmail, subject, body);
    }

    @Override
    public void sendEmailChangeAlert(String oldEmail, String newEmail) {
        String subject = "Cảnh báo bảo mật: Thay đổi địa chỉ email";
        String body = String.format(
                "Chào bạn,\n\nTài khoản của bạn vừa yêu cầu đổi địa chỉ email sang: %s.\n" +
                "Nếu bạn không thực hiện yêu cầu này, vui lòng liên hệ ngay với bộ phận hỗ trợ của chúng tôi để bảo mật tài khoản.\n\n" +
                "Trân trọng,\nSan-Lorenzo Restaurant", newEmail
        );
        sendCustomEmail(oldEmail, subject, body);
    }

    // --- CÁC HÀM PRIVATE PHỤ TRỢ ---

    /**
     * Hàm phụ trợ giúp tái sử dụng logic tạo URL, tránh lặp code (DRY Principle)
     */
    private String buildUrl(String path, String token) {
        return "http://localhost:" + serverPort + path + "?token=" + token;
    }

}
