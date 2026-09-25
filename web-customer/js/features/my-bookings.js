import { callApi, clearUser } from '../common/api.js';

let myBookings = [];
let currentCategory = 'upcoming'; // 'upcoming' or 'history'

const CATEGORIES = {
    UPCOMING: 'upcoming',
    HISTORY: 'history'
};

/**
 * Đảm bảo modal HTML tồn tại trong DOM.
 * Nếu trang hiện tại không có (vd: profile.html), tự inject đúng cấu trúc của reservation.
 * → Một nguồn duy nhất, không copy-paste HTML.
 */
function ensureModalExists() {
    if (document.getElementById('bookings-modal')) return;

    const html = `
    <div id="bookings-modal" class="modal-overlay">
      <div class="modal-card">
        <button class="close-modal" id="close-bookings-btn" onclick="closeBookingsModal()">✕</button>
        <h3 class="modal-title">Your Reservations</h3>
        <div class="modal-tabs">
          <button class="tab-btn active" id="tab-upcoming" onclick="switchBookingTab('upcoming')">Upcoming</button>
          <button class="tab-btn" id="tab-history" onclick="switchBookingTab('history')">History</button>
        </div>
        <div id="bookings-list" class="booking-list"></div>
      </div>
    </div>`;

    document.body.insertAdjacentHTML('beforeend', html);

    // Gán global functions cho onclick attributes
    window.closeBookingsModal = closeBookingsModal;
    window.switchBookingTab = switchBookingTab;
    window.cancelMyBooking = cancelMyBooking;
}

/**
 * Mở Modal danh sách đặt bàn
 */
export async function openBookingsModal() {
    ensureModalExists();

    const modal = document.getElementById('bookings-modal');
    const listHtml = document.getElementById('bookings-list');

    if (!modal || !listHtml) return;

    modal.classList.add('open');
    listHtml.innerHTML = '<div class="loading-bookings">Loading your reservations...</div>';

    // Reset tab về upcoming mặc định
    currentCategory = CATEGORIES.UPCOMING;
    updateTabUI();

    try {
        const { ok, status, data } = await callApi('/reservations/my', 'GET');

        if (status === 401 || status === 403) {
            alert("Session expired. Please log in again.");
            clearUser();
            window.location.href = 'index.html';
            return;
        }

        if (!ok) {
            listHtml.innerHTML = `<p class="error-state">Failed to load reservations. (Error: ${status})</p>`;
            return;
        }

        myBookings = data || [];
        renderBookings();
    } catch (err) {
        console.error("Fetch bookings error:", err);
        listHtml.innerHTML = '<p class="error-state">Unable to connect to server.</p>';
    }
}

/**
 * Đóng Modal
 */
export function closeBookingsModal() {
    const modal = document.getElementById('bookings-modal');
    if (modal) modal.classList.remove('open');
}

/**
 * Chuyển Tab
 */
export function switchBookingTab(tab) {
    currentCategory = tab;
    updateTabUI();
    renderBookings();
}

function updateTabUI() {
    const tabUpcoming = document.getElementById('tab-upcoming');
    const tabHistory = document.getElementById('tab-history');
    if (tabUpcoming) tabUpcoming.classList.toggle('active', currentCategory === CATEGORIES.UPCOMING);
    if (tabHistory) tabHistory.classList.toggle('active', currentCategory === CATEGORIES.HISTORY);
}

/**
 * Render danh sách đơn đặt bàn dựa trên Tab
 */
export function renderBookings() {
    const listHtml = document.getElementById('bookings-list');
    if (!listHtml) return;

    const now = new Date();

    // Logic phân loại (tương tự reservation.js)
    let filtered = myBookings.filter(r => {
        const isPastStatus = ['COMPLETED', 'CANCELLED', 'NO_SHOW', 'EXPIRED'].includes(r.status);
        if (currentCategory === CATEGORIES.UPCOMING) return !isPastStatus;
        return isPastStatus;
    });

    // Sắp xếp: Upcoming (Sắp tới nhất lên đầu), History (Mới đi ăn xong lên đầu)
    filtered.sort((a, b) => {
        const da = new Date(a.startTime).getTime();
        const db = new Date(b.startTime).getTime();
        return currentCategory === CATEGORIES.UPCOMING ? da - db : db - da;
    });

    if (filtered.length === 0) {
        listHtml.innerHTML = `<p class="empty-state">No ${currentCategory} reservations found.</p>`;
        return;
    }

    listHtml.innerHTML = '';
    filtered.forEach(r => {
        const start = new Date(r.startTime);
        const dateStr = start.toLocaleDateString('en-US', { weekday: 'short', day: 'numeric', month: 'short' });
        const timeStr = r.startTime.split('T')[1].substring(0, 5);

        // Chỉ cho hủy nếu trạng thái là RESERVED và cách giờ ăn ít nhất 3 tiếng
        const canCancel = r.status === 'RESERVED' && (start.getTime() - now.getTime() > 3 * 60 * 60 * 1000);

        let statusClass = 'st-default';
        if (r.status === 'RESERVED') statusClass = 'st-reserved';
        if (r.status === 'CANCELLED' || r.status === 'NO_SHOW') statusClass = 'st-cancelled';
        if (r.status === 'COMPLETED' || r.status === 'SEATED') statusClass = 'st-completed';

        listHtml.innerHTML += `
        <div class="booking-item">
          <div class="b-header">
            <p class="b-ref">#${r.reservationId}</p>
            <span class="status-badge ${statusClass}">${r.status}</span>
          </div>
          <div class="b-body">
            <p class="b-detail"><strong>Date:</strong> ${dateStr} at ${timeStr}</p>
            <p class="b-detail"><strong>Guests:</strong> ${r.guestCount} people</p>
            ${r.note ? `<p class="b-note">"${r.note}"</p>` : ''}
          </div>
          <div class="b-footer">
            ${canCancel ? `<button class="btn-cancel-small" onclick="cancelMyBooking(${r.reservationId})">Cancel Reservation</button>` : ''}
            ${r.status === 'RESERVED' && !canCancel ? '<span class="cancel-deadline">Cancellation closed (within 3h)</span>' : ''}
          </div>
        </div>
      `;
    });
}

/**
 * Hủy đơn đặt bàn
 */
export async function cancelMyBooking(id) {
    if (!confirm(`Are you sure you want to cancel reservation #${id}?`)) return;

    try {
        const { ok, data } = await callApi(`/reservations/${id}`, 'DELETE');
        if (ok) {
            alert("Reservation cancelled successfully.");
            // Tải lại danh sách
            const { data: newData } = await callApi('/reservations/my', 'GET');
            myBookings = newData || [];
            renderBookings();
        } else {
            alert(data?.message || "Failed to cancel reservation.");
        }
    } catch (err) {
        alert("An error occurred. Please try again.");
    }
}

// Khi người dùng bấm phím Esc, đóng modal
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') closeBookingsModal();
});
