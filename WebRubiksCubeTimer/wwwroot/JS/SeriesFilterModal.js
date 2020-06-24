$('#StartTimeStamp').datetimepicker({
    format: 'L'
});

$('#EndTimeStamp').datetimepicker({
    format: 'L'
});

$('#AdjustButton').click(function () {
    $('#StartDate').val('');
    $('#EndDate').val('');
    $('#CategoryId').val(0);
    $('#CubeId').prop('selectedIndex', 0);
    $('#CategoryOptionId').prop('selectedIndex', 0);
    $('#CubeId').attr('disabled', true);
    $('#CategoryOptionId').attr('disabled', true);
    $('#SeriesAmount').val($('#DefaultLimit').val())
    $('#AdjustModal').modal('show');
})

$('#CategoryId').change(function () {
    let categoryId = $('#CategoryId').val();
    let userId = $('#UserId').val();
    $.get('/Series/GetUserCubesInCategoryList', { id: userId, categoryId: categoryId },
        function (response) {
            if (response.idFaulted) {
                $('#AdjustModal').modal('hide');
            }
            ResponseCheck(response, HandleCubesListResponse);
        })
})

$('#CubeId').change(function () {
    let cubeId = $('#CubeId').val();
    let userId = $('#UserId').val();
    $.get('/Series/GetUserCategoryOptionsOfCubeList', { id: userId, cubeId: cubeId },
        function (response) {
            if (response.idFaulted) {
                $('#AdjustModal').modal('hide');
            }
            ResponseCheck(response, HandleOptionsListResponse);
        })
})

function HandleCubesListResponse(model) {
    if (model.length > 0) {
        $('#CubeId').empty();
        $('#CubeId').append('<option selected disabled>No cube selected</option>');
        for (var i = 0; i < model.length; i++) {
            $('#CubeId').append('<option value="' + model[i].identity
                + '">' + model[i].manufacturer.name + ' ' + model[i].modelName +
                ' ' + model[i].plasticColor.name + '</option>');
        }
        $('#CubeId').removeAttr('disabled');
        DisableOptionsComboBox();
    }
    else {
        $('#CubeId').attr('disabled', true);
    }
}

function HandleOptionsListResponse(model) {
    if (model.length > 0) {
        $('#CategoryOptionId').empty();
        $('#CategoryOptionId').append('<option selected disabled>No option selected</option>');
        for (var i = 0; i < model.length; i++) {
            $('#CategoryOptionId').append('<option value="' + model[i].identity
                + '">' + model[i].name + '</option>');
        }
        $('#CategoryOptionId').removeAttr('disabled');
    }
    else {
        $('#CategoryOptionId').attr('disabled', true);
    }
}

function DisableOptionsComboBox() {
    $('#CategoryOptionId').empty();
    $('#CategoryOptionId').append('<option selected disabled>No option selected</option>');
    $('#CategoryOptionId').prop('selectedIndex', 0);
    $('#CategoryOptionId').attr('disabled', true);
}