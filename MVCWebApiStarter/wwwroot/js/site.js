$(document).ready(function () {
    $('.dropdown-trigger').dropdown();
    document.querySelectorAll('li.disabled a').forEach(a => a.removeAttribute('href'));
});