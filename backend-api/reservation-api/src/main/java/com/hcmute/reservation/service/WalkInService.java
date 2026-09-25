package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.reservation.ReservationResponse;
import com.hcmute.reservation.model.dto.reservation.WalkInOptionResponse;
import com.hcmute.reservation.model.dto.reservation.WalkInRequest;
import com.hcmute.reservation.model.dto.reservation.WalkInSuggestionResponse;

public interface WalkInService {
    WalkInOptionResponse getWalkInOptions(int guestCount);
    WalkInSuggestionResponse suggestWalkIn(WalkInRequest req);
    ReservationResponse confirmWalkIn(Long suggestionId);
    void cancelWalkInSuggestion(Long suggestionId);
}
