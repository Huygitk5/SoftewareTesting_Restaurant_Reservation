package com.hcmute.reservation.repository;

import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.TableStatus;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;

@Repository
public interface TableInfoRepository extends JpaRepository<TableInfo, Long> {

    List<TableInfo> findByIsActiveTrue();

    List<TableInfo> findByStatusAndIsActiveTrue(TableStatus status);

    // Tìm bàn AVAILABLE có đủ sức chứa, không bị soft-lock, lấy bàn có sức chứa nhỏ nhất trước
    @Query("SELECT t FROM TableInfo t WHERE t.status = 'AVAILABLE' AND t.isActive = true " +
            "AND t.capacity >= :guests AND (t.softLockUntil IS NULL OR t.softLockUntil < CURRENT_TIMESTAMP) " +
            "ORDER BY t.capacity ASC")
    List<TableInfo> findAvailableTablesForGuests(@Param("guests") int guests);

    // Tìm bàn đang bị soft-lock bởi một reservation
    @Query("SELECT t FROM TableInfo t WHERE t.lockedByReservationId = :reservationId")
    List<TableInfo> findByLockedByReservationId(@Param("reservationId") Long reservationId);

    @Query("SELECT DISTINCT m.tableInfo FROM Reservation r JOIN r.tableMappings m " +
            "WHERE r.status = :status AND r.startTime >= :startOfDay AND r.startTime <= :endOfDay " +
            "AND m.tableInfo.isActive = true")
    List<TableInfo> findTablesByReservationStatusAndDate(
            @Param("status") ReservationStatus status,
            @Param("startOfDay") LocalDateTime startOfDay,
            @Param("endOfDay") LocalDateTime endOfDay);

    // Đếm xem bàn này có đang bị khách khác NGỒI (SEATED) ngay lúc này không
    @Query("SELECT COUNT(m) FROM ReservationTableMapping m " +
            "WHERE m.tableInfo.tableId = :tableId " +
            "AND m.reservation.reservationId != :excludeResId " +
            "AND m.reservation.status = 'SEATED'")
    int countOtherSeated(Long tableId, Long excludeResId);
}
