// ==========================================
// MOBILE MENU TOGGLE
// ==========================================
document.addEventListener('DOMContentLoaded', function () {
    const mobileMenuToggle = document.getElementById('mobileMenuToggle');
    const mainNav = document.getElementById('mainNav');

    if (mobileMenuToggle && mainNav) {
        mobileMenuToggle.addEventListener('click', function () {
            this.classList.toggle('active');
            mainNav.classList.toggle('active');
        });

        // Close mobile menu when clicking on a nav link
        const navLinks = mainNav.querySelectorAll('.nav-link');
        navLinks.forEach(link => {
            link.addEventListener('click', function () {
                mobileMenuToggle.classList.remove('active');
                mainNav.classList.remove('active');
            });
        });

        // Close mobile menu when clicking outside
        document.addEventListener('click', function (event) {
            const isClickInsideNav = mainNav.contains(event.target);
            const isClickOnToggle = mobileMenuToggle.contains(event.target);

            if (!isClickInsideNav && !isClickOnToggle && mainNav.classList.contains('active')) {
                mobileMenuToggle.classList.remove('active');
                mainNav.classList.remove('active');
            }
        });
    }

    // ==========================================
    // USER DROPDOWN MENU
    // ==========================================
    const userButton = document.getElementById('userButton');
    const userDropdown = document.getElementById('userDropdown');

    if (userButton && userDropdown) {
        userButton.addEventListener('click', function (event) {
            event.stopPropagation();
            this.classList.toggle('active');
            userDropdown.classList.toggle('active');
        });

        // Close dropdown when clicking outside
        document.addEventListener('click', function (event) {
            if (!userButton.contains(event.target) && !userDropdown.contains(event.target)) {
                userButton.classList.remove('active');
                userDropdown.classList.remove('active');
            }
        });

        // Prevent dropdown from closing when clicking inside
        userDropdown.addEventListener('click', function (event) {
            event.stopPropagation();
        });
    }

    // ==========================================
    // ACTIVE LINK HIGHLIGHT
    // ==========================================
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.nav-link');

    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.style.color = '#e0f7ff';
            link.style.fontWeight = '600';
        }
    });
});


            // ==========================================
// FORCE FOOTER TO BOTTOM - BACKUP SOLUTION
// ==========================================
function forceFooterToBottom() {
    const body = document.body;
    const html = document.documentElement;
    
    // Ensure body uses flexbox
    body.style.display = 'flex';
    body.style.flexDirection = 'column';
    body.style.minHeight = '100vh';
    body.style.margin = '0';
    body.style.padding = '0';
    
    // Ensure html has full height
    html.style.height = '100%';
    html.style.margin = '0';
    html.style.padding = '0';
    
    // Get elements
    const header = document.querySelector('.header');
    const mainContent = document.querySelector('.main-content');
    const footer = document.querySelector('.footer');
    
    if (header && mainContent && footer) {
        // Set flex properties
        header.style.flexShrink = '0';
        mainContent.style.flex = '1';
        footer.style.flexShrink = '0';
        footer.style.marginTop = 'auto';
        footer.style.width = '100%';
    }
}

// Run on page load
document.addEventListener('DOMContentLoaded', function() {
    // Force footer positioning
    forceFooterToBottom();
    
    // Run again after a short delay to catch any dynamic content
    setTimeout(forceFooterToBottom, 100);
    setTimeout(forceFooterToBottom, 500);
    
    // MOBILE MENU TOGGLE
    const mobileMenuToggle = document.getElementById('mobileMenuToggle');
    const mainNav = document.getElementById('mainNav');

    if (mobileMenuToggle && mainNav) {
        mobileMenuToggle.addEventListener('click', function() {
            this.classList.toggle('active');
            mainNav.classList.toggle('active');
        });

        // Close mobile menu when clicking on a nav link
        const navLinks = mainNav.querySelectorAll('.nav-link');
        navLinks.forEach(link => {
            link.addEventListener('click', function() {
                mobileMenuToggle.classList.remove('active');
                mainNav.classList.remove('active');
            });
        });

        // Close mobile menu when clicking outside
        document.addEventListener('click', function(event) {
            const isClickInsideNav = mainNav.contains(event.target);
            const isClickOnToggle = mobileMenuToggle.contains(event.target);
            
            if (!isClickInsideNav && !isClickOnToggle && mainNav.classList.contains('active')) {
                mobileMenuToggle.classList.remove('active');
                mainNav.classList.remove('active');
            }
        });
    }

    // ==========================================
    // USER DROPDOWN MENU
    // ==========================================
    const userButton = document.getElementById('userButton');
    const userDropdown = document.getElementById('userDropdown');

    if (userButton && userDropdown) {
        userButton.addEventListener('click', function(event) {
            event.stopPropagation();
            this.classList.toggle('active');
            userDropdown.classList.toggle('active');
        });

        // Close dropdown when clicking outside
        document.addEventListener('click', function(event) {
            if (!userButton.contains(event.target) && !userDropdown.contains(event.target)) {
                userButton.classList.remove('active');
                userDropdown.classList.remove('active');
            }
        });

        // Prevent dropdown from closing when clicking inside
        userDropdown.addEventListener('click', function(event) {
            event.stopPropagation();
        });
    }

    // ==========================================
    // ACTIVE LINK HIGHLIGHT
    // ==========================================
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.nav-link');

    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.style.color = '#e0f7ff';
            link.style.fontWeight = '600';
        }
    });
});

// Run on window resize
window.addEventListener('resize', forceFooterToBottom);

// Run on window load (after all images and resources)
window.addEventListener('load', forceFooterToBottom);