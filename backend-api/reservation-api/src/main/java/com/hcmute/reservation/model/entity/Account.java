package com.hcmute.reservation.model.entity;

import com.hcmute.reservation.model.enums.UserRole;
import jakarta.persistence.*;
import lombok.*;
import java.time.LocalDateTime;
import java.util.List;

@Entity
@Table(name = "account")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class Account {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "account_id")
    private Long accountId;

    @Column(name = "username", nullable = false, unique = true)
    private String username;

    @Column(name = "password_hash", nullable = false)
    private String passwordHash;

    @Enumerated(EnumType.STRING)
    @Column(name = "role", nullable = false)
    private UserRole role;

    // Session token cho WinForm POS (khác với JWT customer)
    @Column(name = "session_token", length = 512)
    private String sessionToken;

    @Column(name = "session_expires_at")
    private LocalDateTime sessionExpiresAt;

    // Lễ tân xử lý các đơn walk-in hoặc check-in/check-out
    @OneToMany(mappedBy = "account", fetch = FetchType.LAZY)
    private List<Reservation> reservations;

    // Lễ tân thực hiện ghi đè xung đột (mục 3.5.2)
    @OneToMany(mappedBy = "account", fetch = FetchType.LAZY)
    private List<OverrideLog> overrideLogs;
}