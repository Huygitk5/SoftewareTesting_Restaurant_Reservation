package com.hcmute.reservation.controller;

import com.hcmute.reservation.model.dto.report.DailyReservationReport;
import com.hcmute.reservation.model.dto.report.NoShowRateResponse;
import com.hcmute.reservation.service.ReportService;
import lombok.RequiredArgsConstructor;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.time.Year;
import java.time.YearMonth;
import java.util.List;

@RestController
@RequestMapping("/api/reports")
@RequiredArgsConstructor
public class ReportController {

    private final ReportService reportService;

    /** GET /api/reports/reservations-by-date?from=2024-01-01&to=2024-01-31 */
    @GetMapping("/reservations-by-date")
    public ResponseEntity<List<DailyReservationReport>> getReservationsByDate(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate from,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate to) {
        return ResponseEntity.ok(reportService.getReservationsByDate(from, to));
    }

    /** GET /api/reports/reservations-by-month?from=2024-01&to=2024-06 */
    @GetMapping("/reservations-by-month")
    public ResponseEntity<List<DailyReservationReport>> getReservationsByMonth(
            @RequestParam String from,
            @RequestParam String to) {
        return ResponseEntity.ok(reportService.getReservationsByMonth(
                YearMonth.parse(from), YearMonth.parse(to)));
    }

    /** GET /api/reports/reservations-by-year?from=2023&to=2025 */
    @GetMapping("/reservations-by-year")
    public ResponseEntity<List<DailyReservationReport>> getReservationsByYear(
            @RequestParam String from,
            @RequestParam String to) {
        return ResponseEntity.ok(reportService.getReservationsByYear(
                Year.parse(from), Year.parse(to)));
    }

    /** GET /api/reports/no-show-rate?from=2024-01-01&to=2024-01-31 */
    @GetMapping("/no-show-rate")
    public ResponseEntity<NoShowRateResponse> getNoShowRate(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate from,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate to) {
        return ResponseEntity.ok(reportService.getNoShowRate(from, to));
    }

    /** GET /api/reports/no-show-rate-by-month?from=2024-01&to=2024-06 */
    @GetMapping("/no-show-rate-by-month")
    public ResponseEntity<NoShowRateResponse> getNoShowRateByMonth(
            @RequestParam String from,
            @RequestParam String to) {
        return ResponseEntity.ok(reportService.getNoShowRateByMonth(
                YearMonth.parse(from), YearMonth.parse(to)));
    }

    /** GET /api/reports/no-show-rate-by-year?from=2023&to=2025 */
    @GetMapping("/no-show-rate-by-year")
    public ResponseEntity<NoShowRateResponse> getNoShowRateByYear(
            @RequestParam String from,
            @RequestParam String to) {
        return ResponseEntity.ok(reportService.getNoShowRateByYear(
                Year.parse(from), Year.parse(to)));
    }
}
