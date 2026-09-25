package com.hcmute.reservation.security.impl;

import com.hcmute.reservation.exception.UnauthorizedException;
import com.hcmute.reservation.security.AppSessionManager;
import jakarta.servlet.http.HttpSession;
import org.springframework.stereotype.Component;

/**
 * Implementation of SessionManager using standard HttpSession.
 * This encapsulates session attributes to maintain SRP.
 */
@Component
public class AppSessionManagerImpl implements AppSessionManager {

    private static final String ATTR_CUSTOMER_ID = "customerId";
    private static final String ATTR_USER_EMAIL = "userEmail";

    @Override
    public void createSession(HttpSession session, Long customerId, String email) {
        if (session != null) {
            session.setAttribute(ATTR_CUSTOMER_ID, customerId);
            session.setAttribute(ATTR_USER_EMAIL, email);
        }
    }

    @Override
    public void invalidateSession(HttpSession session) {
        if (session != null) {
            session.invalidate();
        }
    }

    @Override
    public Long getCustomerId(HttpSession session) {
        if (session == null) return null;
        Object val = session.getAttribute(ATTR_CUSTOMER_ID);
        return (val instanceof Long) ? (Long) val : null;
    }

    @Override
    public String getEmail(HttpSession session) {
        if (session == null) return null;
        Object val = session.getAttribute(ATTR_USER_EMAIL);
        return (val instanceof String) ? (String) val : null;
    }

    @Override
    public boolean isAuthenticated(HttpSession session) {
        return session != null && session.getAttribute(ATTR_CUSTOMER_ID) != null;
    }

    @Override
    public Long getRequiredCustomerId(HttpSession session) {
        Long id = getCustomerId(session);
        if (id == null) {
            throw new UnauthorizedException("Phiên làm việc hết hạn hoặc không hợp lệ. Vui lòng đăng nhập lại.");
        }
        return id;
    }
}
