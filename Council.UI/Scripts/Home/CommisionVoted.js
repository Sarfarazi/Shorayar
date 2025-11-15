var commisionPagenumber = 1;

var isComm = 0;
var hrefVote = "";
var letterId = "";
var isCommision = "";
$(document).ready(function () {
    if ($("tbody.allCommision tr").length == 0) {
        $("#ExtraCommision").hide();
        $("tbody.allCommision").parent().parent().hide();
    } else {
        if (Number($("#CommisionCount").val()) <= 50)
            $("#ShowCommisionSpan").hide();
        document.getElementById('AllCommisionSpan').innerHTML = ' ' + $("#CommisionCount").val();
        document.getElementById('AnyCommisionSpan').innerHTML = ' ' + $("tbody.allCommision tr").length;
    }
});

$("#MoreCommisions").click(function () {
    //var pageCount = Number($("#CommisionPageNumber").val());
    $.ajax({
        url: "/Home/GetLastCommisionData",
        data: { skip: commisionPagenumber * 50, take: 50 },
        type: "POST",
        success: function (result) {
            $("tbody.allCommision").append(result);
            document.getElementById('AllCommisionSpan').innerHTML = ' ' + $("#CommisionCount").val();
            document.getElementById('AnyCommisionSpan').innerHTML = ' ' + $("tbody.allCommision tr").length;
            commisionPagenumber++;
            // $("#CommisionPageNumber").val(pageCount);
            if ($("tbody.allCommision tr").length == Number($("#CommisionCount").val())) {
                $("#ShowCommisionSpan").hide()
            }
        }

    })
})
$("#AllCommisionsLink").click(function () {
    //var pageCount = Number($("#CommisionPageNumber").val());
    $.ajax({
        url: "/Home/GetLastCommisionData",
        data: { skip: commisionPagenumber * 50, take: -1 },
        type: "POST",
        success: function (result) {
            $("tbody.allCommision").append(result);
            document.getElementById('AnyCommisionSpan').innerHTML = ' ' + $("#CommisionCount").val();
            document.getElementById('AllCommisionSpan').innerHTML = ' ' + $("#CommisionCount").val();
            commisionPagenumber++;
            // $("#CommisionPageNumber").val(pageCount);
            $("#ShowCommisionSpan").hide();
        }

    })
})


$("#search-button, #search-icon").click(function (e) {
    e.preventDefault();
    $("#search-button, #search-form").toggle();
});