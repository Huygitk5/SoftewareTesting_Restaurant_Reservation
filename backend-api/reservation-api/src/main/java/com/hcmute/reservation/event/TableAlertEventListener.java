package com.hcmute.reservation.event;

import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.context.event.EventListener;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Component;

@Slf4j
@Component
@RequiredArgsConstructor
public class TableAlertEventListener {

    private final SimpMessagingTemplate messagingTemplate;

    @EventListener
    public void handleTableStatusChange(TableAlertEvent event) {
        // Gửi tới tất cả các client đang subscribe "/topic/table-alerts"
        messagingTemplate.convertAndSend("/topic/table-alerts", event.getPayload());
    }
}