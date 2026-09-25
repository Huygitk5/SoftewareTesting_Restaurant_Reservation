package com.hcmute.reservation.model.dto.auth;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;
import java.time.LocalDate;

@Data
@AllArgsConstructor
@NoArgsConstructor
@Builder
public class LoginResponse {
    private String token;
    private Long customerId;
    private String name;
    private String email;
    private String phone;
    private String gender;
    private LocalDate dateOfBirth;
}
