package com.hcmute.reservation.controller;

import com.hcmute.reservation.model.dto.waitlist.WaitlistRequest;
import com.hcmute.reservation.model.dto.waitlist.WaitlistResponse;
import com.hcmute.reservation.service.WaitlistService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/waitlist")
@RequiredArgsConstructor
public class WaitlistController {

    private final WaitlistService waitlistService;

    /** POST /api/waitlist */
    @PostMapping
    public ResponseEntity<WaitlistResponse> addToWaitlist(@Valid @RequestBody WaitlistRequest req) {
        return ResponseEntity.status(HttpStatus.CREATED).body(waitlistService.addToWaitlist(req));
    }

    /** GET /api/waitlist */
    @GetMapping
    public ResponseEntity<List<WaitlistResponse>> getWaitlist() {
        return ResponseEntity.ok(waitlistService.getWaitlist());
    }

    /** PATCH /api/waitlist/{id}/seat */
    @PatchMapping("/{id}/seat")
    public ResponseEntity<WaitlistResponse> markAsSeated(@PathVariable Long id) {
        return ResponseEntity.ok(waitlistService.markAsSeated(id));
    }

    /** PATCH /api/waitlist/{id}/missing */
    @PatchMapping("/{id}/missing")
    public ResponseEntity<WaitlistResponse> markAsMissing(@PathVariable Long id) {
        return ResponseEntity.ok(waitlistService.markAsMissing(id));
    }

    /** PATCH /api/waitlist/{id}/re-wait */
    @PatchMapping("/{id}/re-wait")
    public ResponseEntity<WaitlistResponse> revertToWaiting(@PathVariable Long id) {
        return ResponseEntity.ok(waitlistService.revertToWaiting(id));
    }

    /** PATCH /api/waitlist/{id}/skip */
    @PatchMapping("/{id}/skip")
    public ResponseEntity<WaitlistResponse> skipEntry(@PathVariable Long id) {
        return ResponseEntity.ok(waitlistService.skipWaitlistEntry(id));
    }

}
