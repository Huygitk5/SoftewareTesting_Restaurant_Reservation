package com.hcmute.reservation.security;

import com.hcmute.reservation.model.entity.Account;
import com.hcmute.reservation.repository.AccountRepository;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.servlet.http.HttpSession;
import lombok.RequiredArgsConstructor;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

/**
 * Enhanced Filter to handle both WinForm POS tokens and Web Session Cookies.
 * Replaces the old JWT Filter to follow the new Stateful architecture.
 */
@Component
@RequiredArgsConstructor
public class SessionAuthFilter extends OncePerRequestFilter {

    private final AppSessionManager sessionManager;
    private final AccountRepository accountRepository;

    @Override
    protected void doFilterInternal(HttpServletRequest request,
            HttpServletResponse response,
            FilterChain filterChain) throws ServletException, IOException {

        // 1) Legacy Support: Staff session token (WinForm POS)
        String staffToken = request.getHeader("X-Staff-Token");
        if (staffToken != null && !staffToken.isBlank()) {
            handleStaffToken(staffToken);
        }

        // 2) New: Web Session Cookie (Customer)
        // If already authenticated by staff token, we skip customer session check
        if (SecurityContextHolder.getContext().getAuthentication() == null) {
            HttpSession session = request.getSession(false);
            if (sessionManager.isAuthenticated(session)) {
                handleCustomerSession(session);
            }
        }

        filterChain.doFilter(request, response);
    }

    private void handleStaffToken(String staffToken) {
        Optional<Account> accountOpt = accountRepository.findBySessionToken(staffToken);
        if (accountOpt.isPresent()) {
            Account account = accountOpt.get();
            if (account.getSessionExpiresAt() != null &&
                    account.getSessionExpiresAt().isAfter(LocalDateTime.now())) {
                var auth = new UsernamePasswordAuthenticationToken(
                        account, null,
                        List.of(new SimpleGrantedAuthority("ROLE_RECEPTIONIST"),
                                new SimpleGrantedAuthority("ROLE_" + account.getRole().name())));
                SecurityContextHolder.getContext().setAuthentication(auth);
            }
        }
    }

    private void handleCustomerSession(HttpSession session) {
        String email = sessionManager.getEmail(session);
        Long customerId = sessionManager.getCustomerId(session);

        if (email != null && customerId != null) {
            var auth = new UsernamePasswordAuthenticationToken(
                    email, null,
                    List.of(new SimpleGrantedAuthority("ROLE_CUSTOMER")));
            auth.setDetails(customerId);
            SecurityContextHolder.getContext().setAuthentication(auth);
        }
    }
}
