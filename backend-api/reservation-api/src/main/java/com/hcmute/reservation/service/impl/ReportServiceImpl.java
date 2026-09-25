package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.model.dto.report.DailyReservationReport;
import com.hcmute.reservation.model.dto.report.NoShowRateResponse;
import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.model.enums.ReservationType;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.service.ReportService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.sql.Date;
import java.time.*;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import static com.hcmute.reservation.model.enums.ReservationStatus.*;

@Service
@RequiredArgsConstructor
public class ReportServiceImpl implements ReportService {

    private final ReservationRepository reservationRepository;

    @Override
    public List<DailyReservationReport> getReservationsByDate(LocalDate from, LocalDate to) {
        validateDateRange(from, to);

        LocalDateTime fromDt = from.atStartOfDay();
        LocalDateTime toDt = to.atTime(LocalTime.MAX);

        Map<LocalDate, DailyReservationReport> reportByDate = reservationRepository
                .countScheduledByDate(fromDt, toDt).stream()
                .collect(Collectors.toMap(
                        row -> toLocalDate(row[0]),
                        row -> buildReport(toLocalDate(row[0]).toString(), row, 1, 2, 3),
                        (a, b) -> a,
                        LinkedHashMap::new
                ));

        return Stream.iterate(from, date -> !date.isAfter(to), date -> date.plusDays(1))
                .map(date -> reportByDate.getOrDefault(date,
                        new DailyReservationReport(date.toString(), 0L, 0L, 0L, 0L)))
                .collect(Collectors.toList());
    }

    @Override
    public List<DailyReservationReport> getReservationsByMonth(YearMonth from, YearMonth to) {
        if (from.isAfter(to)) {
            throw new BadRequestException("Tháng bắt đầu không được nằm sau tháng kết thúc.");
        }

        LocalDateTime fromDt = from.atDay(1).atStartOfDay();
        LocalDateTime toDt = to.atEndOfMonth().atTime(LocalTime.MAX);

        Map<YearMonth, DailyReservationReport> reportByMonth = reservationRepository
                .countScheduledByMonth(fromDt, toDt).stream()
                .collect(Collectors.toMap(
                        row -> YearMonth.of(((Number) row[0]).intValue(), ((Number) row[1]).intValue()),
                        row -> buildReport(
                                YearMonth.of(((Number) row[0]).intValue(),
                                        ((Number) row[1]).intValue()).toString(),
                                row, 2, 3, 4
                        ),
                        (a, b) -> a,
                        LinkedHashMap::new
                ));

        // Điền đầy đủ các tháng trong khoảng, kể cả tháng không có đơn nào
        return Stream.iterate(from, m -> !m.isAfter(to), m -> m.plusMonths(1))
                .map(month -> reportByMonth.getOrDefault(month,
                        new DailyReservationReport(month.toString(), 0L, 0L, 0L, 0L)))
                .collect(Collectors.toList());
    }

    @Override
    public List<DailyReservationReport> getReservationsByYear(Year from, Year to) {
        if (from.isAfter(to)) {
            throw new BadRequestException("Năm bắt đầu không được nằm sau năm kết thúc.");
        }

        LocalDateTime fromDt = from.atDay(1).atStartOfDay();
        LocalDateTime toDt = to.atMonthDay(MonthDay.of(12, 31)).atTime(LocalTime.MAX);

        Map<Year, DailyReservationReport> reportByYear = reservationRepository
                .countScheduledByYear(fromDt, toDt).stream()
                .collect(Collectors.toMap(
                        row -> Year.of(((Number) row[0]).intValue()),
                        row -> buildReport(
                                Year.of(((Number) row[0]).intValue()).toString(),
                                row, 1, 2, 3
                        ),
                        (a, b) -> a,
                        LinkedHashMap::new
                ));

        return Stream.iterate(from, y -> !y.isAfter(to), y -> y.plusYears(1))
                .map(year -> reportByYear.getOrDefault(year,
                        new DailyReservationReport(year.toString(), 0L, 0L, 0L, 0L)))
                .collect(Collectors.toList());
    }

    // Helper tái sử dụng để build report từ Object[] — tránh DRY
    private DailyReservationReport buildReport(String label, Object[] row,
                                               int onlineIdx, int walkInIdx, int noShowIdx) {
        long totalOnline = ((Number) row[onlineIdx]).longValue();
        long totalWalkIn = ((Number) row[walkInIdx]).longValue();
        long noShowCount = ((Number) row[noShowIdx]).longValue();
        return new DailyReservationReport(label, totalOnline, totalWalkIn,
                totalOnline + totalWalkIn, noShowCount);
    }

    @Override
    public NoShowRateResponse getNoShowRate(LocalDate from, LocalDate to) {
        validateDateRange(from, to);
        return buildNoShowRate(from.atStartOfDay(), to.atTime(LocalTime.MAX));
    }

    @Override
    public NoShowRateResponse getNoShowRateByMonth(YearMonth from, YearMonth to) {
        if (from.isAfter(to)) {
            throw new BadRequestException("Tháng bắt đầu không được nằm sau tháng kết thúc.");
        }
        return buildNoShowRate(from.atDay(1).atStartOfDay(),
                to.atEndOfMonth().atTime(LocalTime.MAX));
    }

    @Override
    public NoShowRateResponse getNoShowRateByYear(Year from, Year to) {
        if (from.isAfter(to)) {
            throw new BadRequestException("Năm bắt đầu không được nằm sau năm kết thúc.");
        }
        return buildNoShowRate(from.atDay(1).atStartOfDay(),
                to.atMonthDay(MonthDay.of(12, 31)).atTime(LocalTime.MAX));
    }

    // Helper dùng chung cho cả 3 — tránh lặp code (DRY)
    private NoShowRateResponse buildNoShowRate(LocalDateTime fromDt, LocalDateTime toDt) {
        long totalOnline = reservationRepository.countReservations(
                fromDt, toDt,
                ReservationType.ONLINE,
                List.of(RESERVED, SEATED, COMPLETED, NO_SHOW));

        long totalWalkIn = reservationRepository.countReservations(
                fromDt, toDt,
                ReservationType.WALK_IN,
                List.of(SEATED, COMPLETED));

        long noShows = reservationRepository.countReservations(
                fromDt, toDt,
                ReservationType.ONLINE,
                List.of(NO_SHOW));

        double rate = totalOnline == 0 ? 0.0
                : Math.round((double) noShows / totalOnline * 10000.0) / 100.0;

        return new NoShowRateResponse(totalOnline, totalWalkIn,
                totalOnline + totalWalkIn, noShows, rate);
    }

    private void validateDateRange(LocalDate from, LocalDate to) {
        if (from.isAfter(to)) {
            throw new BadRequestException("Ngày bắt đầu không được nằm sau ngày kết thúc.");
        }
    }

    private LocalDate toLocalDate(Object value) {
        if (value instanceof LocalDate localDate) {
            return localDate;
        }
        if (value instanceof LocalDateTime localDateTime) {
            return localDateTime.toLocalDate();
        }
        if (value instanceof Date sqlDate) {
            return sqlDate.toLocalDate();
        }
        return LocalDate.parse(value.toString());
    }
}
