package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.model.dto.auth.StaffLoginRequest;
import com.hcmute.reservation.model.dto.auth.StaffLoginResponse;
import com.hcmute.reservation.model.dto.auth.StaffRegisterRequest;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.UnauthorizedException;
import com.hcmute.reservation.model.entity.Account;
import com.hcmute.reservation.repository.AccountRepository;
import com.hcmute.reservation.security.IPasswordHasher;
import com.hcmute.reservation.service.StaffAuthService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.UUID;

@Service
@RequiredArgsConstructor
public class StaffAuthServiceImpl implements StaffAuthService {

    private final AccountRepository accountRepository;
    private final IPasswordHasher passwordHasher;

    @Override
    @Transactional
    public StaffLoginResponse login(StaffLoginRequest req) {
        Account account = accountRepository.findByUsername(req.getUsername())
                .orElseThrow(() -> new UnauthorizedException("Tên đăng nhập hoặc mật khẩu không đúng."));

        if (!passwordHasher.matches(req.getPassword(), account.getPasswordHash())) {
            throw new UnauthorizedException("Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        // Tạo session token (có hiệu lực 8 giờ — 1 ca làm việc)
        String sessionToken = UUID.randomUUID().toString();
        account.setSessionToken(sessionToken);
        account.setSessionExpiresAt(LocalDateTime.now().plusHours(8));
        accountRepository.save(account);

        return new StaffLoginResponse(sessionToken, account.getAccountId(),
                account.getUsername(), account.getRole().name());
    }

    @Override
    @Transactional
    public void logout(String token) {
        if (token == null || token.isBlank()) return;
        // Tìm tài khoản dựa trên session token đang hoạt động
        accountRepository.findBySessionToken(token).ifPresent(account -> {
            account.setSessionToken(null);
            account.setSessionExpiresAt(null);
            accountRepository.save(account);
        });
    }

    @Override
    @Transactional
    public String register(StaffRegisterRequest req) {
        // Kiểm tra xem username đã tồn tại chưa
        if (accountRepository.findByUsername(req.getUsername()).isPresent()) {
            throw new ConflictException("Tên đăng nhập đã tồn tại trong hệ thống.");
        }

        // Tạo tài khoản mới, mã hóa mật khẩu trước khi lưu
        Account newAccount = Account.builder()
                .username(req.getUsername())
                .passwordHash(passwordHasher.hash(req.getPassword()))
                .role(req.getRole())
                .build();

        accountRepository.save(newAccount);
        return "Tạo tài khoản " + req.getRole().name() + " thành công!";
    }
}
