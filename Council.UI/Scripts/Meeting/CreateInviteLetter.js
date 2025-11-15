$(".deactiveSession").click(function(){
    $.ajax({
        url: "/Meeting/DeActiveAllSession",
        type: "Post",
        success: function (result) {
            if(result=="true"){
                $("#SessionModal").modal("hide");

                $("#errorMessagelbl").text('تمام جلسات قبلی غیر فعال شدند')
            }else
                if(result=="false"){

                    $("#errorMessagelbl").text('در غیر فعال کردن جلسات قبلی مشکلی بوجود آمده است')
                }
        }
    })
})
$("#btnInvitePrint").click(function(){
    var letters=[];
    var session_Id = "";
    if ($(".allletters tbody input[type='checkbox']:checked").length > 0) {
        $.each($(".allletters tbody input[type='checkbox']:checked"), function (index, val) {
            item=$(this);
            letters.push($(item).attr("id"));
        })
        $("input[name='letterIds']").val(JSON.stringify(letters));
        $("input[name='sessionId']").val($("#ActiveSession").val());
        $("#inviteForm").attr('target', '_blank');
        $("#inviteForm").submit();
    } else {
        $("#NoSelectItemModal").modal("show");
    }
})
$("#btnInviteSMS").click(function () {
    $("#loader").show();
    $.ajax({
        url: "/Meeting/InviteSMS",
        type: "Post",
        success: function (result) {
            if (result == "ok") {
                $("#NoSelectItemModal div.modal-body p").text('دعوتنامه بصورت اس ام اس برای اعضا ارسال شد')
                $("#NoSelectItemModal").modal("show");

            }
        },
        complete: function (resp) {
            $("#loader").hide();
        }
    })
})
$("#btnInviteEmail").click(function () {
    var letters=[];
    var session_Id = "";
    if ($(".allletters tbody input[type='checkbox']:checked").length > 0) {
        $.each($(".allletters tbody input[type='checkbox']:checked"), function (index, val) {
            item=$(this);
            letters.push($(item).attr("id"));
        })

        $.ajax({
            url: "/Meeting/InviteEmail",
            type: "Post",
            data:{letterIds:JSON.stringify(letters),sessionId:$("#ActiveSession").val()},
            success: function (result) {
                if (result == "ok") {
                    $("#NoSelectItemModal div.modal-body p").text('.دعوتنامه بصورت الکترونیکی برای اعضا ارسال شد')
                    $("#NoSelectItemModal").modal("show");
                }
            }
        })
    } else {
        $("#NoSelectItemModal").modal("show");
    }
});