toastr.options = {
    "positionClass": "toast-bottom-right"
};

const toggleBtn = document.getElementById("themeToggle");
const body = document.getElementById("appBody");
const icon = toggleBtn?.querySelector("i");
const hljsDarkTheme = document.getElementById('hljs-theme-dark');

if (localStorage.getItem("theme") === "dark") {
    body.classList.add("dark-mode");
    if (hljsDarkTheme) {
        hljsDarkTheme.disabled = false;
    }
    if (icon) icon.classList.replace("bi-moon", "bi-sun");
}

toggleBtn?.addEventListener("click", () => {
    body.classList.toggle("dark-mode");
    const isDark = body.classList.contains("dark-mode");

    if (hljsDarkTheme) {
        hljsDarkTheme.disabled = !isDark;
    }

    localStorage.setItem("theme", isDark ? "dark" : "light");
    const themeEvent = new CustomEvent("themeChanged", { detail: { isDark: isDark } });
    document.dispatchEvent(themeEvent);
    if (icon) {
        icon.classList.toggle("bi-moon");
        icon.classList.toggle("bi-sun");
    }
});

document.querySelectorAll('#filter-form select').forEach(el => {
    el.addEventListener('change', () => document.getElementById('filter-form')?.submit());
});

document.querySelectorAll('.tag-chip input[type="checkbox"]').forEach(el => {
    el.addEventListener('change', () => document.getElementById('filter-form')?.submit());
});

const searchInput = document.querySelector('#filter-form input[type="text"]');
let debounceTimer;
searchInput?.addEventListener('input', () => {
    clearTimeout(debounceTimer);
    debounceTimer = setTimeout(() => document.getElementById('filter-form')?.submit(), 400);
});
