let categoryChartData = {
    datasets: [{
        data: [
        ],
        backgroundColor: [
        ],
        label: 'Most favourite categories',
    }],
    labels: [
    ]
}

function InitializeCategoriesChart(tooltip) {
    window.categoryChart = new Chart(document.getElementById('CategoriesPie').getContext('2d'), {
        type: 'pie',
        data: categoryChartData,
        options: {
            responsive: true,
            tooltips: tooltip,
            legend: {
                display: false,
            },
        }
    })
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

    InitializeCategoriesChart(tooltips);
    SendInitialrequest();
})

function SendInitialrequest() {
    $.get('/Home/GetIndexChartData',
        function (response) {
            if (response.isFaulted) {
                let data = {};
                data.responseText = JSON.stringify(response);
                DisplayMessages(data);
            }
            else {
                PopulateCategoryPie(response.jsonData);
            }
        })
}

function PopulateCategoryPie(data) {
    if (data.categoryChartData.length > 0) {
        for (var i = 0; i < data.categoryChartData.length; i++) {
            AddDataToCategoryPie(data.categoryChartData[i].amount,
                data.categoryChartData[i].category.name);
        }
    }
    window.categoryChart.update();
}

function AddDataToCategoryPie(value, label) {
    categoryChartData.datasets[0].data.push(value);
    categoryChartData.datasets[0].backgroundColor
        .push(chartColors[GetColorIndex(categoryChartData.datasets[0].data)]);
    categoryChartData.labels.push(label);
}