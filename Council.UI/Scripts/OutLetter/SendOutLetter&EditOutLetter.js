

$('.ClearFrom').click(function () {
    $("#rowID").val('');
    $("#txtContent").val('');
    $('#Mytbody').empty();
};

function generateCellContent(tagName, text, value, index) {
    try {
        debugger;

        var res = "<input type='hidden' value='" + value + "' name=LetterDetails[" + index + "]." + tagName + " id=LetterDetails_" + index + "_" + tagName + " />";
        res += "<span>" + text + "</span>";
        return res;

    } catch (e) {

    }
}


function addToListOrderDetail() {

    try {

        debugger;

        var count = $("#rowID").val();
        let position = count.search($("#Organ2").val());
        if (position != -1) {
            alert("تکراری است");
            return 0;

        }

        count += "," + $("#Organ2").val();
        $("#rowID").val(count);

        //

        var content = $("#txtContent").val();
        content += "," + $("#Text_Letter").val();
        $("#txtContent").val(content);

        var table = document.getElementById("ListItemTbl");


        var rowId = parseInt(table.rows.length) - 1;
        var td1Html0 = generateCellContent("OrganID", $("#Organ2 option:selected").text(), $("#Organ2").val(), rowId) + "<input type='hidden' value='" + 0 + "' name=LetterDetails[" + rowId + "]." + "LetterDetailID" + " />";
        var td1Html1 = generateCellContent("Text_Letter", $("#Text_Letter").val(), $("#Text_Letter").val(), rowId);
        var td1Html2 = "<a class='fa fa-remove RequiredLabel' onclick='productDelete(this)'> </a>";

        //var newRow = [
        //    [
        //        (rowId + 1), td1Html0, td1Html1, td1Html2
        //    ]
        //];

        var newRow = [
            [
                (rowId + 1), td1Html0, td1Html1
            ]
        ];


        AddToTable("ListItemTbl", newRow, null, true);

        //setTotalAllPriceFinal('OrderDetail_TotallPriceStr');

        //Web.StoreProducts_Store.SetPriceProductStore("OrderDetail_ProductID", "OrderDetail_UnitPriceStr");

        //emptyAddToListOrderDetail();



        return false;

    } catch (e) {

    }

}

function clearCotent() {
    $("#rowID").val('');
    $("#txtContent").val('');
    $('#Mytbody').empty();
}

function productDelete(ctl) {
    $(ctl).parents("tr").remove();
}

function AddToTable(tblId, arrayList, hiddenCell = null, isTfootElement = false) {

    try {

        var table;

        if (!isTfootElement) table = document.getElementById(tblId);
        else table = document.getElementById(tblId).getElementsByTagName('tbody')[0];

        var rowId;

        for (var i = 0; i < arrayList.length; i++) {
            rowId = parseInt(table.rows.length);
            var row = table.insertRow(rowId);
            var indexRow = arrayList[i].length;
            for (var j = 0; j < indexRow; j++) {
                var cell1;
                if (hiddenCell == j)
                    table.rows[i + 1].cells[hiddenCell - 1].innerHTML += arrayList[i][j];
                else {
                    cell1 = row.insertCell(j);
                    cell1.innerHTML = arrayList[i][j];
                }
            }
        }

    } catch (e) {

    }
}

function removeRow(e, currentRowId) {

    try {

        var table = document.getElementById(getTableId(e));
        var rowId = parseInt(table.rows.length);

        //for (var i = currentRowId + 1; i < rowId; i++) {
        //    var r = table.rows[i];
        //    r.innerHTML = r.innerHTML.replace("[" + (i - 1) + "]", "[" + (i - 2) + "]");
        //    r.innerHTML = r.innerHTML.replace("[" + (i - 1) + "]", "[" + (i - 2) + "]");
        //    r.innerHTML = r.innerHTML.replace("[" + (i - 1) + "]", "[" + (i - 2) + "]");
        //    r.innerHTML = r.innerHTML.replace("[" + (i - 1) + "]", "[" + (i - 2) + "]");
        //    r.innerHTML = r.innerHTML.replace("[" + (i - 1) + "]", "[" + (i - 2) + "]");
        //    r.innerHTML = r.innerHTML.replace("[" + (i - 1) + "]", "[" + (i - 2) + "]");
        //    r.innerHTML = r.innerHTML.replace("[" + (i - 1) + "]", "[" + (i - 2) + "]");


        //    r.innerHTML = r.innerHTML.replace("(this," + (i) + ")", "(this," + (i - 1) + ")");
        //    r.innerHTML = r.innerHTML.replace("(this," + (i) + ")", "(this," + (i - 1) + ")");

        //    if (Number.isInteger(parseInt(r.cells[0].innerHTML.trim()))) r.cells[0].innerHTML = i - 1;
        //}
        table.deleteRow(currentRowId);


    } catch (e) {

    }

}
