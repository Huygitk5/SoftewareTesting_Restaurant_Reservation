package com.hcmute.reservation.security.impl;

import com.hcmute.reservation.security.IPasswordHasher;
import org.springframework.stereotype.Component;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.Base64;

@Component
public class SHA256PasswordHasher implements IPasswordHasher {

    @Override
    public String hash(String rawPassword) {
        try {
            MessageDigest digest = MessageDigest.getInstance("SHA-256");
            byte[] encodedHash = digest.digest(rawPassword.getBytes());
            return Base64.getEncoder().encodeToString(encodedHash);
        } catch (NoSuchAlgorithmException e) {
            throw new RuntimeException("Lỗi thuật toán mã hóa SHA-256", e);
        }
    }

    @Override
    public boolean matches(String rawPassword, String encodedPassword) {
        String hashedRaw = hash(rawPassword);
        return hashedRaw.equals(encodedPassword);
    }
}