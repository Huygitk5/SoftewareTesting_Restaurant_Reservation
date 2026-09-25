// ═══════════════════════════════════════════════════════════════
// API HELPER — Base URL + hàm tiện ích gọi API
// ═══════════════════════════════════════════════════════════════

const API_BASE_URL = "http://localhost:8081/api";

/**
 * Gọi API backend.
 * @param {string} endpoint - Đường dẫn API (vd: "/auth/login")
 * @param {string} method   - HTTP method (GET, POST, PUT, DELETE)
 * @param {object|null} body - Request body (sẽ được JSON.stringify)
 * @returns {Promise<{ok: boolean, status: number, data: any}>}
 */
export async function callApi(endpoint, method = "GET", body = null) {
    const headers = { "Content-Type": "application/json" };

    const options = { 
        method, 
        headers,
        credentials: "include" // QUAN TRỌNG: Để trình duyệt gửi Cookie (JSESSIONID) kèm theo
    };
    if (body) {
        options.body = JSON.stringify(body);
    }

    const response = await fetch(`${API_BASE_URL}${endpoint}`, options);

    // Parse JSON response (backend luôn trả JSON)
    let data;
    try {
        data = await response.json();
    } catch {
        data = null;
    }

    return { ok: response.ok, status: response.status, data };
}

/**
 * Lấy thông tin user đã lưu trong localStorage.
 */
export function getStoredUser() {
    return {
        token: localStorage.getItem("authToken"),
        customerId: localStorage.getItem("customerId"),
        name: localStorage.getItem("userName"),
        email: localStorage.getItem("userEmail"),
        phone: localStorage.getItem("userPhone"),
        gender: localStorage.getItem("userGender"),
        dateOfBirth: localStorage.getItem("userDOB"),
    };
}

/**
 * Lưu thông tin user vào localStorage sau khi login thành công.
 */
export function saveUser(loginResponse) {
    localStorage.setItem("isLoggedIn", "true");
    localStorage.setItem("authToken", loginResponse.token);
    localStorage.setItem("customerId", loginResponse.customerId);
    localStorage.setItem("userName", loginResponse.name);
    localStorage.setItem("userEmail", loginResponse.email);
    localStorage.setItem("userPhone", loginResponse.phone);
    localStorage.setItem("userGender", loginResponse.gender || "");
    localStorage.setItem("userDOB", loginResponse.dateOfBirth || "");
}

/**
 * Xóa toàn bộ thông tin user khỏi localStorage (logout).
 */
export function clearUser() {
    localStorage.removeItem("isLoggedIn");
    localStorage.removeItem("authToken");
    localStorage.removeItem("customerId");
    localStorage.removeItem("userName");
    localStorage.removeItem("userEmail");
    localStorage.removeItem("userPhone");
    localStorage.removeItem("userGender");
    localStorage.removeItem("userDOB");
}
