package com.hcmute.reservation.service.impl;
import com.hcmute.reservation.event.ReservationConfirmedEvent;
import com.hcmute.reservation.event.TableStatusChangedEvent;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.mapper.ReservationMapper;
import com.hcmute.reservation.model.dto.reservation.OnlineReservationRequest;
import com.hcmute.reservation.model.dto.reservation.ReservationResponse;
import com.hcmute.reservation.model.entity.*;
import com.hcmute.reservation.model.enums.*;
import com.hcmute.reservation.repository.*;
import com.hcmute.reservation.service.ConfigProviderService;
import com.hcmute.reservation.service.OnlineBookingService;
import com.hcmute.reservation.strategy.TableCombinationAlgorithm;
import lombok.RequiredArgsConstructor;
import org.springframework.context.ApplicationEventPublisher;
import org.springframework.orm.ObjectOptimisticLockingFailureException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.Duration;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.*;
import java.util.stream.Collectors;

import static com.hcmute.reservation.model.enums.ReservationStatus.*;

@Service
@RequiredArgsConstructor
public class OnlineBookingServiceImpl implements OnlineBookingService {

    private final ReservationRepository reservationRepository;
    private final CustomerRepository customerRepository;
    private final TableInfoRepository tableInfoRepository;
    private final ReservationTableMappingRepository mappingRepository;
    private final ApplicationEventPublisher eventPublisher;
    private final TableCombinationAlgorithm algorithm;
    private final ReservationMapper mapper;
    private final ConfigProviderService configProvider;

    @Override
    @Transactional
    public ReservationResponse createOnlineReservation(OnlineReservationRequest req, Long customerId) {
        int durationMinutes = configProvider.getDurationMinutes();
        int bufferMinutes = configProvider.getBufferMinutes();
        int softLockMinutes = configProvider.getSoftLockMinutes();
        String openingTimeStr = configProvider.getOpeningTime();
        String closingTimeStr = configProvider.getClosingTime();
        Double depositPerGuest = configProvider.getDepositPerGuest();

        Customer customer = customerRepository.findById(customerId).orElseThrow(() -> new ResourceNotFoundException("Không tìm thấy khách hàng."));
        LocalDateTime start = req.getStartTime();
        LocalTime openingTime = LocalTime.parse(openingTimeStr);
        LocalTime closingTime = LocalTime.parse(closingTimeStr);
        LocalDateTime closingDateTime = LocalDateTime.of(start.toLocalDate(), closingTime);

        if (start.isBefore(LocalDateTime.now().plusHours(1))) throw new BadRequestException("Vui lòng đặt bàn trước ít nhất 1 tiếng.");
        if (start.toLocalTime().isBefore(openingTime) || !start.isBefore(closingDateTime)) {
            throw new BadRequestException("Giờ đến nằm ngoài thời gian hoạt động của nhà hàng (" + openingTimeStr
                    + " - " + closingTimeStr + ").");
        }
        if (Duration.between(start, closingDateTime).toMinutes() < 60) {
            throw new BadRequestException("Thời gian dùng bữa tối thiểu là 60 phút. Nhà hàng đóng cửa lúc "
                    + closingTimeStr + ", vui lòng chọn giờ đến sớm hơn.");
        }

        LocalDateTime end = start.plusMinutes(durationMinutes).isAfter(closingDateTime) ? closingDateTime : start.plusMinutes(durationMinutes);
        if (reservationRepository.findOverlappingByCustomerId(customerId, start, end).size() >= 2) {
            throw new BadRequestException("Bạn chỉ được phép đặt tối đa 2 đơn trong cùng một khung giờ. " +
                    "Vui lòng chọn khung giờ khác hoặc hoàn tất/hủy các đơn hiện tại.");
        }

        Set<Long> occupiedTableIds = new HashSet<>(reservationRepository.findOccupiedTableIds(start, end.plusMinutes(bufferMinutes)));
        // Tìm bàn đơn
        List<TableInfo> available = tableInfoRepository.findAvailableTablesForGuests(req.getGuestCount())
                .stream().filter(t -> !occupiedTableIds.contains(t.getTableId())).collect(Collectors.toList());

        List<TableInfo> selectedTables = new ArrayList<>();
        if (!available.isEmpty()) {
            selectedTables.add(available.get(0));
        } else {
            List<TableInfo> allAvailable = tableInfoRepository.findByStatusAndIsActiveTrue(TableStatus.AVAILABLE)
                    .stream().filter(t -> !t.isSoftLocked() && !occupiedTableIds.contains(t.getTableId()))
                    .collect(Collectors.toList());

            // Gọi ghép bàn
            selectedTables = algorithm.findBestTableCombination(allAvailable, req.getGuestCount());
            if (selectedTables.isEmpty()) {
                throw new BadRequestException("Hiện không có tổ hợp bàn trống phù hợp để ghép cho "
                        + req.getGuestCount() + " người. Vui lòng chọn giờ khác.");
            }
        }

        // Tạo Reservation với status = CREATED
        Reservation reservation = reservationRepository.save(
                Reservation.builder().customer(customer).type(ReservationType.ONLINE).guestCount(req.getGuestCount())
                        .startTime(start).endTime(end).depositAmount(req.getGuestCount() * depositPerGuest)
                        .status(CREATED).note(req.getNote()).build()
        );

        List<ReservationTableMapping> mappings = new ArrayList<>();
        try {
            for (TableInfo t : selectedTables) {
                t.applySoftLock(reservation.getReservationId(), softLockMinutes);
                tableInfoRepository.saveAndFlush(t);

                ReservationTableMapping mapping = ReservationTableMapping.builder()
                        .reservation(reservation).tableInfo(t).build();
                mappings.add(mappingRepository.save(mapping));

                eventPublisher.publishEvent(
                        new TableStatusChangedEvent(this, t.getTableId(), "OCCUPIED"));
            }
        } catch (ObjectOptimisticLockingFailureException e) {
            throw new ConflictException(
                    "Rất tiếc, bàn bạn chọn vừa được khách hàng khác đặt thành công. Vui lòng chọn lại khung giờ hoặc bàn khác!");
        }

        reservation.setTableMappings(mappings);
        reservation.setStatus(PENDING_PAYMENT);
        reservation = reservationRepository.save(reservation);

        return mapper.toResponse(reservation);
    }

