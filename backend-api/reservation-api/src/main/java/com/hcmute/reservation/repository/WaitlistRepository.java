package com.hcmute.reservation.repository;

import com.hcmute.reservation.model.entity.Waitlist;
import com.hcmute.reservation.model.enums.WaitlistStatus;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface WaitlistRepository extends JpaRepository<Waitlist, Long> {

    List<Waitlist> findByStatusOrderByJoinedAtAsc(WaitlistStatus status);

    // Kiểm tra khách đã được đưa vào waitlist chưa (tránh đưa vào 2 lần)
    @Query("SELECT COUNT(w) > 0 FROM Waitlist w WHERE w.customer.phone = :phone AND w.status = :status")
    boolean existsByCustomerPhoneAndStatus(@Param("phone") String phone, @Param("status") WaitlistStatus status);

    List<Waitlist> findByStatusInOrderByJoinedAtAsc(List<WaitlistStatus> statuses);
}
