package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.auth.StaffLoginRequest;
import com.hcmute.reservation.model.dto.auth.StaffLoginResponse;
import com.hcmute.reservation.model.dto.auth.StaffRegisterRequest;

public interface StaffAuthService {
    StaffLoginResponse login(StaffLoginRequest req);

    void logout(String token);

    String register(StaffRegisterRequest req);
}