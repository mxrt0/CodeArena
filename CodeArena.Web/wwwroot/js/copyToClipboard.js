const copyBtn = document.querySelector('.copy-btn');
if (copyBtn) {
    copyBtn.addEventListener('click', () => {
        const code = copyBtn.parentElement.querySelector('code').innerText;

        navigator.clipboard.writeText(code);
        toastr.success("Copied!", "", {
            timeOut: 1200,
            progressBar: false,
            closeButton: false
        });
        copyBtn.innerHTML = '<i class="bi bi-check"></i>';
        copyBtn.disabled = true;
        setTimeout(() => {
            copyBtn.innerHTML = '<i class="bi bi-clipboard"></i>';
            copyBtn.disabled = false;
        }, 1300);
    })
}
