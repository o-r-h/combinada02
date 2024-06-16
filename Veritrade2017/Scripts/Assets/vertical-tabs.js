$(document).ready(function () {
    $("div.vertical-tab-menu>div.tab-menu-list-group>a").click(function (e) {
        e.preventDefault();
        $(this).siblings("a.active").removeClass("active");
        $(this).addClass("active");

        var index = $(this).index();
        $("div.bhoechie-tab>div.bhoechie-tab-content").removeClass("active");
        $("div.bhoechie-tab>div.bhoechie-tab-content").eq(index).addClass("active");
    });

    var total = 0;
    $("div.bhoechie-tab>div.bhoechie-tab-content").each(function (index, value) {
        if ($(value).hasClass("active")) {
            total++;
        }
    });

    if (total === 0) {
        $("div.bhoechie-tab>div.bhoechie-tab-content").eq(0).addClass("active");
    }
});