    @Override
    @Transactional
    public ReservationResponse confirmPayment(Long id) {
        Reservation reservation = reservationRepository.findById(id).orElseThrow(() -> new ResourceNotFoundException("Reservation not found"));
        if (reservation.getStatus() != PENDING_PAYMENT) throw new BadRequestException("Only PENDING_PAYMENT reservation can be confirmed. Current status: " + reservation.getStatus());

        List<TableInfo> lockedTables = tableInfoRepository.findByLockedByReservationId(reservation.getReservationId());
        if (lockedTables.isEmpty()) throw new ConflictException("Giao dịch thanh toán mất quá nhiều thời gian. Thời gian giữ bàn (5 phút) đã hết và bàn đã bị giải phóng. Vui lòng liên hệ nhà hàng để được hỗ trợ hoàn tiền hoặc xếp bàn mới.");

        reservation.setStatus(RESERVED);
        lockedTables.forEach(t -> {
            t.releaseSoftLock();
            tableInfoRepository.save(t);

            eventPublisher.publishEvent(
                    new TableStatusChangedEvent(this, t.getTableId(), "AVAILABLE"));
        });

        // Chuẩn bị dữ liệu gửi Email (Tránh lazy loading trong Async thread sau này)
        String customerEmail = reservation.getCustomer() != null ? reservation.getCustomer().getEmail() : null;
        String customerName = reservation.getCustomer() != null ? reservation.getCustomer().getName() : "Guest";

        eventPublisher.publishEvent(new ReservationConfirmedEvent(
                this, customerEmail, customerName,
                reservation.getReservationId(), reservation.getStartTime()));

        return mapper.toResponse(reservation);
    }

    @Override
    @Transactional
    public ReservationResponse cancelPayment(Long id) {
        Reservation reservation = reservationRepository.findById(id).orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));
        if (reservation.getStatus() == PENDING_PAYMENT || reservation.getStatus() == CREATED) {
            reservation.setStatus(CANCELLED);
            tableInfoRepository.findByLockedByReservationId(id)
                    .forEach(t -> {
                        t.releaseSoftLock();
                        tableInfoRepository.save(t);
                        eventPublisher.publishEvent(
                                new TableStatusChangedEvent(this, t.getTableId(), "AVAILABLE"));
                    });
        } else throw new BadRequestException("Không thể hủy đơn đặt bàn ở trạng thái hiện tại.");

        return mapper.toResponse(reservation);
    }
}
