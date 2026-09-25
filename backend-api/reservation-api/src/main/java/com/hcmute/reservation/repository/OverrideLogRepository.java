package com.hcmute.reservation.repository;

import com.hcmute.reservation.model.entity.OverrideLog;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;

@Repository
public interface OverrideLogRepository extends JpaRepository<OverrideLog, Long> {

    @Query("SELECT o FROM OverrideLog o WHERE " +
           "(:from IS NULL OR o.createdAt >= :from) AND " +
           "(:to IS NULL OR o.createdAt <= :to) AND " +
           "(:accountId IS NULL OR o.account.accountId = :accountId) " +
           "ORDER BY o.createdAt DESC")
    List<OverrideLog> findByFilters(@Param("from") LocalDateTime from,
                                   @Param("to") LocalDateTime to,
                                   @Param("accountId") Long accountId);
}
