/**
 * THEME MANAGER — San-Lorenzo
 * Logic for initializing and switching Dark/Light modes.
 */

const THEME_KEY = 'sl-theme';

/**
 * getInitialTheme()
 * Priority: LocalStorage -> System Preference -> Default (Dark)
 */
function getInitialTheme() {
    const saved = localStorage.getItem(THEME_KEY);
    if (saved) return saved;

    const preferLight = window.matchMedia('(prefers-color-scheme: light)').matches;
    return preferLight ? 'light' : 'dark';
}

/**
 * applyTheme(theme)
 * Updates the <body> classes, UI icon, and aria-labels.
 */
function applyTheme(theme) {
    const isLight = theme === 'light';
    if (isLight) {
        document.body.classList.add('light-mode');
    } else {
        document.body.classList.remove('light-mode');
    }

    // Update icon and aria-label for the toggle button
    const btn = document.getElementById('theme-toggle');
    if (btn) {
        // Handle both cases: a <span> themed icon or just text content
        const emoji = isLight ? '☀️' : '🌙';
        const iconSpan = btn.querySelector('.theme-icon');
        if (iconSpan) {
            iconSpan.textContent = emoji;
        } else {
            btn.textContent = emoji;
        }
        
        const label = isLight ? 'Switch to dark mode' : 'Switch to light mode';
        btn.setAttribute('aria-label', label);
        btn.title = label;

        if (isLight) {
            btn.classList.add('is-light');
        } else {
            btn.classList.remove('is-light');
        }
    }

    // Persist Choice
    localStorage.setItem(THEME_KEY, theme);

    // Remove the early light mode class from html if it exists
    document.documentElement.classList.remove('light-mode-early');
}

/**
 * toggleTheme()
 * Main interaction for the toggle button.
 */
function toggleTheme() {
    const isLight = document.body.classList.contains('light-mode');
    const targetTheme = isLight ? 'dark' : 'light';
    applyTheme(targetTheme);
}

/**
 * initTheme()
 * Called on startup.
 */
function initTheme() {
    const theme = getInitialTheme();
    applyTheme(theme);

    // Attach click listener to toggle button if it exists
    const btn = document.getElementById('theme-toggle');
    if (btn) {
        // Remove old listener if any to avoid duplicates (though rare here)
        btn.onclick = toggleTheme; 
    }

    // Observer: Change theme automatically if user has NO manual preference
    window.matchMedia('(prefers-color-scheme: light)').addEventListener('change', (e) => {
        if (!localStorage.getItem(THEME_KEY)) {
            applyTheme(e.matches ? 'light' : 'dark');
        }
    });
}

// Attach toggleTheme to window for the onclick handler
window.toggleTheme = toggleTheme;

// Initialize on DOM Content Loaded
document.addEventListener('DOMContentLoaded', initTheme);
