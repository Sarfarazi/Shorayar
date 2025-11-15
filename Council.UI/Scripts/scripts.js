
var counter = 1;
function setEvents() {
    $('input[type=file]').change(function (e) {
        $in = $(this);       
        $in.next().html($in.val());
    });
    $('.removeParent').click(function () {
        $(this).parent().remove();
    });

};
function addNewUploadClick(event) {
    $('#addNewUpload').click(function () {
        counter++;
        var newUpload = "<div><a class='btn btn-info' id='btnFile" + counter + "'> انتخاب فایل ضمیمه" + counter + "</a>"
                        + " <input type='file' class='hidden' name='Uploads' id='inputFile" + counter + "'  value='انتخاب' />"
                        + "<span></span>"
                        + "<i class='fa fa-times removeParent'></i></div>";
        $('#uploads').append(newUpload);
        var $onclick = "document.getElementById('inputFile" + counter + "').click(); return false;"
        $('#btnFile' + counter).attr("onClick", $onclick);

        setEvents();
        event.preventDefault();
    });
}
$(document).ready(function () {
    $('#refreshPage').click(function () {
        location.reload();
    });
    addNewUploadClick();
});

//===================Ready For Council
function readyFotCouncil() {
    $('#submitFormbtn').click(function () {
        var resualts = [];
        $('#usersContetn input[type="radio"]:checked').each(function () {
            var $id = $(this).attr('name');
            var $value = $(this).attr('value');
            var item = { ID: $id, status: $value };
            resualts.push(item)
        });
        var jsonReult = JSON.stringify(resualts);
        if (!(jsonReult == '[]'))
            $('#meetingMembers').val(jsonReult);
    });
}


//========================================Draft
function draftDisplayStart() {
    var $form = $('#draftDisplayForm');
    $form.find('#draftToRequestBtn').click(function () {
        $form.find('#type').val('Request');
        $form.attr('action', '/Draft/SendDraft/');
        $form.submit();
    })
    $form.find('#draftToLetterBtn').click(function () {
        $form.find('#type').val('Letter');
        $form.attr('action', '/Draft/SendDraft/');
        $form.submit();
    })
}
function setPermissionStart() {
    userList = [];

    setKeyUpSearch();
    $('.addUser').click(function () {
        var _firstAndLastName = $('#firstAndLastName' + $(this).attr('id')).html();
        var _ID = $(this).attr('id');
        
        var newItem = {
            firstAndLastName: _firstAndLastName,
            ID: _ID
            
        };

        if (!userIsExist(newItem)) {
            userList.push({
                firstAndLastName: _firstAndLastName,
                ID: _ID
               
            });
            displayUsersInTable();
        }
        else
            alert("کاربر تکراری است");
        $('#otherUsers').val(createResualtUsers());
    });
};

//================================== Publics

function desktopNotify(tittle, subject, sender) {
    if (!("Notification" in window)) {
        alert("This browser does not support desktop notification");
    }
    else if (Notification.permission === "granted") {
        var options = {
            body: subject + '[ ' + sender + ' ]',
            icon: "/Content/images/logo.png"
        }
        var notification = new Notification(tittle, options);
    }
    else if (Notification.permission !== 'denied') {
        Notification.requestPermission(function (permission) {
            if (permission === "granted") {
                var options = {
                    body: subject + '[ ' + sender + ' ]',
                    icon: "/Content/images/logo.png"
                }
                var notification = new Notification(tittle, options);
            }
        });
    }
}
$(document).ready(function () {
    $('input[type=file]').change(function (e) {
        
        $in = $(this);
        $in.next().html($in.val());
        readURL(this);
    });

    breadCrumbCorp();
    $(window).resize(function () {
        breadCrumbCorp();
    });
});
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#signatureImage').attr('src', e.target.result);
            //$('#imgPrew').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}
