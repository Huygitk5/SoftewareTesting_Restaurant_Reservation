// Các hàm hỗ trợ hiển thị Modal mượt mà
function showModalSmooth(modalElement) {
    if (!modalElement) return;
    modalElement.classList.remove("hidden");
    modalElement.classList.add("open");
    modalElement.style.opacity = "1";
    modalElement.style.pointerEvents = "auto";
    modalElement.style.visibility = "visible";
}

export function showLoginModal() { showModalSmooth(document.getElementById("login-modal")); }
export function showRegisterModal() { showModalSmooth(document.getElementById("register-modal")); }
export function showForgotModal() { showModalSmooth(document.getElementById("forgot-modal")); }
export function showResetModal() { showModalSmooth(document.getElementById("reset-modal")); }

export function hideModals() {
    const modals = ["login-modal", "register-modal", "forgot-modal", "reset-modal"];

    modals.forEach(id => {
        const modal = document.getElementById(id);
        if (modal && modal.classList.contains("open")) {
            modal.classList.remove("open");
            modal.style.opacity = "0";
            modal.style.pointerEvents = "none";
            setTimeout(() => modal.classList.add("hidden"), 300);
        }
    });
}

export function initModals() {
    // Xử lý nút close (dấu X)
    document.querySelectorAll(".close-btn").forEach(btn => {
        btn.addEventListener("click", (e) => {
            e.preventDefault();
            hideModals();
        });
    });

    // Click ra ngoài màn hình tối để đóng form
    window.addEventListener("click", (e) => {
        if (e.target.classList.contains("modal-overlay")) {
            hideModals();
        }
    });

    // Chuyển đổi qua lại giữa các Form
    const showRegisterBtn = document.getElementById("show-register-btn");
    const showLoginBtn = document.getElementById("show-login-btn");
    const showForgotBtn = document.getElementById("show-forgot-btn");
    const backToLoginBtn = document.getElementById("back-to-login-btn");

    if (showRegisterBtn) showRegisterBtn.addEventListener("click", (e) => { e.preventDefault(); hideModals(); setTimeout(showRegisterModal, 350); });
    if (showLoginBtn) showLoginBtn.addEventListener("click", (e) => { e.preventDefault(); hideModals(); setTimeout(showLoginModal, 350); });
    if (showForgotBtn) showForgotBtn.addEventListener("click", (e) => { e.preventDefault(); hideModals(); setTimeout(showForgotModal, 350); });
    if (backToLoginBtn) backToLoginBtn.addEventListener("click", (e) => { e.preventDefault(); hideModals(); setTimeout(showLoginModal, 350); });
}