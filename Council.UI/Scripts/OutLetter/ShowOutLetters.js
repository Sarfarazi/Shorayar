$('.addToTabbtn').click(function () {
    $('#addToTabLetterID').attr('value', $(this).attr('id'));
});
var recievedPageNumber = 1;
var sendPageNumber = 1;
var archivedPageNumber = 1;
var removePageNumber = 1;

$(document).ready(function () {
    setTimeout(function () {
        if ($("tbody.removedOutLetter tr").length == 0) {
            $("#ExtraRemoved").hide();
            $("tbody.removedOutLetter").parent().parent().hide();
        } else {
            if (Number($("#RemovedOutLetterCount").val()) <= 50)
                $("#ShowRemovedSpan").hide();
            document.getElementById('AllRemovedSpan').innerHTML = ' ' + $("#RemovedOutLetterCount").val();
            document.getElementById('AnyRemovedSpan').innerHTML = ' ' + $("tbody.removedOutLetter tr").length;
        }
        if ($("tbody.archivedOutLetter tr").length == 0) {
            $("#ExtraArchived").hide();
            $("tbody.archivedOutLetter").parent().parent().hide();
        } else {
            if (Number($("#ArchivedOutLetterCount").val()) <= 50)
                $("#ShowArchivedSpan").hide();
            document.getElementById('AllArchivedSpan').innerHTML = ' ' + $("#ArchivedOutLetterCount").val();
            document.getElementById('AnyArchivedSpan').innerHTML = ' ' + $("tbody.archivedOutLetter tr").length;
        }
        if ($("tbody.sendedOutLetter tr").length == 0) {
            $("#ExtraSended").hide();
            $("tbody.sendedOutLetter").parent().parent().hide();
        } else {
            if (Number($("#SendedOutLetterCount").val()) <= 50)
                $("#ShowSendedSpan").hide();
            document.getElementById('AllSendedSpan').innerHTML = ' ' + $("#SendedOutLetterCount").val();
            document.getElementById('AnySendedSpan').innerHTML = ' ' + $("tbody.sendedOutLetter tr").length;
        }
        if ($("tbody.recievedOutLetter tr").length == 0) {
            $("#ExtraRecived").hide();
            $("tbody.recievedOutLetter").parent().parent().hide();
        } else {
            if (Number($("#RecievedOutLetterCount").val()) <= 10)
                $("#ShowRecivedSpan").hide();
            document.getElementById('AllRecivedSpan').innerHTML = ' ' + $("#RecievedOutLetterCount").val();
            document.getElementById('AnyRecivedSpan').innerHTML = ' ' + $("tbody.recievedOutLetter tr").length;
        }
    }, 500)

})
$("#MoreRecived").click(function () {
    //var pageCount = Number($("#RecievedOutLetterPageNumber").val());
    $.ajax({
        url: "/OutLetter/ExtraRecivedOutLetters",
        data: { skip: recievedPageNumber * 50, take: 50, organId: $("select[name=Organ]").val() },
        type: "POST",
        success: function (result) {
            $("tbody.recievedOutLetter").append(result);
            document.getElementById('AllRecivedSpan').innerHTML = ' ' + $("#RecievedOutLetterCount").val();
            document.getElementById('AnyRecivedSpan').innerHTML = ' ' + $("tbody.recievedOutLetter tr").length;
            recievedPageNumber++;
            // $("#RecievedOutLetterPageNumber").val(pageCount);
            if ($("tbody.recievedOutLetter tr").length == Number($("#RecievedOutLetterCount").val())) {
                $("#ShowRecivedSpan").hide();
            }
        }

    })
})
$("#MoreSended").click(function () {
    //var pageCount = Number($("#SendedOutLetterPageNumber").val());
    $.ajax({
        url: "/OutLetter/ExtraSendedOutLetters",
        data: { skip: sendPageNumber * 50, take: 50, organId: $("select[name=Organ]").val() },
        type: "POST",
        success: function (result) {
            $("tbody.sendedOutLetter").append(result);
            document.getElementById('AllSendedSpan').innerHTML = ' ' + $("#SendedOutLetterCount").val();
            document.getElementById('AnySendedSpan').innerHTML = ' ' + $("tbody.sendedOutLetter tr").length;
            sendPageNumber++;
            //$("#CommisionPageNumber").val(pageCount);
            if ($("tbody.sendedOutLetter tr").length == Number($("#SendedOutLetterCount").val())) {
                $("#ShowSendedSpan").hide();
            }
        }

    })
})
$("#MoreArchived").click(function () {
    //var pageCount = Number($("#ArchivedOutLetterPageNumber").val());
    $.ajax({
        url: "/OutLetter/ExtraArchivedOutLetters",
        data: { skip: archivedPageNumber * 50, take: 50, organId: $("select[name=Organ]").val() },
        type: "POST",
        success: function (result) {
            $("tbody.archivedOutLetter").append(result);
            document.getElementById('AllArchivedSpan').innerHTML = ' ' + $("#ArchivedOutLetterCount").val();
            document.getElementById('AnyArchivedSpan').innerHTML = ' ' + $("tbody.archivedOutLetter tr").length;
            archivedPageNumber++;
            //$("#ArchivedOutLetterPageNumber").val(pageCount);
            if ($("tbody.archivedOutLetter tr").length == Number($("#ArchivedOutLetterCount").val())) {
                $("#ShowArchivedSpan").hide();
            }
        }

    })
})
$("#MoreRemoved").click(function () {
    //var pageCount = Number($("#RemivedOutLetterPageNumber").val());
    $.ajax({
        url: "/OutLetter/ExtraRemovedOutLetters",
        data: { skip: removePageNumber * 50, take: 50, organId: $("select[name=Organ]").val() },
        type: "POST",
        success: function (result) {
            $("tbody.removedOutLetter").append(result);
            document.getElementById('AllRemovedSpan').innerHTML = ' ' + $("#RemovedOutLetterCount").val();
            document.getElementById('AnyRemovedSpan').innerHTML = ' ' + $("tbody.removedOutLetter tr").length;
            removePageNumber++;
            //$("#RemivedOutLetterPageNumber").val(pageCount);
            if ($("tbody.removedOutLetter tr").length == Number($("#RemovedOutLetterCount").val())) {
                $("#ShowRemovedSpan").hide();
            }
        }

    })
})

