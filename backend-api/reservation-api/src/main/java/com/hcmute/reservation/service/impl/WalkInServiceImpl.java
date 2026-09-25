package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.mapper.ReservationMapper;
import com.hcmute.reservation.model.dto.reservation.ReservationResponse;
import com.hcmute.reservation.model.dto.reservation.WalkInOptionResponse;
import com.hcmute.reservation.model.dto.reservation.WalkInRequest;
import com.hcmute.reservation.model.dto.reservation.WalkInSuggestionResponse;
import com.hcmute.reservation.model.entity.Customer;
import com.hcmute.reservation.model.entity.Reservation;
import com.hcmute.reservation.model.entity.ReservationTableMapping;
import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.model.enums.ReservationType;
import com.hcmute.reservation.model.enums.TableStatus;
import com.hcmute.reservation.repository.CustomerRepository;
import com.hcmute.reservation.repository.ReservationRepository;
import com.hcmute.reservation.repository.ReservationTableMappingRepository;
import com.hcmute.reservation.repository.TableInfoRepository;
import com.hcmute.reservation.service.ConfigProviderService;
import com.hcmute.reservation.service.TableReleaseService;
import com.hcmute.reservation.service.WalkInService;
import com.hcmute.reservation.strategy.TableCombinationAlgorithm;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.orm.ObjectOptimisticLockingFailureException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.*;
import java.util.stream.Collectors;

import static com.hcmute.reservation.model.enums.ReservationStatus.*;

@Slf4j
@Service
@RequiredArgsConstructor
public class WalkInServiceImpl implements WalkInService {

    private final ReservationRepository reservationRepository;
    private final TableInfoRepository tableInfoRepository;
    private final ReservationTableMappingRepository mappingRepository;
    private final CustomerRepository customerRepository;
    private final TableCombinationAlgorithm algorithm;
    private final ReservationMapper mapper;
    private final ConfigProviderService configProvider;
    private final TableReleaseService tableReleaseService;

