package com.hcmute.reservation.security.impl;

import com.hcmute.reservation.security.IPasswordHasher;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.context.annotation.Primary;
import org.springframework.stereotype.Component;

@Component
@Primary // thuật toán ưu tiên được Spring inject
public class BCryptPasswordHasher implements IPasswordHasher {

    private final PasswordEncoder delegate = new BCryptPasswordEncoder();

    @Override
    public String hash(String rawPassword) {
        return delegate.encode(rawPassword);
    }

    @Override
    public boolean matches(String rawPassword, String encodedPassword) {
        return delegate.matches(rawPassword, encodedPassword);
    }
}