$("#organSearch").click(function () {
    $.ajax({
        url: "/OutLetter/SearchOrganLetters",
        data: { skip: 0, take: 50, organId: $("select[name=Organ]").val() },
        type: "POST",
        success: function (result) {

            $("tbody.removedOutLetter").html(result.removed.Data);
            $("tbody.archivedOutLetter").html(result.archived.Data);
            $("tbody.sendedOutLetter").html(result.sended.Data);
            $("tbody.recievedOutLetter").html(result.recieved.Data);
            document.getElementById('AllRemovedSpan').innerHTML = ' ' + $("#RemovedOutLetterCount").val();
            document.getElementById('AnyRemovedSpan').innerHTML = ' ' + $("tbody.removedOutLetter tr").length;
            document.getElementById('AllArchivedSpan').innerHTML = ' ' + $("#ArchivedOutLetterCount").val();
            document.getElementById('AnyArchivedSpan').innerHTML = ' ' + $("tbody.archivedOutLetter tr").length;
            document.getElementById('AllSendedSpan').innerHTML = ' ' + $("#SendedOutLetterCount").val();
            document.getElementById('AnySendedSpan').innerHTML = ' ' + $("tbody.sendedOutLetter tr").length;
            document.getElementById('AllRecivedSpan').innerHTML = ' ' + $("#RecievedOutLetterCount").val();
            document.getElementById('AnyRecivedSpan').innerHTML = ' ' + $("tbody.recievedOutLetter tr").length;
            recievedPageNumber++;
            sendPageNumber++;
            removePageNumber++;
            archivedPageNumber++;

            if ($("tbody.removedOutLetter tr").length == Number($("#RemovedOutLetterCount").val())) {
                $("#ShowRemovedSpan").hide();
            }
            if ($("tbody.archivedOutLetter tr").length == Number($("#ArchivedOutLetterCount").val())) {
                $("#ShowArchivedSpan").hide();
            }
            if ($("tbody.sendedOutLetter tr").length == Number($("#SendedOutLetterCount").val())) {
                $("#ShowSendedSpan").hide();
            }
            if ($("tbody.recievedOutLetter tr").length == Number($("#RecievedOutLetterCount").val())) {
                $("#ShowRecivedSpan").hide();
            }
        }
    })
})
function GetOutletterNote(Id) {
    $.ajax({
        url: "/OutLetter/OutletterNote",
        data: { letterId: Id },
        type: "POST",
        dataType: "Json",
        success: function (result) {

            $("#NoteModal .modal-body").empty();
            $("#NoteModal .modal-body").append(result);
            $("#NoteModal").modal("show")
        }
    })
}