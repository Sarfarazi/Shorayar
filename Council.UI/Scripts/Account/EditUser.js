$('select#SelectedType').multipleSelect({
    filter: true,
    selectAll: false,
    minimumCountSelected: 4,
    delimiter: ', ',
    formatCountSelected: () => '# از % مورد',
    formatNoMatchesFound: () => 'نقشی پیدا نشد',
    onClick: function () {
        var selected = $('select#SelectedType').multipleSelect('getSelects', 'value');
        $("#SelectedTypes").val(selected);
    }
});

var SelectedTypes = $("#SelectedTypes").val();
if (SelectedTypes) {
    $('select#SelectedType').multipleSelect('setSelects', SelectedTypes.split(','))
}

$.validator.setDefaults({ ignore: null });