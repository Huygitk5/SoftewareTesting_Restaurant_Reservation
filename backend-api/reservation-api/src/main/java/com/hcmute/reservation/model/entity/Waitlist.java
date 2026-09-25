package com.hcmute.reservation.model.entity;

import com.hcmute.reservation.model.enums.WaitlistStatus;
import jakarta.persistence.*;
import lombok.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "waitlist")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class Waitlist {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "waitlist_id")
    private Long waitlistId;

    // Bắt buộc có customer để lễ tân gọi tên (mục 3.3.5)
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "customer_id", nullable = false)
    private Customer customer;

    @Column(name = "guest_count", nullable = false)
    private int guestCount;

    @Column(name = "allow_short_seating", nullable = false)
    @Builder.Default
    private boolean allowShortSeating = false;

    @Column(name = "joined_at", nullable = false, updatable = false)
    private LocalDateTime joinedAt;

    @Enumerated(EnumType.STRING)
    @Column(name = "status", nullable = false)
    @Builder.Default
    private WaitlistStatus status = WaitlistStatus.WAITING;

    @PrePersist
    private void prePersist() {
        this.joinedAt = LocalDateTime.now();
    }

    // Gọi khi có bàn trống và khách đồng ý → SEATED
    public void seat() {
        this.status = WaitlistStatus.SEATED;
    }

    // Bỏ qua khách (không liên lạc được / từ chối) → SKIPPED
    public void skip() {
        this.status = WaitlistStatus.SKIPPED;
    }

    public void markAsMissing() {
        this.status = WaitlistStatus.MISSING;
    }

    // Khách quay lại sau khi bị đánh dấu vắng mặt → WAITING
    public void revertToWaiting() { this.status = WaitlistStatus.WAITING; }
}