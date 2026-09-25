package com.hcmute.reservation.model.entity;

import com.hcmute.reservation.model.enums.TableStatus;
import jakarta.persistence.*;
import lombok.*;
import java.time.LocalDateTime;
import java.util.List;

@Entity
@Table(name = "table_info")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class TableInfo {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "table_id")
    private Long tableId;

    @Column(name = "capacity", nullable = false)
    private int capacity;

    @Enumerated(EnumType.STRING)
    @Column(name = "status", nullable = false)
    @Builder.Default
    private TableStatus status = TableStatus.AVAILABLE;

    // Bàn có đang hoạt động không (Manager có thể tắt bàn)
    @Column(name = "is_active", nullable = false)
    @Builder.Default
    private Boolean isActive = Boolean.TRUE;

    // Optimistic Locking — JPA tự động tăng version mỗi lần UPDATE
    @Version
    @Column(name = "version", nullable = false)
    private int version;

    // Soft Lock: khóa tạm thời trong 5 phút khi khách chờ thanh toán
    @Column(name = "soft_lock_until")
    private LocalDateTime softLockUntil;

    @Column(name = "locked_by_reservation_id")
    private Long lockedByReservationId;

    // Quan hệ N-N với Reservation thông qua bảng trung gian
    @OneToMany(mappedBy = "tableInfo", fetch = FetchType.LAZY)
    private List<ReservationTableMapping> mappings;

    public boolean isSoftLocked() {
        return softLockUntil != null && LocalDateTime.now().isBefore(softLockUntil);
    }

    public void applySoftLock(Long reservationId, int minutes) {
        this.softLockUntil = LocalDateTime.now().plusMinutes(minutes);
        this.lockedByReservationId = reservationId;
        this.status = TableStatus.OCCUPIED;
    }

    public void releaseSoftLock() {
        this.softLockUntil = null;
        this.lockedByReservationId = null;
        this.status = TableStatus.AVAILABLE;
    }
}
