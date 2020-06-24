function OpenEmailModal(userId, userName, email) {
    $('#EmailOptionsHeader').html('Email actions for user ' + userName);
    document.getElementById('EmailSendUserId').setAttribute('value', userId);
    document.getElementById('EmailSendUserEmail').setAttribute('value', email);
    $('#CustomMessageLink').attr('href', '/EmailAdmin/CustomEmail/' + userId);
    $('#SendUserEmail').modal('show');
}

function ResendVerificationCode() {
    let userId = document.getElementById('EmailSendUserId').value;
    $('#SendUserEmail').modal('hide');
    $.ajax({
        url: '/EmailAdmin/ResendEmailVerificationCode',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(userId),
        error: function () {
            alert("Error while trying to send emaile message.");
        },
        complete: function (jsonData) {
            DisplayMessages(jsonData);
        }
    });
};

function SendPasswordResetRequest() {
    let userEmail = document.getElementById('EmailSendUserEmail').value;
    CreatePasswordResetRequest(userEmail, ValidateRequestResponse);
}

function ValidateRequestResponse(jsonData) {
    let parsedData = JSON.parse(jsonData.responseText);
    if (parsedData.isFaulted) {
        $('#SendUserEmail').modal('hide');
        DisplayMessages(jsonData);
    }
    else {
        let userEmail = document.getElementById('EmailSendUserEmail').value;
        SendPasswordResetMessage(userEmail, DisplayResetPasswordResult);
    }
}

function DisplayResetPasswordResult(jsonData) {
    $('#SendUserEmail').modal('hide');
    DisplayMessages(jsonData);
}