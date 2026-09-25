package com.hcmute.reservation.service.impl;

import com.hcmute.reservation.exception.BadRequestException;
import com.hcmute.reservation.exception.ConflictException;
import com.hcmute.reservation.exception.UnauthorizedException;
import com.hcmute.reservation.model.dto.auth.*;
import com.hcmute.reservation.model.entity.Customer;
import com.hcmute.reservation.repository.CustomerRepository;
import com.hcmute.reservation.security.IPasswordHasher;
import com.hcmute.reservation.service.AuthService;
import com.hcmute.reservation.service.EmailService;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.time.LocalDateTime;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;

@Service
@RequiredArgsConstructor
public class AuthServiceImpl implements AuthService {

    private final CustomerRepository customerRepository;
    private final IPasswordHasher passwordHasher;
    private final EmailService emailService;

    private final Map<String, Boolean> tokenApprovalMap = new ConcurrentHashMap<>();
    private final Map<String, Boolean> verifyApprovalMap = new ConcurrentHashMap<>();

    @Value("${app.base-url}")
    private String baseUrl;

    @Override
    @Transactional
    public String register(RegisterRequest req) {
        String verificationToken = UUID.randomUUID().toString();
        LocalDateTime expiresAt = LocalDateTime.now().plusMinutes(30);

        return customerRepository.findByEmail(req.getEmail())
                .map(existing -> handleExistingUnverifiedCustomer(existing, req, verificationToken, expiresAt))
                .orElseGet(() -> createNewCustomer(req, verificationToken, expiresAt));
    }

    @Override
    @Transactional
    public void resendVerification(String email) {
        Customer customer = customerRepository.findByEmail(email)
                .orElseThrow(() -> new BadRequestException("Không tìm thấy người dùng với email này."));
        if (customer.getIsVerified()) {
            throw new BadRequestException("Tài khoản này đã được xác minh.");
        }

        String newToken = UUID.randomUUID().toString();
        customer.setVerificationToken(newToken);
        customer.setVerificationTokenExpiresAt(LocalDateTime.now().plusMinutes(30));
        customerRepository.save(customer);

        emailService.sendVerificationEmail(email, newToken);
    }

    @Override
    @Transactional
    public String verifyEmail(String token) {
        Customer customer = customerRepository.findByVerificationToken(token)
                .orElseThrow(() -> new BadRequestException("Liên kết xác minh không tồn tại."));

        if (customer.getVerificationTokenExpiresAt() != null &&
                customer.getVerificationTokenExpiresAt().isBefore(LocalDateTime.now())) {
            throw new BadRequestException("Liên kết xác minh đã hết hạn. Vui lòng yêu cầu mã mới.");
        }

        // Handle Email Change Flow
        if (customer.getPendingEmail() != null) {
            customer.setEmail(customer.getPendingEmail());
            customer.setPendingEmail(null);
        }

        customer.setIsVerified(true);
        customer.setVerificationToken(null);
        customer.setVerificationTokenExpiresAt(null);
        customerRepository.save(customer);
        return token;
    }

    @Override
    public LoginResponse login(LoginRequest req) {
        Customer customer = customerRepository.findByEmail(req.getEmail())
                .orElseThrow(() -> new UnauthorizedException("Email hoặc mật khẩu không đúng."));

        checkVerificationStatus(customer);

        if (!passwordHasher.matches(req.getPassword(), customer.getPasswordHash())) {
            throw new UnauthorizedException("Email hoặc mật khẩu không đúng.");
        }
        return mapToLoginResponse(customer);
    }

    @Override
    @Transactional
    public LoginResponse updateProfile(Long customerId, CustomerProfileUpdateRequest req) {
        Customer customer = customerRepository.findById(customerId)
                .orElseThrow(() -> new BadRequestException("Không tìm thấy người dùng."));

        // Handle Email Change
        if (!req.getEmail().equalsIgnoreCase(customer.getEmail())) {
            // Check if email already taken
            if (customerRepository.findByEmail(req.getEmail()).isPresent()) {
                throw new ConflictException("Email này đã được sử dụng bởi tài khoản khác.");
            }

            String verificationToken = UUID.randomUUID().toString();
            customer.setPendingEmail(req.getEmail());
            customer.setVerificationToken(verificationToken);
            customer.setVerificationTokenExpiresAt(LocalDateTime.now().plusMinutes(30));

            // Send Verification to NEW email
            emailService.sendVerificationEmail(req.getEmail(), verificationToken);
            // Send Alert to OLD email
            emailService.sendEmailChangeAlert(customer.getEmail(), req.getEmail());
        }

        customer.setName(req.getName());
        customer.setPhone(req.getPhone());
        customer.setGender(req.getGender());
        customer.setDateOfBirth(req.getDateOfBirth());

        customerRepository.save(customer);
        return mapToLoginResponse(customer);
    }

    @Override
    @Transactional
    public void changePassword(Long customerId, ChangePasswordRequest req) {
        Customer customer = customerRepository.findById(customerId)
                .orElseThrow(() -> new BadRequestException("Không tìm thấy người dùng."));

        if (!passwordHasher.matches(req.getCurrentPassword(), customer.getPasswordHash())) {
            throw new UnauthorizedException("Mật khẩu hiện tại không chính xác.");
        }

        customer.setPasswordHash(passwordHasher.hash(req.getNewPassword()));
        customerRepository.save(customer);
    }

