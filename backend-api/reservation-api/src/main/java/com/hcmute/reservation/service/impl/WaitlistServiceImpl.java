package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.ResourceNotFoundException;
import com.hcmute.reservation.model.dto.table.AvailableWindowResponse;
import com.hcmute.reservation.model.dto.waitlist.WaitlistRequest;
import com.hcmute.reservation.model.dto.waitlist.WaitlistResponse;
import com.hcmute.reservation.model.entity.Customer;
import com.hcmute.reservation.model.entity.Waitlist;
import com.hcmute.reservation.model.enums.WaitlistStatus;
import com.hcmute.reservation.repository.CustomerRepository;
import com.hcmute.reservation.repository.WaitlistRepository;
import com.hcmute.reservation.service.AvailabilityApiService;
import com.hcmute.reservation.service.WaitlistService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

@Service
@RequiredArgsConstructor

public class WaitlistServiceImpl implements WaitlistService {

    private final WaitlistRepository waitlistRepository;
    private final CustomerRepository customerRepository;
    private final AvailabilityApiService availabilityApiService;

    @Override
    @Transactional
    public WaitlistResponse addToWaitlist(WaitlistRequest req) {
        boolean alreadyWaiting = waitlistRepository
                .existsByCustomerPhoneAndStatus(req.getPhone(), WaitlistStatus.WAITING);
        if (alreadyWaiting) {
            throw new ConflictException("Số điện thoại " + req.getPhone() + " đã có trong danh sách chờ.");
        }

        ensureWaitlistIsNeeded(req.getGuestCount(), req.isAllowShortSeating());

        Customer customer = resolveWalkInCustomer(req);

        Waitlist entry = Waitlist.builder()
                .customer(customer)
                .guestCount(req.getGuestCount())
                .allowShortSeating(req.isAllowShortSeating())
                .status(WaitlistStatus.WAITING)
                .build();

        return toResponse(waitlistRepository.save(entry), null);
    }

    @Override
    @Transactional(readOnly = true)
    public List<WaitlistResponse> getWaitlist() {
        LocalDateTime assessmentTime = LocalDateTime.now();
        Set<Long> consumedTableIds = new HashSet<>();
        List<WaitlistResponse> responses = new ArrayList<>();

        List<Waitlist> waitingList = waitlistRepository.findByStatusInOrderByJoinedAtAsc(
                List.of(WaitlistStatus.WAITING, WaitlistStatus.MISSING)
        );
        for (Waitlist entry : waitingList) {
            AvailableWindowResponse suggestion = findBestWindow(entry, assessmentTime, consumedTableIds);
            if (suggestion != null) {
                consumedTableIds.addAll(extractSuggestedTableIds(suggestion));
            }
            responses.add(toResponse(entry, suggestion));
        }

        return responses;
    }

    @Override
    @Transactional
    public WaitlistResponse markAsSeated(Long id) {
        Waitlist entry = getWaitlistOrThrow(id);

        if (entry.getStatus() != WaitlistStatus.WAITING && entry.getStatus() != WaitlistStatus.MISSING) {
            throw new BadRequestException("Chỉ có thể xác nhận vào bàn cho khách đang ở trạng thái WAITING hoặc MISSING.");
        }

        entry.seat();

        return toResponse(waitlistRepository.save(entry), null);
    }

    @Override
    @Transactional
    public WaitlistResponse skipWaitlistEntry(Long id) {
        Waitlist entry = getWaitlistOrThrow(id);

        if (entry.getStatus() != WaitlistStatus.WAITING && entry.getStatus() != WaitlistStatus.MISSING) {
            throw new BadRequestException("Chỉ có thể hủy xếp bàn cho khách đang ở trạng thái WAITING hoặc MISSING.");
        }
        entry.skip();
        return toResponse(waitlistRepository.save(entry), null);
    }

    @Override
    @Transactional
    public WaitlistResponse markAsMissing(Long id) {
        Waitlist entry = getWaitlistOrThrow(id);

        if (entry.getStatus() != WaitlistStatus.WAITING) {
            throw new BadRequestException("Chỉ có thể đánh dấu vắng mặt cho khách đang ở trạng thái WAITING.");
        }

        entry.markAsMissing();

        return toResponse(waitlistRepository.save(entry), null);
    }

