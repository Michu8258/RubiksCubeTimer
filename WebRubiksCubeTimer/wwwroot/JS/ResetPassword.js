$('#SendButton').click(function () {
    let emailAddress = document.getElementById("EmailAddress").value;
    CreatePasswordResetRequest(emailAddress, VerifyRequest);
});

function CreatePasswordResetRequest(emailAddress, callback) {
    $.ajax({
        url: '/Login/AddPasswordResetRequest',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(emailAddress),
        complete: function (jsonData) {
            callback(jsonData, emailAddress);
        }
    });
};

function VerifyRequest(jsonResult, emailAddress) {
    let parsedData = JSON.parse(jsonResult.responseText);
    if (parsedData.isFaulted) {
        $('#ResetPasswordHeader').html('Send request failed');
        RemplaceModalMessages(parsedData.messages);
        document.getElementById("PasswordResetConfirmButton").classList.add('d-none');
        $('#ResetPasswordModal').modal('show');
    }
    else {
        SendPasswordResetMessage(emailAddress, DisplayRequestResultModal);
    }
}

function SendPasswordResetMessage(emailAddress, callback) {
    $.ajax({
        url: '/EmailAdmin/SendPasswordResetEmail',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(emailAddress),
        complete: function (jsonData) {
            callback(jsonData);
        }
    });
};

function RemplaceModalMessages(messages) {
    $('#ResetPasswordBody').html('');
    for (var i = 0; i < messages.length; i++) {
        var para = document.createElement("P");
        var t = document.createTextNode(messages[i]);
        para.appendChild(t);
        document.getElementById("ResetPasswordBody").appendChild(para);
    }
}

function DisplayRequestResultModal(jsonData) {
    let parsedData = JSON.parse(jsonResult.responseText);
    if (parsedData.isFaulted) {
        $('#ResetPasswordHeader').html('Send request failed');
        document.getElementById("PasswordResetConfirmButton").classList.add('d-none');
    }
    else {
        $('#ResetPasswordHeader').html('Go to next step');
        document.getElementById("PasswordResetConfirmButton").classList.remove('d-none');
    }
    RemplaceModalMessages(parsedData.messages);
    $('#ResetPasswordModal').modal('show');
}

$('#PasswordResetConfirmButton').click(function () {
    let button = document.getElementById("SendButton");
    if (!button.classList.contains('d-none')) {
        document.location = '/Login/VerifyCode?email=' + document.getElementById("EmailAddress").value;
    }
});

$('#AlreadyButton').click(function () {
    document.location = '/Login/VerifyCode?email=' + document.getElementById("EmailAddress").value;
})