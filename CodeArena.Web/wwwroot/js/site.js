toastr.options = {
    "positionClass": "toast-bottom-right"
};

const toggleBtn = document.getElementById("themeToggle");
const body = document.getElementById("appBody");
const icon = toggleBtn?.querySelector("i");

if (localStorage.getItem("theme") === "dark") {
    body.classList.add("dark-mode");
    if (icon) icon.classList.replace("bi-moon", "bi-sun");
}

toggleBtn?.addEventListener("click", () => {
    body.classList.toggle("dark-mode");

    const isDark = body.classList.contains("dark-mode");
    document.getElementById('hljs-theme-dark').disabled = !isDark;
    localStorage.setItem("theme", isDark ? "dark" : "light");

    if (icon) {
        icon.classList.toggle("bi-moon");
        icon.classList.toggle("bi-sun");
    }
});
