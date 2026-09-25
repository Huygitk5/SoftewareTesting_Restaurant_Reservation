package com.hcmute.reservation.repository;

import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.ReservationType;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;

@Repository
public interface ReservationRepository extends JpaRepository<Reservation, Long> {

    List<Reservation> findByStatus(ReservationStatus status);

    List<Reservation> findByCustomer_CustomerIdOrderByStartTimeDesc(Long customerId);


    List<Reservation> findByStatusOrderByStartTimeAsc(ReservationStatus status);

    List<Reservation> findByStatusAndEndTimeBefore(ReservationStatus status, LocalDateTime endTime);

    @Query("SELECT r FROM Reservation r JOIN r.tableMappings m WHERE m.tableInfo.tableId = :tableId " +
            "AND r.status IN ('RESERVED','SEATED') AND r.startTime > :now ORDER BY r.startTime ASC")
    List<Reservation> findNextBookingForTable(@Param("tableId") Long tableId, @Param("now") LocalDateTime now);

    @Query("SELECT DISTINCT r FROM Reservation r LEFT JOIN FETCH r.tableMappings m " +
              "WHERE r.status = 'RESERVED' AND r.startTime BETWEEN :now AND :until " +
              "ORDER BY r.startTime ASC")
    List<Reservation> findUpcoming(@Param("now") LocalDateTime now, @Param("until") LocalDateTime until);

    // Lấy các đơn đang SEATED kèm theo bàn
    @Query("SELECT DISTINCT r FROM Reservation r LEFT JOIN FETCH r.tableMappings m " +
            "WHERE r.status = :status ORDER BY r.startTime ASC")
    List<Reservation> findByStatusWithTables(@Param("status") ReservationStatus status);

    // Lấy các đơn của 1 khách hàng đang giao nhau với khoảng thời gian [startTime, endTime]
    @Query("SELECT r FROM Reservation r WHERE r.customer.customerId = :customerId " +
            "AND r.status IN ('CREATED', 'PENDING_PAYMENT', 'RESERVED', 'SEATED') " +
            "AND r.startTime < :endTime AND r.endTime > :startTime")
    List<Reservation> findOverlappingByCustomerId(@Param("customerId") Long customerId,
                                                  @Param("startTime") LocalDateTime startTime,
                                                  @Param("endTime") LocalDateTime endTime);

    @Query("SELECT r FROM Reservation r WHERE r.status = 'PENDING_PAYMENT' AND r.createdAt < :expiredBefore")
    List<Reservation> findExpiredPendingPayments(@Param("expiredBefore") LocalDateTime expiredBefore);

    @Query("SELECT r FROM Reservation r WHERE r.type = 'ONLINE' " +
            "AND r.status = 'RESERVED' AND r.startTime < :graceCutoff")
    List<Reservation> findNoShows(@Param("graceCutoff") LocalDateTime graceCutoff);

    @Query("SELECT r FROM Reservation r WHERE r.status = 'SEATED' AND r.endTime < :now")
    List<Reservation> findOverstayed(@Param("now") LocalDateTime now);

    // Group by day
    @Query("SELECT DATE(r.startTime), " +
            "SUM(CASE WHEN r.type = 'ONLINE' AND r.status IN ('RESERVED','SEATED','COMPLETED','NO_SHOW') THEN 1 ELSE 0 END), " +
            "SUM(CASE WHEN r.type = 'WALK_IN' AND r.status IN ('SEATED','COMPLETED') THEN 1 ELSE 0 END), " +
            "SUM(CASE WHEN r.status = 'NO_SHOW' THEN 1 ELSE 0 END) " +
            "FROM Reservation r " +
            "WHERE r.startTime BETWEEN :from AND :to " +
            "GROUP BY DATE(r.startTime) ORDER BY DATE(r.startTime)")
    List<Object[]> countScheduledByDate(@Param("from") LocalDateTime from, @Param("to") LocalDateTime to);

