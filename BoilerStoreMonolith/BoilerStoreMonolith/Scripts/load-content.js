// Load content
$(function () {
    var $catalogue = $(".catalogue");
    var $links = $catalogue.find("a");
    var prevHistoryState = "";

    $links.each(function (link) {
        $links[link].addEventListener("click", function (e) {
            e.preventDefault();

            if (prevHistoryState !== this.href) {
                prevHistoryState = this.href;
                history.pushState(this.href, this.href, this.href);
                getContent(this.href);
            }
        });
    });

    function getContent(url) {
        $.ajax({
            type: "POST",
            url: url,
            data: { isAjax: true }
        }).done(function (data) {
            $("#page-content").html(data);
        });
    }

    window.addEventListener("popstate", function (e) {
        if (!e.state) {
            window.location = "/";
        } else {
            getContent(e.state);
        }
    });
});

// Hide sidebar on home page
$(
    function () {
        console.log(1)
        if (window.location.pathname !== "/") {
            $($('#sidebar-menu')[0]).show();
        }
    }
);

