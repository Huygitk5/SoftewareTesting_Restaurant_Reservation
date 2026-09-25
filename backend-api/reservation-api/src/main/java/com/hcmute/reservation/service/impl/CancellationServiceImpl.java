package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.service.CancellationService;
import com.hcmute.reservation.service.TableReleaseService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;

import static com.hcmute.reservation.model.enums.ReservationStatus.*;

@Slf4j
@Service
@RequiredArgsConstructor
public class CancellationServiceImpl implements CancellationService {

    private final ReservationRepository reservationRepository;
    private final TableReleaseService tableReleaseService;

    @Override
    @Transactional
    public void cancelReservation(Long id, Long customerId) {
        Reservation reservation = reservationRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại."));

        if (customerId != null && (reservation.getCustomer() == null ||
                !reservation.getCustomer().getCustomerId().equals(customerId))) {
            throw new BadRequestException("Bạn không có quyền hủy đơn này.");
        }

        // Đơn đang chờ thanh toán → hủy và giải phóng soft-lock
        if (reservation.getStatus() == PENDING_PAYMENT) {
            reservation.setStatus(CANCELLED);
            reservationRepository.save(reservation);
            tableReleaseService.releaseLockedTable(reservation.getReservationId());
            return;
        }

        if (reservation.getStatus() == SEATED) {
            throw new BadRequestException(
                    "Khách đã nhận bàn (SEATED). Không thể hủy đơn. Nếu khách muốn rời đi, vui lòng sử dụng chức năng Check-out (Trả bàn).");
        }

        if (reservation.getStatus() == NO_SHOW || reservation.getStatus() == CANCELLED) {
            throw new BadRequestException("Đơn đặt bàn này đã được xử lý (Hủy hoặc Vắng mặt) từ trước.");
        }

        if (reservation.getStatus() != RESERVED) {
            throw new BadRequestException("Chỉ có thể hủy đơn đang chờ khách đến (Trạng thái RESERVED).");
        }

        // Kiểm tra thời gian: Chỉ khách hàng mới bị giới hạn 3 tiếng
        if (customerId != null) {
            if (reservation.getStartTime().isBefore(LocalDateTime.now().plusHours(3))) {
                throw new BadRequestException(
                        "Bạn chỉ có thể hủy đơn đặt bàn tối thiểu 3 tiếng trước giờ hẹn. Vui lòng liên hệ nhà hàng để được hỗ trợ.");
            }
        }

        reservation.cancel();
        reservationRepository.save(reservation);
    }
}