var breadCrumbCorp = function () {
    var breadCrumb = $('#breadCrumb :nth-child(2)');
    if ($('#breadCrumb a:hidden').length > 0) {
        breadCrumb.show();
    } else {
        breadCrumb.hide();
    }
}
function showNotify(tittle, message, url, subject, sender) {
    $.notify({
        // options
        icon: 'fa fa-warning',
        title: tittle,
        message: message,
        url: url,
        target: '_blank'
    }, {
        // settings
        element: 'body',
        position: null,
        type: "success",
        allow_dismiss: true,
        newest_on_top: false,
        showProgressbar: false,
        placement: {
            from: "bottom",
            align: "left"
        },
        offset: 20,
        spacing: 10,
        z_index: 1031,
        delay: 9999999,
        timer: 1000,
        url_target: '_blank',
        mouse_over: null,
        animate: {
            enter: 'animated fadeInUp',
            exit: 'animated fadeOutDown'
        },
        onShow: null,
        onShown: null,
        onClose: null,
        onClosed: null,
        icon_type: 'class',
        template: '<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0} closeOnClick" role="alert">' +
            '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
            '<span data-notify="icon"></span> ' +
            '<span data-notify="title">{1} برای نمایش  کلیک کنید</span><br /> ' +
            '<span data-notify="title">موضوع : ' + subject + '</span><br /> ' +
            '<span data-notify="title">فرستنده : ' + sender + '</span><br /> ' +
            '<span data-notify="message">{2}</span>' +
            '<div class="progress" data-notify="progressbar">' +
                '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
            '</div>' +
            '<a href="{3}" target="{4}" data-notify="url"></a>' +
        '</div>'
    });
}

//================================== End Publics


//================================== Layout
function layoutStart() {
    $('.closeOnClick').click(function () {
        $(this).remove();
    });
}
//================================== End Layout


