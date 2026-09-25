package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.event.TableStatusChangedEvent;
import com.hcmute.reservation.repository.TableInfoRepository;
import com.hcmute.reservation.service.TableReleaseService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Slf4j
@Service
@RequiredArgsConstructor
public class TableReleaseServiceImpl implements TableReleaseService {

    private final TableInfoRepository tableInfoRepository;
    private final ApplicationEventPublisher eventPublisher;

    @Override
    @Transactional
    public void releaseLockedTable(Long reservationId) {
        tableInfoRepository.findByLockedByReservationId(reservationId).forEach(t -> {
            t.releaseSoftLock();
            tableInfoRepository.save(t);
            eventPublisher.publishEvent(
                    new TableStatusChangedEvent(this, t.getTableId(), "AVAILABLE"));
        });
    }
}
