package com.hcmute.reservation.event;

import com.hcmute.reservation.service.EmailService;
import org.springframework.context.event.EventListener;
import org.springframework.scheduling.annotation.Async;
import org.springframework.stereotype.Component;

@Component
public class EmailNotificationListener {

    private final EmailService emailService;

    public EmailNotificationListener(EmailService emailService) {
        this.emailService = emailService;
    }

    // @Async giúp hàm này chạy ở một luồng (thread) khác, không làm chậm response thanh toán
    // Không dùng @Transactional ở đây nữa vì data đã được truyền trực tiếp qua Event, không cần truy vấn DB
    @Async
    @EventListener
    public void handleReservationConfirmedEvent(ReservationConfirmedEvent event) {
        if (event.getCustomerEmail() == null) return;

        // Gọi service gửi Email
        emailService.sendReservationConfirmationEmail(
                event.getCustomerEmail(),
                event.getCustomerName(),
                event.getReservationId(),
                event.getStartTime()
        );
    }
}