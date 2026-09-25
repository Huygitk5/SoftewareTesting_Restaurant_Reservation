import { updateHeaderAuthUI } from '../components/header.js';
import { showLoginModal, showResetModal, hideModals } from '../components/modal.js';
import { callApi, saveUser, clearUser } from '../common/api.js';
import { showToast } from '../common/utils.js';

// ═══════════════════════════════════════════════════════════════
// HELPER: Hiển thị lỗi từ backend lên form
// ═══════════════════════════════════════════════════════════════
function showFormError(form, message) {
    let errorEl = form.querySelector(".form-error");
    if (!errorEl) {
        errorEl = document.createElement("p");
        errorEl.className = "form-error";
        errorEl.style.cssText = "color:#e74c3c;font-size:0.9rem;text-align:center;margin-top:12px;line-height:1.5;";
        form.appendChild(errorEl);
    }
    errorEl.innerHTML = message;
}

function clearFormError(form) {
    const errorEl = form.querySelector(".form-error");
    if (errorEl) errorEl.innerHTML = "";
}

// ═══════════════════════════════════════════════════════════════
// HELPER: Hiển thị thông báo thành công lên form
// ═══════════════════════════════════════════════════════════════
function showFormSuccess(form, message) {
    let successEl = form.querySelector(".form-success");
    if (!successEl) {
        successEl = document.createElement("p");
        successEl.className = "form-success";
        successEl.style.cssText = "color:#27ae60;font-size:0.9rem;text-align:center;margin-top:12px;line-height:1.5;";
        form.appendChild(successEl);
    }
    successEl.innerHTML = message;
}

// ═══════════════════════════════════════════════════════════════
// KIỂM TRA TRẠNG THÁI ĐĂNG NHẬP
// ═══════════════════════════════════════════════════════════════
export function checkIsLoggedIn() {
    return localStorage.getItem("isLoggedIn") === "true";
}

// ═══════════════════════════════════════════════════════════════
// KIỂM TRA URL — Xử lý link từ email (verify-email / reset-password)
// ═══════════════════════════════════════════════════════════════
async function checkUrlActions() {
    const urlParams = new URLSearchParams(window.location.search);
    const token = urlParams.get("token");
    const action = urlParams.get("action");
    const path = window.location.pathname;

    // ── CASE 0: Verify Success — chỉ hiển thị thông báo, không cần token ──
    if (action === 'verify-success') {
        showToast("Email của bạn đã được xác minh thành công! Giờ bạn có thể đăng nhập.", "success");
        window.history.replaceState({}, document.title, window.location.pathname);
        // Có thể tự động mở modal login luôn cho user
        setTimeout(() => showLoginModal(), 400);
        return;
    }

    if (!token) return;

    // ── CASE 1: Verify Email — link trỏ đến frontend verify-email.html hoặc có ?action=verify ──
    if (path.includes("verify-email") || action === "verify") {
        try {
            const { ok, data } = await callApi(`/auth/verify-email?token=${token}`, "GET");
            if (ok) {
                // Thay vì alert, chuyển hướng về trang chính với param 'verify-success'
                window.location.href = window.location.origin + window.location.pathname.split('/').slice(0, -1).join('/') + "/index.html?action=verify-success";
            } else {
                showToast(data?.message || "Verification failed. The link may be expired.", "error");
                // Xoá params khỏi URL và chuyển về trang chính
                window.location.href = window.location.origin + window.location.pathname.split('/').slice(0, -1).join('/') + "/index.html";
            }
        } catch {
            showToast("Unable to connect to server. Please try again.", "error");
        }
        // Xoá params khỏi URL và chuyển về trang chính để login
        window.location.href = window.location.origin + window.location.pathname.split('/').slice(0, -1).join('/') + "/index.html";
        return;
    }

    // ── CASE 2: Reset Password — link trỏ đến reset-password.html hoặc có ?action=reset ──
    if (path.includes("reset-password") || action === "reset") {
        window.__resetToken = token;
        // Chờ modal sẳn sàng rồi mở
        setTimeout(() => showResetModal(), 500);
    }
}

