export function initHeaderScroll() {
    const header = document.getElementById("main-header");
    if (!header) return;

    const isAlwaysScrolled = header.classList.contains("scrolled");

    if (!isAlwaysScrolled) {
        window.addEventListener("scroll", () => {
            if (window.scrollY > 50) {
                header.classList.add("scrolled");
            } else {
                header.classList.remove("scrolled");
            }
        });
        if (window.scrollY > 50) header.classList.add("scrolled");
    }
}

// Hàm cập nhật chữ ở Header khi Login/Logout
export function updateHeaderAuthUI(isLoggedIn) {
    const authBtn = document.getElementById("header-auth-btn");
    const authGroup = document.getElementById("nav-auth-group");
    const greeting = document.getElementById("user-greeting");
    const bookingsLink = document.getElementById("view-bookings-link");

    if (isLoggedIn) {
        const userName = localStorage.getItem("userName");
        if (authBtn) authBtn.style.display = "none";
        if (authGroup) authGroup.style.display = "flex";
        if (greeting) greeting.textContent = userName ? `Hi, ${userName}` : "Hi, Guest";
        if (bookingsLink) bookingsLink.style.display = "";
        initGearDropdown();
    } else {
        if (authBtn) authBtn.style.display = "flex";
        if (authGroup) authGroup.style.display = "none";
        if (bookingsLink) bookingsLink.style.display = "none";
    }
}

// Gear dropdown toggle
function initGearDropdown() {
    const gearBtn = document.getElementById("gear-btn");
    const gearDropdown = document.getElementById("gear-dropdown");
    if (!gearBtn || !gearDropdown) return;

    // Tránh gắn listener trùng
    if (gearBtn.dataset.init) return;
    gearBtn.dataset.init = "true";

    gearBtn.addEventListener("click", (e) => {
        e.stopPropagation();
        gearDropdown.classList.toggle("open");
    });

    // Đóng khi click ngoài
    document.addEventListener("click", (e) => {
        if (!gearDropdown.contains(e.target)) {
            gearDropdown.classList.remove("open");
        }
    });

    // Đóng khi nhấn Escape
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") gearDropdown.classList.remove("open");
    });
}