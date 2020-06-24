let solvesIDs = [];

function InsertAdminButtons(data) {
    return '<td><button class="btn badge badge-danger text-white" onclick="DeleteSerie(\''
        + data.identity + '\', \'' + data.startTimeStamp + '\',  \'' + currentPage + '\')">Delete</button></td>' +
        '<td><button class="btn badge badge-primary text-white" onclick="ModifySerie(\'' + data.identity + '\')">Manage</button></td>';
}

function DeleteSerie(serieId, timestamp, page) {
    currentPage = page;
    $('#SerieToDelete').text('Identity: ' + serieId + ', Date and time: ' + timestamp);
    $('#SerieToDeleteId').val(serieId);
    $('#DeleteSerieModal').modal('show');
}

$('#DeleteSerieButton').click(function () {
    let serieId = $('#SerieToDeleteId').val();
    $('#DeleteSerieModal').modal('hide');
    $.post('/SeriesAdmin/DeleteSerie', { serieId: serieId },
        function (response) {
            ResponseCheck(response, HandleSerieDeleteResponse);
        })
})

function HandleSerieDeleteResponse() {
    let newPage = currentPage;
    if (pageRecordsAmount == 1 && currentPage > 1) {
        newPage--;
    }
    GetSeriesData(newPage);
}

function ModifySerie(serieId) {
    let userId = $('#UserId').val();
    $.get('/Series/GetSerieChartData', { id: userId, serieId: serieId },
        function (response) {
            ResponseCheck(response, HandleSerieModificationData);
        })
}

function HandleSerieModificationData(data) {
    $('#ModifiedSerieId').val(data.serie.identity);
    $('#SerieModifId').text(' (' + data.serie.identity + ')');
    PopulateSolvesTable(data.solves);
    $('#ModifyTimesModal').modal('show');
}

function PopulateSolvesTable(solves) {
    $('#SerieModifBody').empty();
    solvesIDs = [];
    if (solves.length < 1) {
        AddNoSolvesRow();
    }
    else {
        for (var i = 0; i < solves.length; i++) {
            AddSingleSolveToTable(solves[i], i + 1)
        }
    }
}

function AddNoSolvesRow() {
    $('#SerieModifBody').append(
        '<tr>' +
        '<td class="align-middle text-center" colspan="11">No data to display</td>' +
        '</tr>'
    );
}

function AddSingleSolveToTable(d, rowNumber) {
    solvesIDs.push(d.identity);
    $('#SerieModifBody').append(
        '<tr>' +
        '<td class="align-middle">' + rowNumber + '</td>' +
        '<td class="align-middle">' + d.identity + '</td>' +
        '<td class="align-middle">' + d.finishTimeSpan.substring(0, 19).replace('T', ' ') + '</td>' +
        '<td class="align-middle">' + GetTimefromMiliseconds(d.duration.totalMilliseconds) + '</td>' +
        '<td class="align-middle"><input id="NewDuration' + d.identity + '" placeholder="00:00:00.000" oninput="VerifyTime(' + d.identity + ', this.value)" type="text"/></td>' +
        '<td class="align-middle">' + BoolToYN(d.dnf) + '</td>' +
        '<td class="align-middle"><input id="NewDNF' + d.identity + '" type="checkbox"/></td>' +
        '<td class="align-middle">' + BoolToYN(d.penaltyTwoSeconds) + '</td>' +
        '<td class="align-middle"><input id="NewPenalty' + d.identity + '" type="checkbox"/></td>' +
        '<td class="align-middle"><input id="NewModify' + d.identity + '" type="checkbox"/></td>' +
        '<td class="align-middle"><input id="NewDelete' + d.identity + '" type="checkbox"/></td>' +
        '</tr>'
    );
}

function CollectChangedData() {
    if (solvesIDs.length > 0) {
        let serieId = $('#ModifiedSerieId').val();
        let userId = $('#UserId').val();
        let collectedData = [];
        for (var i = 0; i < solvesIDs.length; i++) {
            let item = {};
            item.identity = solvesIDs[i];
            item.serieId = serieId;
            item.userId = userId;
            item.newDuration = $('#NewDuration' + solvesIDs[i]).val();
            item.newDnf = $('#NewDNF' + solvesIDs[i]).prop('checked');
            item.newPenalty = $('#NewPenalty' + solvesIDs[i]).prop('checked');
            item.newModify = $('#NewModify' + solvesIDs[i]).prop('checked');
            item.newDelete = $('#NewDelete' + solvesIDs[i]).prop('checked');
            collectedData.push(item);
        }
        return collectedData;
    }
    return null;
}

$('#SaveModifChanges').click(function () {
    let data = CollectChangedData();
    $.post('/SeriesAdmin/ModifySerie', { model: data },
        function (response) {
            ResponseCheck(response, HandleModificationResponse);
        })
})

function HandleModificationResponse(data) {
    ModifySerie($('#ModifiedSerieId').val())
}

$('#CloseModifModal').click(function () {
    $('#ModifyTimesModal').modal('hide');
    GetSeriesData(currentPage);
})