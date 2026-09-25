import { callApi, saveUser, getStoredUser, clearUser } from '../common/api.js';
import { showToast } from '../common/utils.js';
import { updateHeaderAuthUI } from '../components/header.js';

export async function initProfile() {
    // 1. Kiểm tra đăng nhập
    const { ok, data: userData, status } = await callApi("/auth/me", "GET");
    
    if (!ok) {
        if (status === 401 || status === 403) {
            clearUser();
            window.location.href = "index.html";
            return;
        }
        showToast("Unable to load profile data.", "error");
    }

    // Cập nhật local storage và Header
    if (userData) {
        saveUser(userData);
        updateHeaderAuthUI(true);
        populateProfileForm(userData);
    }

    // 2. Xử lý Chuyển Tab
    initTabs();

    // 3. Xử lý Gender Pills
    initGenderPills();

    // 4. Xử lý Form Profile Update
    const profileForm = document.getElementById('edit-profile-form');
    if (profileForm) {
        profileForm.addEventListener('submit', handleProfileUpdate);
    }

    // 5. Xử lý Form Password Change
    const passwordForm = document.getElementById('change-password-form');
    if (passwordForm) {
        passwordForm.addEventListener('submit', handlePasswordChange);
        initPasswordStrength();
    }
}

function populateProfileForm(user) {
    document.getElementById('full-name').value = user.name || "";
    document.getElementById('phone').value = user.phone || "";
    document.getElementById('email').value = user.email || "";
    document.getElementById('dob').value = user.dateOfBirth || "";
    
    // Set gender pills
    const gender = user.gender || "Male";
    const pills = document.querySelectorAll('#gender-pills-new .pill-flat');
    pills.forEach(pill => {
        if (pill.innerText.trim() === gender) {
            pill.classList.add('active');
        } else {
            pill.classList.remove('active');
        }
    });
}

function initTabs() {
    const tabs = document.querySelectorAll('.profile-tab');
    const panes = document.querySelectorAll('.tab-pane');

    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            const targetId = tab.getAttribute('data-tab');
            
            tabs.forEach(t => t.classList.remove('active'));
            panes.forEach(p => p.classList.remove('active'));

            tab.classList.add('active');
            document.getElementById(targetId).classList.add('active');
        });
    });
}

function initGenderPills() {
    const pills = document.querySelectorAll('#gender-pills-new .pill-flat');
    pills.forEach(pill => {
        pill.addEventListener('click', () => {
            pills.forEach(p => p.classList.remove('active'));
            pill.classList.add('active');
        });
    });
}

async function handleProfileUpdate(e) {
    e.preventDefault();
    const btnSave = document.getElementById('btn-save');
    const originalText = btnSave.innerText;
    
    const name = document.getElementById('full-name').value.trim();
    const phone = document.getElementById('phone').value.trim();
    const emailInput = document.getElementById('email').value.trim();
    const dob = document.getElementById('dob').value;
    const gender = document.querySelector('#gender-pills-new .pill-flat.active').innerText.trim();

    if (!name || !phone || !emailInput) {
        showToast("Please fill in all required fields.", "error");
        return;
    }

    const currentEmail = getStoredUser().email;
    const emailChanged = emailInput.toLowerCase() !== currentEmail.toLowerCase();

    btnSave.disabled = true;
    btnSave.innerText = "Saving...";

    try {
        const { ok, data, status } = await callApi("/auth/profile", "PUT", {
            name, phone, email: emailInput, dateOfBirth: dob || null, gender
        });

        if (ok) {
            saveUser(data);
            
            if (emailChanged) {
                // Hiển thị thông báo chờ xác thực
                const verifyToast = showToast(
                    "<strong>Xác thực email mới:</strong> Vui lòng kiểm tra hộp thư của bạn và nhấn vào link xác nhận. <br><small>Hệ thống sẽ tự động cập nhật khi bạn hoàn tất.</small>", 
                    "info", 
                    0
                );

                // Bắt đầu polling chờ xác thực
                const pollInterval = setInterval(async () => {
                    const { ok: meOk, data: meData } = await callApi("/auth/me", "GET");
                    if (meOk && meData.email.toLowerCase() === emailInput.toLowerCase()) {
                        clearInterval(pollInterval);
                        saveUser(meData);
                        verifyToast.update("🎉 Xác thực thành công! Thông tin cá nhân đã được cập nhật.", "success");
                        setTimeout(() => verifyToast.close(), 4000);
                    }
                }, 3000);

                // Dừng polling sau 10 phút
                setTimeout(() => clearInterval(pollInterval), 10 * 60 * 1000);

            } else {
                showToast("Profile updated successfully!", "success");
            }
            setTimeout(() => clearValidationClasses(profileForm), 3000);
        } else {
            showToast(data?.message || "Failed to update profile.", "error");
        }
    } catch (err) {
        showToast("Connection error.", "error");
    } finally {
        btnSave.disabled = false;
        btnSave.innerText = originalText;
    }
}

