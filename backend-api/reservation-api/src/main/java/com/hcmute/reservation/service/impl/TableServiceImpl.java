package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.dto.table.FloorMapTableResponse;
import com.hcmute.reservation.model.dto.table.TableRequest;
import com.hcmute.reservation.model.dto.table.TableResponse;
import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.ReservationStatus;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.TableInfoRepository;
import com.hcmute.reservation.service.TableService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor

public class TableServiceImpl implements  TableService {

    private final TableInfoRepository tableInfoRepository;

    @Override
    @Transactional(readOnly = true)
    public List<TableResponse> getAllTables() {
        return tableInfoRepository.findAll().stream()
                .map(this::toResponse).collect(Collectors.toList());
    }

    @Override
    @Transactional
    public TableResponse createTable(TableRequest req) {
        rejectManualStatusChange(req);
        validateCapacity(req.getCapacity());

        TableInfo table = TableInfo.builder()
                .capacity(req.getCapacity())
                .isActive(req.getIsActive() != null ? req.getIsActive() : true)
                .status(TableStatus.AVAILABLE)
                .build();
        return toResponse(tableInfoRepository.save(table));
    }

    @Override
    @Transactional
    public TableResponse updateTable(Long id, TableRequest req) {
        rejectManualStatusChange(req);
        if (req.getVersion() == null) {
            throw new BadRequestException("Thiếu version của bàn. Vui lòng tải lại dữ liệu trước khi cập nhật.");
        }

        TableInfo table = findTableOrThrow(id);
        validateCapacity(req.getCapacity());

        // Optimistic Locking
        if (req.getVersion().intValue() != table.getVersion()) {
            throw new ConflictException("Dữ liệu bàn đã bị thay đổi bởi một giao dịch khác. Vui lòng tải lại.");
        }

        // Logic giảm sức chứa
        if (req.getCapacity() < table.getCapacity()) {
            if (table.isSoftLocked()) {
                throw new BadRequestException("Không thể giảm sức chứa khi bàn đang được giữ tạm cho một giao dịch thanh toán.");
            }
            checkCapacityConflictWithActiveReservations(table, req.getCapacity());
        }

        // Logic vô hiệu hóa bàn (Deactive)
        if (req.getIsActive() != null && !req.getIsActive() && table.getIsActive()) {
            if (hasActiveOrBlockingReservations(table) || table.isSoftLocked()) {
                throw new BadRequestException("Không thể vô hiệu hóa bàn đang được khóa tạm, có đơn đặt chỗ hoặc đang phục vụ.");
            }
        }

        table.setCapacity(req.getCapacity());
        if (req.getIsActive() != null) table.setIsActive(req.getIsActive());

        return toResponse(tableInfoRepository.save(table));
    }

    @Override
    @Transactional
    public void deleteTable(Long id) {
        TableInfo table = findTableOrThrow(id);

        // Chặn ngay nếu bàn đang Soft Locked
        if (table.isSoftLocked()) {
            throw new BadRequestException("Không thể xóa bàn đang được khóa tạm chờ khách thanh toán.");
        }

        if (hasActiveOrBlockingReservations(table) || table.getStatus() != TableStatus.AVAILABLE) {
            throw new BadRequestException("Không thể xóa bàn đang có đơn đặt chỗ hoặc không ở trạng thái trống.");
        }

        table.setIsActive(false);
        tableInfoRepository.save(table);
    }

    // Lấy trạng thái bàn hiện tại của tất cả bàn đang hoạt động trong nhà hàng
    @Override
    @Transactional(readOnly = true)
    public List<FloorMapTableResponse> getFloorMap() {
        LocalDateTime now = LocalDateTime.now();
        LocalDateTime endOfDay = now.toLocalDate().atTime(LocalTime.MAX);

        // chỉ lấy các bàn active
        return tableInfoRepository.findByIsActiveTrue().stream()
                .map(table -> buildFloorMapResponse(table, now, endOfDay))
                .collect(Collectors.toList());
    }

    @Override
    @Transactional(readOnly = true)
    public List<TableResponse> getReservedTablesToday() {
        LocalDateTime startOfDay = LocalDate.now().atStartOfDay();
        LocalDateTime endOfDay = LocalDate.now().atTime(LocalTime.MAX);

        return tableInfoRepository.findTablesByReservationStatusAndDate(
                        ReservationStatus.RESERVED, startOfDay, endOfDay).stream()
                .map(this::toResponse)
                .collect(Collectors.toList());
    }

    private TableInfo findTableOrThrow(Long id) {
        return tableInfoRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Bàn #" + id + " không tồn tại."));
    }

    private void rejectManualStatusChange(TableRequest req) {
        if (req.getStatus() != null) {
            throw new BadRequestException("Trạng thái bàn được hệ thống quản lý tự động, không thể cập nhật thủ công qua API này.");
        }
    }