    @Override
    @Transactional(readOnly = true)
    public WalkInOptionResponse getWalkInOptions(int guestCount) {
        int durationMinutes = configProvider.getDurationMinutes();
        int bufferMinutes = configProvider.getBufferMinutes();
        int maxCapacityOverflow = configProvider.getMaxCapacityOverflow();

        if (guestCount <= 0) throw new BadRequestException("So luong khach phai lon hon 0.");

        LocalDateTime now = LocalDateTime.now();
        LocalDateTime blockUntil = now.plusMinutes(durationMinutes + bufferMinutes);

        List<TableInfo> candidateTables = tableInfoRepository.findByStatusAndIsActiveTrue(TableStatus.AVAILABLE);
        Set<Long> occupiedTableIds = new HashSet<>(reservationRepository.findOccupiedTableIds(now, blockUntil));

        List<TableInfo> cleanTables = candidateTables.stream()
                .filter(table -> !occupiedTableIds.contains(table.getTableId()))
                .collect(Collectors.toList());
        List<TableInfo> partialTables = candidateTables.stream()
                .filter(table -> occupiedTableIds.contains(table.getTableId()))
                .collect(Collectors.toList());

        List<WalkInOptionResponse.TableOption> preferredOptions = new ArrayList<>();

        cleanTables.stream()
                .filter(table -> table.getCapacity() >= guestCount)
                .filter(table -> table.getCapacity() <= guestCount + maxCapacityOverflow)
                .sorted(Comparator.comparingInt(TableInfo::getCapacity).thenComparing(TableInfo::getTableId))
                .limit(5)
                .forEach(table -> preferredOptions.add(WalkInOptionResponse.TableOption.builder()
                        .tableIds(List.of(table.getTableId()))
                        .totalCapacity(table.getCapacity())
                        .type("FULL_AVAILABLE")
                        .availableUntil(null)
                        .build()));

        algorithm.findWalkInOptionCombinations(cleanTables, guestCount).stream()
                .filter(combo -> combo.size() > 1)
                .sorted(Comparator.<List<TableInfo>>comparingInt(List::size)
                        .thenComparingInt(combo -> combo.stream().mapToInt(TableInfo::getCapacity).sum()))
                .limit(5)
                .forEach(combo -> preferredOptions.add(buildWalkInOption(combo, "MERGE_AVAILABLE", null)));

        if (!preferredOptions.isEmpty()) {
            return WalkInOptionResponse.builder()
                    .groups(List.of(WalkInOptionResponse.OptionGroup.builder()
                            .groupName("Ưu tiên (Trống hoàn toàn)")
                            .options(preferredOptions)
                            .build()))
                    .build();
        }

        Map<Long, LocalDateTime> partialAvailableUntilByTableId = new HashMap<>();
        List<Long> partialTableIds = partialTables.stream().map(TableInfo::getTableId).collect(Collectors.toList());
        if (!partialTableIds.isEmpty()) {
            reservationRepository.findNextBookingForTables(partialTableIds, now)
                    .forEach(row -> partialAvailableUntilByTableId.put((Long) row[0], (LocalDateTime) row[1]));
        }

        List<WalkInOptionResponse.TableOption> fallbackOptions = new ArrayList<>();
        partialTables.stream()
                .filter(table -> table.getCapacity() >= guestCount && table.getCapacity() <= guestCount + maxCapacityOverflow)
                .sorted(Comparator.comparingInt(TableInfo::getCapacity).thenComparing(TableInfo::getTableId)).limit(5)
                .forEach(table -> {
                    LocalDateTime availableUntil = partialAvailableUntilByTableId.get(table.getTableId());
                    if (availableUntil != null) {
                        fallbackOptions.add(WalkInOptionResponse.TableOption.builder()
                                .tableIds(List.of(table.getTableId()))
                                .totalCapacity(table.getCapacity())
                                .type("PARTIAL_AVAILABLE")
                                .availableUntil(availableUntil)
                                .build());
                    }
                });
        Set<Long> partialTableIdSet = partialTables.stream().map(TableInfo::getTableId).collect(Collectors.toSet());
        List<TableInfo> cleanAndPartial = new ArrayList<>(cleanTables);
        cleanAndPartial.addAll(partialTables);

        List<List<TableInfo>> mixedCombinations = algorithm.findWalkInOptionCombinations(cleanAndPartial, guestCount);
        mixedCombinations.stream()
                .filter(combo -> combo.size() > 1)
                .filter(combo -> combo.stream().map(TableInfo::getTableId).anyMatch(partialTableIdSet::contains))
                .sorted(Comparator.<List<TableInfo>>comparingInt(List::size)
                        .thenComparingInt(combo -> combo.stream().mapToInt(TableInfo::getCapacity).sum()))
                .limit(5)
                .forEach(combo -> {
                    LocalDateTime availableUntil = combo.stream()
                            .map(TableInfo::getTableId)
                            .filter(partialTableIdSet::contains)
                            .map(partialAvailableUntilByTableId::get)
                            .filter(Objects::nonNull)
                            .min(LocalDateTime::compareTo)
                            .orElse(null);
                    if (availableUntil != null) {
                        fallbackOptions.add(buildWalkInOption(combo, "PARTIAL_MERGED_AVAILABLE", availableUntil));
                    }
                });

        return WalkInOptionResponse.builder()
                .groups(List.of(WalkInOptionResponse.OptionGroup.builder()
                        .groupName("Dự phòng (Vướng lịch sau)")
                        .options(fallbackOptions)
                        .build()))
                .build();
    }