    // Group by month
    @Query("SELECT YEAR(r.startTime), MONTH(r.startTime), " +
            "SUM(CASE WHEN r.type = 'ONLINE' AND r.status IN ('RESERVED','SEATED','COMPLETED','NO_SHOW') THEN 1 ELSE 0 END), " +
            "SUM(CASE WHEN r.type = 'WALK_IN' AND r.status IN ('SEATED','COMPLETED') THEN 1 ELSE 0 END), " +
            "SUM(CASE WHEN r.status = 'NO_SHOW' THEN 1 ELSE 0 END) " +
            "FROM Reservation r " +
            "WHERE r.startTime BETWEEN :from AND :to " +
            "GROUP BY YEAR(r.startTime), MONTH(r.startTime) " +
            "ORDER BY YEAR(r.startTime), MONTH(r.startTime)")
    List<Object[]> countScheduledByMonth(@Param("from") LocalDateTime from, @Param("to") LocalDateTime to);

    // Group by year
    @Query("SELECT YEAR(r.startTime), " +
            "SUM(CASE WHEN r.type = 'ONLINE' AND r.status IN ('RESERVED','SEATED','COMPLETED','NO_SHOW') THEN 1 ELSE 0 END), " +
            "SUM(CASE WHEN r.type = 'WALK_IN' AND r.status IN ('SEATED','COMPLETED') THEN 1 ELSE 0 END), " +
            "SUM(CASE WHEN r.status = 'NO_SHOW' THEN 1 ELSE 0 END) " +
            "FROM Reservation r " +
            "WHERE r.startTime BETWEEN :from AND :to " +
            "GROUP BY YEAR(r.startTime) " +
            "ORDER BY YEAR(r.startTime)")
    List<Object[]> countScheduledByYear(@Param("from") LocalDateTime from, @Param("to") LocalDateTime to);

    @Query("SELECT COUNT(r) FROM Reservation r " +
            "WHERE (:type IS NULL OR r.type = :type) " +
            "AND (:#{#statuses == null} = true OR r.status IN :statuses) " +
            "AND r.startTime BETWEEN :from AND :to")
    long countReservations(@Param("from") LocalDateTime from,
                           @Param("to") LocalDateTime to,
                           @Param("type") ReservationType type,
                           @Param("statuses") List<ReservationStatus> statuses);

    @Query("SELECT DISTINCT m.tableInfo.tableId FROM ReservationTableMapping m " +
           "WHERE m.reservation.status IN ('CREATED','PENDING_PAYMENT','RESERVED','SEATED') " +
           "AND m.reservation.startTime < :endTime AND m.reservation.endTime > :startTime")
    List<Long> findOccupiedTableIds(@Param("startTime") LocalDateTime startTime,
                                    @Param("endTime") LocalDateTime endTime);

    @Query("SELECT DISTINCT r FROM Reservation r JOIN FETCH r.tableMappings m " +
            "WHERE r.status = 'COMPLETED' AND r.endTime < :now " +
            "AND m.tableInfo.status IN ('OCCUPIED', 'OVERSTAY')")
    List<Reservation> findCompletedWithReleasableTables(@Param("now") LocalDateTime now);

    @Query("SELECT m.tableInfo.tableId, MIN(r.startTime) " +
            "FROM Reservation r JOIN r.tableMappings m " +
            "WHERE m.tableInfo.tableId IN :tableIds " +
            "AND r.status IN ('RESERVED','SEATED') " +
            "AND r.startTime > :now " +
            "GROUP BY m.tableInfo.tableId")
    List<Object[]> findNextBookingForTables(@Param("tableIds") List<Long> tableIds, @Param("now") LocalDateTime now);

    @Query("SELECT r FROM Reservation r WHERE r.status = 'RESERVED' " +
            "AND r.startTime BETWEEN :now AND :windowEnd")
    List<Reservation> findUpcomingReservationsForAlert(@Param("now") LocalDateTime now,
                                                       @Param("windowEnd") LocalDateTime windowEnd);
}