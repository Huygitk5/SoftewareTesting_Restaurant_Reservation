package com.hcmute.reservation.controller;

import com.hcmute.reservation.model.dto.reservation.*;
import com.hcmute.reservation.service.*;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.time.LocalTime;
import java.util.List;
import java.util.Map;

@RestController
@RequestMapping("/api/reservations")
@RequiredArgsConstructor
public class ReservationController {

    private final ReservationService reservationService;
    private final AvailabilityApiService availabilityApiService;
    private final OnlineBookingService onlineBookingService;
    private final InHouseService inHouseService;
    private final WalkInService walkInService;
    private final ChangeTableService changeTableService;
    private final CancellationService cancellationService;

    /** GET /api/reservations/availability?date=&time=&guests= */
    @GetMapping("/availability")
    public ResponseEntity<Map<String, Object>> checkAvailability(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.TIME) LocalTime time,
            @RequestParam int guests) {
        return ResponseEntity.ok(availabilityApiService.checkAvailability(date, time, guests));
    }

    /** GET /api/reservations/availability/slots?date=&guests=&slots= */
    @GetMapping("/availability/slots")
    public ResponseEntity<Map<String, Boolean>> checkSlotsAvailability(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date,
            @RequestParam int guests,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.TIME) List<LocalTime> slots) {
        return ResponseEntity.ok(availabilityApiService.checkSlotsAvailability(date, guests, slots));
    }

    /** POST /api/reservations/online */
    @PostMapping("/online")
    public ResponseEntity<ReservationResponse> createOnline(
            @Valid @RequestBody OnlineReservationRequest req,
            Authentication auth) {
        Long customerId = (auth.getDetails() instanceof Number n) ? n.longValue() : null;
        return ResponseEntity.status(HttpStatus.CREATED)
                .body(onlineBookingService.createOnlineReservation(req, customerId));
    }

    /** GET /api/reservations/my */
    @GetMapping("/my")
    public ResponseEntity<List<ReservationResponse>> getMyReservations(Authentication auth) {
        Long customerId = (auth.getDetails() instanceof Number n) ? n.longValue() : null;
        return ResponseEntity.ok(reservationService.getReservationsByCustomer(customerId));
    }

    /** GET /api/reservations/active */

    @GetMapping("/active")
    public ResponseEntity<List<ReservationResponse>> getActive() {
        return ResponseEntity.ok(reservationService.getActiveReservations());
    }

    /** GET /api/reservations/upcoming?minutes=30 */
    @GetMapping("/upcoming")
    public ResponseEntity<List<ReservationResponse>> getUpcoming(
            @RequestParam(defaultValue = "30") int minutes) {
        return ResponseEntity.ok(reservationService.getUpcomingReservations(minutes));
    }

    /** GET /api/reservations/{id} */
    @GetMapping("/{id}")
    public ResponseEntity<ReservationResponse> getById(@PathVariable Long id) {
        return ResponseEntity.ok(reservationService.getById(id));
    }

    /** DELETE /api/reservations/{id} */
    @DeleteMapping("/{id}")
    public ResponseEntity<Map<String, String>> cancelReservation(
            @PathVariable Long id, Authentication auth) {
        // auth.getDetails() là Long cho CUSTOMER token; null cho STAFF token
        Long customerId = (auth.getDetails() instanceof Long id2) ? id2 : null;
        cancellationService.cancelReservation(id, customerId);
        return ResponseEntity.ok(Map.of("message", "Đơn đặt bàn đã được hủy thành công."));
    }

    /** POST /api/reservations/{id}/payment/confirm */
    @PostMapping("/{id}/payment/confirm")
    public ResponseEntity<ReservationResponse> confirmPayment(@PathVariable Long id) {
        return ResponseEntity.ok(onlineBookingService.confirmPayment(id));
    }

    /** POST /api/reservations/{id}/payment/cancel */
    @PostMapping("/{id}/payment/cancel")
    public ResponseEntity<ReservationResponse> cancelPayment(@PathVariable Long id) {
        return ResponseEntity.ok(onlineBookingService.cancelPayment(id));
    }

    /**
     * GET /api/reservations/walk-in/options?guestCount=
     * Chi doc du lieu, khong tao reservation va khong soft-lock ban.
     */
    @GetMapping("/walk-in/options")
    public ResponseEntity<WalkInOptionResponse> getWalkInOptions(
            @RequestParam int guestCount) {
        return ResponseEntity.ok(walkInService.getWalkInOptions(guestCount));
    }

    /**
     * POST /api/reservations/walk-in/suggest
     Tìm bàn gợi ý, soft-lock lại, trả về WalkInSuggestionResponse.
     */
    @PostMapping("/walk-in/suggest")
    public ResponseEntity<WalkInSuggestionResponse> suggestWalkIn(
            @Valid @RequestBody WalkInRequest req) {
        return ResponseEntity.ok(walkInService.suggestWalkIn(req));
    }

    /**
     * POST /api/reservations/walk-in/confirm/{suggestionId}
     Lễ tân xác nhận → tạo reservation chính thức (SEATED) + OCCUPIED.
     */
    @PostMapping("/walk-in/confirm/{suggestionId}")
    public ResponseEntity<ReservationResponse> confirmWalkIn(
            @PathVariable Long suggestionId) {
        return ResponseEntity.status(HttpStatus.CREATED)
                .body(walkInService.confirmWalkIn(suggestionId));
    }

    /**
     * POST /api/reservations/walk-in/cancel-suggestion/{suggestionId}
    Lễ tân hủy gợi ý → giải phóng soft-lock, hủy reservation tạm.
     * Idempotent: gọi nhiều lần không gây lỗi.
     */
    @PostMapping("/walk-in/cancel-suggestion/{suggestionId}")
    public ResponseEntity<Map<String, String>> cancelWalkInSuggestion(
            @PathVariable Long suggestionId) {
        walkInService.cancelWalkInSuggestion(suggestionId);
        return ResponseEntity.ok(Map.of("message", "Da huy goi y ban thanh cong."));
    }

    /** POST /api/reservations/{id}/change-table */
    @PostMapping("/{id}/change-table")
    public ResponseEntity<ReservationResponse> changeTable(
            @PathVariable Long id,
            @Valid @RequestBody ChangeTableRequest req) {
        return ResponseEntity.ok(changeTableService.changeTable(id, req));
    }

    /** POST /api/reservations/{id}/check-in */
    @PostMapping("/{id}/check-in")
    public ResponseEntity<ReservationResponse> checkIn(@PathVariable Long id) {
        return ResponseEntity.ok(inHouseService.checkIn(id));
    }

    /** POST /api/reservations/{id}/check-out */
    @PostMapping("/{id}/check-out")
    public ResponseEntity<ReservationResponse> checkOut(@PathVariable Long id) {
        return ResponseEntity.ok(inHouseService.checkOut(id));
    }

}
