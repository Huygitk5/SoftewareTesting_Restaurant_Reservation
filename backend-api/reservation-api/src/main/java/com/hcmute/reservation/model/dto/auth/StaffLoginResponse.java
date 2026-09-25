package com.hcmute.reservation.model.dto.auth;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class StaffLoginResponse {
    private String sessionToken;
    private Long accountId;
    private String username;
    private String role;
}
