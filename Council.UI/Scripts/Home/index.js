var opinionPagenumber = 1;
var commisionPagenumber = 1;
var allLetterPageNumber = 1;
var UsersVoted = [];
var isComm = 0;
var hrefVote = "";
var letterId = "";
var isCommision = "";

//$(document).ready(function () {
//    if ($("#vottingUL li").length == 0) {
//        $("#vottingUL").parent().hide();
//    } else {
//        $("#vottingUL").parent().show();
//    }

//    if ($("#MyLettersLink").length > 0) {
//        $("#MyLettersLink").click();
//        $("#MyLettersLink").parent().addClass('tabactive');
//    }
//});

$(document).ready(function () {
    $(".scrtabs-tab-container ul li").each(function () {        
        if ($(this).hasClass('active')) {
            $(this).find('a').click();
        }
    })
});

$(".wizard-steps a").click(function () {

    var link = $(this);
    $.each($(".wizard-steps a"), function () {

        $(this).parent().removeClass('tabactive');
    })
    $(link).parent().addClass('tabactive');
});
$("#DoCouncilLink,#DoCouncilMenu").click(function () {
    $("#DoCouncilLoader").fadeIn();

    $.ajax({
        type: "POST",
        url: "/Home/GetLastReadyForVote",
        success: function (result) {

            $("#DoCouncilLoader").fadeOut();
            $("#DoCouncil").empty();
            $("#DoCouncil").append(result);
        }
    })
});
$("#AllowCouncilLink,#AllowCouncilMenu").click(function () {
    $("#AllowCouncilLoader").fadeIn();
    $.ajax({
        url: "/Home/GetLastAllowForVote",
        type: "POST",
        success: function (result) {
            $("#AllowCouncilLoader").fadeOut();
            $("#AllowCouncil").empty();
            $("#AllowCouncil").append(result);
        }
    })
});
$("#ReadyForCouncilLink,#ReadyForCouncilMenu").click(function () {
    $("#ReadyForCouncilLoader").fadeIn();
    $.ajax({
        url: "/Home/GetLastReadyForCouncil",
        type: "POST",
        success: function (result) {
            $("#ReadyForCouncilLoader").fadeOut();
            $("#ReadyForCouncil").empty();
            $("#ReadyForCouncil").append(result);
        }
    })
});

$("#ReadyForCommissionLink,#ReadyForCouncilMenu").click(function () {
    $("#ReadyForCommisionLoader").fadeIn();
    $.ajax({
        url: "/Home/GetLastReadyForCommsion",
        type: "POST",
        success: function (result) {
            $("#ReadyForCommisionLoader").fadeOut();
            $("#ReadyForCommsion").empty();
            $("#ReadyForCommsion").append(result);
        }
    })
});

$("#MyLettersLink,#MyLettersMenu").click(function () {
    $("#MyLettersLoader").fadeIn();
    $.ajax({
        url: "/Home/LastMyLetter",
        type: "POST",
        success: function (result) {
            $("#MyLettersLoader").fadeOut();
            $("#MyLetters").empty();
            $("#MyLetters").append(result);
        }
    })
});

$("#MyLettersLink,#MyLettersMenu").click(function () {
    $("#MyOutLettersLoader").fadeIn();
    $.ajax({
        url: "/Home/LastMyOutLetter",
        type: "POST",
        success: function (result) {
            $("#MyOutLettersLoader").fadeOut();
            $("#MyOutLetters").empty();
            $("#MyOutLetters").append(result);
        }
    })
});



$("#ShowAllResultLink,#ShowAllResultMenu").click(function () {
    $("#ShowAllResultLoader").fadeIn();
    $.ajax({
        url: "/Home/AllVotedResult",
        type: "POST",
        success: function (result) {
            $("#ShowAllResultLoader").fadeOut();
            $("#ShowAllResult").empty();
            $("#ShowAllResult").append(result);
        }
    })
})
$("#ShowAllRuleLink,#ShowAllRuleMenu").click(function () {
    $("#ShowAllRuleLoader").fadeIn();
    $.ajax({
        url: "/Home/AllRuleResult",
        type: "POST",
        success: function (result) {
            $("#ShowAllRuleLoader").fadeOut();
            $("#ShowAllRule").empty();
            $("#ShowAllRule").append(result);
        }
    })
});
$("#MoreLetters").click(function () {
    //var pageCount =Number($("#PageNumber").val());
    $.ajax({
        url: "/Home/GetExtraLetters",
        data: { skip: allLetterPageNumber * 50, take: 50 },
        type: "POST",
        success: function (result) {

            //pageCount++;
            allLetterPageNumber++;
            $("tbody.NewLettersUl").append(result);
            //$("#PageNumber").val(pageCount);
            document.getElementById('AllLetterSpan').innerHTML = ' ' + $("#AllLettersCount").val();
            document.getElementById('AnyLetterSpan').innerHTML = ' ' + $("tbody.NewLettersUl tr").length;
            if ($("tbody.NewLettersUl tr").length == Number($("#LettersCount").val())) {
                $("#SpanAllLettersLink").hide()
            }
        }
    })
})
$("#AllLettersLink").click(function () {
    //var pageCount = Number($("#PageNumber").val());
    $.ajax({
        url: "/Home/GetExtraLetters",
        data: { skip: allLetterPageNumber * 50, take: -1 },
        type: "POST",
        success: function (result) {

            allLetterPageNumber++;
            $("tbody.NewLettersUl").append(result);
            //$("#PageNumber").val(pageCount);
            document.getElementById('AllLetterSpan').innerHTML = ' ' + $("#AllLettersCount").val();
            document.getElementById('AnyLetterSpan').innerHTML = ' ' + $("tbody.NewLettersUl tr").length;
            $("#SpanAllLettersLink").hide();
        }
    })
})
$("#disAgreeRadio").click(function () {
    $("#imposibleRadio").prop("checked", false);
    $("#agreeRadio").prop("checked", false);
})
$("#imposibleRadio").click(function () {
    $("#disAgreeRadio").prop("checked", false);
    $("#agreeRadio").prop("checked", false);
})
$("#agreeRadio").click(function () {
    $("#imposibleRadio").prop("checked", false);
    $("#disAgreeRadio").prop("checked", false);
})

