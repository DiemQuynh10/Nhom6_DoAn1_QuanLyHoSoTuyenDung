// Khi DOM sẵn sàng => thêm class để fade-in (nếu bạn đã thêm transition fade)
document.addEventListener('DOMContentLoaded', () => {
    document.body.classList.add('page-loaded');

    // Toggle mật khẩu: click vào icon khóa để show/hide
    document.querySelectorAll('body.login-page .input-group.password .icon').forEach(icon => {
        icon.addEventListener('click', () => {
            const input = icon.parentElement.querySelector('input');
            if (!input || input.type !== 'password' && input.type !== 'text') return;
            if (input.type === 'password') {
                input.type = 'text';
                icon.textContent = '🙈';  // biểu tượng khi đang hiện password
            } else {
                input.type = 'password';
                icon.textContent = '🔒';  // biểu tượng khi đang ẩn password
            }
        });
    });
});

// Nếu bạn dùng fade transition, cũng có thể thêm đoạn này:
window.addEventListener('beforeunload', () => {
    document.body.classList.remove('page-loaded');
    document.body.classList.add('page-exiting');
});
