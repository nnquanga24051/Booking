document.addEventListener('DOMContentLoaded', function () {
    const contactForm = document.getElementById('contactForm');
    const formMessage = document.getElementById('formMessage');
    const faqItems = document.querySelectorAll('.faq-item');
    const chatBtn = document.querySelector('.chat-btn');

    if (contactForm) {
        contactForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const fullName = document.getElementById('fullName').value;
            const email = document.getElementById('email').value;
            const phone = document.getElementById('phone').value;
            const message = document.getElementById('message').value;

            if (!fullName || !email || !phone || !message) {
                showMessage('Vui lòng điền đầy đủ thông tin bắt buộc!', 'error');
                return;
            }

            if (!validateEmail(email)) {
                showMessage('Email không hợp lệ!', 'error');
                return;
            }

            if (!validatePhone(phone)) {
                showMessage('Số điện thoại không hợp lệ!', 'error');
                return;
            }

            const submitBtn = contactForm.querySelector('.submit-btn');
            submitBtn.disabled = true;
            submitBtn.textContent = '⏳ Đang gửi...';

            setTimeout(() => {
                showMessage('Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi trong thời gian sớm nhất.', 'success');
                contactForm.reset();
                submitBtn.disabled = false;
                submitBtn.textContent = '📤 Gửi Tin Nhắn';
            }, 2000);
        });
    }

    function validateEmail(email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    }

    function validatePhone(phone) {
        const re = /^[0-9]{10,11}$/;
        return re.test(phone.replace(/\s/g, ''));
    }

    function showMessage(text, type) {
        formMessage.textContent = text;
        formMessage.className = 'form-message ' + type;
        formMessage.style.display = 'block';

        setTimeout(() => {
            formMessage.style.display = 'none';
        }, 5000);
    }

    faqItems.forEach(item => {
        const question = item.querySelector('.faq-question');

        question.addEventListener('click', function () {
            const isActive = item.classList.contains('active');

            faqItems.forEach(faq => faq.classList.remove('active'));

            if (!isActive) {
                item.classList.add('active');
            }
        });
    });

    if (chatBtn) {
        chatBtn.addEventListener('click', function () {
            alert('Tính năng chat trực tuyến sẽ sớm được ra mắt!\n\nHiện tại bạn có thể liên hệ qua:\n📞 Hotline: +84 123 456 789\n📧 Email: booking@gmail.com');
        });
    }

    const observerOptions = {
        threshold: 0.15,
        rootMargin: '0px 0px -50px 0px'
    };

    const animateOnScroll = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '0';
                entry.target.style.transform = 'translateY(30px)';

                setTimeout(() => {
                    entry.target.style.transition = 'opacity 0.8s ease, transform 0.8s ease';
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }, 100);

                animateOnScroll.unobserve(entry.target);
            }
        });
    }, observerOptions);

    const animatedElements = document.querySelectorAll(
        '.contact-info-card, .contact-form, .info-box, .map-card'
    );

    animatedElements.forEach(el => animateOnScroll.observe(el));

    const infoCards = document.querySelectorAll('.contact-info-card');
    infoCards.forEach((card, index) => {
        card.style.animationDelay = `${index * 0.15}s`;
    });

    const phoneInput = document.getElementById('phone');
    if (phoneInput) {
        phoneInput.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, '');
            if (value.length > 11) {
                value = value.slice(0, 11);
            }
            e.target.value = value;
        });
    }

    const nameInput = document.getElementById('fullName');
    if (nameInput) {
        nameInput.addEventListener('input', function (e) {
            e.target.value = e.target.value.replace(/[^a-zA-ZÀ-ỹ\s]/g, '');
        });
    }

    const formInputs = document.querySelectorAll('.field-input, .field-textarea');
    formInputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.parentElement.style.transform = 'translateX(5px)';
        });

        input.addEventListener('blur', function () {
            this.parentElement.style.transform = 'translateX(0)';
        });
    });
});