document.addEventListener('DOMContentLoaded', function () {
    var myCarousel = document.querySelector('#carouselExampleControls');
    var carousel = new bootstrap.Carousel(myCarousel, {
        interval: 8000,
        wrap: true
    });
});