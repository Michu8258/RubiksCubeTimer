$(document).ready(function () {
    InitializaPagination();
})

$('#AddColorButton').click(function () {
    $('#NewColorInput').val('');
    document.getElementById("AddConfirmColorButton").setAttribute('disabled', true);
    document.getElementById("NewColorLength").classList.add('invisible');
    $('#AddColorModal').modal('show');
});

function NewColorLength(value) {
    if (value.length < 5) {
        document.getElementById("AddConfirmColorButton").setAttribute('disabled', true);
        document.getElementById("NewColorLength").classList.remove('invisible');
    }
    else {
        $('#AddConfirmColorButton').removeAttr('disabled');
        document.getElementById("NewColorLength").classList.add('invisible');
    }
};

$('#AddConfirmColorButton').click(function () {
    let color = $('#NewColorInput').val();
    SendAddPlasticColorRequest(color);
});

function SendAddPlasticColorRequest(color) {
    $.ajax({
        url: '/Cubes/AddCubePlasticColor',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(color),
        complete: function (response) {
            HandlleAddColorData(JSON.parse(response.responseText), response);
        }
    });
};

function HandlleAddColorData(response, notParsed) {
    if (response.isFaulted) {
        DisplayMessages(notParsed);
    }
    else {
        if ($('#PlasticColorTableAmount').val() == '0') {
            $('#ColorTableBody').empty();
        }
        AddColorTableRow(response.jsonData, 'PlasticColorTable');
    }
};

function DeleteColor(color) {
    $('#ColorToDelete').text(color);
    $('#DeleteColorModal').modal('show');
};

$('#DeleteColorButton').click(function () {
    $('#DeleteColorModal').modal('hide');
    SendDeletePlasticColorRequest($('#ColorToDelete').text());
});

function SendDeletePlasticColorRequest(color) {
    $.ajax({
        url: '/Cubes/DeleteCubePlasticColor',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(color),
        complete: function (response) {
            HandlleDeleteColorData(JSON.parse(response.responseText), response);
        }
    });
};

function HandlleDeleteColorData(response, notParsed) {
    if (response.isFaulted) {
        DisplayMessages(notParsed);
    }
    else {
        $('#ColorTableBody').empty();
        $('#PlasticColorTableAmount').val(0);
        if (response.jsonData.length >= 1) {
            for (var i = 0; i < response.jsonData.length; i++) {
                AddColorTableRow(response.jsonData[i], 'PlasticColorTable');
            }
        }
        else {
            $('#PlasticColorTable').append('<tr><td colspan="3" class="text-center">No colors defined</td></tr>');
        }
    }
};

function AddColorTableRow(rowData, tableId) {
    $('#PlasticColorTable').append('<tr><td class="align-middle">' + rowData.name + '</td><td>' +
        '<button class="btn btn-sm btn-danger text-white p-1" ' +
        'onclick = "DeleteColor(\'' + rowData.name + '\')" > Delete</button ></td ></tr > ');
    let rowCount = $('#' + tableId + ' tr').length - 1;
    $('#' + tableId + 'Amount').val(rowCount);
};

$('#AddManufacturer').click(function () {
    $('#NewManuName').val('');
    $('#NewManuNameAlert').addClass('d-none');
    $('#NewManuCountry').val('');
    $('#NewManuCountryAlert').addClass('d-none');
    $('#NewManuYear').val('');
    $('#NewManuYearAlert').addClass('d-none');
    $('#AddManuModal').modal('show');
})

$('#AddManuButton').click(function () {
    if (ValidateManuModel('NewManuName', 'NewManuNameAlert', 'NewManuCountry',
        'NewManuCountryAlert', 'NewManuYear', 'NewManuYearAlert')) {
        $('#AddManuModal').modal('hide');
        SendAddManufacturerRequest($('#NewManuName').val(),
            $('#NewManuCountry').val(), $('#NewManuYear').val());
    }
});

function ValidateManuModel(nameId, nameAlert, countryId, countryAlert, yearId, yearAlert) {
    let ok1 = 0;
    let ok2 = 0;
    let ok3 = 0;
    if ($('#' + nameId).val().length < 1 || !(/^[a-zA-Z0-9- ]*$/.test($('#' + nameId).val()))) {
        $('#' + nameAlert).removeClass('d-none');
    }
    else {
        $('#' + nameAlert).addClass('d-none');
        ok1 = 1;
    }
    if ($('#' + countryId).val().length < 1 || !(/^[a-zA-Z0-9- ]*$/.test($('#' + countryId).val()))) {
        $('#' + countryAlert).removeClass('d-none');
    }
    else {
        $('#' + countryAlert).addClass('d-none');
        ok2 = 1;
    }
    if ($('#' + yearId).val() < 0 || $('#' + yearId).val() > 2500 || $('#' + yearId).val().length < 1) {
        $('#' + yearAlert).removeClass('d-none');
    }
    else {
        $('#' + yearAlert).addClass('d-none');
        ok3 = 1;
    }

    return ok1 + ok2 + ok3 == 3;
}

