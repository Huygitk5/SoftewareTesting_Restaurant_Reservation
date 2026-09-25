package com.hcmute.reservation.security;

import jakarta.servlet.http.HttpSession;

/**
 * Interface responsible for session data management.
 * Follows SRP by isolating session-related logic from business services.
 */
public interface AppSessionManager {
    /**
     * Store authentication details in the current session.
     * @param session The current HTTP session
     * @param customerId The ID of the authenticated customer
     * @param email The email of the authenticated customer
     */
    void createSession(HttpSession session, Long customerId, String email);

    /**
     * Clear the current session.
     * @param session The current HTTP session
     */
    void invalidateSession(HttpSession session);

    /**
     * Get Customer ID from the session.
     * @param session The current HTTP session
     * @return Customer ID or null
     */
    Long getCustomerId(HttpSession session);

    /**
     * Get Email from the session.
     * @param session The current HTTP session
     * @return Email or null
     */
    String getEmail(HttpSession session);

    /**
     * Check if a session is valid and authenticated.
     * @param session The current HTTP session
     * @return true if authenticated
     */
    boolean isAuthenticated(HttpSession session);
    /**
     * Get Customer ID from the session, throwing UnauthorizedException if not found.
     * @param session The current HTTP session
     * @return Customer ID
     */
    Long getRequiredCustomerId(HttpSession session);
}