// ═══════════════════════════════════════════════════════════════
// HÀM KHỞI TẠO AUTH — Gắn event listener cho tất cả form
// ═══════════════════════════════════════════════════════════════
export async function initAuth() {
    const isLoggedIn = checkIsLoggedIn();
    updateHeaderAuthUI(isLoggedIn);

    // XÁC THỰC SESSION THẬT (Sync with Backend)
    if (isLoggedIn) {
        const { ok, status, data } = await callApi("/auth/me", "GET");
        if (!ok && (status === 401 || status === 403)) {
            console.warn("Session expired on backend. Clearing local auth state.");
            clearUser();
            updateHeaderAuthUI(false);
        } else if (ok && data) {
            // Cập nhật lại thông tin mới nhất (optional)
            saveUser(data); 
            updateHeaderAuthUI(true);
        }
    }

    // Kiểm tra URL có chứa token verify-email hoặc reset-password không
    checkUrlActions();

    // Nút "Book a Table" / "Login" trên header
    const authActionBtns = document.querySelectorAll(".auth-action-btn");
    authActionBtns.forEach(btn => {
        btn.addEventListener("click", (e) => {
            e.preventDefault();
            if (checkIsLoggedIn()) {
                window.location.href = "reservation.html";
            } else {
                showLoginModal();
            }
        });
    });

    // ─────────────────────────────────────────────────────────
    // 1. FORM LOGIN → POST /api/auth/login
    // ─────────────────────────────────────────────────────────
    const loginForm = document.getElementById("login-form");
    if (loginForm) {
        loginForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            clearFormError(loginForm);

            const email = document.getElementById("login-email").value.trim();
            const password = document.getElementById("login-password").value;

            const submitBtn = loginForm.querySelector("button[type='submit']");
            submitBtn.innerText = "Authenticating...";
            submitBtn.disabled = true;

            try {
                const { ok, status, data } = await callApi("/auth/login", "POST", { email, password });

                if (ok) {
                    saveUser(data);
                    hideModals();
                    loginForm.reset();
                    updateHeaderAuthUI(true);
                    window.location.href = "reservation.html";
                } else {
                    const errorMsg = data?.message || data?.error || "Login failed. Please try again.";
                    
                    // Nếu lỗi do chưa xác minh, cho người dùng nút Gửi lại link
                    if (errorMsg.includes("chưa xác minh") || errorMsg.includes("mã đã hết hạn") || status === 401) {
                         showFormError(loginForm, `${errorMsg} <br><a href="#" id="resend-verify-link" data-email="${email}" style="color: var(--gold); text-decoration: underline; font-weight: 600;">Gửi lại mã xác minh</a>`);
                         
                         // Gắn sự kiện cho link mới
                         document.getElementById("resend-verify-link")?.addEventListener("click", async (e) => {
                             e.preventDefault();
                             const resendEmail = e.target.getAttribute("data-email");
                             try {
                                 const resendResult = await callApi("/auth/resend-verification", "POST", { email: resendEmail });
                                 if (resendResult.ok) {
                                     showToast("Mã xác minh mới đã được gửi! Vui lòng kiểm tra email.", "success");
                                     if (resendResult.data.token) {
                                         startVerifyPolling(resendResult.data.token);
                                     }
                                 } else {
                                     showToast(resendResult.data.message || "Không thể gửi lại mã.", "error");
                                 }
                             } catch (err) {
                                 showToast("Lỗi kết nối khi gửi lại mã.", "error");
                             }
                         });
                    } else {
                        showFormError(loginForm, errorMsg);
                    }
                }
            } catch (err) {
                showFormError(loginForm, "Unable to connect to server. Please try again later.");
            } finally {
                submitBtn.innerText = "Login";
                submitBtn.disabled = false;
            }
        });
    }

    // ─────────────────────────────────────────────────────────
    // 2. FORM REGISTER → POST /api/auth/register
    //    Sau khi register thành công → KHÔNG tự login
    //    Phải xác minh email trước
    // ─────────────────────────────────────────────────────────
    const registerForm = document.getElementById("register-form");
    if (registerForm) {
        registerForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            clearFormError(registerForm);

            const name = document.getElementById("reg-name").value.trim();
            const phone = document.getElementById("reg-phone").value.trim();
            const email = document.getElementById("reg-email").value.trim();
            const password = document.getElementById("reg-password").value;
            const confirmPassword = document.getElementById("reg-confirm-password").value;

            // --- FRONTEND VALIDATION ---
            if (!name || !email || !phone || !password) {
                showFormError(registerForm, "Vui lòng nhập đầy đủ tất cả các trường.");
                return;
            }

            // Email regex
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(email)) {
                showFormError(registerForm, "Email không đúng định dạng.");
                return;
            }

            // Vietnamese Phone regex
            const phoneRegex = /^(0|\+84)(\s|\.)?((3[2-9])|(5[689])|(7[06-9])|(8[1-689])|(9[0-46-9]))(\d)(\s|\.)?(\d{3})(\s|\.)?(\d{3})$/;
            if (!phoneRegex.test(phone)) {
                showFormError(registerForm, "Số điện thoại không đúng định dạng Việt Nam.");
                return;
            }

            if (password.length < 6) {
                showFormError(registerForm, "Mật khẩu phải từ 6 ký tự trở lên.");
                return;
            }

            if (password !== confirmPassword) {
                showFormError(registerForm, "Mật khẩu nhập lại không khớp!");
                return;
            }

            const submitBtn = registerForm.querySelector("button[type='submit']");
            submitBtn.innerText = "Registering...";
            submitBtn.disabled = true;

            try {
                const { ok, data } = await callApi("/auth/register", "POST", {
                    name, phone, email, password
                });

                if (ok) {
                    // Nếu backend trả về token, bắt đầu polling chờ verify
                    if (data.token) {
                        startVerifyPolling(data.token);
                        hideModals();
                        registerForm.reset();
                        showToast("Account created! Please check your email to verify (Link expires in 30m). This page will update automatically.", "success", 8000);
                    } else {
                        hideModals();
                        registerForm.reset();
                        showToast("Account created successfully! Please check your email to verify.", "success", 6000);
                        setTimeout(() => showLoginModal(), 400);
                    }
                } else {
                    const errorMsg = data?.message || data?.error || "Registration failed. Please try again.";
                    showFormError(registerForm, errorMsg);
                }
            } catch (err) {
                showFormError(registerForm, "Unable to connect to server. Please try again later.");
            } finally {
                submitBtn.innerText = "Register";
                submitBtn.disabled = false;
            }
        });
    }

    // ─────────────────────────────────────────────────────────
    // 3. FORM QUÊN MẬT KHẨU → POST /api/auth/forgot-password
    //    Gửi email chứa link reset → user click link → qua form reset
    // ─────────────────────────────────────────────────────────
    const forgotForm = document.getElementById("forgot-form");
    if (forgotForm) {
        forgotForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            clearFormError(forgotForm);

            const email = document.getElementById("forgot-email").value.trim();

            const submitBtn = forgotForm.querySelector("button[type='submit']");
            submitBtn.innerText = "Sending...";
            submitBtn.disabled = true;

            try {
                const { ok, data } = await callApi("/auth/forgot-password", "POST", { email });

                if (ok) {
                    // Hiển thị thông báo thành công ngay trên form
                    showFormSuccess(forgotForm, "A password reset link has been sent to your email. Please check your inbox and click the link. This page will automatically update once you confirm.");
                    forgotForm.querySelector("button[type='submit']").style.display = "none";
                    forgotForm.querySelector(".form-group").style.display = "none";
                    
                    // BẮT ĐẦU POLLING: Đợi user click link trong mail
                    if (data.token) {
                        startResetPolling(data.token);
                    }
                } else {
                    const errorMsg = data?.message || data?.error || "Failed to send reset email.";
                    showFormError(forgotForm, errorMsg);
                }
            } catch (err) {
                showFormError(forgotForm, "Unable to connect to server. Please try again later.");
            } finally {
                submitBtn.innerText = "Continue";
                submitBtn.disabled = false;
            }
        });
    }

    // Hàm Polling: Kiểm tra xem link reset đã được click chưa
    function startResetPolling(token) {
        const pollInterval = setInterval(async () => {
            try {
                const { ok, data } = await callApi(`/auth/check-reset-status?token=${token}`, "GET");
                if (ok && data.approved) {
                    clearInterval(pollInterval);
                    hideModals(); // Đóng modal forgot
                    window.__resetToken = token;
                    showResetModal(); // Tự động mở modal reset password mới
                }
            } catch (err) {
                console.error("Polling error:", err);
            }
        }, 2000); // Check mỗi 2 giây

        // Dừng polling sau 15 phút nếu user không làm gì
        setTimeout(() => clearInterval(pollInterval), 15 * 60 * 1000);
    }

    // Hàm Polling: Đợi email được xác minh
    function startVerifyPolling(token) {
        const pollInterval = setInterval(async () => {
            try {
                const { ok, data } = await callApi(`/auth/check-verify-status?token=${token}`, "GET");
                if (ok && data.approved) {
                    clearInterval(pollInterval);
                    hideModals();
                    showToast("Email verified successfully! You can now log in.", "success");
                    setTimeout(() => showLoginModal(), 600);
                }
            } catch (err) {
                console.error("Verification polling error:", err);
            }
        }, 3000);

        // Dừng sau 10 phút
        setTimeout(() => clearInterval(pollInterval), 10 * 60 * 1000);
    }

    // ─────────────────────────────────────────────────────────
    // 4. FORM ĐẶT LẠI MẬT KHẨU → POST /api/auth/reset-password
    //    Token lấy từ URL (?token=xxx) khi user click link trong email
    // ─────────────────────────────────────────────────────────
    const resetForm = document.getElementById("reset-form");
    if (resetForm) {
        resetForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            clearFormError(resetForm);

            const newPassword = document.getElementById("reset-password").value;
            const confirmPassword = document.getElementById("reset-confirm").value;

            if (newPassword !== confirmPassword) {
                showFormError(resetForm, "Passwords do not match! Please try again.");
                return;
            }

            // Lấy token từ URL query params
            const token = window.__resetToken || new URLSearchParams(window.location.search).get("token");
            if (!token) {
                showFormError(resetForm, "Reset token is missing. Please use the link from your email.");
                return;
            }

            const submitBtn = resetForm.querySelector("button[type='submit']");
            submitBtn.innerText = "Updating...";
            submitBtn.disabled = true;

            try {
                const { ok, data } = await callApi("/auth/reset-password", "POST", {
                    token,
                    newPassword
                });

                if (ok) {
                    hideModals();
                    resetForm.reset();
                    // Xóa token khỏi URL
                    window.history.replaceState({}, document.title, window.location.pathname);
                    window.__resetToken = null;
                    showToast("Password updated successfully! Please login with your new password.", "success");
                    setTimeout(() => showLoginModal(), 400);
                } else {
                    const errorMsg = data?.message || data?.error || "Failed to reset password.";
                    showFormError(resetForm, errorMsg);
                }
            } catch (err) {
                showFormError(resetForm, "Unable to connect to server. Please try again later.");
            } finally {
                submitBtn.innerText = "Update Password";
                submitBtn.disabled = false;
            }
        });
    }

    // ─────────────────────────────────────────────────────────
    // 5. LOGOUT — Xóa toàn bộ thông tin user
    // ─────────────────────────────────────────────────────────
    const logoutBtn = document.getElementById("logout-btn");
    if (logoutBtn) {
        logoutBtn.addEventListener("click", async (e) => {
            e.preventDefault();
            
            // Goị API logout để xoá session trên server
            try {
                await callApi("/auth/logout", "POST");
            } catch (err) {
                console.error("Logout API failed:", err);
            }

            clearUser();
            updateHeaderAuthUI(false);
            if (window.location.pathname.includes("reservation.html")) {
                window.location.href = "index.html";
            }
        });
    }
}