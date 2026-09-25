// Khởi tạo hiệu ứng scroll (fade-in -> appear)
export function initAnimations() {
    const fadeElements = document.querySelectorAll(".fade-in");
    const appearOptions = { threshold: 0.2, rootMargin: "0px 0px -50px 0px" };

    const appearOnScroll = new IntersectionObserver(function(entries, observer) {
        entries.forEach(entry => {
            if (!entry.isIntersecting) return;
            entry.target.classList.add("appear");
            observer.unobserve(entry.target);
        });
    }, appearOptions);

    fadeElements.forEach(el => appearOnScroll.observe(el));
}

// Khởi tạo chức năng Đóng/Mở con mắt (Password)
export function initPasswordToggle() {
    const togglePasswordBtns = document.querySelectorAll(".toggle-password");

    const eyeOpenSvg = `<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"/><circle cx="12" cy="12" r="3"/></svg>`;
    const eyeClosedSvg = `<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M9.88 9.88a3 3 0 1 0 4.24 4.24"/><path d="M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68"/><path d="M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61"/><line x1="2" y1="2" x2="22" y2="22"/></svg>`;

    togglePasswordBtns.forEach(btn => {
        btn.addEventListener("click", function() {
            const input = this.previousElementSibling;
            if (input.type === "password") {
                input.type = "text";
                this.innerHTML = eyeClosedSvg;
            } else {
                input.type = "password";
                this.innerHTML = eyeOpenSvg;
            }
        });
    });
}

/**
 * Hiển thị thông báo Toast chuyên nghiệp
 */
export function showToast(message, type = 'info', duration = 4000) {
    let container = document.querySelector('.toast-container');
    if (!container) {
        container = document.createElement('div');
        container.className = 'toast-container';
        document.body.appendChild(container);
    }
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `<div class="toast-content">${message}</div><button class="toast-close">&times;</button>`;
    container.appendChild(toast);
    
    const closeToast = () => {
        toast.classList.add('toast-out');
        toast.addEventListener('animationend', () => toast.remove());
    };
    
    toast.querySelector('.toast-close').onclick = closeToast;
    if (duration > 0) setTimeout(closeToast, duration);

    return {
        element: toast,
        close: closeToast,
        update: (newMessage, newType) => {
            if (newMessage) toast.querySelector('.toast-content').innerHTML = newMessage;
            if (newType) {
                toast.className = `toast toast-${newType}`;
            }
        }
    };
}