//================================== Letter Scripts
var reciverList = [];
var jsonResult = '';
function letterStart() {
    $('#btnAddLetterRefrence').click(function () {
        if ($('#recivers').val().length > 0)
            $('#addLetterRefrenceForm').submit();
    });
    setKeyUpSearch();
    $('#filterRecivers').keyup(function () {
        var _filter = $(this).val();
        $('.firstAndLastName').each(function (i, element) {
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
    $('.addStatement1').click(function () {
        var _id = $(this).attr("id");
        $('#Transcript1').val(($('#statement' + _id).html()));
    });
    $('.addStatement2').click(function () {
        var _id = $(this).attr("id");
        $('#Transcript2').val(($('#statement' + _id).html()));
    });
    $('.addStatement3').click(function () {
        var _id = $(this).attr("id");
        $('#Transcript3').val(($('#statement' + _id).html()));
    });
    $('.addStatement4').click(function () {
        var _id = $(this).attr("id");
        $('#Transcript4').val(($('#statement' + _id).html()));
    });
    $('.addStatement5').click(function () {
        var _id = $(this).attr("id");
        $('#Transcript5').val(($('#statement' + _id).html()));
    });
    $('.addReciver').click(function () {
        var _firstAndLastName = $('#firstAndLastName' + $(this).attr('id')).html();
        var _ID = $(this).attr('id');
        var newItem = {
            firstAndLastName: _firstAndLastName,
            ID: _ID
        };

        if (!reciverIsExist(newItem)) {
            reciverList.push({
                firstAndLastName: _firstAndLastName,
                ID: _ID
            });
            displayReciversInTable();
        }
        else
            alert("گیرنده تکراری است");
        $('#recivers').val(createResualtRecivers());
    });
    $('#submitLetter').click(function () {
        var aaa = $('#recivers').val();
        if ($('#recivers').val() != "")
            $('#letterForm').submit();
    });
    $('#printLetter').click(function () {
        getPrintableLetter(false);
    });

    $('#printLetterWithRefrences').click(function () {
        getPrintableLetter(true);
    });
};
var getPrintableLetter = function (withRefrences) {
    var refrences = [];
    refrences = getTimelineItems();
    var sender = $('#letterSender').html();
    var number = $('#letterNumber').html();
    var date = $('#letterDate').html();
    var subject = $('#letterSubject').html();
    var content = $('#letterContent').html();
    var result = '';
    for (var i = 0; i < refrences.length; i++) {
        var recivers = '';
        for (j = 0; j < refrences[i].recivers.length; j++) {
            recivers += '[' + refrences[i].recivers[j] + '] ';
        }
        if (i == 0)
            result += '<div  style="border:1px solid #ddd; padding:5px;font-family:Tahoma">' +
                                '<table width="100%" style="direction:rtl">' +
                                    '<tr>' +
                                        '<td>شماره نامه : ' + number + '</td>' +
                                        '<td>فرستنده : ' + refrences[0].sender + '</td>' +
                                        '<td>تاریخ : ' + date + '</td>' +
                                    '</tr>' +
                                    '<tr>' +
                                        '<td colspan="3"><hr style="border:1px solid #ddd"/></td>' +
                                    '</tr>' +
                                    '<tr>' +
                                     '<td colspan="3">گیرندگان : ' + recivers + '<br /> <br /> </td>' +
                                    '</tr>' +
                                    '<tr>' +
                                        '<td colspan="3">موضوع : ' + subject + '</td>' +
                                    '</tr>' +
                                    '<tr>' +
                                        '<td colspan="3">' + content + '</td>' +
                                    '</tr>' +
                                '</table>' +
                             '</div><br />';
        else
            if (withRefrences)
                result += '<div  style="border:1px solid #ddd; padding:5px;font-family:Tahoma;font-size:11px">' +
                                 '<table width="100%" style="direction:rtl">' +
                                     '<tr>' +
                                         '<td colspan="2">' +
                                            'ارجاع نامه توسط : ' + refrences[i].sender +
                                         '</td>' +
                                         '<td>' +
                                            'تاریخ : ' + refrences[i].date +
                                         '</td>' +
                                     '</tr>' +
                                        '<tr>' +
                                            '<td colspan="3"><hr style="border:1px solid #ddd"/></td>' +
                                        '</tr>' +
                                     '<tr>' +
                                         '<td colspan="3">گیرندگان : ' + recivers + '<br /><br />  رونوشت:<br />' + refrences[i].transcript + '</td>' +
                                     '</tr>' +
                                 '</table>' +
                              '</div><br />';

    }
    //var style = '<style>@media print{color:red}</style>';
    var printWindow = window.open('', '');
    printWindow.document.write('<html  moznomarginboxes mozdisallowselectionprint><head><title>' + subject + '</title>');
    //printWindow.document.write("<link href='~/Content/print.css' media='print' rel='stylesheet' type='text/css' />\n");
    //printWindow.document.write("<link href='~/Content/print.css' media='screen' rel='stylesheet' type='text/css' />\n");
    printWindow.document.write('</head><body>');
    printWindow.document.write(result);
    printWindow.document.write('</body></html>');
    printWindow.document.close();
    printWindow.print();
}
var getTimelineItems = function () {
    var result = [];
    $('.timeline-label').each(function () {
        var sender = $(this).find('span.sender').html();
        var recivers = [];
        $(this).find('ul li .reciver').each(function (i, element) {
            recivers.push($(this).html());
        });
        var transcript = $(this).find('.Transcript').html();
        var date = $(this).parent().find('.date').html();
        var item = {
            sender: sender,
            recivers: recivers,
            transcript: transcript,
            date: date
        };
        recivers = [];
        result.push(item);
    });
    return result;
}
var reciverIsExist = function (newItem) {
    var index = jQuery.inArray(
        JSON.stringify(newItem),
        $.map(reciverList, JSON.stringify));
    return index == -1 ? false : true;
};
function reciverListRemoveItem(ID) {
    for (var i = 0; i < reciverList.length; i++)
        if (reciverList[i].ID === ID) {
            reciverList.splice(i, 1);
            break;
        }
    displayReciversInTable();
    $('#recivers').val(createResualtRecivers());
}
function displayReciversInTable() {
    $('#reciversTBody').empty();
    for (var i = 0; i < reciverList.length; i++) {
        var _ID = reciverList[i].ID;
        var _firstAndLastName = reciverList[i].firstAndLastName;
        var _tr = '<tr>'
                + '<td  class="hidden">' + _ID + '</td>'
                + '<td>' + _firstAndLastName + '</td>'
                + '<td>'
                    + '<select class="form-control statusSelect">'
                        + '<option value="1">جهت مشاهده و ارجاع</option>'
                        + '<option value="0">جهت اطلاع</option>'
                    + '</select>'
                + '</td>'
                + '<td><a hre="#" class="removeTR">حذف</a></td>'
         + '</tr>';
        $().parent().parent().children().first();
        $('#reciversTBody').append(_tr);

    }
    $('.removeTR').click(function () {
        var removeID = $(this).parent().parent().children().first().html();
        reciverListRemoveItem(removeID);
    });
    $('.statusSelect').change(function () {
        $('#recivers').val(createResualtRecivers());
    });

}
var createResualtRecivers = function () {
    var result = [];
    $('#reciversTBody tr').each(function () {
        var _td = $(this).find("td");
        var _ID = _td.eq(0).html();
        var _status = _td.eq(2).find("select").val();
        var item = { ID: _ID, status: _status };
        result.push(item);
    });
    return JSON.stringify(result) == '[]' ? "" : JSON.stringify(result);
}
//================================ End Letter Scripts

//================================ Set Permission
var userList = [];

var jsonUserResult = '';
var userIsExist = function (newItem) {
    var index = jQuery.inArray(
    JSON.stringify(newItem),
    $.map(userList, JSON.stringify));
    return index == -1 ? false : true;
}
function displayUsersInTable() {
    $('#usersTBody').empty();
    for (var i = 0; i < userList.length; i++) {
        var _ID = userList[i].ID;
        var _firstAndLastName = userList[i].firstAndLastName;
        
        var _tr = '<tr>'
                + '<td  class="hidden">' + _ID + '</td>'
                + '<td>' + _firstAndLastName + '</td>'
                 
                + '<td><a href="#" class="removeTR">حذف</a></td>'
         + '</tr>';
        $().parent().parent().children().first();
        $('#usersTBody').append(_tr);

    }
    $('.removeTR').click(function () {
        var removeID = $(this).parent().parent().children().first().html();
        userListRemoveItem(removeID);
    });
}
function setPermissionStart() {
    setKeyUpSearch();
    $('.addUser').click(function () {
        var _firstAndLastName = $('#firstAndLastName' + $(this).attr('id')).html();
        var _ID = $(this).attr('id');
        var _groupName = $('#group' + $(this).attr('id')).html();
        var newItem = {
            firstAndLastName: _firstAndLastName,
            ID: _ID,
            Group: _groupName
        };

        if (!userIsExist(newItem)) {
            userList.push({
                firstAndLastName: _firstAndLastName,
                ID: _ID
                ,
                Group: _groupName
            });
            displayUsersInTable();
        }
        else
            alert("کاربر تکراری است");
        $('#otherUsers').val(createResualtUsers());
    });
};
function userListRemoveItem(ID) {
    for (var i = 0; i < userList.length; i++)
        if (userList[i].ID === ID) {
            userList.splice(i, 1);
            break;
        }
    displayUsersInTable();
    $('#otherUsers').val(createResualtUsers());
}
var createResualtUsers = function () {
    var result = [];
    $('#usersTBody tr').each(function () {
        var _td = $(this).find("td");
        var _ID = _td.eq(0).html();
        var item = { ID: _ID };
        result.push(item);
    });
    return JSON.stringify(result) == '[]' ? "" : JSON.stringify(result);
}
//================================ End Set Permission



//================================ Users
function peopleStart() {
    $('#filterUsers').keyup(function () {
        var _filter = $(this).val();
        $('.usersTR').each(function (i, element) {
            if (_filter.length < 1)
                $(this).parent().show()
            else {
                if ($(this).find('.firstName').html().indexOf(_filter) >= 0 || $(this).find('.lastName').html().indexOf(_filter) >= 0)
                    $(this).show();
                else
                    $(this).hide();
            }
        });
    });
}
function userStart() {
    setKeyUpSearch();
}
//================================ End Users

function setKeyUpSearch() {
    $('#filterRecivers').keyup(function () {
        var _filter = $(this).val();
        $('.firstAndLastName').each(function (i, element) {
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
}

//=============================SearchBoxDropDown
(function ($) {

    $.fn.searchit = function (options) {

        return this.each(function () {

            $.fn.searchit.globals = $.fn.searchit.globals || {
                counter: 0
            }
            $.fn.searchit.globals.counter++;
            var $counter = $.fn.searchit.globals.counter;

            var $t = $(this);
            var opts = $.extend({}, $.fn.searchit.defaults, options);

            // Setup default text field and class
            if (opts.textField == null) {
                $t.before("<input type='textbox' id='__searchit" + $counter + "'><br>");
                opts.textField = $('#__searchit' + $counter);
            }
            if (opts.textField.length > 1) opts.textField = $(opts.textField[0]);

            if (opts.textFieldClass) opts.textField.addClass(opts.textFieldClass);
            //MY CODE-------------------------------------------------------------------
            if (opts.selected) opts.textField.val($(this).find(":selected").val());
            //MY CODE ENDS HERE -------------------------------------------------------
            if (opts.dropDown) {
                $t.css("padding", "5px")
                    .css("margin", "-5px -20px -5px -5px");

                $t.wrap("<div id='__searchitWrapper" + $counter + "' />");
                opts.wrp = $('#__searchitWrapper' + $counter);
                opts.wrp.css("display", "inline-block")
                    .css("vertical-align", "top")
                    .css("overflow", "hidden")
                    .css("border", "solid grey 1px")
                    .css("position", "absolute")
                    .hide();
                if (opts.dropDownClass) opts.wrp.addClass(opts.dropDownClass);
            }

            opts.optionsFiltered = [];
            opts.optionsCache = [];

            // Save listbox current content
            $t.find("option").each(function (index) {
                opts.optionsCache.push(this);
            });

            // Save options 
            $t.data('opts', opts);

            // Hook listbox click
            $t.click(function (event) {
                _opts($t).textField.val($(this).find(":selected").text());
                _opts($t).wrp.hide();
                event.stopPropagation();
            });

            // Hook html page click to close dropdown
            $("html").click(function () {
                _opts($t).wrp.hide();
            });

            // Hook the keyboard and we're done
            _opts($t).textField.keyup(function (event) {
                if (event.keyCode == 13) {
                    $(this).val($t.find(":selected").text());
                    _opts($t).wrp.hide();
                    return;
                }
                setTimeout(_findElementsInListBox($t, $(this)), 50);
            })

        })


        function _findElementsInListBox(lb, txt) {

            if (!lb.is(":visible")) {
                _showlb(lb);
            }

            _opts(lb).optionsFiltered = [];
            var count = _opts(lb).optionsCache.length;
            var dropDown = _opts(lb).dropDown;
            var searchText = txt.val().toLowerCase();

            // find match (just the old classic loop, will make the regexp later)
            $.each(_opts(lb).optionsCache, function (index, value) {
                if ($(value).text().toLowerCase().indexOf(searchText) > -1) {
                    // save matching items 
                    _opts(lb).optionsFiltered.push(value);
                }

                // Trigger a listbox reload at the end of cycle    
                if (!--count) {
                    _filterListBox(lb);
                }
            });
        }

        function _opts(lb) {
            return lb.data('opts');
        }

        function _showlb(lb) {
            if (_opts(lb).dropDown) {
                var tf = _opts(lb).textField;
                lb.attr("size", _opts(lb).size);
                _opts(lb).wrp.show().offset({
                    top: tf.offset().top + tf.outerHeight(),
                    left: tf.offset().left
                });
                _opts(lb).wrp.css("width", tf.outerWidth() + "px");
                lb.css("width", (tf.outerWidth() + 25) + "px");
            }
        }

        function _filterListBox(lb) {
            lb.empty();

            if (_opts(lb).optionsFiltered.length == 0) {
                lb.append("<option>" + _opts(lb).noElementText + "</option>");
            } else {
                $.each(_opts(lb).optionsFiltered, function (index, value) {
                    lb.append(value);
                });
                lb[0].selectedIndex = 0;
            }
        }
    }

    $.fn.searchit.defaults = {
        textField: null,
        textFieldClass: null,
        dropDown: true,
        dropDownClass: null,
        size: 5,
        filtered: true,
        noElementText: "No elements found",
        //MY CODE------------------------------------------
        selected: false
        //MY CODE ENDS ------------------------------------
    }

}(jQuery))


