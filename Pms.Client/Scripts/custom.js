function loadHeader() {
    $("#pms-header").load("header.html");
}

function loadFooter() {
    $("#pms-footer").load("footer.html");
}

$(document).ready(function () {
    loadHeader();
    loadFooter();
});
