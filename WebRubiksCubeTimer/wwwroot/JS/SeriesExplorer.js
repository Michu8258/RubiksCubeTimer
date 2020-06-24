let currentChartData = [];
let currentSerie = {};

$(document).ready(function () {
    GetSeriesData(1);
    let tooltips = {
        enabled: true,
        displayColors: false,
        titleFontSize: 14,
        bodyFontSize: 14,
        titleAlign: 'center',
        callbacks: {
            label: function (tooltipItem, data) {
                var solve = currentChartData[tooltipItem.index];
                let label = [];
                label.push('Duration: ' + GetTimefromMiliseconds(solve.duration.totalMilliseconds));
                label.push('Time stamp ' + solve.finishTimeSpan.substring(0, 19).replace('T', ' '));
                label.push('Was DNF: ' + BoolToYN(solve.dnf));
                label.push('Penalty (2s): ' + BoolToYN(solve.penaltyTwoSeconds));
                label.push('Category: ' + currentSerie.cube.category.name);
                label.push('Cube: ' + currentSerie.cube.manufacturer.name + ' ' + currentSerie.cube.modelName + ' ' + currentSerie.cube.plasticColor.name);
                label.push('Category option: ' + currentSerie.serieOption.name);
                var scrambleParts = GetScrambleFormatted(solve.scramble);
                if (scrambleParts.length == 1) {
                    label.push('Scramble: ' + scrambleParts[0]);
                }
                if (scrambleParts.length > 1) {
                    label.push('Scramble:');
                    for (var i = 0; i < scrambleParts.length; i++) {
                        label.push(scrambleParts[i]);
                    }
                }
                return label;
            }
        }
    };
    InitializeCartesianChart(document.getElementById('SolvesChart').getContext('2d'), 'bar', tooltips)
})

function GetScrambleFormatted(scramble) {
    let output = [];
    if (scramble) {
        let splitedMoves = scramble.split(" ");
        let elementNumber = 1;
        let tempScramble = ''
        for (var i = 0; i < splitedMoves.length; i++) {
            tempScramble += splitedMoves[i];
            if (elementNumber == 10) {
                output.push(tempScramble);
                tempScramble = '';
                elementNumber = 0;
            }
            else {
                tempScramble += ' ';
            }
            if (i == splitedMoves.length) {
                output.push(tempScramble);
                break;
            }
            elementNumber++;
        }
        return output;
    }
    else {
        output.push('No scramble');
    }
    return output;
}

function GetSeriesData(newPage) {
    let userId = $('#UserId').val();
    let limit = $('#SeriesAmount').val();
    let startDate = $('#StartDate').val();
    let endDate = $('#EndDate').val();
    let category = $('#CategoryId').val();
    let cube = $('#CubeId').val();
    let option = $('#CategoryOptionId').val();
    $.get('/Series/GetSeriesData',
        {
            id: userId, limit: limit, startDate: startDate, endDate: endDate,
            categoryId: category, cubeId: cube, categoryOptionId: option, page: newPage
        },
        function (response) {
            ResponseCheck(response, HandleSeriesResponse);
        }
    )
}

function HandleSeriesResponse(jsonData) {
    amountOfPages = jsonData.pagination.amountOfPages;
    currentPage = jsonData.pagination.currentPage;
    pageSize = jsonData.pagination.pageSize;
    pageRecordsAmount = jsonData.serieData.length;
    PopulateSeriesTable(jsonData.serieData)
}

function PopulateSeriesTable(serieData) {
    $('#SeriesCollection').empty();
    if (serieData.length < 1) {
        AddNoResultsRow();
    }
    else {
        for (var i = 0; i < serieData.length; i++) {
            AddSingleSerieDataToTable(serieData[i], i+1);
        }
    }
    CreatePagination();
}

function AddNoResultsRow() {
    $('#SeriesCollection').append(
        '<tr>' +
        '<td class="align-middle text-center" colspan="' + $('#Columns').val() + '">No data to display</td>' + 
        '</tr>'
    );
}

function AddSingleSerieDataToTable(d, i) {
    var tableCols = $('#Columns').val();
    $('#SeriesCollection').append(
        '<tr>' +
        '<td class="align-middle"><button class="btn badge badge-primary" title="Download series report" onclick="SerieReport(\'' +
            d.identity + '\')">' + GetRecordNumber(i) + '</button></td > ' +
        '<td class="align-middle">' + d.startTimeStamp.substring(0, 19).replace('T', ' ') + '</td>' +
        '<td class="align-middle">' + d.cube.category.name + '</td>' +
        '<td class="align-middle">' + d.cube.manufacturer.name + ' ' + d.cube.modelName + ' ' + d.cube.plasticColor.name + '</td>' +
        '<td class="align-middle">' + GetTimefromMiliseconds(d.shortestResult.totalMilliseconds) + '</td>' +
        '<td class="align-middle">' + GetTimefromMiliseconds(d.longestResult.totalMilliseconds) + '</td>' +
        '<td class="align-middle">' + GetTimefromMiliseconds(d.averageTime.totalMilliseconds) + '</td>' +
        '<td class="align-middle">' + GetTimefromMiliseconds(d.meanOf3.totalMilliseconds) + '</td>' +
        '<td class="align-middle">' + GetTimefromMiliseconds(d.averageOf5.totalMilliseconds) + '</td>' +
        '<td class="align-middle">' + BoolToYN(d.atLeastOneDNF) + '</td>' +
        '<td class="align-middle">' +
        '<button class="btn badge badge-dark text-white" onclick="ShowDetailsChart(\'' + d.identity + '\')">' + d.solvesAmount + ' solve(s)</button>' +
        '</td>' +
        GetAdminOption(tableCols, d) +
        '</tr>'
    );
}

function GetRecordNumber(index) {
    return ((currentPage - 1) * pageSize + index);
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
    GetSeriesData(newPage);
}

$('#UpdateButton').click(function () {
    GetSeriesData(1);
})

function ShowDetailsChart(serieId) {
    let userId = $('#UserId').val();
    $.get('/Series/GetSerieChartData', { id: userId, serieId: serieId },
        function (response) {
            ResponseCheck(response, HandleSolvesTrendData);
        })
}

function HandleSolvesTrendData(data) {
    currentSerie = data.serie;
    currentChartData = data.solves;
    $('#TrendHeader').text(currentSerie.cube.manufacturer.name + ' ' + currentSerie.cube.modelName + ' ' + currentSerie.cube.plasticColor.name);
    RemoveChartData();
    if (data.solves.length > 0) {
        for (var i = 0; i < data.solves.length; i++) {
            chartData.labels.push(chartData.datasets[0].data.length + 1);
            chartData.datasets[0].data.push(Math.trunc(data.solves[i].duration.totalMilliseconds) / 1000);
        }
        TakeCareOfTimeUnit();
        $('#ChartModal').modal('show');
    }
}

function GetAdminOption(amount, data) {
    if (amount > 11) {
        return InsertAdminButtons(data);
    }
}

function SerieReport(serieId) {
    $.get('/Series/GetSerieReport/', { serieId: serieId },
        function (report) {
            saveData(report, "RubiksCubeOnlineTimer" + serieId + ".txt");
        });
}

function saveData(data, fileName) {
    var a = document.createElement("a");
    document.body.appendChild(a);
    a.style = "display: none";

    var json = JSON.stringify(data),
        blob = new Blob([data], { type: "text/plain;charset=utf-8" }),
        url = window.URL.createObjectURL(blob);
    a.href = url;
    a.download = fileName;
    a.click();
    window.URL.revokeObjectURL(url);
}