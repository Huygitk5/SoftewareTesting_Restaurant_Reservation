package com.hcmute.reservation.model.entity;

import jakarta.persistence.*;
import lombok.*;

@Entity
@Table(name = "reservation_table_mapping")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class ReservationTableMapping {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "mapping_id")
    private Long mappingId;

    // Bảng trung gian giải quyết quan hệ N-N cho nghiệp vụ Ghép bàn (mục 3.2.4)
    // 1 Reservation có thể chiếm nhiều bàn, 1 bàn phục vụ nhiều lượt qua các khung giờ

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "reservation_id", nullable = false)
    private Reservation reservation;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "table_id", nullable = false)
    private TableInfo tableInfo;
}