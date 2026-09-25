package com.hcmute.reservation.repository;

import com.hcmute.reservation.model.entity.Customer;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface CustomerRepository extends JpaRepository<Customer, Long> {
    Optional<Customer> findByEmail(String email);
    Optional<Customer> findByVerificationToken(String token);
    Optional<Customer> findByResetToken(String resetToken);
    boolean existsByEmail(String email);
    Optional<Customer> findByPhoneAndPasswordHashIsNull(String phone);
}

