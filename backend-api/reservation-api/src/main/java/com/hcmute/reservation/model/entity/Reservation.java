package com.hcmute.reservation.model.entity;

import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.ReservationType;
import jakarta.persistence.*;
import lombok.*;
import java.time.LocalDateTime;
import java.util.List;

@Entity
@Table(name = "reservation")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class Reservation {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "reservation_id")
    private Long reservationId;

    // Nullable: Walk-in vãng lai không bắt buộc có customer
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "customer_id", nullable = true)
    private Customer customer;

    // Nullable: Online booking không cần account nhân viên
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "account_id", nullable = true)
    private Account account;

    @Enumerated(EnumType.STRING)
    @Column(name = "type", nullable = false)
    private ReservationType type;

    @Column(name = "guest_count", nullable = false)
    private int guestCount;

    @Column(name = "start_time", nullable = false)
    private LocalDateTime startTime;

    // end_time = start_time + 120p (Duration) + 10p (Buffer) — mục 3.2.2
    @Column(name = "end_time", nullable = false)
    private LocalDateTime endTime;

    //Tien coc
    @Column(name = "deposit_amount")
    private Double depositAmount;

    // Vòng đời: CREATED → PENDING_PAYMENT → RESERVED → SEATED → COMPLETED
    //                                     ↘ EXPIRED / CANCELLED
    //                                                  ↘ NO_SHOW
    @Enumerated(EnumType.STRING)
    @Column(name = "status", nullable = false)
    @Builder.Default
    private ReservationStatus status = ReservationStatus.CREATED;

    // Nullable — ghi chú tùy chọn của khách
    @Column(name = "note")
    private String note;

    @Column(name = "created_at", nullable = false, updatable = false)
    private LocalDateTime createdAt;

    // Quan hệ 1-N với bảng trung gian (Ghép bàn)
    @OneToMany(mappedBy = "reservation", cascade = CascadeType.ALL, fetch = FetchType.LAZY)
    private List<ReservationTableMapping> tableMappings;

    // Quan hệ 1-0..1 với Override_Log
    @OneToOne(mappedBy = "reservation", fetch = FetchType.LAZY)
    private OverrideLog overrideLog;

    @PrePersist
    private void prePersist() {
        this.createdAt = LocalDateTime.now();
    }

    // ── Business methods ──────────────────────────────────────────

    // Mục 3.4.1: Check-in → SEATED
    public void checkIn() {
        this.status = ReservationStatus.SEATED;
        this.startTime = LocalDateTime.now();
    }

    // Mục 3.4.4: Check-out → COMPLETED
    public void checkOut() {
        this.status = ReservationStatus.COMPLETED;
        this.endTime = LocalDateTime.now();
    }

    // Mục 3.2.7 / 3.4.4: Hủy đơn → CANCELLED
    public void cancel() {
        this.status = ReservationStatus.CANCELLED;
    }

    // Mục 3.4.2: Quá Grace Period → NO_SHOW
    public void markNoShow() {
        this.status = ReservationStatus.NO_SHOW;
    }

    // Mục 3.2.7: Quá 5 phút chưa thanh toán → EXPIRED
    public void markExpired() {
        this.status = ReservationStatus.EXPIRED;
    }

    // Mục 3.4.3: Kiểm tra Overstay
    public boolean isOverstay() {
        return LocalDateTime.now().isAfter(this.endTime)
                && this.status == ReservationStatus.SEATED;
    }
}