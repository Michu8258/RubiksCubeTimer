$(document).ready(function () {
    InitializaPagination();
})

function OpenRemoveModal(cubeId, userId) {
    $('#CubeIdRemove').val(cubeId);
    $('#UserIdRemove').val(userId);
    $('#RemoveCubeModal').modal('show');
}

$('#RemoveCubeButton').click(function () {
    $('#RemoveCubeModal').modal('hide');
    let userId = $('#UserIdRemove').val();
    let cubeId = $('#CubeIdRemove').val();
    $.post('/User/RemoveCubeFromCollection', { userId: userId, cubeId: cubeId },
        function (response) {
            if (response.isFaulted) {
                let data = {};
                data.responseText = JSON.stringify(response);
                DisplayMessages(data);
            }
            else {
                location.reload();
            }
        })
})

function OpenRatingModal(userId, cubeId, cubeDescription) {
    $('#CubeIdRate').val(cubeId);
    $('#UserIdRate').val(userId);
    $('#RatingModalDesc').text(cubeDescription);
    $('#AddCubeRatingModal').modal('show');
}

function SendRatingRequest(rating) {
    $('#AddCubeRatingModal').modal('hide');
    let userId = $('#UserIdRate').val();
    let cubeId = $('#CubeIdRate').val();
    $.post('/User/AddCubeRatingForUser', { userId: userId, cubeId: cubeId, rating: rating },
        function (response) {
            if (response.isFaulted) {
                let data = {};
                data.responseText = JSON.stringify(response);
                DisplayMessages(data);
            }
            else {
                location.reload();
            }
        })
}

function OpenReRatingModal(userId, cubeId, description, currentRating) {
    $('#CurrentCubeRate').text(currentRating);
    $('#RCubeIdRate').val(cubeId);
    $('#RUserIdRate').val(userId);
    $('#ReratingModalDesc').text(description);
    $('#ModCubeRatingModal').modal('show');
}

function SendReRatingRequest(rating) {
    $('#ModCubeRatingModal').modal('hide');
    let userId = $('#RUserIdRate').val();
    let cubeId = $('#RCubeIdRate').val();
    $.post('/User/ModifyCubeRatingForUser', { userId: userId, cubeId: cubeId, newRating: rating },
        function (response) {
            if (response.isFaulted) {
                let data = {};
                data.responseText = JSON.stringify(response);
                DisplayMessages(data);
            }
            else {
                location.reload();
            }
        })
}

function ShowCubeInfo(cubeId) {
    $.get('/User/GetCubeInfo', { cubeId: cubeId },
        function (response) {
            if (response.isFaulted) {
                let data = {};
                data.responseText = JSON.stringify(response);
                DisplayMessages(data);
            }
            else {
                PopulateCubeInfoModal(response.jsonData);
            }
        })
}

function PopulateCubeInfoModal(data) {
    $('#InfoManCountry').text(data.manufacturerCountry);
    $('#InfoManYear').text(data.manufacturerFoundationYear);
    $('#InfoUsersAmount').text(data.usersUsingThisCube);
    $('#InfoOptList').empty();
    if (data.permittedCategoryOptions.length > 0) {
        for (var i = 0; i < data.permittedCategoryOptions.length; i++) {
            $('#InfoOptList').append('<strong class="d-flex">' + data.permittedCategoryOptions[i] + '</strong>');
        }
    }
    $('#CubeInfoModal').modal('show');
}

$('#CategoryId').change(function () {
    $('#ManufacturerId').prop('selectedIndex', 0);
    $('#CubeId').prop('selectedIndex', 0);
    let userId = $('#CurrentUserId').val();
    let categoryId = $('#CategoryId').val();
    $.get('/User/GetAvailableToAddManufacturers/', { categoryId: categoryId, userId: userId },
        function (response) {
            ResponseCheck(response, PopulateManufacurers)
        })
})

function PopulateManufacurers(data) {
    SetDefaultsComboBox('ManufacturerId', 'No manufacturer selected');
    SetDefaultsComboBox('CubeId', 'No cube selected');
    for (var i = 0; i < data.length; i++) {
        $('#ManufacturerId').append('<option value="' + data[i].identity
            + '">' + data[i].name + '</option>');
    }
    if (data.length > 0) {
        $('#ManufacturerId').removeAttr('disabled');
    }
}

$('#ManufacturerId').change(function () {
    $('#CubeId').prop('selectedIndex', 0);
    let userId = $('#CurrentUserId').val();
    let categoryId = $('#CategoryId').val();
    let manuId = $('#ManufacturerId').val();
    $.get('/User/GetAvailableToAddCubes/', { categoryId: categoryId, manufacturerId: manuId, userId: userId },
        function (response) {
            ResponseCheck(response, PopulateCubes)
        })
})

function PopulateCubes(data) {
    SetDefaultsComboBox('CubeId', 'No cube selected');
    for (var i = 0; i < data.length; i++) {
        $('#CubeId').append('<option value="' + data[i].identity
            + '">' + data[i].modelName + '</option>');
    }
    if (data.length > 0) {
        $('#CubeId').removeAttr('disabled');
    }
}

function SetDefaultsComboBox(id, defaultText) {
    $('#' + id).empty();
    $('#' + id).append('<option selected disabled>' + defaultText + '</option>');
    $('#' + id).prop('selectedIndex', 0);
    $('#' + id).attr('disabled', true);
    EnableAddButton();
}

$('#CubeId').change(function () {
    EnableAddButton();
})

function EnableAddButton() {
    if ($('#CubeId').val() < 1) {
        $('#AddCubeToCollection').attr('disabled', true);
    }
    else {
        $('#AddCubeToCollection').removeAttr('disabled');
    }
}

$('#AddCubeToCollection').click(function () {
    let userId = $('#CurrentUserId').val();
    let cubeId = $('#CubeId').val();
    document.location = '/User/AddCubeTouserCollection?userId=' + userId + '&cubeId=' + cubeId;
})