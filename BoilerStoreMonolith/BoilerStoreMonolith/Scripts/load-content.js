$(function () {

    var $catalogue = $('.catalogue');
    var $links = $catalogue.find('a');
    $links.each(function (link) {
        $links[link].addEventListener('click', function (e) {
            e.preventDefault();

            $href = this.href;
            getContent($href);
        });
    });

    function getContent(url){
        $.ajax({
            type: 'POST',
            url: url,

        }).done(function (data) {
            $('#page-content').html(data);
        });
    }
});
