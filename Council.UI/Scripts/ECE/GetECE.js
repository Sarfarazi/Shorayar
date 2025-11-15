$(document).on('click', '#EmailList .page-number', function () {
    $("#loader").show();
    var page = $(this).html();
    $.ajax({
        url: '/ECE/GetByPage',
        type: 'GET',
        data: { page: page },
        success: function (data) {
            $("#EmailList").html(data);

            $("#EmailList .pagination").find('a').each(function () {
                if ($(this).text() == page) {
                    $(this).addClass('current');
                }
            });
        },
        complete: function (resp) {
            $("#loader").hide();
        }
    });
});


$(document).on("click", "#GetEmails", function (e) {
    e.preventDefault();
    $("#loader").show();
    $.ajax({
        url: '/ECE/_GetECE',
        type: 'GET',
        data: '',
        success: function (data) {
            $("#EmailList").html(data);

            $("#EmailList .pagination").find('a').each(function () {
                if ($(this).text() == 1) {
                    $(this).addClass('current');
                }
            });
        },
        complete: function (resp) {
            $("#loader").hide();
        }
    });
});