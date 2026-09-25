package com.hcmute.reservation.model.dto.waitlist;

import com.hcmute.reservation.model.enums.WaitlistStatus;
import lombok.Builder;
import lombok.Data;

import java.time.LocalDateTime;
import java.util.List;

@Data
@Builder
public class WaitlistResponse {
    private Long waitlistId;
    private String customerName;
    private String customerPhone;
    private int guestCount;
    private boolean allowShortSeating;
    private LocalDateTime joinedAt;
    private WaitlistStatus status;
    private boolean readyToSeat;
    private String seatingType;
    private LocalDateTime suggestedAvailableUntil;
    private List<Long> suggestedTableIds;
}
