package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.mapper.ReservationMapper;
import com.hcmute.reservation.model.dto.reservation.*;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.repository.*;
import com.hcmute.reservation.service.ReservationService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.*;
import java.util.stream.Collectors;

import static com.hcmute.reservation.model.enums.ReservationStatus.*;

@Slf4j
@Service
@RequiredArgsConstructor
public class ReservationServiceImpl implements  ReservationService {

    private final ReservationRepository reservationRepository;
    private final ReservationMapper mapper;

    @Override
    @Transactional(readOnly = true)
    public ReservationResponse getById(Long id) {
        Reservation r = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));
        return mapper.toResponse(r);
    }

    @Override
    @Transactional(readOnly = true)
    public List<ReservationResponse> getReservationsByCustomer(Long customerId) {
        return reservationRepository.findByCustomer_CustomerIdOrderByStartTimeDesc(customerId)
                .stream()
                .map(mapper::toResponse)
                .collect(Collectors.toList());
    }

    @Override
    @Transactional(readOnly = true)
    public List<ReservationResponse> getActiveReservations() {
        return reservationRepository.findByStatusOrderByStartTimeAsc(SEATED)
                .stream().map(mapper::toResponse).collect(Collectors.toList());
    }

    @Override
    @Transactional(readOnly = true)
    public List<ReservationResponse> getUpcomingReservations(int minutes) {
        LocalDateTime now = LocalDateTime.now();
        LocalDateTime startWindow = now.minusMinutes(15);
        LocalDateTime endWindow = now.plusMinutes(minutes);
        return reservationRepository.findUpcoming(startWindow, endWindow)
                .stream().map(mapper::toResponse).collect(Collectors.toList());
    }
}