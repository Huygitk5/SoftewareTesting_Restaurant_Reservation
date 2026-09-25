# `changeTable()`

```java
public ReservationResponse changeTable(Long reservationId, ChangeTableRequest req) {

    int bufferMinutes = configProvider.getBufferMinutes(); [1]

    Reservation reservation = reservationRepository.findById(reservationId)
            .orElseThrow(() -> new ResourceNotFoundException(
                    "Đơn #" + reservationId + " không tồn tại."
            )); [1]

    // Tách điều kiện && thành 2 node
    if (reservation.getStatus() != SEATED [2]
            && reservation.getStatus() != RESERVED [3]) {

        throw new BadRequestException(
                "Chỉ có thể đổi bàn khi đơn ở trạng thái RESERVED hoặc SEATED..."
        ); [4]
    }

    // Tách riêng từng dòng khai báo thành 1 Node
    int oldTotalCapacity = reservation.getTableMappings()
            .stream()
            .mapToInt(m -> m.getTableInfo().getCapacity())
            .sum(); [5]

    int newTotalCapacity = 0; [5]

    Set<Long> currentTableIds = reservation.getTableMappings()
            .stream()
            .map(m -> m.getTableInfo().getTableId())
            .collect(Collectors.toSet()); [5]

    List<TableInfo> newTables = new ArrayList<>(); [5]

    // Vòng lặp 1
    for (Long tableId : req.getTableIds()) [6] {

        TableInfo table = tableInfoRepository.findById(tableId)
                .orElseThrow(() -> new ResourceNotFoundException(
                        "Bàn #" + tableId + " không tồn tại."
                )); [7]

        if (!table.getIsActive() [8]) {

            throw new BadRequestException(
                    "Bàn #" + tableId + " đang bị vô hiệu hóa."
            ); [9]
        }

        newTotalCapacity += table.getCapacity(); [10]

        if (!currentTableIds.contains(tableId) [11]) {

            if (table.isSoftLocked() [12]) {

                throw new ConflictException(
                        "Bàn #" + tableId
                        + " đang được giữ tạm cho giao dịch thanh toán khác."
                ); [13]
            }

            if (table.getStatus() != TableStatus.AVAILABLE [14]) {

                throw new ConflictException(
                        "Bàn #" + tableId + " hiện không trống..."
                ); [15]
            }

            // Tách riêng từng dòng khai báo bên trong if
            LocalDateTime start = reservation.getStartTime(); [16]

            LocalDateTime end = reservation.getEndTime()
                    .plusMinutes(bufferMinutes); [16]

            List<Long> overlappingReservationTableIds =
                    reservationRepository.findOccupiedTableIds(start, end); [16]

            overlappingReservationTableIds.removeAll(currentTableIds); [16]

            if (overlappingReservationTableIds.contains(tableId) [17]) {

                throw new ConflictException(
                        "Bàn #" + tableId
                        + " đã có lịch đặt trùng trong khung giờ này."
                ); [18]
            }
        }

        newTables.add(table); [19]
    }

    if (newTotalCapacity > reservation.getGuestCount() + 2 [20]) {

        throw new BadRequestException(
                "Không thể đổi bàn! Chỉ được phép chuyển sang bàn lớn hơn tối đa 2 chỗ..."
        ); [21]
    }

    if (reservation.getTableMappings() != null [22]) {

        // Vòng lặp 2
        for (ReservationTableMapping mapping :
                reservation.getTableMappings()) [23] {

            TableInfo oldTable = mapping.getTableInfo(); [24]

            boolean isKeptTable =
                    req.getTableIds().contains(oldTable.getTableId()); [24]

            if (!isKeptTable [25]) {

                oldTable.setStatus(TableStatus.AVAILABLE); [26]

                tableInfoRepository.save(oldTable); [26]

                eventPublisher.publishEvent(
                        new TableStatusChangedEvent(
                                this,
                                oldTable.getTableId(),
                                "AVAILABLE"
                        )
                ); [26]
            }
        }

        mappingRepository.deleteAll(reservation.getTableMappings()); [27]

        reservation.getTableMappings().clear(); [27]
    }

    List<ReservationTableMapping> newMappings =
            new ArrayList<>(); [28]

    try {

        // Vòng lặp 3
        for (TableInfo table : newTables) [29] {

            if (reservation.getStatus() == SEATED [30]) {

                table.setStatus(TableStatus.OCCUPIED); [31]

                eventPublisher.publishEvent(
                        new TableStatusChangedEvent(
                                this,
                                table.getTableId(),
                                "OCCUPIED"
                        )
                ); [31]
            }

            tableInfoRepository.saveAndFlush(table); [32]

            ReservationTableMapping mapping =
                    ReservationTableMapping.builder()
                            .reservation(reservation)
                            .tableInfo(table)
                            .build(); [32]

            newMappings.add(
                    mappingRepository.save(mapping)
            ); [32]
        }

    } catch (ObjectOptimisticLockingFailureException e) [33] {

        throw new ConflictException(
                "Một trong các bàn vừa bị thay đổi bởi giao dịch khác..."
        ); [34]
    }

    reservation.setTableMappings(newMappings); [35]

    return mapper.toResponse(
            reservationRepository.save(reservation)
    ); [35]
}   