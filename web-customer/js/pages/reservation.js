import { callApi, getStoredUser, clearUser } from '../common/api.js';
import { initAuth } from '../features/auth.js';
import { openBookingsModal, closeBookingsModal, switchBookingTab, cancelMyBooking } from '../features/my-bookings.js';

const S = { party: 2, date: null, dateStr: null, time: null, week: 0, MAX_W: 4, reservationId: null };
const DAYS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
const MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
const today = new Date(); today.setHours(0, 0, 0, 0);
const dow = today.getDay(), toMon = dow === 0 ? 6 : dow - 1;
const thisMonday = new Date(today); thisMonday.setDate(today.getDate() - toMon);

let countdownInterval = null;
let timeLeft = 300; // 5 phút = 300 giây

const ICON_MAP = {
    danger: `<svg viewBox="0 0 24 24" fill="none" stroke="#E76B6B" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" width="32" height="32"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>`,
    info: `<svg viewBox="0 0 24 24" fill="none" stroke="var(--accent-gold)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" width="32" height="32"><circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/></svg>`,
    success: `<svg viewBox="0 0 24 24" fill="none" stroke="#48BB78" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" width="32" height="32"><polyline points="20 6 9 17 4 12"/></svg>`
};

/**
 * Hiển thị một Modal tùy chỉnh thay thế cho alert/confirm truyền thống.
 * @param {string} title - Tiêu đề Modal
 * @param {string} message - Nội dung thông báo
 * @param {boolean} isConfirm - Nếu true, hiển thị 2 nút (Yes/No), ngược lại chỉ show 1 nút (OK)
 * @param {string} type - 'info', 'danger', 'success'
 * @returns {Promise<boolean>} Prosise trả về true nếu chọn Yes/OK, false nếu chọn No
 */
function showModal(title, message, isConfirm = false, type = 'info') {
    return new Promise((resolve) => {
        const root = document.getElementById('modal-root');
        const t = document.getElementById('modal-title');
        const m = document.getElementById('modal-message');
        const iconDiv = document.getElementById('modal-icon');
        const btnYes = document.getElementById('modal-btn-yes');
        const btnNo = document.getElementById('modal-btn-no');

        t.textContent = title;
        m.textContent = message;
        iconDiv.innerHTML = ICON_MAP[type] || ICON_MAP.info;
        root.classList.add('open');

        if (isConfirm) {
            btnNo.style.display = 'block';
            btnNo.textContent = 'Cancel';
            btnYes.textContent = 'Confirm';
        } else {
            btnNo.style.display = 'none';
            btnYes.textContent = 'Got it';
        }

        const cleanup = (val) => {
            root.classList.remove('open');
            btnYes.onclick = null;
            btnNo.onclick = null;
            resolve(val);
        };

        btnYes.onclick = () => cleanup(true);
        btnNo.onclick = () => cleanup(false);
    });
}

// ─── HÀM EXPORT CHO HTML GỌI ───
window.setParty = setParty;
window.confirmParty = confirmParty;
window.confirmDate = confirmDate;
window.confirmTime = confirmTime;
window.editStep = editStep;
window.goToPayment = goToPayment;
window.backToBooking = backToBooking;
window.confirmReservation = confirmReservation;
window.cancelPayment = cancelPayment;
window.openBookingsModal = openBookingsModal;
window.closeBookingsModal = closeBookingsModal;
window.switchBookingTab = switchBookingTab;
window.cancelMyBooking = cancelMyBooking;

// ─── LÔ-GIC TIMER ───
function startTimer() {
    clearInterval(countdownInterval);
    timeLeft = 300;
    updateTimerDisplay();

    countdownInterval = setInterval(async () => {
        timeLeft--;
        updateTimerDisplay();
        if (timeLeft <= 0) {
            clearInterval(countdownInterval);
            await showModal("Time Expired", "The 5-minute table lock has expired. Please select a new slot to continue.");
            backToBooking();
        }
    }, 1000);
}