    @Override
    @Transactional
    public WaitlistResponse revertToWaiting(Long id) {
        Waitlist entry = getWaitlistOrThrow(id);

        if (entry.getStatus() != WaitlistStatus.MISSING) {
            throw new BadRequestException("Chỉ có thể đưa khách quay lại hàng đợi khi khách đang ở trạng thái MISSING.");
        }

        entry.revertToWaiting();

        return toResponse(waitlistRepository.save(entry), null);
    }

    private Waitlist getWaitlistOrThrow(Long id) {
        return waitlistRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Hàng đợi #" + id + " không tồn tại."));
    }

    private void ensureWaitlistIsNeeded(int guestCount, boolean allowShortSeating) {
        if (hasImmediateSeatingOption(guestCount, allowShortSeating, LocalDateTime.now())) {
            throw new BadRequestException("Hiện vẫn còn phương án xếp bàn ngay cho nhóm khách này, không nên đưa vào waitlist.");
        }
    }

    private Customer resolveWalkInCustomer(WaitlistRequest req) {
        return customerRepository.findByPhoneAndPasswordHashIsNull(req.getPhone())
                .map(existing -> {
                    if (req.getName() != null && !req.getName().isBlank() && !req.getName().equals(existing.getName())) {
                        existing.setName(req.getName());
                        return customerRepository.save(existing);
                    }
                    return existing;
                })
                .orElseGet(() -> customerRepository.save(Customer.builder()
                        .name(req.getName())
                        .phone(req.getPhone())
                        .isVerified(true)
                        .build()));
    }

    private boolean hasImmediateSeatingOption(int guestCount, boolean allowShortSeating, LocalDateTime assessmentTime) {
        return availabilityApiService.getAvailableWindows(guestCount, assessmentTime).stream()
                .anyMatch(window -> isWindowAllowed(window, allowShortSeating));
    }

    private AvailableWindowResponse findBestWindow(Waitlist waitlist, LocalDateTime assessmentTime, Set<Long> consumedTableIds) {
        return availabilityApiService.getAvailableWindows(waitlist.getGuestCount(), assessmentTime).stream()
                .filter(window -> isWindowAllowed(window, waitlist.isAllowShortSeating()))
                .filter(window -> consumedTableIds.stream().noneMatch(extractSuggestedTableIds(window)::contains))
                .findFirst()
                .orElse(null);
    }

    private boolean isWindowAllowed(AvailableWindowResponse window, boolean allowShortSeating) {
        return window.getAvailability() == AvailableWindowResponse.Availability.FULL_AVAILABLE || allowShortSeating;
    }

    private List<Long> extractSuggestedTableIds(AvailableWindowResponse window) {
        if (window.getMergeCandidateIds() != null && !window.getMergeCandidateIds().isEmpty()) {
            return window.getMergeCandidateIds();
        }
        if (window.getTableId() != null) {
            return List.of(window.getTableId());
        }
        return List.of();
    }

    private WaitlistResponse toResponse(Waitlist waitlist, AvailableWindowResponse suggestion) {
        WaitlistResponse.WaitlistResponseBuilder builder = WaitlistResponse.builder()
                .waitlistId(waitlist.getWaitlistId())
                .customerName(waitlist.getCustomer().getName())
                .customerPhone(waitlist.getCustomer().getPhone())
                .guestCount(waitlist.getGuestCount())
                .allowShortSeating(waitlist.isAllowShortSeating())
                .joinedAt(waitlist.getJoinedAt())
                .status(waitlist.getStatus());

        if (suggestion != null) {
            builder.readyToSeat(true)
                    .seatingType(suggestion.getAvailability().name())
                    .suggestedAvailableUntil(suggestion.getAvailableUntil())
                    .suggestedTableIds(extractSuggestedTableIds(suggestion));
        }

        return builder.build();
    }
}
