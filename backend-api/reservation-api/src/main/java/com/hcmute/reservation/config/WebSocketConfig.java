package com.hcmute.reservation.config;

import org.springframework.context.annotation.Configuration;
import org.springframework.messaging.simp.config.MessageBrokerRegistry;
import org.springframework.web.socket.config.annotation.EnableWebSocketMessageBroker;
import org.springframework.web.socket.config.annotation.StompEndpointRegistry;
import org.springframework.web.socket.config.annotation.WebSocketMessageBrokerConfigurer;

@Configuration
@EnableWebSocketMessageBroker
public class WebSocketConfig implements WebSocketMessageBrokerConfigurer {

    @Override
    public void configureMessageBroker(MessageBrokerRegistry config) {
        // Topic để Staff và Customer đăng ký lắng nghe
        config.enableSimpleBroker("/topic");
        // Prefix cho các request gửi từ Client lên Server
        config.setApplicationDestinationPrefixes("/app");
    }

    @Override
    public void registerStompEndpoints(StompEndpointRegistry registry) {
        // Endpoint để kết nối, cho phép WinForms và Web truy cập
        registry.addEndpoint("/ws-reservation")
                .setAllowedOriginPatterns("*")
                .withSockJS();

        registry.addEndpoint("/ws-reservation-native")
                .setAllowedOriginPatterns("*");
    }
}
