package com.hcmute.reservation.repository;

import com.hcmute.reservation.model.entity.ReservationTableMapping;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface ReservationTableMappingRepository extends JpaRepository<ReservationTableMapping, Long> {

    List<ReservationTableMapping> findByReservation_ReservationId(Long reservationId);

    @Query("SELECT m FROM ReservationTableMapping m WHERE m.tableInfo.tableId = :tableId " +
           "AND m.reservation.status IN ('RESERVED','SEATED')")
    List<ReservationTableMapping> findActiveByTableId(@Param("tableId") Long tableId);
}
