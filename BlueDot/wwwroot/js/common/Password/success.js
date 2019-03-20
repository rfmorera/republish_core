$("#homeLink").on('click', function (e) {
    e.preventDefault();
    window.location.href = $("#home").attr('href');
});