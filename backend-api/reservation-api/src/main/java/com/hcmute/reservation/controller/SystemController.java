package com.hcmute.reservation.controller;

import com.hcmute.reservation.service.SystemSchedulerService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.Map;

@RestController
@RequestMapping("/api/system")
@RequiredArgsConstructor
public class SystemController {

    private final SystemSchedulerService schedulerService;

    /** POST /api/system/expire-reservations */
    @PostMapping("/expire-reservations")
    public ResponseEntity<Map<String, Object>> expireReservations() {
        return ResponseEntity.ok(schedulerService.runSpecificJob("expire-reservations"));
    }

    /** POST /api/system/release-no-show */
    @PostMapping("/release-no-show")
    public ResponseEntity<Map<String, Object>> releaseNoShow() {
        return ResponseEntity.ok(schedulerService.runSpecificJob("release-no-show"));
    }

    /** POST /api/system/check-overstay */
    @PostMapping("/check-overstay")
    public ResponseEntity<Map<String, Object>> checkOverstay() {
        return ResponseEntity.ok(schedulerService.runSpecificJob("check-overstay"));
    }

    /** POST /api/system/release-completed */
    @PostMapping("/release-completed")
    public ResponseEntity<Map<String, Object>> releaseCompleted() {
        return ResponseEntity.ok(schedulerService.runSpecificJob("release-completed"));
    }

    @PostMapping("/run-all-jobs")
    public ResponseEntity<Map<String, Object>> runAllJobs() {
        return ResponseEntity.ok(schedulerService.runAllJobs());
    }
}
