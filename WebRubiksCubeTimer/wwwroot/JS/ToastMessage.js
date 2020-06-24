function DisplayMessages(data) {
    let parsedData = JSON.parse(data.responseText);
    document.getElementById("MessageToastBody").innerHTML = '';
    if (parsedData.isFaulted) {
        document.getElementById("MessageToastHeader").classList.add('bg-danger');
        document.getElementById("MessageToastBody").classList.add('bg-danger');
        document.getElementById("MessageToastHeader").innerHTML = 'Error';
    }
    else {
        document.getElementById("MessageToastHeader").classList.add('bg-success');
        document.getElementById("MessageToastBody").classList.add('bg-success');
        document.getElementById("MessageToastHeader").innerHTML = 'Success';
    }
    for (var i = 0; i < parsedData.messages.length; i++) {
        var para = document.createElement("P");
        var t = document.createTextNode(parsedData.messages[i]);
        para.appendChild(t);
        document.getElementById("MessageToastBody").appendChild(para);
    }
    $("#MessageToast").toast('show');
}

function ResponseCheck(response, callback) {
    if (response.isFaulted) {
        let data = {};
        data.responseText = JSON.stringify(response);
        DisplayMessages(data);
    }
    else {
        callback(response.jsonData);
    }
}