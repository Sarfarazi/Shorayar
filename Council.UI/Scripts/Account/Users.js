var oi = 1;

$('select#SelectedType').multipleSelect({
    filter: true,
    selectAll: false,
    minimumCountSelected: 4,
    delimiter: ', ',
    formatCountSelected: () => '# از % مورد',
    formatNoMatchesFound: () => 'نقشی پیدا نشد',
    keepOpen:false,
    onClick: function (select) {
        console.log(oi);
        oi++;
        var MemberRoleValue = $('select#SelectedType option').filter(function () { return $(this).html() == "عضو شورا"; }).val();
        console.log(MemberRoleValue);
        console.log(select.label);

        if (select.checked) {
            console.log("check");
            switch (select.label) {
                case "دبیر جلسه 1":
                case "دبیر جلسه 2":
                case "رئیس شورا":
                case "نائب رییس":
                    console.log("select.label is: " + select.label);
                    $("select#SelectedType option").each(function (index) {
                        if ($(this).val() === MemberRoleValue) {
                            console.log("/////////");
                            $(this).prop("selected", "selected");
                            $(this).prop('disabled', true);
                        }
                        else if ($(this).val() !== select.value) {
                            $(this).prop('disabled', true);
                        }
                    });
                    $("select#SelectedType option").promise().done(function () {
                        $('select#SelectedType').multipleSelect('refresh');
                    });
                    break;

                case "دبیرخانه":

                    break;

                case "مدیر سیستم":

                    break;
                case "مهمان":

                    break;
            }
        }
        else {
            console.log("un check");
            $("select#SelectedType option").each(function () {
                console.log(">>>>> " + $(this).text());
                $(this).prop("disabled", false);
                $(this).removeProp("selected");
            });

            $("select#SelectedType option").promise().done(function () {
                $('select#SelectedType').multipleSelect('refresh');
            });
        }

        var selected = $('select#SelectedType').multipleSelect('getSelects', 'value');
        $("#SelectedTypes").val(selected);
        console.log(selected);
    }
});

var SelectedTypes = $("#SelectedTypes").val();
if (SelectedTypes) {
    $('select#SelectedType').multipleSelect('setSelects', SelectedTypes.split(','))
}

$.validator.setDefaults({ ignore: null });


$('#usersSearch').keyup(function () {
    var _filter = $(this).val();
    $('.userFirstAndLastName').each(function (i, element) {
        if (_filter.length < 1)
            $(this).parent().show()
        else {
            if ($(this).html().indexOf(_filter) >= 0)
                $(this).parent().show()
            else
                $(this).parent().hide();
        }

    });
});
$(".userTable tr .userActive").parent().parent()
$("#allUser").click(function () {
    $("#userActive").prop("checked", false);
    $("#userNoActive").prop("checked", false);
    $('.userTable tr').each(function (i, element) {
        $(this).show();
    });
});
$("#userActive").click(function () {
    $("#allUser").prop("checked", false);
    $("#userNoActive").prop("checked", false);
    $('.userTable tr .userStatusTd span').each(function (i, element) {
        if ($(this).hasClass('userActive'))
            $(this).parent().parent().show()
        else
            $(this).parent().parent().hide()
    });

});
$("#userNoActive").click(function () {
    $("#allUser").prop("checked", false);
    $("#userActive").prop("checked", false);
    $('.userTable tr .userStatusTd span').each(function (i, element) {
        if ($(this).hasClass('userDeActive'))
            $(this).parent().parent().show()
        else
            $(this).parent().parent().hide()
    });
});