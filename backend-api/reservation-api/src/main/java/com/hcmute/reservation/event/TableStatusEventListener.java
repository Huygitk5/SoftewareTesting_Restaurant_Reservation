package com.hcmute.reservation.event;

import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Component;
import org.springframework.transaction.event.TransactionPhase;
import org.springframework.transaction.event.TransactionalEventListener;

@Slf4j
@Component
@RequiredArgsConstructor
public class TableStatusEventListener {

    private final SimpMessagingTemplate messagingTemplate;

    // Chỉ kích hoạt khi giao dịch Database đã lưu thành công!
    @TransactionalEventListener(phase = TransactionPhase.AFTER_COMMIT)
    public void handleTableStatusChange(TableStatusChangedEvent event) {
        log.info("Sending realtime update for Table #{}: {}",
                event.getPayload().getTableId(), event.getPayload().getStatus());

        // Gửi tới tất cả các client đang subscribe "/topic/tables"
        messagingTemplate.convertAndSend("/topic/tables", event.getPayload());
    }
}