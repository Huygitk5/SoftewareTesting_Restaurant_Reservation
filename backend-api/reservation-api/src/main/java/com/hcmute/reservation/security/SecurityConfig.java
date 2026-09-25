package com.hcmute.reservation.security;

import lombok.RequiredArgsConstructor;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.HttpMethod;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;
import org.springframework.web.cors.CorsConfiguration;
import org.springframework.web.cors.CorsConfigurationSource;
import org.springframework.web.cors.UrlBasedCorsConfigurationSource;

import java.util.List;

@Configuration
@EnableWebSecurity
@RequiredArgsConstructor
public class SecurityConfig {

    private final SessionAuthFilter sessionAuthFilter;

    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }

    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {
        http
            .csrf(csrf -> csrf.disable())
            .cors(cors -> cors.configurationSource(corsConfigurationSource()))
            .sessionManagement(s -> s
                    .sessionCreationPolicy(SessionCreationPolicy.IF_REQUIRED)
                    .maximumSessions(1)
            )
            .addFilterBefore(sessionAuthFilter, UsernamePasswordAuthenticationFilter.class)
            .authorizeHttpRequests(auth -> auth
                // ── Public: OPTIONS preflight ─────────────────────────────────────
                .requestMatchers(HttpMethod.OPTIONS, "/**").permitAll()
                // ── Public: customer auth ─────────────────────────────────────────
                .requestMatchers(
                    "/api/auth/register",
                    "/api/auth/verify-email",
                    "/api/auth/login",
                    "/api/auth/forgot-password",
                    "/api/auth/reset-password",
                    "/api/auth/reset-password-page",
                    "/api/auth/check-reset-status",
                    "/api/auth/check-verify-status",
                    "/api/auth/resend-verification"
                ).permitAll()
                // ── Public: static content ────────────────────────────────────────
                .requestMatchers("/", "/index.html", "/web-customer/**", "/assets/**", "/js/**").permitAll()
                .requestMatchers("/ws-reservation/**", "/ws-reservation-native/**").permitAll()
                // ── Public: staff auth (POS login) ────────────────────────────────
                .requestMatchers("/api/staff/auth/login").permitAll()
                // ── Public: availability check ────────────────────────────────────
                .requestMatchers(HttpMethod.GET, "/api/reservations/availability").permitAll()

                // ── CUSTOMER only ─────────────────────────────────────────────────
                .requestMatchers(HttpMethod.POST, "/api/reservations/online").authenticated()
                .requestMatchers(HttpMethod.GET, "/api/reservations/my").authenticated()
                .requestMatchers(HttpMethod.DELETE, "/api/reservations/**").authenticated()

                // ── MANAGER only ──────────────────────────────────────────────────
                .requestMatchers(HttpMethod.POST, "/api/staff/auth/register").hasRole("MANAGER")
                .requestMatchers(HttpMethod.POST, "/api/tables").hasRole("MANAGER")
                .requestMatchers(HttpMethod.PUT, "/api/tables/*").hasRole("MANAGER")
                .requestMatchers(HttpMethod.DELETE, "/api/tables/*").hasRole("MANAGER")
                .requestMatchers("/api/reports/**").hasRole("MANAGER")
                .requestMatchers(HttpMethod.GET, "/api/override-logs").hasRole("MANAGER")
                // System scheduler endpoints — MANAGER hoặc internal cron
                .requestMatchers("/api/system/**").hasRole("MANAGER")
                .requestMatchers("/api/configs", "/api/configs/**").hasRole("MANAGER")

                // ── STAFF or MANAGER ──────────────────────────────────────────────
                .requestMatchers(HttpMethod.POST, "/api/reservations/walk-in").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.GET, "/api/reservations/walk-in/options").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.POST, "/api/reservations/*/change-table").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.POST, "/api/reservations/*/check-in").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.POST, "/api/reservations/*/check-out").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.POST, "/api/reservations/*/override").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers("/api/waitlist/**").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.GET, "/api/tables/floor-map").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.GET, "/api/reservations/active").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.GET, "/api/reservations/upcoming").hasAnyRole("RECEPTIONIST", "MANAGER")
                .requestMatchers(HttpMethod.GET, "/api/tables/available-windows").permitAll()

                // ── Any authenticated user ────────────────────────────────────────
                .anyRequest().authenticated()
            );

        return http.build();
    }

    @Bean
    public CorsConfigurationSource corsConfigurationSource() {
        CorsConfiguration config = new CorsConfiguration();
        config.setAllowedOriginPatterns(List.of("http://localhost:*", "http://127.0.0.1:*"));
        config.setAllowedMethods(List.of("GET", "POST", "PUT", "DELETE", "OPTIONS"));
        config.setAllowedHeaders(List.of("Authorization", "Content-Type", "X-Staff-Token"));
        config.setAllowCredentials(true);
        UrlBasedCorsConfigurationSource source = new UrlBasedCorsConfigurationSource();
        source.registerCorsConfiguration("/**", config);
        return source;
    }
}