function stopTimer() { clearInterval(countdownInterval); }

function updateTimerDisplay() {
    const m = Math.floor(timeLeft / 60).toString().padStart(2, '0');
    const s = (timeLeft % 60).toString().padStart(2, '0');
    document.getElementById('timer-text').textContent = `${m}:${s}`;

    const percentage = (timeLeft / 300) * 100;
    document.querySelector('.timer-prog').setAttribute('stroke-dasharray', `${percentage}, 100`);

    const pill = document.getElementById('timer-pill');
    const prog = document.querySelector('.timer-prog');
    const text = document.getElementById('timer-text');

    if (timeLeft <= 60) {
        pill.style.borderColor = '#E76B6B';
        pill.style.background = '#FFF5F5';
        prog.style.stroke = '#E76B6B';
        text.style.color = '#E76B6B';
    } else {
        pill.style.borderColor = 'rgba(163, 131, 84, 0.2)';
        pill.style.background = 'var(--gold-pale)';
        prog.style.stroke = 'var(--gold)';
        text.style.color = 'var(--gold)';
    }
}

// ─── CÁC BƯỚC ĐẶT BÀN ───
function setParty(n) {
    n = Math.max(1, Math.min(30, parseInt(n) || 1)); S.party = n;
    const pInput = document.getElementById('party-val');
    if (pInput) pInput.value = n;
    document.querySelectorAll('.party-chip').forEach(c => {
        const v = parseInt(c.dataset.v);
        c.classList.toggle('sel', v === n || (c.dataset.v === '8' && n >= 8));
    });
    updateSummary();
}

function confirmParty() {
    const sv = document.getElementById('sv-1');
    sv.textContent = `${S.party} ${S.party === 1 ? 'Guest' : 'Guests'}`; sv.classList.remove('ph');
    doneStep(1); openStep(2);
}

