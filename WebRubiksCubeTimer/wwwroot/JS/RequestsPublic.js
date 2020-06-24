$(document).ready(function () {
    GetContent(1);
})

function GetContent(page) {
    let startDate = $('#StartDate').val();
    let endDate = $('#EndDate').val();
    let topicFilter = $('#TopicFilter').val();
    $.get('/Requests/GetPublicRequestsData', {
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
            PopulateRequests(response.jsonData);
        }
    })
}

function AddRequest(request) {
    let container = '<div class="bg-request rounded px-3 py-2 my-2">' +
        CreateRequestHeading(request) +
        CreateRequest(request) +
        CreateResponses(request) +
        CreateRequestFooter(request) +
        '</div>';

    $('#Requests').append(container);
}

function CreateRequestFooter(request) {
    return '<div class="row row-cols-2">' +
        '<div class="col-12">' +
        '<span>Replies amount: <strong>' + request.request.repliesAmount + '</strong></span>' +
        '<span> Status: <strong>' + GetStatus(request.request.caseClosed) + '</strong></span>' +
        '<span> Last update by: <strong>' + GetLastResponseCreator(request.request.newChangesByAdmin, request.request.newChangesByUser) + '</strong></span>' +
        '</div>' +
        '</div>';
}