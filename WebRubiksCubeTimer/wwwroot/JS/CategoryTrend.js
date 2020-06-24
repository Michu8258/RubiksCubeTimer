let currentChartData = [];

$(document).ready(function () {
    let tooltips = {
        enabled: true,
        displayColors: false,
        titleFontSize: 14,
        bodyFontSize: 14,
        titleAlign: 'center',
        callbacks: {
            label: function (tooltipItem, data) {
                var serie = currentChartData[tooltipItem.index];
                let label = [];
                label.push('Date: ' + serie.startTimeStamp.substring(0, 19).replace('T', ' '));
                label.push('Serie Identity: ' + serie.identity);
                label.push('Category: ' + serie.cube.category.name);
                label.push('Cube: ' + serie.cube.manufacturer.name + ' ' + serie.cube.modelName + ' ' + serie.cube.plasticColor.name);
                label.push('Category option: ' + serie.serieOption.name);
                label.push('Best result: ' + GetTimefromMiliseconds(serie.shortestResult.totalMilliseconds));
                label.push('Worst result: ' + GetTimefromMiliseconds(serie.longestResult.totalMilliseconds));
                label.push('Average time: ' + GetTimefromMiliseconds(serie.averageTime.totalMilliseconds));
                label.push('Mean of 3: ' + GetTimefromMiliseconds(serie.meanOf3.totalMilliseconds));
                label.push('Average of 5: ' + GetTimefromMiliseconds(serie.averageOf5.totalMilliseconds));
                label.push('DNF?: ' + serie.atLeastOneDNF);
                return label;
            }
        }
    }

    InitializeCartesianChart(document.getElementById('Trend').getContext('2d'), 'line', tooltips);
    GetDefaultChartData();
})

function GetDefaultChartData() {
    let userId = $('#UserId').val();
    $.get('/Series/GetTrendDefaultData', { id: userId },
        function (response) {
            ResponseCheck(response, CreateChartContent);
        })
}

$('#UpdateButton').click(function () {
    let userId = $('#UserId').val();
    let limit = $('#SeriesAmount').val();
    let startDate = $('#StartDate').val();
    let endDate = $('#EndDate').val();
    let category = $('#CategoryId').val();
    let cube = $('#CubeId').val();
    let option = $('#CategoryOptionId').val();
    $.get('/Series/GetTrendFilteredData', {
        id: userId, limit: limit, startDate: startDate, endDate: endDate, 
        categoryId: category, cubeId: cube, categoryOptionId: option
    },
        function (response) {
            $('#TrendDescriptionHeader').text(response.jsonData.trendDescription);
            ResponseCheck(response, CreateChartContent);
        }
    )
})

function CreateChartContent(data) {
    currentChartData = data.series;
    RemoveChartData();
    if (data.series.length > 0) {
        for (var i = 0; i < data.series.length; i++) {
            chartData.labels.push(chartData.datasets[0].data.length + 1);
            chartData.datasets[0].data.push(Math.trunc(data.series[i].averageTime.totalMilliseconds) / 1000);
        }
        TakeCareOfTimeUnit();
    }
}