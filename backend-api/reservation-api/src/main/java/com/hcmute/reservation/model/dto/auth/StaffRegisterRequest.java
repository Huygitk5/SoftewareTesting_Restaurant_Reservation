package com.hcmute.reservation.model.dto.auth;

import com.hcmute.reservation.model.enums.UserRole;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;
import lombok.Data;

@Data
public class StaffRegisterRequest {
    @NotBlank
    private String username;

    @NotBlank
    @Size(min = 6, message = "Mật khẩu phải có ít nhất 6 ký tự")
    private String password;

    @NotNull
    private UserRole role;
}