function SendAddManufacturerRequest(name, country, year) {
    let model = { "name": name, "country": country, "year": year};

    $.ajax({
        url: '/Cubes/AddManufacturer',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(model),
        complete: function (response) {
            HandleAddManuResponse(JSON.parse(response.responseText), response);
        }
    });
};

function HandleAddManuResponse(response, notParsed) {
    if (response.isFaulted) {
        DisplayMessages(notParsed);
    }
    else {
        if ($('#AmountManu').val() == '0') {
            $('#ManuTableBody').empty();
        }
        AddManuTableRow(response.jsonData);
    }
}

function AddManuTableRow(rowData) {
    $('#ManuTable').append('<tr><td class="align-middle">' + rowData.name + '</td>' +
        '<td class="align-middle">' + rowData.country + '</td>' +
        '<td class="align-middle">' + rowData.foundationYear + '</td>' +
        '<td class="align-middle"><button class="btn btn-sm btn-danger text-white" onclick="DeleteManuOpenModal(\'' + rowData.name + '\')">Delete</button></td>' +
        '<td class="align-middle"><button class="btn btn-sm btn-secondary text-white" onclick="ModifyManuModal(\'' + rowData.identity + '\', \'' + rowData.name +
        '\', \'' + rowData.country + '\', \'' + rowData.foundationYear + '\')">Modify</button></td></tr>');
    let rowCount = $('#ManuTable tr').length - 1;
    $('#AmountManu').val(rowCount);
}

function DeleteManuOpenModal (name) {
    $('#ManuToDelete').text(name);
    $('#DeleteManuModal').modal('show');
}

$('#DeleteManuButton').click(function () {
    $.ajax({
        url: '/Cubes/DeleteManufacturer',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify($('#ManuToDelete').text()),
        complete: function (response) {
            ReconstructManuTable(JSON.parse(response.responseText), response);
        }
    });
})

function ReconstructManuTable(response, notParsed) {
    if (response.isFaulted) {
        DisplayMessages(notParsed);
    }
    else {
        $('#ManuTableBody').empty();
        $('#AmountManu').val(0);
        if (response.jsonData.length >= 1) {
            for (var i = 0; i < response.jsonData.length; i++) {
                AddManuTableRow(response.jsonData[i]);
            }
        }
        else {
            $('#ManuTable').append('<tr><td colspan="5" class="text-center">No manufacturers defined</td></tr>');
        }
    }
};

function ModifyManuModal(id, name, country, year) {
    $('#ModManuIdent').val(id);
    $('#ModifyManuNameH').html(name);
    $('#ModManuName').val(name);
    $('#ModManuCountry').val(country);
    $('#ModManuYear').val(year);
    $('#ModManuNameAlert').addClass('d-none');
    $('#ModManuCountryAlert').addClass('d-none');
    $('#ModManuYearAlert').addClass('d-none');
    $('#ModifyManuModal').modal('show');
}

$('#ModManuButton').click(function () {
    if (ValidateManuModel('ModManuName', 'ModManuNameAlert', 'ModManuCountry',
        'ModManuCountryAlert', 'ModManuYear', 'ModManuYearAlert')) {
        $('#ModifyManuModal').modal('hide');
        let model = {
            "identity": $('#ModManuIdent').val(), "name": $('#ModManuName').val(),
            "country": $('#ModManuCountry').val(), "year": $('#ModManuYear').val()
        }
        SendManuUpdateRequest(model);
    }
});

function SendManuUpdateRequest(model) {
    $.ajax({
        url: '/Cubes/ModifyManufacturer',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(model),
        complete: function (response) {
            ReconstructManuTable(JSON.parse(response.responseText), response);
        }
    });
}

function DeleteCubeButton(id, modelName) {
    $('#CubeToDelete').val(id);
    $('#CubeToDeleteName').text(modelName)
    $('#DeleteCubeModal').modal('show');
}

$('#DeleteCubeButton').click(function () {
    let id = $('#CubeToDelete').val();
    $.post('/Cubes/DeleteCube', { identity: id },
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