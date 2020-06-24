$('#SelectButton').click(function () {
    $('#ExcludeModal').modal('show');
})

$('#ExcludeModal').on('hide.bs.modal', function () {
    let excludes = $('[id^="ExcludedDomains_"]');
    let excludesString = [];
    if (excludes.length > 0) {
        for (var i = 0; i < excludes.length; i++) {
            if (excludes[i].checked) {
                excludesString.push(excludes[i].placeholder)
            }
        }
    }
    let text = excludesString.join(', ');
    $('#Excluded').val(text);
})

function CheckLengthsOfInpute() {
    let subject = $('#Subject').val();
    let message = $('#Message').val();
    if (subject.length > 0 && message.length > 0) {
        $('#SendButton').removeAttr('disabled');
    }
    else {
        $('#SendButton').attr('disabled', true);
    }
}

$('#SendButton').click(function () {
    $('#ConfirmModal').modal('show');
})