    @Transactional
    public WalkInSuggestionResponse suggestWalkIn(WalkInRequest req) {
        int durationMinutes = configProvider.getDurationMinutes();
        int bufferMinutes = configProvider.getBufferMinutes();
        int softLockMinutes = configProvider.getSoftLockMinutes();

        LocalDateTime now = LocalDateTime.now();
        LocalDateTime defaultEnd = now.plusMinutes(durationMinutes);
        LocalDateTime checkEnd = req.getEndTime() != null ? req.getEndTime() : defaultEnd;
        LocalDateTime blockUntil = checkEnd.plusMinutes(bufferMinutes);

        // ── Chọn bàn theo 3 case ──────────────────────────────────────

        List<TableInfo> selectedTables = new ArrayList<>();
        String availabilityType = "FULL_AVAILABLE";
        Set<Long> occupiedIds = new HashSet<>(
                reservationRepository.findOccupiedTableIds(now, blockUntil));

        if (req.getTableId() != null && !req.getTableId().isEmpty()) {
            // Case A: Lễ tân chỉ định bàn cụ thể
            for (Long tableId : req.getTableId()) {
                TableInfo t = tableInfoRepository.findById(tableId)
                        .orElseThrow(() -> new ResourceNotFoundException("Ban #" + tableId + " khong ton tai."));
                if (t.getStatus() != TableStatus.AVAILABLE || t.isSoftLocked()) {
                    throw new ConflictException("Ban #" + tableId + " hien khong kha dung.");
                }
                selectedTables.add(t);
            }

            // Kiểm tra xem có CÓ BẤT KỲ bàn nào trong danh sách dính lịch tương lai không?
            boolean hasPartialTable = selectedTables.stream()
                    .anyMatch(t -> occupiedIds.contains(t.getTableId()));

            // Xác định availabilityType
            if (selectedTables.size() > 1) {
                availabilityType = hasPartialTable ? "PARTIAL_MERGED_AVAILABLE" : "MERGED_AVAILABLE";
            } else {
                availabilityType = hasPartialTable ? "PARTIAL_AVAILABLE" : "FULL_AVAILABLE";
            }

        } else if (req.isMergeTables()) {
            // Case B: Ghép bàn tự động
            // Lấy danh sách bàn trống, chỉ lấy các bàn nhỏ hơn guestCount để ghép
            List<TableInfo> availableMerge = tableInfoRepository
                    .findByStatusAndIsActiveTrue(TableStatus.AVAILABLE)
                    .stream()
                    .filter(t -> !t.isSoftLocked() && !occupiedIds.contains(t.getTableId()))
                    .filter(t -> t.getCapacity() < req.getGuestCount())
                    .collect(Collectors.toList());

            selectedTables = algorithm.findBestTableCombination(availableMerge, req.getGuestCount());

            if (selectedTables.isEmpty()) {
                throw new BadRequestException("Không có tổ hợp bàn ghép nào phù hợp (yêu cầu sức chứa từ "
                        + req.getGuestCount() + " đến " + (req.getGuestCount() + 2) + " chỗ).");
            }
            availabilityType = "MERGED_AVAILABLE";
        } else {
            // Case C: Hệ thống tự chọn — FULL ưu tiên, fallback merge, sau đó PARTIAL
            // 1. Tìm bàn đơn FULL_AVAILABLE (Giới hạn sức chứa +2)
            List<TableInfo> fullAvailable = tableInfoRepository
                    .findAvailableTablesForGuests(req.getGuestCount())
                    .stream()
                    .filter(t -> !occupiedIds.contains(t.getTableId()))
                    .filter(t -> t.getCapacity() <= req.getGuestCount() + 2)
                    .collect(Collectors.toList());

            if (!fullAvailable.isEmpty()) {
                selectedTables.add(fullAvailable.get(0));
                availabilityType = "FULL_AVAILABLE";
            } else {
                // 2. Thử ghép bàn (nếu không có bàn đơn thỏa mãn)
                List<TableInfo> allFree = tableInfoRepository
                        .findByStatusAndIsActiveTrue(TableStatus.AVAILABLE)
                        .stream()
                        .filter(t -> !t.isSoftLocked() && !occupiedIds.contains(t.getTableId()))
                        .filter(t -> t.getCapacity() < req.getGuestCount())
                        .collect(Collectors.toList());

                List<TableInfo> merged = algorithm.findBestTableCombination(allFree, req.getGuestCount());

                if (!merged.isEmpty()) {
                    selectedTables.addAll(merged);
                    availabilityType = "MERGED_AVAILABLE";
                } else {
                    // 3. Fallback: PARTIAL AVAILABLE (Bàn trống tạm thời, vướng lịch đặt sau)
                    List<TableInfo> partialAvailable = tableInfoRepository
                            .findAvailableTablesForGuests(req.getGuestCount())
                            .stream()
                            .filter(t -> t.getCapacity() <= req.getGuestCount() + 2)
                            .collect(Collectors.toList());
                    if (partialAvailable.isEmpty()) {
                        throw new BadRequestException("Nha hang hien da het ban trong phu hop.");
                    }
                    selectedTables.add(partialAvailable.get(0));
                    availabilityType = "PARTIAL_AVAILABLE";
                }
            }
        }

        // ── Tính endTime chính xác (kiểm tra booking kế tiếp) ─────────

        LocalDateTime calculatedEndTime = defaultEnd;
        if (req.getEndTime() != null) {
            calculatedEndTime = req.getEndTime();
        } else {
            LocalDateTime earliestNextBooking = null;
            for (TableInfo t : selectedTables) {
                List<Reservation> nextBookings = reservationRepository.findNextBookingForTable(t.getTableId(), now);
                if (!nextBookings.isEmpty()) {
                    LocalDateTime nextStart = nextBookings.get(0).getStartTime();
                    if (earliestNextBooking == null || nextStart.isBefore(earliestNextBooking)) {
                        earliestNextBooking = nextStart;
                    }
                }
            }
            if (earliestNextBooking != null) {
                LocalDateTime maxAllowed = earliestNextBooking.minusMinutes(bufferMinutes);
                if (maxAllowed.isBefore(calculatedEndTime)) {
                    calculatedEndTime = maxAllowed;
                }
            }
        }

        // ── Resolve customer (tùy chọn với walk-in) ───────────────────
        Customer customer = resolveWalkInCustomer(req);

        // ── Tạo Reservation tạm (CREATED) + soft-lock các bàn ─────────
        // Scheduler sẽ tự hủy nếu lễ tân không confirm trong softLockMinutes.
        Reservation reservation = Reservation.builder()
                .customer(customer)
                .type(ReservationType.WALK_IN)
                .guestCount(req.getGuestCount())
                .startTime(now)
                .endTime(calculatedEndTime)
                .status(CREATED)
                .note(req.getNote())
                .build();
        reservation = reservationRepository.save(reservation);

        List<ReservationTableMapping> mappings = new ArrayList<>();
        try {
            for (TableInfo t : selectedTables) {
                t.applySoftLock(reservation.getReservationId(), softLockMinutes);
                tableInfoRepository.saveAndFlush(t);

                ReservationTableMapping mapping = ReservationTableMapping.builder()
                        .reservation(reservation)
                        .tableInfo(t)
                        .build();
                mappings.add(mappingRepository.save(mapping));
            }
        } catch (ObjectOptimisticLockingFailureException e) {
            // Bàn vừa bị lễ tân khác cướp trong tích tắc — hủy reservation tạm
            reservation.setStatus(CANCELLED);
            reservationRepository.save(reservation);
            throw new ConflictException(
                    "Mot trong cac ban goi y vua duoc xep cho khach khac. Vui long thu lai.");
        }

        reservation.setTableMappings(mappings);

        // ── Build response ─────────────────────────────────────────────

        final String finalAvailabilityType = availabilityType;
        List<WalkInSuggestionResponse.SuggestedTable> suggestedTableDtos = selectedTables.stream()
                .map(t -> WalkInSuggestionResponse.SuggestedTable.builder()
                        .tableId(t.getTableId())
                        .capacity(t.getCapacity())
                        .availabilityType(finalAvailabilityType)
                        .build())
                .collect(Collectors.toList());

        return WalkInSuggestionResponse.builder()
                .suggestionId(reservation.getReservationId())
                .suggestedTables(suggestedTableDtos)
                .lockExpiresAt(LocalDateTime.now().plusMinutes(softLockMinutes))
                .guestCount(req.getGuestCount())
                .startTime(reservation.getStartTime())
                .endTime(reservation.getEndTime())
                .availabilityType(availabilityType)
                .build();
    }

