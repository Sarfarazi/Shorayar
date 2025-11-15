var opinionPagenumber = 1;
var isComm = 0;
var hrefVote = "";
var letterId = "";
var isCommision = "";
$(document).ready(function () {

    if ($("tbody.allOpinion tr").length == 0 || $("tbody.allOpinion tr td").length == 1) {
        if ($("tbody.allOpinion tr").length == 0) {
            $("#ExtraOpinion").hide();
            $("tbody.allOpinion").parent().parent().parent().hide();
        }
        if ($("tbody.allOpinion tr td").length == 1) {
            $("#ExtraOpinion").hide();

        }

    } else {
        if (Number($("#OpinionCount").val()) <= 50)
            $("#ShowOpinionSpan").hide();
        document.getElementById('AllOpinionSpan').innerHTML = ' ' + $("#OpinionCount").val();
        document.getElementById('AnyOpinionSpan').innerHTML = ' ' + $("tbody.allOpinion tr").length;
    }

});

$("#MoreOpinions").click(function () {
    // var pageCount = Number($("#OpinionPageNumber").val());
    $.ajax({
        url: "/Home/GetLastOpinionData",
        data: { skip: opinionPagenumber * 50, take: 50 },
        type: "POST",
        success: function (result) {
            $("tbody.allOpinion").append(result);
            document.getElementById('AllOpinionSpan').innerHTML = ' ' + $("#OpinionCount").val();
            document.getElementById('AnyOpinionSpan').innerHTML = ' ' + $("tbody.allOpinion tr").length;
            opinionPagenumber++;
            //$("#OpinionPageNumber").val(pageCount);
            if ($("tbody.allOpinion tr").length == Number($("#OpinionCount").val())) {
                $("#ShowOpinionSpan").hide()
            }
        }

    })
})
$("#AllOpinionsLink").click(function () {
    //var pageCount = Number($("#CommisionPageNumber").val());
    $.ajax({
        url: "/Home/GetLastOpinionData",
        data: { skip: opinionPagenumber * 50, take: -1 },
        type: "POST",
        success: function (result) {
            $("tbody.allOpinion").append(result);
            document.getElementById('AllOpinionSpan').innerHTML = ' ' + $("#OpinionCount").val();
            document.getElementById('AnyOpinionSpan').innerHTML = ' ' + $("#OpinionCount").val();
            opinionPagenumber++;
            //$("#OpinionPageNumber").val(pageCount);
            $("#ShowOpinionSpan").hide()
        }

    })
})


$("#search-button, #search-icon").click(function (e) {
    e.preventDefault();
    $("#search-button, #search-form").toggle();
});