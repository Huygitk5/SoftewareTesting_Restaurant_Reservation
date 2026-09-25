package com.hcmute.reservation.event;

import com.hcmute.reservation.model.dto.table.TableAlert;
import lombok.Getter;
import org.springframework.context.ApplicationEvent;

@Getter
public class TableAlertEvent extends ApplicationEvent {
    private final TableAlert payload;

    public TableAlertEvent(Object source, Long tableId, String alertType) {
        super(source);
        this.payload = new TableAlert(tableId, alertType);
    }
}