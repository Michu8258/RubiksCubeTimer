let categoryChartData = {
    datasets: [{
        data: [
        ],
        backgroundColor: [
        ],
        label: 'Your favourite categories',
    }],
    labels: [
    ]
}

let cubeChartData = {
    datasets: [{
        data: [
        ],
        backgroundColor: [
        ],
        label: 'Your favourite cubes',
    }],
    labels: [
    ]
}

function InitializeCategoryChart(chartObject, tooltip, chartData) {
    window.categoryChart = new Chart(chartObject, GetPieChartConfig(chartData, tooltip));
}

function InitializeCubesChart(chartObject, tooltip, chartData) {
    window.cubeChart = new Chart(chartObject, GetPieChartConfig(chartData, tooltip));
}

$(document).ready(function () {
    let tooltips = {
        enabled: true,
        displayColors: true,
        titleFontSize: 14,
        bodyFontSize: 14,
        titleAlign: 'center',
        bodyAlign: 'center',
        bodySpacing: 5,
        callbacks: {
            label: function (tooltipItem, data) {
                data.labels[tooltipItem.index]
                return ' ' + data.labels[tooltipItem.index] + ': ' + data.datasets[0].data[tooltipItem.index] + ' series';
            }
        }
    };

    InitializeCategoryChart(document.getElementById('CategoryPie').getContext('2d'),
        tooltips, categoryChartData);
    InitializeCubesChart(document.getElementById('CubePie').getContext('2d'),
        tooltips, cubeChartData);
    InitialDataRequest();
})

function InitialDataRequest() {
    let userId = $('#UserID').val();
    $.get('/User/GetChartsData', { id: userId },
        function (response) {
            if (response.isFaulted) {
                let data = {};
                data.responseText = JSON.stringify(response);
                DisplayMessages(data);
            }
            else {
                PopulateCharts(response.jsonData);
            }
        })
}

function AddDataToPieChart(destination, value, label) {
    destination.datasets[0].data.push(value);
    destination.datasets[0].backgroundColor
        .push(chartColors[GetColorIndex(destination.datasets[0].data)]);
    destination.labels.push(label);
}

function PopulateCharts(data) {
    if (data.categoryChartData.length > 0) {
        for (var i = 0; i < data.categoryChartData.length; i++) {
            AddDataToPieChart(categoryChartData, data.categoryChartData[i].amount,
                data.categoryChartData[i].category.name);
        }
    }
    if (data.cubeChartData.length > 0) {
        for (var i = 0; i < data.cubeChartData.length; i++) {
            AddDataToPieChart(cubeChartData, data.cubeChartData[i].amount,
                data.cubeChartData[i].cube.manufacturer.name + ' ' +
                data.cubeChartData[i].cube.modelName + ' ' +
                data.cubeChartData[i].cube.plasticColor.name);
        }
    }
    window.categoryChart.update();
    window.cubeChart.update();
}