async function handlePasswordChange(e) {
    e.preventDefault();
    const btnUpdate = document.getElementById('btn-update-pwd');
    const originalText = btnUpdate.innerText;
    const passwordForm = document.getElementById('change-password-form');

    const currentPassword = document.getElementById('current-password').value;
    const newPassword = document.getElementById('new-password').value;
    const confirmPassword = document.getElementById('confirm-password').value;

    if (newPassword !== confirmPassword) {
        showToast("Passwords do not match.", "error");
        return;
    }

    btnUpdate.disabled = true;
    btnUpdate.innerText = "Updating...";

    try {
        const { ok, data } = await callApi("/auth/password", "PUT", {
            currentPassword, newPassword
        });

        if (ok) {
            showToast("Password changed successfully!", "success");
            passwordForm.reset();
            // Reset validation states
            clearValidationClasses(passwordForm);
            document.querySelector('.pwd-strength-container').classList.remove('visible');
            document.getElementById('confirm-hint').textContent = "";
        } else {
            showToast(data?.message || "Failed to change password.", "error");
        }
    } catch (err) {
        showToast("Connection error.", "error");
    } finally {
        btnUpdate.disabled = false;
        btnUpdate.innerText = originalText;
    }
}

function clearValidationClasses(form) {
    form.querySelectorAll('.flat-input').forEach(input => {
        input.classList.remove('is-valid', 'is-invalid');
    });
}

function initPasswordStrength() {
    const newPwdInput = document.getElementById('new-password');
    const confirmPwdInput = document.getElementById('confirm-password');
    const strengthContainer = document.querySelector('.pwd-strength-container');
    const strengthLabel = document.querySelector('.strength-label');

    newPwdInput.addEventListener('input', () => {
        const val = newPwdInput.value;
        if (!val) {
            strengthContainer.classList.remove('visible');
            return;
        }
        strengthContainer.classList.add('visible');
        let strength = 0;
        if (val.length >= 6) strength++;
        if (/[A-Z]/.test(val)) strength++;
        if (/[0-9]/.test(val)) strength++;
        if (/[^A-Za-z0-9]/.test(val)) strength++;

        strengthContainer.className = 'pwd-strength-container visible';
        if (strength <= 1) {
            strengthContainer.classList.add('strength-weak');
            strengthLabel.textContent = 'Weak';
        } else if (strength <= 3) {
            strengthContainer.classList.add('strength-medium');
            strengthLabel.textContent = 'Medium';
        } else {
            strengthContainer.classList.add('strength-strong');
            strengthLabel.textContent = 'Strong';
        }
    });

    confirmPwdInput.addEventListener('input', () => {
        const confirmHint = document.getElementById('confirm-hint');
        if (confirmPwdInput.value === newPwdInput.value && confirmPwdInput.value !== "") {
            confirmPwdInput.classList.add('is-valid');
            confirmPwdInput.classList.remove('is-invalid');
            confirmHint.textContent = "Passwords match ✓";
            confirmHint.className = "validation-hint success";
        } else {
            confirmPwdInput.classList.add('is-invalid');
            confirmPwdInput.classList.remove('is-valid');
            confirmHint.textContent = "Passwords do not match";
            confirmHint.className = "validation-hint error";
        }
    });
}