$("a.doVote").click(function () {
    var voteVal = "";
    if (UsersVoted != null && UsersVoted != undefined && UsersVoted.length > 0) {
        console.log("****1");
        if ($("#imposibleRadio").prop("checked"))
            voteVal = 3;
        if ($("#agreeRadio").prop("checked"))
            voteVal = 1;
        if ($("#disAgreeRadio").prop("checked"))
            voteVal = 2;
        $.ajax({
            url: "/Letter/SaveVoting",
            data: { LetterID: letterId, users: JSON.stringify(UsersVoted), votValue: voteVal },
            type: "POST",
            dataType: "Json",
            success: function (result) {
                console.log("****2");
                console.log(result);
                if (result == "ok") {
                    $("a.doVote").attr("href", hrefVote)
                    if (isComm == "1") {
                        $.ajax({
                            url: "/Letter/CommissionEndMeeting",
                            data: { LetterID: letterId },
                            type: "POST",
                            dataType: "Json",
                            success: function (result) {
                                $('#VoteUserModal').modal('hide');
                                $("#DoCouncilLink").click();
                            }
                        })
                    } else {
                        $.ajax({
                            url: "/Letter/EndMeeting",
                            data: { LetterID: letterId },
                            type: "POST",
                            dataType: "Json",
                            success: function (result) {
                                $('#VoteUserModal').modal('hide');
                                $("#DoCouncilLink").click();
                            }
                        })
                    }
                }
            }
        })
    }
    else {
        $("a.doVote").attr("href", hrefVote)
        if (isComm == "1") {
            $.ajax({
                url: "/Letter/CommissionEndMeeting",
                data: { LetterID: letterId },
                type: "POST",
                dataType: "Json",
                success: function (result) {
                    $('#VoteUserModal').modal('hide');
                    $("#DoCouncilLink").click();
                }
            })
        } else {
            $.ajax({
                url: "/Letter/EndMeeting",
                data: { LetterID: letterId },
                type: "POST",
                dataType: "Json",
                success: function (result) {
                    $('#VoteUserModal').modal('hide');
                    $("#DoCouncilLink").click();
                }
            })
        }
    }
});

function EndVotingFunction2(id, href, isCommision) {
    letterId = id;
    isComm = isCommision;
    $.ajax({
        url: "/Letter/CheckEndMeeting",
        data: { LetterID: id },
        type: "POST",
        dataType: "Json",
        success: function (result) {
            console.log(result);
            console.log("----here");
            UsersVoted = result.notVoted;
            isCommision = result.isCommission;
            console.log(UsersVoted.length);
            if (UsersVoted.length > 0) {
                $("table.notVoteUsr tr").remove();
                $.each(result.notVoted, function (index, val) {
                    $("table.notVoteUsr tbody").append('<tr><td>' + val.FirstName + ' ' + val.LastName + '</td></tr>');
                })
                //$("a.doVote").attr("href", href);
                hrefVote = href;
            }
            else {
                $("#VoteUserModal .modal-body").html('عضوی که شرکت نکرده باشد وجود ندارد');
            }

            $('#VoteUserModal').modal('show');
        },
        error: function (result) {
            Console.log('error');
        }
    })
}

function CheckVotingFunction(id, href, isCommision) {
    letterId = id;
    isComm = isCommision;
    $.ajax({
        url: "/Letter/CheckEndMeeting",
        data: { LetterID: id },
        type: "POST",
        dataType: "Json",
        success: function (result) {
            console.log(result);            
            if (result.message == "notEnd") {
                console.log("----here");
                UsersVoted = result.notVoted;
                isCommision = result.isCommission;
                console.log(UsersVoted.length);
                if (UsersVoted.length > 0) {
                    $("table.notVoteUsr tr").remove();
                    $.each(result.notVoted, function (index, val) {
                        $("table.notVoteUsr tbody").append('<tr><td>' + val.FirstName + ' ' + val.LastName + '</td></tr>');
                    })
                    //$("a.doVote").attr("href", href);
                    hrefVote = href;
                }
                else {
                    $("#VoteUserModal .modal-body").html('عضوی که شرکت نکرده باشد وجود ندارد');
                }

                $('#VoteUserModal').modal('show');
            }
        },
        error: function (result) {
            Console.log('error');
        }
    })
}

function EndVotingFunction(id, href, isCommision) {
    letterId = id;
    isComm = isCommision;
    $.ajax({
        url: "/Letter/CheckEndMeeting",
        data: { LetterID: id },
        type: "POST",
        dataType: "Json",
        success: function (result) {
            console.log(result);
            if (isComm == "1") {
                $.ajax({
                    url: "/Letter/CommissionEndMeeting",
                    data: { LetterID: id },
                    type: "POST",
                    dataType: "Json",
                    success: function (resp) {
                        //$("#DoCouncilLink").click();
                    }
                })

            } else {
                $.ajax({
                    url: "/Letter/EndMeeting",
                    data: { LetterID: id },
                    type: "POST",
                    dataType: "Json",
                    success: function (resp) {
                        //$("#DoCouncilLink").click();
                    }
                })
            }
        },
        error: function (result) {
            Console.log('error');
        }
    })
}


$("#search-button, #search-icon").click(function (e) {
    e.preventDefault();
    $("#search-button, #search-form").toggle();
});