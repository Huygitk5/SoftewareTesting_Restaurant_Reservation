# `changeTable()`

```java
public ReservationResponse checkIn(Long id) {
    // [1] Gom toàn bộ phần khởi tạo và kiểm tra tồn tại đơn
    int durationMinutes = configProvider.getDurationMinutes(); [1]
    int gracePeriodMinutes = configProvider.getGracePeriodMinutes(); [1]
    Reservation reservation = reservationRepository.findById(id)
            .orElseThrow(() -> new ResourceNotFoundException("Đơn đặt bàn #" + id + " không tồn tại.")); [1]

    // [2] Kiểm tra trạng thái đơn
    if (reservation.getStatus() != RESERVED) [2] {
        throw new BadRequestException(
                "Đơn không ở trạng thái RESERVED. (Lưu ý: Đơn Walk-in hoặc đơn khách đã vào bàn sẽ không thể check-in lại)."); [3]
    }

    // [4] Gom khởi tạo các biến thời gian và danh sách bàn
    LocalDateTime now = LocalDateTime.now(); [4]
    LocalDateTime startTime = reservation.getStartTime(); [4]
    List<TableInfo> assignedTables = reservation.getTableMappings().stream()
            .map(ReservationTableMapping::getTableInfo)
            .toList(); [4]

    // [5] Kịch bản A: Quá grace period
    if (now.isAfter(startTime.plusMinutes(gracePeriodMinutes))) [5] {
        // [6] Gom các lệnh đánh dấu No-Show và ném ngoại lệ
        reservation.markNoShow(); [6]
        reservationRepository.save(reservation); [6]
        throw new BadRequestException("Đơn đặt bàn đã quá giờ giữ chỗ (" + gracePeriodMinutes
                + " phút). Đơn đã bị hủy theo chính sách No-Show."); [6]
    }

    // [7] Lệnh tính toán trạng thái bàn hiện tại
    boolean isOriginalTablesAvailable = assignedTables.stream()
            .allMatch(t -> t.getStatus() == TableStatus.AVAILABLE && !t.isSoftLocked()); [7]

    // [8] Kịch bản B & C
    if (!isOriginalTablesAvailable) [8] {
        boolean hasAlternativeTable = assignmentService.findAlternativeTables(reservation); [9]

        if (!hasAlternativeTable) [10] {
            if (now.isBefore(startTime)) [11] {
                throw new ConflictException(
                        "Bàn đặt trước hiện chưa trống và không có bàn thay thế phù hợp. Mời quý khách ngồi chờ ở khu vực Waitlist."); [12]
            } else {
                throw new ConflictException(
                        "OVERSTAY_CONFLICT: Bàn gốc đang bị khách ca trước ngồi quá giờ và không có bàn thay thế trống. Vui lòng sử dụng chức năng Override để xử lý."); [13]
            }
        } else {
            // [14] Gom các lệnh load lại danh sách bàn
            reservationRepository.flush(); [14]
            assignedTables = new ArrayList<>(reservation.getTableMappings().stream()
                    .map(ReservationTableMapping::getTableInfo)
                    .toList()); [14]
        }
    }

    // [15] Bắt đầu block check-in
    reservation.checkIn(); [15]
    
    if (now.isBefore(startTime)) [16] {
        reservation.setEndTime(now.plusMinutes(durationMinutes)); [17]
    } else {
        reservation.setEndTime(startTime.plusMinutes(durationMinutes)); [18]
    }

    // [19] Vòng lặp cập nhật trạng thái bàn
    for (TableInfo t : assignedTables) [19] {
        // [20] Gom các lệnh xử lý lưu DB và phát sự kiện
        t.setStatus(TableStatus.OCCUPIED); [20]
        tableInfoRepository.save(t); [20]
        eventPublisher.publishEvent(
                new TableStatusChangedEvent(this, t.getTableId(), "OCCUPIED")); [20]
    }
    
    // [21] Kết thúc thành công
    return mapper.toResponse(reservationRepository.save(reservation)); [21]
}
