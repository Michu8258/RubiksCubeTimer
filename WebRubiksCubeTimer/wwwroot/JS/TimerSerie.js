let startTime;
let elapsedTime;
let miliseconds = 0;
let interval;
let result = 0;

window.addEventListener('keydown', function (e) {
    if (e.keyCode == 32 && e.target == document.body) {
        e.preventDefault();
    }
});

$(window).bind('beforeunload', function () {
    $.post('/Timer/MarkSeriesAsFinished', { serieId: $('#SeriesID').val() },
        function (response) {
        })
});

$(document).ready(function () {
    let tooltips = {
        enabled: true,
        displayColors: false,
        titleAlign: 'center'
    };
 
    InitializeCartesianChart(document.getElementById('TimesChart').getContext('2d'), 'bar', tooltips);
    $('#TableSerieStartTime').text(GetDate(new Date(), false, false));
})

function GetDate(date, withMiliseconds, dumbNotation) {
    let day = date.getDate();
    if (day < 10) {
        day = '0' + day;
    }
    //that + 1 is because js enumerates moths from 0 => 0 - january, 11 - december
    let month = date.getMonth() + 1;
    if (month < 10) {
        month = '0' + month;
    }
    let year = date.getFullYear();
    let hour = date.getHours();
    if (hour < 10) {
        hour = '0' + hour;
    }
    let minutes = date.getMinutes();
    if (minutes < 10) {
        minutes = '0' + minutes;
    }
    let seconds = date.getSeconds();
    if (seconds < 10) {
        seconds = '0' + seconds;
    }
    let miliseconds = date.getMilliseconds();
    if (miliseconds < 10) {
        miliseconds = '00' + miliseconds;
    }
    else if (miliseconds < 100) {
        miliseconds = '0' + miliseconds;
    }

    let output = '';
    if (dumbNotation == true) {
        output = month + '-' + day + '-' + year + ' ' + hour + ':' + minutes + ':' + seconds;
    }
    else {
        output = day + '-' + month + '-' + year + ' ' + hour + ':' + minutes + ':' + seconds;
    }

    if (withMiliseconds === true) {
        return output + '.' + miliseconds;
    }

    return output;
}

$('body').keyup(function (e) {
    if (e.keyCode == 32) {
        if (miliseconds >= 0 && !$('#SerieFinished').is(':checked')) {
            LockInterface();
            startTime = Date.now();
            interval = setInterval(function () {
                $("#MeasuredTime").text(GetTimefromMiliseconds(miliseconds));
                miliseconds += 20;
            }, 20);
        }

    }
}).keydown(function (e) {
    if (e.keyCode == 32) {
        if (miliseconds > 0 && !$('#SerieFinished').is(':checked')) {
            clearInterval(interval);
            miliseconds = -10;
            SolveDone(Date.now() - startTime);
        }
    }
})

function SolveDone(elapsedTime) {
    $('#MeasuredTime').text(GetTimefromMiliseconds(elapsedTime));
    result = elapsedTime;
    $('#MarkAsDNF').prop('checked', false);
    $('#AddPenalty').prop('checked', false);
    $('#MarkAsFinished').prop('checked', false);
    $('#SolveDone').removeClass('d-none');
}

$('#DiscardSolveResult').click(function () {
    $('#SolveDone').addClass('d-none');
    $('#MeasuredTime').text(GetTimefromMiliseconds(0));
    miliseconds = 0;
    UnlockInterface();
    Scramble();
})

$('#SendSolveResult').click(function () {
    $('#SolveDone').addClass('d-none');
    $('#MeasuredTime').text(GetTimefromMiliseconds(0));
    miliseconds = 0;
    $.post('/Timer/AddSolve/', {
        userId: $('#UserID').val(), categoryId: $('#CategoryID').val(), cubeId: $('#CubeID').val(),
        optionId: $('#OptionID').val(), seriesDone: $('#MarkAsFinished').is(':checked'),
        miliseconds: result, dnf: $('#MarkAsDNF').is(':checked'), penalty: $('#AddPenalty').is(':checked'),
        seriesId: $('#SeriesID').val(), timeStamp: GetDate(new Date(), true, false), scramble: GetCurrentScramble()
    },
        function (response) {
            if (response.isFaulted) {
                let data = {};
                data.responseText = JSON.stringify(response);
                DisplayMessages(data);
            }
            else {
                HandleAddTimeResponse(response.jsonData);
            }
        });
    UnlockInterface();
    Scramble();
})

function HandleAddTimeResponse(response) {
    $('#SeriesID').val(response.seriesID);
    $('#TableSerieId').text(response.seriesID);
    $('#UserID').val(response.userID);
    $('#TableSerieStartTime').text(response.seriesStartTime);
    if (response.seriesFinished == true) {
        $('#SerieFinished').attr('checked', true);
    }
    if (response.atLeastOneDNF == true) {
        $('#AtLeastOneDNF').attr('checked', true);
    }
    UpdateTimeStatistics(response);
    AddNewTimeToTable(response);
    AddNewTimeToChart(response);
}

function UpdateTimeStatistics(response) {
    $('#BestTime').text(GetTimefromMiliseconds(response.bestTimeMS));
    $('#WorstTime').text(GetTimefromMiliseconds(response.worstTimeMS));
    $('#AverageTime').text(GetTimefromMiliseconds(response.averageTimeMS));
    $('#MeanOf3').text(GetTimefromMiliseconds(response.meanOf3MS));
    $('#AverageOf5').text(GetTimefromMiliseconds(response.averageOf5MS));
}

function AddNewTimeToTable(response) {
    if (response.newTimeNumber == 1) {
        $('#SolvesTableBody').empty();
    }
    AddResultToTimesTable(response);
}

function AddResultToTimesTable(result) {
    $('#SolvesData').append(
        '<tr>' +
        '<td class="align-middle">' + result.newTimeNumber + '</td>' +
        '<td class="align-middle">' + GetTimefromMiliseconds(result.newTimeDurationMS) + '</td>' +
        '<td class="align-middle">' + result.newTimeSpan + '</td>' +
        GetCheckboxString(result.newTimeDNF) +
        GetCheckboxString(result.newTimePenalty) +
        '</tr>'
    );
}

function GetCheckboxString(checked) {
    if (checked == true) {
        return '<td class="align-middle"><input type="checkbox" disabled checked /></td>';
    }
    else {
        return '<td class="align-middle"><input type="checkbox" disabled /></td>';
    }
}

function AddNewTimeToChart(response) {
    chartData.labels.push(chartData.datasets[0].data.length + 1);
    chartData.datasets[0].data.push(response.newTimeDurationMS / 1000);
    TakeCareOfTimeUnit();
}