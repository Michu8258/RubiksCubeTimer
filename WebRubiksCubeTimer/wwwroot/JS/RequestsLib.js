let maxTopicLength;
let maxMessageLength;
let maxResponseLength;
let idOfRequestForResponse;
let idOfRequestForStateChange;
let showResponsesFor;

$(document).ready(function () {
    maxTopicLength = $('#TopicMaxLength').val();
    maxMessageLength = $('#MessageMaxLength').val();

    $('#TopicAmount').text(LimitCharAmount($('#Topic'), maxTopicLength));
    $('#MessageAmount').text(LimitCharAmount($('#Message'), maxMessageLength));
})

$('#Topic').on('input', function () {
    $('#TopicAmount').text(LimitCharAmount($('#Topic'), maxTopicLength));
})

$('#Message').on('input', function () {
    $('#MessageAmount').text(LimitCharAmount($('#Message'), maxMessageLength));
})

function LimitCharAmount(control, maxLength) {
    let text = control.val();
    if (text != undefined) {
        if (text.length > maxLength) {
            control.val(control.val().substring(0, maxLength));
        }
        return control.val().length + '/' + maxLength;
    }
    return '0/0';
}

function EraseRequests() {
    $('#Requests').empty();
}

function PopulateRequests(data) {
    amountOfPages = data.pagination.amountOfPages;
    currentPage = data.pagination.currentPage;
    pageSize = data.pagination.pageSize;
    pageRecordsAmount = data.requests.length;
    maxResponseLength = data.maxResponseLength;
    EraseRequests();
    if (data.requests.length > 0) {
        for (var i = 0; i < data.requests.length; i++) {
            AddRequest(data.requests[i]);
        }
    }
    else {
        CreateNoContentDiv();
    }
    CreatePagination();
}

function CreateNoContentDiv(){
    $('#Requests').append('<div class="bg-warning rounded px-3 py-2 my-2 text-center">No requests to display</div>');
}

function CreateRequestHeading(request) {
    return '<div class="row row-cols-2">' +
        '<div class="col-9">' +
        '<h6><strong>' + request.request.userName + '</strong></h6>' +
        '<label>' + request.request.creationTime.substring(0, 19).replace('T', ' ') + '</label>' +
        '</div>' +
        '<div class="col-3">' +
        '<button type="button" class="btn btn-sm btn-dark text-white d-flex float-right"' +
        'onclick="ShowHideResponses(\'' + request.request.identity + '\')" >Replies</button > ' +
        '</div>' +
        '</div>';
}

function CreateRequest(request) {
    return '<div class="bg-message rounded px-3 py-2 my-1">' +
        '<h6>' + request.request.topic + '</h6>' +
        '<div>' + request.request.message + '</div>' +
        '</div>';
}

function CreateResponses(request) {
    var responses = '';
    if (request.responses.length > 0) {
        for (var i = 0; i < request.responses.length; i++) {
            responses += CreateSingleResponse(request.responses[i]);
        }
    }
    if (request.request.identity == showResponsesFor) {
        showResponsesFor = 0;
        return '<div id="Responses' + request.request.identity + '">' +
            responses +
            '</div>';
    }
    else {
        return '<div id="Responses' + request.request.identity + '" style="display: none;">' +
            responses +
            '</div>';
    }
}

function CreateSingleResponse(response) {
    return '<div class="bg-response rounded px-3 py-2 my-2">' +
        '<h6>' + response.userName + ' on ' + response.responseTime.substring(0, 19).replace('T', ' ') + '</h6>' +
        '<div>' + response.message + '</div>' +
        '</div>';
}

function GetStatus(boolean) {
    if (boolean == false) {
        return 'Opened';
    }

    return 'Closed';
}

function GetLastResponseCreator(admin, user) {
    if (admin == true) {
        return 'Admin/Modarator';
    }
    if (user == true) {
        return 'Requester';
    }
    return 'Unknown';
}

function ShowHideResponses(requestID) {
    $('#Responses' + requestID).toggle();
}

function ChangePage(newPage) {
    GetContent(newPage);
}

$('#StartTimeStamp').datetimepicker({
    format: 'L'
});

$('#EndTimeStamp').datetimepicker({
    format: 'L'
});

$('#FilterButton').click(function () {
    GetContent(1);
})

function AddNewResponse(requestID) {
    idOfRequestForResponse = requestID;
    $('#NewResponseText').val('');
    $('#MessageCharAmount').text(LimitCharAmount($('#NewResponseText'), maxResponseLength));
    $('#AddResponseModal').modal('show');
}

$('#NewResponseText').on('input', function () {
    $('#MessageCharAmount').text(LimitCharAmount($('#NewResponseText'), maxResponseLength));
})