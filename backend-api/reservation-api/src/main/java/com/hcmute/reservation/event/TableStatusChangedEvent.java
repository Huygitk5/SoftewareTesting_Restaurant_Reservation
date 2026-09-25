package com.hcmute.reservation.event;

import com.hcmute.reservation.model.dto.table.TableUpdate;
import lombok.Getter;
import org.springframework.context.ApplicationEvent;

@Getter
public class TableStatusChangedEvent extends ApplicationEvent {
    private final TableUpdate payload;

    public TableStatusChangedEvent(Object source, Long tableId, String status) {
        super(source);
        this.payload = new TableUpdate(tableId, status);
    }
}