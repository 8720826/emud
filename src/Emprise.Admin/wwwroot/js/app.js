(function($) {
  'use strict';

    $(function () {
        var contentHeight = $(".admin-content-body").height();
        var sidebarHeight = $(".admin-sidebar").height();
        console.log("contentHeight=" + contentHeight + ",sidebarHeight=" + sidebarHeight);

        if (contentHeight < sidebarHeight) {
            //$(".admin-content").height(sidebarHeight);
        }
    });
})(jQuery);
