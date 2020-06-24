$(document).ready(function () {
    GetContent(1);
})

function GetContent(page) {
    let startDate = $('#StartDate').val();
    let endDate = $('#EndDate').val();
    let topicFilter = $('#TopicFilter').val();
    $.get('/Requests/GetArchivedRequests', {
        startDate: startDate, endDate: endDate,
        topicContains: topicFilter, page: page
    },
        function (response) {
            if (response.isFaulted) {
                let data = {};
                data.responseText = JSON.stringify(response);
                DisplayMessages(data);
            }
            else {
                SetPaginationData(response.jsonData);
                PopulateRequestsTable(response.jsonData);
                CreatePagination();
            }
        })
}

function SetPaginationData(response) {
    amountOfPages = response.pagination.amountOfPages;
    currentPage = response.pagination.currentPage;
    pageSize = response.pagination.pageSize;
    pageRecordsAmount = response.requests.length;
}

function PopulateRequestsTable(data) {
    EraseTableContent();
    if (data.requests.length > 0) {
        AddRequestsToTable(data.requests);
    }
    else {
        AddNoResultsToTable();
    }
}

function EraseTableContent() {
    $('#Requests').empty();
}

function AddNoResultsToTable() {
    $('#Requests').append(
        '<tr>' +
        '<td class="align-middle text-center" colspan="7">No data to display</td>' +
        '</tr>'
    );
}

function AddRequestsToTable(requests) {
    for (var i = 0; i < requests.length; i++) {
        AddSingleRequestToTable(requests[i], i + 1);
    }
}

function AddSingleRequestToTable(request, index) {
    $('#Requests').append(
        '<tr>' +
        '<td class="align-middle">' + GetRecordNumber(index) + '</td>' +
        '<td class="align-middle">' + request.request.userName + '</td>' +
        '<td class="align-middle">' + request.request.creationTime.substring(0, 19).replace('T', ' ') + '</td>' +
        '<td class="align-middle">' + request.request.topic + '</td>' +
        CreateResponsesAmount(request) +
        '<td class="align-middle">' + BoolToYN(request.request.privateRequest) + '</td>' +
        CreateChangeStateButton(request) +
        '</tr>'
    );
}

function GetRecordNumber(index) {
    return ((currentPage - 1) * pageSize + index);
}

function CreateResponsesAmount(request) {
    return '<td class="align-middle">' +
        '<button type="button" class="btn badge badge-sm badge-primary text-white" onclick="ShowResponses(\'' +
        request.request.identity + '\')">' + request.request.repliesAmount + ' (show conversation)</button>' +
        '</td>';
}

function CreateChangeStateButton(request) {
    return '<td class="align-middle">' +
        '<button type="button" class="btn badge badge-sm badge-warning text-white" onclick="ChangeRequestState(\'' +
        request.request.identity + '\', \'' + request.request.caseClosed + '\')">Change state</button>' +
        '</td>';
}

function BoolToYN(value) {
    if (value == true) {
        return 'Yes';
    }
    else {
        return 'No';
    }
}

function ChangePage(newPage) {
    GetContent(newPage);
}

function ShowResponses(requestId) {
    $.get('/Requests/GetResponses', { requestId: requestId },
        function (response) {
            if (response.isFaulted) {
                let data = {};
                data.responseText = JSON.stringify(response);
                DisplayMessages(data);
            }
            else {
                ResponsesModal(response.jsonData);
            }
        })
}

function ResponsesModal(data) {
    let a = data;
    $('#ConvTimeSpan').text(data.request.creationTime.substring(0, 19).replace('T', ' '));
    $('#ConvUserName').text(data.request.userName);
    $('#ConvMessage').text(data.request.message);
    EraseModalRepliesContent();
    PopulateRepliesModalWithReplies(data);
    $('#ConversationModal').modal('show');
}

function EraseModalRepliesContent() {
    $('#ModalResponses').empty();
}

function PopulateRepliesModalWithReplies(data) {
    if (data.responses.length > 0) {
        $('#ModalResponses').append('<label class="mt-2">Replies: </label>');
        for (var i = 0; i < data.responses.length; i++) {
            AddSingleResponseToModal(data.responses[i]);
        }
    }
}

function AddSingleResponseToModal(response) {
    $('#ModalResponses').append(
        '<div class="bg-message rounded p-2 my-1">' +
        '<div><span>' + response.userName + '</span> on <span>' +
        response.responseTime.substring(0, 19).replace('T', ' ') +
        '</span> replied: </span>' +
        '<div><span>' + response.message + '</span></div>' +
        '</div>'
    );
}

function ChangeRequestState(requestID, currentState) {
    idOfRequestForStateChange = requestID;
    $('#CurrentRequestState').text(GetStatus(currentState));
    $('input[name="state"]').prop('checked', false);
    $('#ChangeStateModal').modal('show');
}

$('#ConfirmStateChange').click(function () {
    if ($("input[name='state']:checked").val()) {
        let close = $('#NewStateClosed').is(":checked");
        let id = idOfRequestForStateChange;
        idOfRequestForStateChange = 0;
        $('#ChangeStateModal').modal('hide');
        $.post('/Requests/ChangeRequestStatus', { requestId: id, closed: close },
            function (response) {
                if (response.isFaulted) {
                    let data = {};
                    data.responseText = JSON.stringify(response);
                    DisplayMessages(data);
                }
                else {
                    let newPage = currentPage;
                    if (pageRecordsAmount == 1 && currentPage > 1) {
                        newPage--;
                    }
                    GetContent(newPage);
                    RefreshRequestsSidebar();
                }
            });
    }
})