    @Transactional
    public ReservationResponse confirmWalkIn(Long suggestionId) {
        int softLockMinutes = configProvider.getSoftLockMinutes();

        Reservation reservation = reservationRepository.findById(suggestionId)
                .orElseThrow(() -> new ResourceNotFoundException(
                        "Goi y #" + suggestionId + " khong ton tai hoac da het han."));

        // Guard: chỉ cho confirm đúng loại và đúng trạng thái
        if (reservation.getType() != ReservationType.WALK_IN) {
            throw new BadRequestException("Chi co the xac nhan goi y Walk-in qua endpoint nay.");
        }
        if (reservation.getStatus() == EXPIRED ||
                reservation.getStatus() == CANCELLED) {
            throw new BadRequestException(
                    "Goi y ban da het han hoac bi huy. Vui long tim ban lai.");
        }
        if (reservation.getStatus() != CREATED) {
            throw new BadRequestException(
                    "Trang thai khong hop le de xac nhan: " + reservation.getStatus());
        }

        // Lấy bàn đang soft-lock
        List<TableInfo> lockedTables = tableInfoRepository.findByLockedByReservationId(suggestionId);

        if (lockedTables.isEmpty()) {
            // Scheduler đã giải phóng soft-lock trước khi lễ tân bấm confirm
            reservation.setStatus(EXPIRED);
            reservationRepository.save(reservation);
            throw new ConflictException(
                    "Thoi gian giu ban da het (" + softLockMinutes + " phut). Vui long tim ban lai.");
        }

        // CREATED -> SEATED (walk-in không cần qua RESERVED)
        reservation.setStatus(SEATED);
        reservation = reservationRepository.save(reservation);

        try {
            for (TableInfo t : lockedTables) {
                t.setSoftLockUntil(null);
                t.setLockedByReservationId(null);
                t.setStatus(TableStatus.OCCUPIED);
                tableInfoRepository.saveAndFlush(t);
            }
        } catch (ObjectOptimisticLockingFailureException e) {
            throw new ConflictException(
                    "Ban vua bi thay doi boi giao dich khac. Vui long tai lai va thu lai.");
        }

        // Reload để toResponse có đủ tableMappings
        return mapper.toResponse(reservationRepository.findById(reservation.getReservationId())
                .orElse(reservation));
    }

