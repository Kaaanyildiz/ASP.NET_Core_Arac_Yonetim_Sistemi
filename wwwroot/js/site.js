// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Sayfa Yükleme Animasyonu
document.addEventListener('DOMContentLoaded', function() {
    // Sayfa yükleme animasyonu
    const body = document.querySelector('body');
    if (body) {
        body.classList.add('loaded');
    }
    
    // Sayfanın tüm içeriği yüklendiğinde loader'ı gizle
    window.addEventListener('load', function() {
        const loader = document.getElementById('page-loader');
        if (loader) {
            setTimeout(function() {
                loader.style.opacity = '0';
                setTimeout(function() {
                    loader.style.display = 'none';
                }, 500);
            }, 500);
        }
    });
    
    // Navbar scroll efekti
    const navbar = document.getElementById('mainNav');
    if (navbar) {
        window.addEventListener('scroll', function() {
            if (window.scrollY > 50) {
                navbar.classList.add('navbar-scrolled');
            } else {
                navbar.classList.remove('navbar-scrolled');
            }
        });
    }
    
    // Sayfa içi link tıklamaları için smooth scroll
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            e.preventDefault();
            
            const targetId = this.getAttribute('href');
            if (targetId === '#') return;
            
            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
    
    // Back to top button işlevselliği
    const backToTopButton = document.querySelector('.back-to-top');
    if (backToTopButton) {
        window.addEventListener('scroll', function() {
            if (window.scrollY > 300) {
                backToTopButton.style.display = 'block';
            } else {
                backToTopButton.style.display = 'none';
            }
        });
        
        backToTopButton.addEventListener('click', function() {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }
    
    // Bildirim toast'larını geliştir
    const toastElements = document.querySelectorAll('.toast');
    if (toastElements.length > 0) {
        toastElements.forEach(toast => {
            // Bootstrap Toast sınıfının başlatılması
            const bsToast = new bootstrap.Toast(toast, {
                delay: 5000
            });
            bsToast.show();
            
            // Toast kapatma animasyonu
            const closeBtn = toast.querySelector('.btn-close');
            if (closeBtn) {
                closeBtn.addEventListener('click', function() {
                    toast.style.opacity = '0';
                    setTimeout(function() {
                        bsToast.hide();
                    }, 300);
                });
            }
        });
    }
    
    // Kartlara tıklandığında ek efekt
    const cardElements = document.querySelectorAll('.card');
    if (cardElements.length > 0) {
        cardElements.forEach(card => {
            card.addEventListener('mouseenter', function() {
                this.classList.add('card-hover');
            });
            
            card.addEventListener('mouseleave', function() {
                this.classList.remove('card-hover');
            });
            
            // Kartın linke sahip olup olmadığını kontrol et
            const cardLink = card.querySelector('a.card-link');
            if (cardLink) {
                card.classList.add('card-clickable');
                card.addEventListener('click', function(e) {
                    // Eğer tıklama kartın içindeki bir düğmeden geldiyse, 
                    // kartın tamamını link olarak kullanma
                    if (!e.target.closest('button')) {
                        cardLink.click();
                    }
                });
            }
        });
    }
    
    // AOS (Animate on Scroll) kütüphanesini başlat (eğer yüklüyse)
    if (typeof AOS !== 'undefined') {
        AOS.init({
            duration: 800,
            easing: 'ease-in-out',
            once: true
        });
    }
});

// Dark Mode İşlemleri
document.addEventListener('DOMContentLoaded', () => {
    // Kullanıcının mevcut tercihini kontrol et
    const currentTheme = localStorage.getItem('theme') || 'light';
    
    // Tema tercihi varsa uygula
    if (currentTheme) {
        document.documentElement.setAttribute('data-theme', currentTheme);
        
        // Dark mode ise, switch'i işaretle
        if (currentTheme === 'dark') {
            if (document.querySelector('#theme-switch')) {
                document.querySelector('#theme-switch').checked = true;
            }
        }
    }
    
    // Sayfa yüklendiğinde gövdeyi görünür yap (CSS'te opacity: 0 idi)
    setTimeout(() => {
        document.body.style.opacity = 1;
    }, 100);
    
    // Gecikmeli olarak AOS'u başlat
    setTimeout(() => {
        AOS.init({
            duration: 1000,
            once: false,
            mirror: true
        });
    }, 500);

    // Tema değiştirme fonksiyonu
    function switchTheme(e) {
        if (e.target.checked) {
            document.documentElement.setAttribute('data-theme', 'dark');
            localStorage.setItem('theme', 'dark');
        } else {
            document.documentElement.setAttribute('data-theme', 'light');
            localStorage.setItem('theme', 'light');
        }    
    }

    // Dark mode toggle switch event listener
    const toggleSwitch = document.querySelector('#theme-switch');
    if (toggleSwitch) {
        toggleSwitch.addEventListener('change', switchTheme, false);
    }
    
    // Parallax efekti için
    window.addEventListener('scroll', function() {
        const parallaxElements = document.querySelectorAll('.parallax');
        let scrollPosition = window.pageYOffset;
        
        parallaxElements.forEach(element => {
            element.style.transform = 'translateY(' + scrollPosition * 0.5 + 'px)';
        });
    });
});
