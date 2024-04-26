
(function ($) {

    "use strict";

    // MENU
    $('.navbar-collapse a').on('click', function () {
        $(".navbar-collapse").collapse('hide');
    });

    // CUSTOM LINK
    $('.smoothscroll').click(function () {
        var el = $(this).attr('href');
        var elWrapped = $(el);
        var header_height = $('.navbar').height();

        scrollToDiv(elWrapped, header_height);
        return false;

        function scrollToDiv(element, navheight) {
            var offset = element.offset();
            var offsetTop = offset.top;
            var totalScroll = offsetTop - navheight;

            $('body,html').animate({
                scrollTop: totalScroll
            }, 300);
        }
    });

})(window.jQuery);






// Impedisci la chiusura della navbar quando si clicca sul link degli alberghi in modalità mobile


var dropdownToggles = document.querySelectorAll('.navbar-nav .dropdown-toggle');
dropdownToggles.forEach(function (toggle) {
    toggle.addEventListener('click', function (event) {
        if (window.innerWidth < 992) { // Solo in modalità mobile
            var dropdownMenu = toggle.nextElementSibling;
            dropdownMenu.addEventListener('show.bs.dropdown', function () {
                toggle.setAttribute('data-bs-toggle', ''); // Rimuovi il toggle automatico del dropdown-menu
            };









            $(document).ready(function () {
                $('.customer-logos').slick({
                    slidesToShow: 6,
                    slidesToScroll: 1,
                    autoplay: true,
                    autoplaySpeed: 1500,
                    arrows: false,
                    dots: false,
                    pauseOnHover: false,
                    responsive: [{
                        breakpoint: 768,
                        setting: {
                            slidesToShow: 4
                        }
                    }, {
                        breakpoint: 520,
                        setting: {
                            slidesToShow: 3
                        }
                    }]
                });
            });