function SetDefaultsComboBox(id, defaultText) {
    $('#' + id).empty();
    $('#' + id).append('<option selected disabled>' + defaultText + '</option>');
    $('#' + id).prop('selectedIndex', 0);
    $('#' + id).attr('disabled', true);
    EnableCreateButton();
}

$('#SelectedCategoryId').change(function () {
    $('#SelectedManufacturerId').prop('selectedIndex', 0);
    $('#SelectedCubeId').prop('selectedIndex', 0);
    $('#CategoryOptionId').prop('selectedIndex', 0);
    let userId = $('#UserIdentity').val();
    let catId = $('#SelectedCategoryId').val();
    $.get('/User/GetManufacturersOfUserCubes/', { userId: userId, categoryId: catId },
        function (response) {
            ResponseCheck(response, PopulateManufacturers)
        })
})

function PopulateManufacturers(data) {
    SetDefaultsComboBox('SelectedManufacturerId', 'Please select');
    SetDefaultsComboBox('SelectedCubeId', 'Please select');
    SetDefaultsComboBox('CategoryOptionId', 'Please select');
    for (var i = 0; i < data.length; i++) {
        $('#SelectedManufacturerId').append('<option value="' + data[i].identity
            + '">' + data[i].name + '</option>');
    }
    if (data.length > 0) {
        $('#SelectedManufacturerId').removeAttr('disabled');
    }
}

$('#SelectedManufacturerId').change(function () {
    $('#SelectedCubeId').prop('selectedIndex', 0);
    $('#CategoryOptionId').prop('selectedIndex', 0);
    let userId = $('#UserIdentity').val();
    let catId = $('#SelectedCategoryId').val();
    let manId = $('#SelectedManufacturerId').val();
    $.get('/User/GetCubesOfUserCubes/', { userId: userId, categoryId: catId, manufacturerId: manId },
        function (response) {
            ResponseCheck(response, PopulateCubes)
        })
})

function PopulateCubes(data) {
    SetDefaultsComboBox('SelectedCubeId', 'Please select');
    SetDefaultsComboBox('CategoryOptionId', 'Please select');
    for (var i = 0; i < data.length; i++) {
        $('#SelectedCubeId').append('<option value="' + data[i].identity
            + '">' + data[i].modelName + ' ' + data[i].plasticColor.name + '</option>');
    }
    if (data.length > 0) {
        $('#SelectedCubeId').removeAttr('disabled');
    }
}

$('#SelectedCubeId').change(function () {
    $('#CategoryOptionId').prop('selectedIndex', 0);
    let userId = $('#UserIdentity').val();
    let catId = $('#SelectedCategoryId').val();
    let manId = $('#SelectedManufacturerId').val();
    let cubeId = $('#SelectedCubeId').val();
    $.get('/User/GetCategoryOptionsForCube/', {
        userId: userId, categoryId: catId,
        manufacturerId: manId, cubeId: cubeId
    },
        function (response) {
            ResponseCheck(response, PopulateOptions)
        })
})

function PopulateOptions(data) {
    SetDefaultsComboBox('CategoryOptionId', 'Please select');
    for (var i = 0; i < data.length; i++) {
        $('#CategoryOptionId').append('<option value="' + data[i].identity
            + '">' + data[i].name + '</option>');
    }
    if (data.length > 0) {
        $('#CategoryOptionId').removeAttr('disabled');
    }
}

function EnableCreateButton() {
    if ($('#CategoryOptionId').val() < 1) {
        $('#CreateButton').attr('disabled', true);
    }
    else {
        $('#CreateButton').removeAttr('disabled');
    }
}

$('#CategoryOptionId').change(function () {
    EnableCreateButton();
})
