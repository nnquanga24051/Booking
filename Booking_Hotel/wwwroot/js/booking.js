document.addEventListener('DOMContentLoaded', function () {
    const filterForm = document.getElementById('filterForm');
    const sortSelect = document.getElementById('sortBy');
    const roomsList = document.querySelector('.rooms-list');
    const checkinInput = document.querySelector('input[name="checkin"]');
    const checkoutInput = document.querySelector('input[name="checkout"]');

    const today = new Date().toISOString().split('T')[0];
    if (checkinInput) {
        checkinInput.setAttribute('min', today);
    }

    if (checkinInput && checkoutInput) {
        checkinInput.addEventListener('change', function () {
            const checkinDate = new Date(this.value);
            const nextDay = new Date(checkinDate);
            nextDay.setDate(nextDay.getDate() + 1);
            const minCheckout = nextDay.toISOString().split('T')[0];
            checkoutInput.setAttribute('min', minCheckout);

            if (checkoutInput.value && checkoutInput.value <= this.value) {
                checkoutInput.value = minCheckout;
            }
        });
    }

    if (sortSelect && roomsList) {
        sortSelect.addEventListener('change', function () {
            const rooms = Array.from(roomsList.querySelectorAll('.room-item'));
            const sortValue = this.value;

            rooms.sort((a, b) => {
                switch (sortValue) {
                    case 'price-asc':
                        return getPriceValue(a) - getPriceValue(b);
                    case 'price-desc':
                        return getPriceValue(b) - getPriceValue(a);
                    case 'name':
                        return getRoomName(a).localeCompare(getRoomName(b));
                    default:
                        return 0;
                }
            });

            rooms.forEach(room => roomsList.appendChild(room));
        });
    }

    function getPriceValue(roomElement) {
        const priceText = roomElement.querySelector('.price-amount').textContent;
        return parseInt(priceText.replace(/[^\d]/g, ''));
    }

    function getRoomName(roomElement) {
        return roomElement.querySelector('.room-title').textContent;
    }

    if (filterForm) {
        filterForm.addEventListener('reset', function () {
            setTimeout(() => {
                if (checkinInput) checkinInput.removeAttribute('value');
                if (checkoutInput) checkoutInput.removeAttribute('value');
            }, 0);
        });
    }

    const pageButtons = document.querySelectorAll('.page-btn:not(.disabled)');
    pageButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            if (!this.classList.contains('active') && !this.classList.contains('disabled')) {
                pageButtons.forEach(b => b.classList.remove('active'));

                if (!this.textContent.includes('Trước') && !this.textContent.includes('Sau')) {
                    this.classList.add('active');
                }

                window.scrollTo({
                    top: 0,
                    behavior: 'smooth'
                });
            }
        });
    });

    const roomItems = document.querySelectorAll('.room-item');
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '0';
                entry.target.style.transform = 'translateY(20px)';

                setTimeout(() => {
                    entry.target.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }, 100);

                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    roomItems.forEach(item => observer.observe(item));
});