    private void validateCapacity(Integer capacity) {
        if (capacity == null || capacity <= 0) {
            throw new BadRequestException("Sức chứa bàn phải lớn hơn 0.");
        }
    }

    private boolean isBlockingReservationStatus(ReservationStatus status) {
        return status == ReservationStatus.CREATED
                || status == ReservationStatus.PENDING_PAYMENT
                || status == ReservationStatus.RESERVED
                || status == ReservationStatus.SEATED;
    }

    /**
     * Tái sử dụng (DRY): Kiểm tra xem bàn này có đơn đặt chỗ nào đang chặn/đang ngồi không.
     */
    private boolean hasActiveOrBlockingReservations(TableInfo table) {
        if (table.getMappings() == null) return false;
        return table.getMappings().stream()
                .filter(m -> m != null && m.getReservation() != null)
                .anyMatch(m -> isBlockingReservationStatus(m.getReservation().getStatus()));
    }

    private void checkCapacityConflictWithActiveReservations(TableInfo table, int newCapacityRequest) {
        if (table.getMappings() == null) return;

        boolean hasConflict = table.getMappings().stream()
                .filter(m -> m != null && m.getReservation() != null)
                .anyMatch(m -> {
                    Reservation res = m.getReservation();
                    if (!isBlockingReservationStatus(res.getStatus())) return false;

                    int currentTotalCapacity = res.getTableMappings().stream()
                            .mapToInt(mapping -> mapping.getTableInfo().getCapacity())
                            .sum();
                    int newTotalCapacity = currentTotalCapacity - table.getCapacity() + newCapacityRequest;

                    return res.getGuestCount() > newTotalCapacity;
                });

        if (hasConflict) {
            throw new BadRequestException("Không thể giảm sức chứa do bàn đang có đơn đặt trước vượt quá tổng số lượng này.");
        }
    }

    /**
     * Tách logic phức tạp của hàm getFloorMap ra đây. Xóa bỏ biến resTime ở level class.
     */
    private FloorMapTableResponse buildFloorMapResponse(TableInfo t, LocalDateTime now, LocalDateTime endOfDay) {
        String customerName = null;
        Long resId = null;
        TableStatus currentStatus = t.getStatus();
        ReservationStatus currentResStatus = null;
        LocalDateTime resTime = null; // Biến cục bộ an toàn cho đa luồng
        LocalDateTime nextResTime = null;

        if (t.getMappings() != null && !t.getMappings().isEmpty()) {
            var safeMappings = t.getMappings().stream()
                    .filter(m -> m != null && m.getReservation() != null)
                    .collect(Collectors.toList());

            var seatedMapping = safeMappings.stream()
                    .filter(m -> m.getReservation().getStatus() == ReservationStatus.SEATED)
                    .findFirst();

            var reservedMapping = safeMappings.stream()
                    .filter(m -> m.getReservation().getStatus() == ReservationStatus.RESERVED)
                    .filter(m -> m.getReservation().getStartTime().isAfter(now) && m.getReservation().getStartTime().isBefore(endOfDay))
                    .min(Comparator.comparing(m -> m.getReservation().getStartTime()));

            // lấy ra giờ bắt đầu của đơn tiếp theo
            if (reservedMapping.isPresent()) {
                nextResTime = reservedMapping.get().getReservation().getStartTime();
            }

            var activeMapping = seatedMapping.isPresent() ? seatedMapping : reservedMapping;

            if (activeMapping.isPresent()) {
                Reservation res = activeMapping.get().getReservation();
                resId = res.getReservationId();
                currentResStatus = res.getStatus();
                resTime = res.getStartTime(); // Cập nhật biến cục bộ

                if (res.getCustomer() != null) {
                    customerName = res.getCustomer().getName();
                }
                if (res.getStatus() == ReservationStatus.SEATED) {
                    currentStatus = (res.getEndTime() != null && now.isAfter(res.getEndTime())) ? TableStatus.OVERSTAY : TableStatus.OCCUPIED;
                }
            }
        }

        return FloorMapTableResponse.builder()
                .tableId(t.getTableId())
                .capacity(t.getCapacity())
                .status(currentStatus)
                .isActive(t.getIsActive())
                .currentReservationId(resId)
                .currentReservationStatus(currentResStatus)
                .currentCustomerName(customerName)
                .currentReservationTime(resTime) // Dùng biến cục bộ
                .nextReservationTime(nextResTime)
                .build();
    }

    private TableResponse toResponse(TableInfo t) {
        return TableResponse.builder()
                .tableId(t.getTableId())
                .capacity(t.getCapacity())
                .status(t.getStatus())
                .isActive(t.getIsActive())
                .version(t.getVersion())
                .build();
    }

}
