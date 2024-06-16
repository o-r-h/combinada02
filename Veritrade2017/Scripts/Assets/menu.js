$(document).ready(function() {
    var plan = $("div.title-fixed");

    /**
     * Function that manage the direction of the scroll in plans & price section
     */
    function offSetManagerPlanes() {
        var yOffset = 0;
        var currYOffSet = window.pageYOffset;
        var width = window.innerWidth;

        if (width >= 768 && width < 992) {
            yOffset = 780;
        } else if (width >= 992 && width < 1280) {
            yOffset = 820;
        } else if (width >= 1280 && width < 1366) {
            yOffset = 870;
        } else if (width >= 1366 && width < 1560) {
            yOffset = 865;
        } else {
            yOffset = 865;
        }

        if (yOffset < currYOffSet) {
            plan.css("display", "table");
        } else {
            plan.css("display", "none");
        }
    }

    /**
     * bind to the document scroll detection
     */
    window.onscroll = function() {
        offSetManagerPlanes();
    }

    offSetManagerPlanes();
});