    @Override
    @Transactional
    public String forgotPassword(ForgotPasswordRequest req) {
        String token = UUID.randomUUID().toString();
        customerRepository.findByEmail(req.getEmail()).ifPresent(customer -> {
            customer.setResetToken(token);
            customer.setResetTokenExpiresAt(LocalDateTime.now().plusMinutes(10));
            customerRepository.save(customer);
            emailService.sendResetPasswordEmail(req.getEmail(), token);
        });
        return token;
    }

    @Override
    @Transactional
    public void resetPassword(ResetPasswordRequest req) {
        Customer customer = customerRepository.findByResetToken(req.getToken())
                .orElseThrow(() -> new BadRequestException("Token đặt lại mật khẩu không hợp lệ."));

        if (customer.getResetTokenExpiresAt() == null ||
                customer.getResetTokenExpiresAt().isBefore(LocalDateTime.now())) {
            throw new BadRequestException("Token đã hết hạn. Vui lòng yêu cầu lại.");
        }

        customer.setPasswordHash(passwordHasher.hash(req.getNewPassword()));
        customer.setResetToken(null);
        customer.setResetTokenExpiresAt(null);
        customerRepository.save(customer);
    }

    @Override
    public void approveVerification(String token) {
        verifyApprovalMap.put(token, true);
    }

    @Override
    public boolean isVerificationApproved(String token) {
        return verifyApprovalMap.getOrDefault(token, false);
    }

    @Override
    public void removeVerificationApproval(String token) {
        verifyApprovalMap.remove(token);
    }

    @Override
    public boolean validateResetToken(String token) {
        return customerRepository.findByResetToken(token)
                .map(c -> c.getResetTokenExpiresAt() != null && c.getResetTokenExpiresAt().isAfter(LocalDateTime.now()))
                .orElse(false);
    }

    @Override
    public void approveReset(String token) {
        tokenApprovalMap.put(token, true);
    }

    @Override
    public boolean isResetApproved(String token) {
        return tokenApprovalMap.getOrDefault(token, false);
    }

    @Override
    public void removeResetApproval(String token) {
        tokenApprovalMap.remove(token);
    }

    @Override
    public String getLatestVerificationToken(String email) {
        return customerRepository.findByEmail(email).map(Customer::getVerificationToken).orElse(null);
    }

    @Override
    public String getBaseUrl() {
        return baseUrl;
    }

    @Override
    public LoginResponse getCustomerInfo(Long customerId) {
        Customer customer = customerRepository.findById(customerId)
                .orElseThrow(() -> new RuntimeException("Không tìm thấy khách hàng."));
        return mapToLoginResponse(customer);
    }

    private LoginResponse mapToLoginResponse(Customer customer) {
        return LoginResponse.builder()
                .customerId(customer.getCustomerId())
                .name(customer.getName())
                .email(customer.getEmail())
                .phone(customer.getPhone())
                .gender(customer.getGender())
                .dateOfBirth(customer.getDateOfBirth())
                .token("SESSION_MANAGED")
                .build();
    }

    private String handleExistingUnverifiedCustomer(Customer c, RegisterRequest req, String token,
            LocalDateTime expiresAt) {
        if (c.getIsVerified()) {
            throw new ConflictException("Email đã được đăng ký: " + req.getEmail());
        }

        c.setName(req.getName());
        c.setPhone(req.getPhone());
        c.setPasswordHash(passwordHasher.hash(req.getPassword()));

        c.setVerificationToken(token);
        c.setVerificationTokenExpiresAt(expiresAt);
        customerRepository.save(c);

        emailService.sendVerificationEmail(req.getEmail(), token);
        return token;
    }

    private String createNewCustomer(RegisterRequest req, String token, LocalDateTime expiresAt) {
        Customer customer = Customer.builder()
                .name(req.getName())
                .phone(req.getPhone())
                .email(req.getEmail())
                .passwordHash(passwordHasher.hash(req.getPassword()))
                .isVerified(false)
                .verificationToken(token)
                .verificationTokenExpiresAt(expiresAt)
                .build();

        customerRepository.save(customer);
        emailService.sendVerificationEmail(req.getEmail(), token);
        return token;
    }

    @Override
    public String handleEmailVerification(String token) {
        try {
            verifyEmail(token);
            approveVerification(token);
            return baseUrl + "/auth-success.html";
        } catch (Exception e) {
            String errorMsg = URLEncoder.encode(e.getMessage(), StandardCharsets.UTF_8);
            return baseUrl + "/auth-error.html?msg=" + errorMsg;
        }
    }

    @Override
    public String handleResetPasswordPage(String token) {
        if (validateResetToken(token)) {
            approveReset(token);
            return baseUrl + "/auth-success.html";
        }
        String errorMsg = URLEncoder.encode("Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.", StandardCharsets.UTF_8);
        return baseUrl + "/auth-error.html?msg=" + errorMsg;
    }

    private void checkVerificationStatus(Customer customer) {
        if (!customer.getIsVerified()) {
            if (customer.getVerificationTokenExpiresAt() != null &&
                    customer.getVerificationTokenExpiresAt().isBefore(LocalDateTime.now())) {
                resendVerification(customer.getEmail());
                throw new UnauthorizedException(
                        "Tài khoản chưa xác minh và mã đã hết hạn. Một mã mới đã được gửi tới email của bạn.");
            }
            throw new UnauthorizedException(
                    "Tài khoản chưa được xác minh. Vui lòng kiểm tra email (mã có hiệu lực trong 30 phút).");
        }
    }
}
