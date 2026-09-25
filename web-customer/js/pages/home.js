import { initAnimations, initPasswordToggle } from '../common/utils.js';
import { initHeaderScroll } from '../components/header.js';
import { initModals, showLoginModal } from '../components/modal.js';
import { initAuth, checkIsLoggedIn } from '../features/auth.js';
import { openBookingsModal, closeBookingsModal, switchBookingTab, cancelMyBooking } from '../features/my-bookings.js';

// Đưa hàm ra phạm vi global để HTML gọi được
window.openBookingsModal = openBookingsModal;
window.closeBookingsModal = closeBookingsModal;
window.switchBookingTab = switchBookingTab;
window.cancelMyBooking = cancelMyBooking;

document.addEventListener("DOMContentLoaded", () => {
    initHeaderScroll();     // Cuộn header
    initAnimations();       // Hiệu ứng hiện dần
    initModals();           // Logic đóng/mở Modal
    initPasswordToggle();   // Logic con mắt password
    initAuth();             // Logic đăng nhập/đăng ký

    // --- BẮT BUỘC ĐĂNG NHẬP KHI BẤM RESERVE Ở TRANG CHỦ ---
    const reserveLinks = document.querySelectorAll('a[href="reservation.html"]');
    reserveLinks.forEach(link => {
        link.addEventListener('click', (e) => {
            if (!checkIsLoggedIn()) {
                e.preventDefault();
                showLoginModal();
            }
        });
    });

    // Gắn sự kiện cho link "My Bookings" trong header
    const viewBookingsLink = document.getElementById('view-bookings-link');
    if (viewBookingsLink) {
        viewBookingsLink.addEventListener('click', (e) => {
            e.preventDefault();
            openBookingsModal();
        });
    }

    // Gắn sự kiện cho các tab trong modal (nếu cần thủ công, nhưng ở đây dùng global render)
    const tabUpcoming = document.getElementById('tab-upcoming');
    const tabHistory = document.getElementById('tab-history');
    if (tabUpcoming) tabUpcoming.onclick = () => switchBookingTab('upcoming');
    if (tabHistory) tabHistory.onclick = () => switchBookingTab('history');

    const closeBookingsBtn = document.getElementById('close-bookings-btn');
    if (closeBookingsBtn) closeBookingsBtn.onclick = closeBookingsModal;
});