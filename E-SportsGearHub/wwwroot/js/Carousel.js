/* cyberpunk-carousel.js - JavaScript for the cyberpunk theme carousel */

document.addEventListener('DOMContentLoaded', function () {
    // Initialize the carousel functionality
    initCyberpunkCarousel();

    // Add hover effects to product cards
    initProductCardEffects();
});

/**
 * Initialize the cyberpunk carousel with navigation and auto-rotation
 */
function initCyberpunkCarousel() {
    const carousel = document.querySelector('.cyberpunk-carousel');

    // If no carousel exists on the page, exit function
    if (!carousel) return;

    const carouselItems = carousel.querySelectorAll('.carousel-item');
    const prevButton = carousel.querySelector('.prev');
    const nextButton = carousel.querySelector('.next');
    const indicators = carousel.querySelectorAll('.carousel-indicators button');
    let currentIndex = 0;
    let autoRotateInterval;

    // Update the active slide and indicators
    function updateCarousel() {
        carouselItems.forEach(item => item.classList.remove('active'));
        indicators.forEach(indicator => indicator.classList.remove('active'));

        carouselItems[currentIndex].classList.add('active');
        indicators[currentIndex].classList.add('active');
    }

    // Previous button click handler
    prevButton.addEventListener('click', function (e) {
        e.preventDefault();
        currentIndex = (currentIndex - 1 + carouselItems.length) % carouselItems.length;
        updateCarousel();
        resetAutoRotate();
    });

    // Next button click handler
    nextButton.addEventListener('click', function (e) {
        e.preventDefault();
        currentIndex = (currentIndex + 1) % carouselItems.length;
        updateCarousel();
        resetAutoRotate();
    });

    // Indicator button click handlers
    indicators.forEach((indicator, index) => {
        indicator.addEventListener('click', function () {
            currentIndex = index;
            updateCarousel();
            resetAutoRotate();
        });
    });

    // Start auto-rotation
    function startAutoRotate() {
        autoRotateInterval = setInterval(() => {
            currentIndex = (currentIndex + 1) % carouselItems.length;
            updateCarousel();
        }, 5000); // Rotate every 5 seconds
    }

    // Reset auto-rotation (called after manual navigation)
    function resetAutoRotate() {
        clearInterval(autoRotateInterval);
        startAutoRotate();
    }

    // Add glitch effect on caption text
    const glitchTexts = carousel.querySelectorAll('.glitch-text');
    glitchTexts.forEach(text => {
        // Set data-text attribute for pseudo-elements
        text.setAttribute('data-text', text.textContent);

        // Random glitch effect
        setInterval(() => {
            const shouldGlitch = Math.random() > 0.9;
            if (shouldGlitch) {
                text.classList.add('glitching');
                setTimeout(() => {
                    text.classList.remove('glitching');
                }, 200);
            }
        }, 2000);
    });

    // Pause auto-rotation when hovering over the carousel
    carousel.addEventListener('mouseenter', () => {
        clearInterval(autoRotateInterval);
    });

    // Resume auto-rotation when mouse leaves the carousel
    carousel.addEventListener('mouseleave', () => {
        startAutoRotate();
    });

    // Start auto-rotation on page load
    startAutoRotate();
}

/**
 * Initialize hover effects for product cards
 */
function initProductCardEffects() {
    const productCards = document.querySelectorAll('.product-card');

    productCards.forEach(card => {
        // Create neon glow effect on hover
        card.addEventListener('mouseenter', function () {
            const randomColor = getRandomNeonColor();
            this.style.boxShadow = `0 0 20px ${randomColor}`;
            this.style.borderColor = randomColor;
        });

        card.addEventListener('mouseleave', function () {
            this.style.boxShadow = '';
            this.style.borderColor = '';
        });

        // Add click event to cards (optional, for mobile users)
        card.addEventListener('click', function (e) {
            // Only trigger if the click was on the card itself, not on buttons
            if (e.target === this || e.target.classList.contains('product-img') ||
                e.target.classList.contains('product-name') ||
                e.target.classList.contains('product-desc') ||
                e.target.classList.contains('product-price')) {

                const viewBtn = this.querySelector('.view-btn');
                if (viewBtn) {
                    viewBtn.click();
                }
            }
        });
    });

    // Add glitch effect to buttons on hover
    const cyberButtons = document.querySelectorAll('.cyber-btn');
    cyberButtons.forEach(btn => {
        btn.addEventListener('mouseenter', function () {
            this.classList.add('btn-glitch');
        });

        btn.addEventListener('mouseleave', function () {
            this.classList.remove('btn-glitch');
        });
    });
}

/**
 * Helper function to get random neon color
 */
function getRandomNeonColor() {
    const neonColors = [
        'rgba(0, 255, 255, 0.7)',  // Cyan
        'rgba(255, 0, 255, 0.7)',  // Magenta
        'rgba(255, 255, 0, 0.7)',  // Yellow
        'rgba(0, 255, 0, 0.7)',    // Green
        'rgba(255, 0, 128, 0.7)'   // Pink
    ];

    return neonColors[Math.floor(Math.random() * neonColors.length)];
}

/**
 * Add to cart functionality with animation
 */
function addToCart(button, productId) {
    const form = button.closest('form');

    // Create a glowing effect when adding to cart
    button.classList.add('adding');

    setTimeout(() => {
        form.submit();
    }, 300);

    return false;
}