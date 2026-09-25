export function initBooking() {
    const bookingForm = document.getElementById('booking-form');
    const dateContainer = document.getElementById('date-container');
    const timeContainer = document.getElementById('time-slots-container');

    const hiddenParty = document.getElementById('selected-party');
    const hiddenDate = document.getElementById('res-date');
    const hiddenTime = document.getElementById('selected-time');

    if (!bookingForm) return;

    /* =========================================
       1. XỬ LÝ CHỌN SỐ NGƯỜI (COMBO QUICK SELECT & STEPPER)
       ========================================= */
    const partyInput = document.getElementById('party-input');
    const btnMinus = document.getElementById('btn-minus');
    const btnPlus = document.getElementById('btn-plus');
    const quickBtns = document.querySelectorAll('.quick-btn');

    // Hàm cập nhật giá trị tập trung
    function updateParty(val) {
        let n = parseInt(val) || 1;
        if (n < 1) n = 1;

        partyInput.value = n;
        hiddenParty.value = n;

        // Highlight nút chọn nhanh nếu khớp số
        quickBtns.forEach(btn => {
            if (parseInt(btn.dataset.value) === n) {
                btn.classList.add('selected');
            } else {
                btn.classList.remove('selected');
            }
        });
    }

    // Gắn sự kiện cho các nút chọn nhanh
    quickBtns.forEach(btn => {
        btn.addEventListener('click', () => updateParty(btn.dataset.value));
    });

    // Sự kiện nút Stepper
    btnMinus.addEventListener('click', () => updateParty(parseInt(partyInput.value) - 1));
    btnPlus.addEventListener('click', () => updateParty(parseInt(partyInput.value) + 1));
    partyInput.addEventListener('input', () => updateParty(partyInput.value));
    partyInput.addEventListener('blur', () => updateParty(partyInput.value));

    // Khởi tạo mặc định
    updateParty(2);

    /* =========================================
       3. RENDER NÚT CHỌN GIỜ
       ========================================= */
    const allTimeSlots = ["17:00", "17:30", "18:00", "18:30", "19:00", "19:30", "20:00", "20:30", "21:00", "21:30", "22:00"];

    function renderTimeSlots(dateStr) {
        timeContainer.innerHTML = '';
        hiddenTime.value = '';
        const seed = dateStr.charCodeAt(dateStr.length - 1) || 0;

        allTimeSlots.forEach((slot, index) => {
            const btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'time-slot';
            btn.innerText = slot;

            const isBooked = (seed + index) % 3 === 0 || (seed + index) % 5 === 0;

            if (isBooked) {
                btn.classList.add('booked');
                btn.disabled = true;
            } else {
                btn.addEventListener('click', () => {
                    document.querySelectorAll('.time-slot').forEach(el => el.classList.remove('selected'));
                    btn.classList.add('selected');
                    hiddenTime.value = slot;
                });
            }
            timeContainer.appendChild(btn);
        });
    }

    /* =========================================
       2. RENDER NÚT CHỌN NGÀY (CỐ ĐỊNH TỪ THỨ 2)
       ========================================= */
    const daysName = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    const monthsName = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    let currentWeekOffset = 0;
    const MAX_WEEKS = 4;

    const prevWeekBtn = document.getElementById('prev-week-btn');
    const nextWeekBtn = document.getElementById('next-week-btn');

    function getLocalDateStr(d) {
        return `${d.getFullYear()}-${String(d.getMonth()+1).padStart(2,'0')}-${String(d.getDate()).padStart(2,'0')}`;
    }

    const today = new Date(); today.setHours(0,0,0,0);
    const dayOfWeek = today.getDay();
    const daysToMonday = dayOfWeek === 0 ? 6 : dayOfWeek - 1;
    const mondayOfThisWeek = new Date(today);
    mondayOfThisWeek.setDate(today.getDate() - daysToMonday);

    function renderDates(offset) {
        if (!dateContainer) return;
        dateContainer.innerHTML = '';

        for (let i = 0; i < 7; i++) {
            const d = new Date(mondayOfThisWeek);
            d.setDate(d.getDate() + (offset * 7) + i);
            const dateStr = getLocalDateStr(d);
            const isToday = d.getTime() === today.getTime();
            const isPast = d.getTime() < today.getTime();

            const btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'pill-btn';
            btn.dataset.date = dateStr;
            if (isToday) btn.classList.add('is-today');

            btn.innerHTML = `<strong class="day-text" style="display:block; margin-bottom:4px;">${daysName[d.getDay()]}</strong><span style="font-size:13px;">${d.getDate()} ${monthsName[d.getMonth()]}</span>`;

            if (isPast) {
                btn.disabled = true; btn.style.opacity = '0.3'; btn.style.cursor = 'not-allowed';
            } else {
                btn.addEventListener('click', () => {
                    document.querySelectorAll('#date-container .pill-btn').forEach(el => el.classList.remove('selected'));
                    btn.classList.add('selected');
                    hiddenDate.value = dateStr;
                    renderTimeSlots(dateStr);
                });
            }
            if (hiddenDate.value === dateStr) btn.classList.add('selected');
            dateContainer.appendChild(btn);
        }
        prevWeekBtn.disabled = offset <= 0;
        nextWeekBtn.disabled = offset >= MAX_WEEKS;
    }

    if (prevWeekBtn && nextWeekBtn) {
        prevWeekBtn.addEventListener('click', () => { currentWeekOffset--; renderDates(currentWeekOffset); });
        nextWeekBtn.addEventListener('click', () => { currentWeekOffset++; renderDates(currentWeekOffset); });
    }

    renderDates(0);
    const todayBtn = dateContainer.querySelector(`[data-date="${getLocalDateStr(today)}"]`);
    if (todayBtn) todayBtn.click();

    bookingForm.addEventListener('submit', (e) => {
        e.preventDefault();

        // Kiểm tra checkbox đã đọc thông báo chưa (mặc dù HTML required đã lo một phần)
        const acceptNotice = document.getElementById('accept-notice');
        if (!acceptNotice.checked) {
            alert('Vui lòng xác nhận bạn đã đọc Thông báo từ nhà hàng.');
            return;
        }

        if (!hiddenTime.value) {
            alert('Vui lòng chọn khung giờ bạn muốn đặt!');
            return;
        }

        const guestName = document.getElementById('guest-name').value;
        const guestPhone = document.getElementById('guest-phone').value;

        // Giả lập lưu dữ liệu booking vào localStorage để trang Payment có thể đọc
        const bookingData = {
            partySize: hiddenParty.value,
            date: hiddenDate.value,
            time: hiddenTime.value,
            guestName: guestName,
            guestPhone: guestPhone
        };
        localStorage.setItem('currentBooking', JSON.stringify(bookingData));

        // Chuyển hướng sang trang thanh toán
        window.location.href = "payment.html"; // Bạn sẽ cần tạo file này
    });
}