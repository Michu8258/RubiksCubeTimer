$('#AddOption').click(function () {
    $('#NewOptionInput').val('');
    $('#NewOptionIssues').addClass('d-none');
    $('#AddOptionModal').modal('show');
});

$('#ConfirmOptionAdd').click(function () {
    let name = $('#NewOptionInput').val();
    if (name.length < 5 || !(/^[a-zA-Z0-9- ]*$/.test(name))) {
        $('#NewOptionIssues').removeClass('d-none');
    }
    else {
        $('#NewOptionIssues').addClass('d-none');
        $('#AddOptionModal').modal('hide');
        SendNewOptionRequest(name);
    }
});

function HandleResponse(response) {
    let deserialized = JSON.parse(response.responseText);
    if (deserialized.isFaulted) {
        DisplayMessages(response);
    }
    else {
        location.reload();
    }
}

function HandleDeserializedResponse(deserialized) {
    if (deserialized.isFaulted) {
        let data = {};
        data.responseText = JSON.stringify(deserialized);
        DisplayMessages(data);
    }
    else {
        location.reload();
    }
}

function SendNewOptionRequest(name) {
    $.ajax({
        url: '/Categories/AddCategoryOption',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(name),
        complete: function (response) {
            HandleResponse(response);
        }
    })
}

function DeleteCategoryOption(name) {
    $('#OptionToDelete').text(name);
    $('#DeleteOptionModal').modal('show');
}

$('#DeleteOptionButton').click(function () {
    let option = $('#OptionToDelete').text();
    $.ajax({
        url: '/Categories/DeleteCategoryOption',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(option),
        complete: function (response) {
            HandleResponse(response);
        }
    })
})

function UpdateCategoryOption(id, name) {
    $('#UpdateOptionId').val(id);
    $('#UpdateOptionInput').val(name);
    $('#ModifyOptionIssues').addClass('d-none');
    $('#UpdateOptionModal').modal('show');
}

$('#ConfirmOptionModify').click(function () {
    let newName = $('#UpdateOptionInput').val();
    if (newName.length < 5 || !(/^[a-zA-Z0-9- ]*$/.test(newName))) {
        $('#ModifyOptionIssues').removeClass('d-none');
    }
    else {
        let id = $('#UpdateOptionId').val();
        $('#ModifyOptionIssues').addClass('d-none');
        $('#UpdateOptionModal').modal('hide');
        $.post('/Categories/UpdateCategoryOption', { identity: id, name: newName },
            function (response) {
                HandleDeserializedResponse(response);
            });
    }
})

$('#AddCategory').click(function () {
    $('#NewCategoryIssues').addClass('d-none');
    $('#NewCategoryIssuesOpt').addClass('d-none');
    $('#NewCategoryIssuesTime').addClass('d-none');
    $('#NewCategoryInput').val('');
    $('#AddCategoryModal').modal('show');
})

$('#ConfirmCategoryAdd').click(function () {
    let newName = $('#NewCategoryInput').val();
    let amount = CheckIfOneSelected('OptionID_');
    let ok1 = false;
    let ok2 = false;
    let ok3 = false;
    if (newName.length < 5 || !(/^[a-zA-Z0-9- ]*$/.test(newName))) {
        $('#NewCategoryIssues').removeClass('d-none');
    }
    else {
        $('#NewCategoryIssues').addClass('d-none');
        ok1 = true;
    }
    if (!amount) {
        $('#NewCategoryIssuesOpt').removeClass('d-none');
    }
    else {
        $('#NewCategoryIssuesOpt').addClass('d-none');
        ok2 = true;
    }
    ok3 = VerifyTime(1, $('#NewDuration1').val())
    if (ok3 || $('#NewDuration1').val() == '') {
        $('#NewCategoryIssuesTime').addClass('d-none');
        ok3 = true;
    }
    else {
        $('#NewCategoryIssuesTime').removeClass('d-none');
        ok3 = false;
    }
    if (ok1 && ok2 && ok3) {
        $('#AddCategoryModal').modal('hide');
        SendNewCategoryRequest(newName, 'OptionID_');
    }
})

