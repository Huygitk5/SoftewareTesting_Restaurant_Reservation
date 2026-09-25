package com.hcmute.reservation.config;

import org.springframework.cache.annotation.EnableCaching;
import org.springframework.context.annotation.Configuration;

@Configuration
@EnableCaching
public class CacheConfig {
    // Caffeine spec is configured in application.properties:
    // spring.cache.type=caffeine
    // spring.cache.caffeine.spec=maximumSize=500,expireAfterWrite=15s
}