    @Transactional
    public void cancelWalkInSuggestion(Long suggestionId) {
        Reservation reservation = reservationRepository.findById(suggestionId)
                .orElseThrow(() -> new ResourceNotFoundException(
                        "Goi y #" + suggestionId + " khong ton tai."));

        if (reservation.getType() != ReservationType.WALK_IN) {
            throw new BadRequestException("Chi co the huy goi y Walk-in qua endpoint nay.");
        }

        if (reservation.getStatus() == CREATED) {
            reservation.setStatus(CANCELLED);
            reservationRepository.save(reservation);
            tableReleaseService.releaseLockedTable(suggestionId);
        }
    }

    private WalkInOptionResponse.TableOption buildWalkInOption(List<TableInfo> tables,
                                                               String type,
                                                               LocalDateTime availableUntil) {
        List<Long> tableIds = tables.stream()
                .map(TableInfo::getTableId)
                .sorted()
                .collect(Collectors.toList());
        int totalCapacity = tables.stream()
                .mapToInt(TableInfo::getCapacity)
                .sum();

        return WalkInOptionResponse.TableOption.builder()
                .tableIds(tableIds)
                .totalCapacity(totalCapacity)
                .type(type)
                .availableUntil(availableUntil)
                .build();
    }

    private Customer resolveWalkInCustomer(WalkInRequest req) {
        if (req.getCustomerPhone() == null || req.getCustomerPhone().isBlank()) {
            return null;
        }
        return customerRepository.findByPhoneAndPasswordHashIsNull(req.getCustomerPhone())
                .orElseGet(() -> customerRepository.save(Customer.builder()
                        .name(req.getCustomerName() != null && !req.getCustomerName().isBlank()
                                ? req.getCustomerName()
                                : "Khach Walk-in")
                        .phone(req.getCustomerPhone())
                        .isVerified(true)
                        .build()));
    }
}
