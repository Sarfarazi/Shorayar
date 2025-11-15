var path = window.location.pathname;
$.each($(".sub-menu a"), function (index, value) {
    var array = $(".sub-menu a");
    var item = $(array[index]).attr("href")
    if ($(array[index]).attr("href") == path) {
        $(array[index]).parent().parent().toggleClass('in');
        $(array[index]).css("color", "#ecce85");
    }
});

//$(".user-menu").on("click", function () {
//    $(".user-menu ul.dropdown-menu").toggle();
//});

$("html").click(function (event) {
    var otarget = $(event.target);
    if (!otarget.parents('.dropdown-toggle').length && otarget.attr('class') != ".dropdown-toggle" && !otarget.parents('.dropdown-toggle').length) {
        $('.dropdown-menu').hide();
    }
});

$(document).on("click", ".dropdown-toggle", function () {
    $(this).next(".dropdown-menu").toggle();
    return false;
});


$("label:contains('*')").each(function () {
    $(this).html($(this).html().replace("*", "<span class='custom-rq'>*</span>"));
});

$(document).on('click', ".closebtn", function () {
    var div = this.parentElement;
    div.style.opacity = "0";
    setTimeout(function () { div.style.display = "none"; }, 600);
});


//$(".table-responsive").on('click', '.dropdown-toggle', function (event) {
//    if ($('.dropdown-menu').length) {
//        var elm = $('.dropdown-menu'),
//          docHeight = $(document).height(),
//          docWidth = $(document).width(),
//          btn_offset = $(this).offset(),
//          btn_width = $(this).outerWidth(),
//          btn_height = $(this).outerHeight(),
//          elm_width = elm.outerWidth(),
//          elm_height = elm.outerHeight(),
//          table_offset = $(".table-responsive").offset(),
//          table_width = $(".table-responsive").width(),
//          table_height = $(".table-responsive").height(),

//          tableoffright = table_width + table_offset.left,
//          tableoffbottom = table_height + table_offset.top,
//          rem_tablewidth = docWidth - tableoffright,
//          rem_tableheight = docHeight - tableoffbottom,
//          elm_offsetleft = btn_offset.left,
//          elm_offsetright = btn_offset.left + btn_width,
//          elm_offsettop = btn_offset.top + btn_height,
//          btn_offsetbottom = elm_offsettop,

//          left_edge = (elm_offsetleft - table_offset.left) < elm_width,
//          top_edge = btn_offset.top < elm_height,
//          right_edge = (table_width - elm_offsetleft) < elm_width,
//          bottom_edge = (tableoffbottom - btn_offsetbottom) < elm_height;

//        var table_offset_bottom = docHeight - (table_offset.top + table_height);
//        var touchTableBottom = (btn_offset.top + btn_height + (elm_height * 2)) - table_offset.top;
//        var bottomedge = touchTableBottom > table_offset_bottom;

//        if (left_edge) {
//            $(this).addClass('left-edge');
//        } else {
//            $('.dropdown-menu').removeClass('left-edge');
//        }
//        if (bottom_edge) {
//            $(this).parent().addClass('dropup');
//            // $(this).parent().removeClass('dropdown');
//        } else {
//            $(this).parent().removeClass('dropup');
//            // $(this).parent().addClass('dropdown');
//        }

//    }
//});
////use if table height is below 300
//var table_smallheight = $('.table-responsive'),
//				positioning = table_smallheight.parent();

//if (table_smallheight.height() < 320) {
//    positioning.addClass('positioning');
//    $('.table-responsive .dropdown,.table-responsive .dropup').css('position', 'static');

//} else {
//    positioning.removeClass('positioning');
//    $('.table-responsive .dropdown,.table-responsive .dropup').css('position', 'relative');
//}