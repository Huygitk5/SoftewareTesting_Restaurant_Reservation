package com.hcmute.reservation.security;

public interface IPasswordHasher {
    /**
     * Mã hóa mật khẩu gốc.
     */
    String hash(String rawPassword);

    /**
     * Kiểm tra mật khẩu gốc có khớp với mật khẩu đã mã hóa hay không.
     */
    boolean matches(String rawPassword, String encodedPassword);
}