function CheckIfOneSelected(prefix) {
    let objects = $('[id^=' + prefix + ']');
    let amount = 0;
    for (var i = 0; i < objects.length; i++) {
        if (objects[i].checked) {
            amount++;
        }
    }
    return amount >= 1;
}

function SendNewCategoryRequest(name, prefix) {
    let objects = $('[id^=' + prefix + ']');
    let options = [];
    for (var i = 0; i < objects.length; i++) {
        if (objects[i].checked) {
            options.push(objects[i].name);
        }
    }
    $.post('/Categories/AddCategory/', {
        name: name, options: options,
        minimalDuration: $('#NewDuration1').val()
    },
        function (response) {
            HandleDeserializedResponse(response);
        });
}

function DeleteCategory(id, name) {
    $('#CategoryToDelete').text(name);
    $('#CategoryIdDelete').val(id);
    $('#DeleteCategoryModal').modal('show');
}

$('#DeleteCategoryButton').click(function () {
    let id = $('#CategoryIdDelete').val();
    $.post('/Categories/DeleteCategory', { identity: id },
        function (response) {
            HandleDeserializedResponse(response);
        });
});

function ModifCategoryButt(id, name, duration) {
    $('#ModCategoryIssues').addClass('d-none');
    $('#ModCategoryIssuesOpt').addClass('d-none');
    $('#ModCategoryIssuesTime').addClass('d-none');
    $('#ModCategoryInput').val(name);
    $('#ModCategoryId').val(id);
    $('#NewDuration2').val(duration);
    $('#ModCategoryOriginalName').text(name);
    $.get('/Categories/GetCategoryOptions', { identity: id },
        function (response) {
            //let deserilized = JSON.parse(response);
            if (response.length > 0) {
                for (var i = 0; i < response.length; i++) {
                    let chId = 'OptionMID_' + response[i].replace(' ', '_');
                    $('#' + chId).prop('checked', true);
                }
                $('#ModCategoryModal').modal('show');
            }
        });
};

$('#ConfirmCategoryMod').click(function () {
    let newName = $('#ModCategoryInput').val();
    let id = $('#ModCategoryId').val();
    let amount = CheckIfOneSelected('OptionMID_');
    let ok1 = false;
    let ok2 = false;
    let ok3 = false;
    if (newName.length < 5 || !(/^[a-zA-Z0-9- ]*$/.test(newName))) {
        $('#ModCategoryIssues').removeClass('d-none');
    }
    else {
        $('#ModCategoryIssues').addClass('d-none');
        ok1 = true;
    }
    if (!amount) {
        $('#ModCategoryIssuesOpt').removeClass('d-none');
    }
    else {
        $('#ModCategoryIssuesOpt').addClass('d-none');
        ok2 = true;
    }
    ok3 = VerifyTime(2, $('#NewDuration2').val())
    if (ok3 || $('#NewDuration2').val() == '') {
        $('#ModCategoryIssuesTime').addClass('d-none');
        ok3 = true;
    }
    else {
        $('#ModCategoryIssuesTime').removeClass('d-none');
        ok3 = false;
    }
    if (ok1 && ok2 && ok3) {
        $('#ModCategoryModal').modal('hide');
        SendModCategoryRequest(id, newName, 'OptionMID_');
    }
})

function SendModCategoryRequest(id, name, prefix) {
    let objects = $('[id^=' + prefix + ']');
    let options = [];
    for (var i = 0; i < objects.length; i++) {
        if (objects[i].checked) {
            options.push(objects[i].name);
        }
    }
    $.post('/Categories/ModifyCategory', {
        identity: id, newName: name,
        options: options, minimalDuration: $('#NewDuration2').val()
    },
        function (response) {
            HandleDeserializedResponse(response);
        });
}