function getDs(d) { return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`; }

function renderDates() {
    const grid = document.getElementById('date-grid'), lbl = document.getElementById('date-nav-lbl'), o = S.week;
    if (!grid) return;
    grid.innerHTML = '';
    const ws = new Date(thisMonday); ws.setDate(thisMonday.getDate() + o * 7);
    const we = new Date(ws); we.setDate(ws.getDate() + 6);
    lbl.textContent = o === 0 ? 'This Week' : o === 1 ? 'Next Week' : `${MONTHS[ws.getMonth()]} ${ws.getDate()} – ${MONTHS[we.getMonth()]} ${we.getDate()}`;
    document.getElementById('prev-week').disabled = o <= 0;
    document.getElementById('next-week').disabled = o >= S.MAX_W;

    for (let i = 0; i < 7; i++) {
        const d = new Date(ws); d.setDate(ws.getDate() + i);
        const ds = getDs(d), past = d < today, isTod = d.getTime() === today.getTime();
        const btn = document.createElement('button');
        btn.className = 'd-chip' + (isTod ? ' today' : '') + (S.dateStr === ds ? ' sel' : '');
        btn.disabled = past;
        btn.innerHTML = `<span class="dn">${DAYS[d.getDay()]}</span><span class="dd">${d.getDate()}</span>`;
        if (!past) btn.addEventListener('click', () => {
            S.date = d; S.dateStr = ds;
            document.querySelectorAll('.d-chip').forEach(c => c.classList.remove('sel'));
            btn.classList.add('sel');
            const cta = document.getElementById('date-cta');
            cta.disabled = false; cta.innerHTML = 'Continue <span class="arr">→</span>';
            updateSummary(); renderTimes();
        });
        grid.appendChild(btn);
    }
}

const LUNCH = ['12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00'];
const DINNER = ['18:00', '18:30', '19:00', '19:30', '20:00', '20:30', '21:00', '21:30'];

function renderTimes() {
    fillSlots('tg-lunch', LUNCH);
    fillSlots('tg-dinner', DINNER);
}

async function fillSlots(id, slots) {
    const grid = document.getElementById(id);
    if (!grid || !S.dateStr) return;
    grid.innerHTML = '<div class="loading-slots">Checking availability...</div>';

    try {
        // Gọi API kiểm tra giờ (có bàn trống đáp ứng slot)
        const { ok, data } = await callApi(`/reservations/availability/slots?date=${S.dateStr}&guests=${S.party}&slots=${slots.join(',')}`);

        grid.innerHTML = '';
        slots.forEach((slot) => {
            const isAvailable = ok && data && data[slot] === true;
            const btn = document.createElement('button');
            btn.className = 't-chip' + (!isAvailable ? ' bkd' : '') + (S.time === slot ? ' sel' : '');
            btn.disabled = !isAvailable;
            btn.innerHTML = `<span>${slot}</span>`;

            if (isAvailable) {
                btn.addEventListener('click', () => {
                    S.time = slot;
                    document.querySelectorAll('.t-chip').forEach(c => c.classList.remove('sel'));
                    btn.classList.add('sel');
                    const cta = document.getElementById('time-cta');
                    cta.disabled = false;
                    cta.innerHTML = 'Continue <span class="arr">→</span>';
                    updateSummary();
                });
            }
            grid.appendChild(btn);
        });
    } catch (err) {
        console.error("Failed to fetch slots:", err);
        grid.innerHTML = '<div class="error-slots">Error loading slots</div>';
    }
}

function confirmDate() {
    if (!S.dateStr) return;
    const sv = document.getElementById('sv-2');
    sv.textContent = `${DAYS[S.date.getDay()]}, ${S.date.getDate()} ${MONTHS[S.date.getMonth()]}`; sv.classList.remove('ph');
    doneStep(2); openStep(3); renderTimes();
}

function confirmTime() {
    if (!S.time) return;
    const sv = document.getElementById('sv-3'); sv.textContent = S.time; sv.classList.remove('ph');
    doneStep(3); openStep(4);
    loadStep4AutoFill();
}

function loadStep4AutoFill() {
    const user = getStoredUser();
    const nameInput = document.getElementById('guest-name');
    const emailInput = document.getElementById('guest-email');
    const phoneInput = document.getElementById('guest-phone');

    if (nameInput) nameInput.value = user.name || "";
    if (emailInput) emailInput.value = user.email || "";
    if (phoneInput) phoneInput.value = user.phone || "";
}

function doneStep(n) { const el = document.getElementById(`step-${n}`); el.classList.remove('active'); el.classList.add('completed'); setProg(n, 'done'); }

function openStep(n) {
    const el = document.getElementById(`step-${n}`); el.classList.remove('completed'); el.classList.add('active'); setProg(n, 'active');
    setTimeout(() => el.scrollIntoView({ behavior: 'smooth', block: 'nearest' }), 80);
    if (n === 2) renderDates(); if (n === 3 && S.dateStr) renderTimes();
}

function editStep(n) {
    for (let i = n; i <= 4; i++) { const el = document.getElementById(`step-${i}`); el.classList.remove('active', 'completed'); setProg(i, ''); }
    if (n <= 2) { S.date = null; S.dateStr = null; } if (n <= 3) { S.time = null; }
    document.getElementById('proceed-btn').classList.remove('ready');
    openStep(n); updateSummary();
}

function setProg(n, st) { const el = document.querySelector(`.prog-item[data-s="${n}"]`); if (!el) return; el.classList.remove('active', 'done'); if (st) el.classList.add(st); }

function updateSummary() {
    document.getElementById('sum-guests').innerHTML = S.party ? `${S.party} ${S.party === 1 ? 'Guest' : 'Guests'}` : '<span class="empty">Not set</span>';
    document.getElementById('sum-date').innerHTML = S.date ? `${DAYS[S.date.getDay()]}, ${S.date.getDate()} ${MONTHS[S.date.getMonth()]}` : '<span class="empty">Not set</span>';
    document.getElementById('sum-time').innerHTML = S.time ? S.time : '<span class="empty">Not set</span>';
    document.getElementById('sum-dep').innerHTML = S.party ? `${(S.party * 50000).toLocaleString()} VND` : '<span class="empty">—</span>';
    if (S.party && S.dateStr && S.time) document.getElementById('proceed-btn').classList.add('ready');
}

async function goToPayment() {
    // Gom Occasion và Special Request thành Note
    const occasions = Array.from(document.querySelectorAll('.occ-chip.sel')).map(c => c.dataset.v);
    const specialReq = document.getElementById('guest-note').value.trim();

    let fullNote = "";
    if (occasions.length > 0) {
        fullNote = "Occasions: " + occasions.join(', ');
    }
    if (specialReq) {
        fullNote = fullNote ? fullNote + " | Notes: " + specialReq : specialReq;
    }

    const bookingData = {
        startTime: `${S.dateStr}T${S.time}:00`,
        guestCount: S.party,
        note: fullNote
    };

    // Gọi API Đặt bàn Online (Backend sẽ soft-lock bàn)
    const { ok, data } = await callApi('/reservations/online', 'POST', bookingData);
    if (!ok) {
        await showModal("Reservation Failed", data?.message || "The selected slot is no longer available. Please try another time.");
        return;
    }

    S.reservationId = data.reservationId;

    document.getElementById('booking-page').style.display = 'none';
    document.getElementById('payment-page').style.display = 'block';
    window.scrollTo(0, 0);

    startTimer();

    const dep = data.depositAmount || (S.party * 50000); // Backend dùng VND
    document.getElementById('p-guests').textContent = `Deposit (${S.party} guest${S.party > 1 ? 's' : ''} × ${dep / S.party} VND)`;
    document.getElementById('p-deposit').textContent = `${dep.toLocaleString()} VND`;
    document.getElementById('p-total').textContent = `${dep.toLocaleString()} VND`;
    if (S.date) document.getElementById('p-date').textContent = `${DAYS[S.date.getDay()]}, ${S.date.getDate()} ${MONTHS[S.date.getMonth()]}`;
    if (S.time) document.getElementById('p-time').textContent = S.time;
}

async function backToBooking() {
    if (S.reservationId) {
        const ok = await showModal("Table Locked", "Your table is still locked for you. Going back now will keep the timer running. Do you want to continue?", true, 'info');
        if (!ok) return;
    }
    stopTimer();
    document.getElementById('booking-page').style.display = 'block';
    document.getElementById('payment-page').style.display = 'none';
    window.scrollTo(0, 0);
}

async function cancelPayment() {
    if (!S.reservationId) return;
    const okConfirm = await showModal("Release Table", "Are you sure you want to cancel this reservation and release the table back to other guests?", true, 'danger');
    if (!okConfirm) return;

    try {
        const { ok, data } = await callApi(`/reservations/${S.reservationId}`, 'DELETE');
        if (ok) {
            await showModal("Cancelled", "Reservation has been cancelled and the table is now available.", false, 'success');
            S.reservationId = null;
            stopTimer();
            // Quay về bước 4
            document.getElementById('booking-page').style.display = 'block';
            document.getElementById('payment-page').style.display = 'none';
            window.scrollTo(0, 0);
        } else {
            await showModal("Error", data?.message || "Failed to cancel reservation.");
        }
    } catch (err) {
        await showModal("Error", "An unexpected error occurred. Please try again.");
    }
}

async function confirmReservation() {
    if (!S.reservationId) return;

    // Gọi API xác nhận thanh toán (Backend sẽ chốt bàn vào Mapping)
    const { ok, data } = await callApi(`/reservations/${S.reservationId}/payment/confirm`, 'POST');

    if (!ok) {
        await showModal("Payment Error", data?.message || "We could not confirm your payment. Please check your card details and try again.");
        return;
    }

    stopTimer();
    S.reservationId = null;
    document.getElementById('payment-page').style.display = 'none';
    const sp = document.getElementById('success-page'); sp.style.display = 'flex';
    window.scrollTo(0, 0);

    const refId = 'RS-' + S.reservationId;
    document.getElementById('success-ref').textContent = 'Reservation · ' + refId;
}

// ─── KHỞI TẠO DOM ───
document.addEventListener("DOMContentLoaded", async () => {
    // 1. Xác thực session thực tế với Backend (Quan trọng để tránh Stale UI)
    await initAuth();

    // 2. Sau khi initAuth hoàn tất, kiểm tra quyền truy cập để bảo vệ Route
    const user = getStoredUser();
    if (!user.customerId) {
        window.location.href = 'index.html';
        return;
    }

    // 3. Quản lý sự kiện cho My Bookings
    const viewBookingsLink = document.getElementById('view-bookings-link');
    if (viewBookingsLink) {
        viewBookingsLink.addEventListener('click', async (e) => {
            e.preventDefault();

            // Nếu đang giữ bàn, yêu cầu xác nhận trước khi mở "My Bookings"
            if (S.reservationId) {
                const ok = await showModal("Reservation Pending", "Leaving this page will cancel your current booking process. Do you wish to proceed?", true);
                if (!ok) return;
                await cancelPaymentDirectly();
            }

            openBookingsModal();
        });
    }

    const tabUpcoming = document.getElementById('tab-upcoming');
    const tabHistory = document.getElementById('tab-history');
    if (tabUpcoming) tabUpcoming.onclick = () => switchBookingTab('upcoming');
    if (tabHistory) tabHistory.onclick = () => switchBookingTab('history');

    const cancelPaymentDirectly = async () => {
        if (S.reservationId) {
            await callApi(`/reservations/${S.reservationId}`, 'DELETE');
            S.reservationId = null;
        }
    };

    const closeBookingsBtn = document.getElementById('close-bookings-btn');
    if (closeBookingsBtn) closeBookingsBtn.onclick = closeBookingsModal;

    // Gắn sự kiện các nút
    const pInput = document.getElementById('party-val');
    if (pInput) {
        pInput.addEventListener('input', () => setParty(pInput.value));
    }

    const minusBtn = document.getElementById('minus-btn');
    const plusBtn = document.getElementById('plus-btn');
    if (minusBtn) minusBtn.addEventListener('click', () => setParty(S.party - 1));
    if (plusBtn) plusBtn.addEventListener('click', () => setParty(S.party + 1));

    document.querySelectorAll('.party-chip').forEach(c => c.addEventListener('click', () => setParty(c.dataset.v)));

    const prevWeekBtn = document.getElementById('prev-week');
    const nextWeekBtn = document.getElementById('next-week');
    if (prevWeekBtn) prevWeekBtn.addEventListener('click', () => { S.week--; renderDates(); });
    if (nextWeekBtn) nextWeekBtn.addEventListener('click', () => { S.week++; renderDates(); });

    const cardNum = document.getElementById('card-num');
    if (cardNum) cardNum.addEventListener('input', function () { this.value = this.value.replace(/\D/g, '').replace(/(\d{4})(?=\d)/g, '$1 ').substring(0, 19); });

    const logoutBtn = document.getElementById('logout-btn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', async (e) => {
            e.preventDefault();
            if (S.reservationId) {
                const ok = await showModal("Sign Out", "Signing out will cancel your pending reservation. Process with logout?", true);
                if (!ok) return;
                await cancelPaymentDirectly();
            }
            clearUser();
            window.location.href = 'index.html';
        });
    }

    // (Combined with above listener)

    // Click listener cho Occasion chips
    document.querySelectorAll('.occ-chip').forEach(c => {
        c.addEventListener('click', () => {
            c.classList.toggle('sel');
        });
    });

    // Cài đặt trạng thái ban đầu
    setParty(2);
    updateSummary();
});