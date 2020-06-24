let unit = 's';
let chartData = {
    labels: [],
    datasets: [{
        label: 'Solve time [s]',
        backgroundColor: 'rgba(54, 162, 235, 0.4)',
        borderColor: 'rgba(54, 162, 235, 1)',
        borderWidth: 1,
        data: []
    }]
}

let chartColors = [
    'rgb(255, 99, 132)',
    'rgb(255, 159, 64)',
    'rgb(255, 205, 86)',
    'rgb(75, 192, 192)',
    'rgb(54, 162, 235)',
    'rgb(153, 102, 255)',
    'rgb(201, 203, 207)',
    'rgb(86, 101, 115)',
    'rgb(39, 174, 96)',
    'rgb(204, 255, 153)',
];

function InitializeCartesianChart(chartObject, type, tooltips) {
    window.timesChart = new Chart(chartObject, {
        type: type,
        data: chartData,
        options: {
            legend: {
                display: false,
            },
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true,
                        callback: function (value, index, values) {
                            if (unit == 's') {
                                return (value).toFixed(1) + ' [' + unit + ']';
                            }
                            if (unit == 'min') {
                                let placeholder = value;
                                return (placeholder / 60).toFixed(1) + ' [' + unit + ']';
                            }
                        }
                    }
                }]
            },
            tooltips: tooltips,
        }
    });
}

function TakeCareOfTimeUnit() {
    var maxVal = Math.max.apply(Math, chartData.datasets[0].data);
    if (unit == 's' && maxVal > 120) {
        unit = 'min';
    }
    if (unit == 'min' && maxVal < 2) {
        unit = 's';
    }
    window.timesChart.update();
}

function GetTimefromMiliseconds(s) {
    s = Math.round(s);
    function pad(n, z, y) {
        let l = n.toString().length;
        if (l < z) {
            let sf = '';
            for (var i = 0; i < z-l; i++) {
                sf += '0';
            }
            return sf + n;
        }
        return n.toString().substring(0, y);
    }

    var ms = s % 1000;
    s = (s - ms) / 1000;
    var secs = s % 60;
    s = (s - secs) / 60;
    var mins = s % 60;
    var hrs = (s - mins) / 60;
    return pad(hrs, 2, 10) + ':' + pad(mins, 2, 2) + ':' + pad(secs, 2, 2) + '.' + pad(ms, 3, 3);
}

function RemoveChartData() {
    chartData.labels = [];
    chartData.datasets[0].data = [];
    TakeCareOfTimeUnit();
}

function GetPieChartConfig(chartData, tooltip) {
    return {
        type: 'pie',
        data: chartData,
        options: {
            responsive: true,
            tooltips: tooltip,
            legend: {
                display: false,
            },
        }
    }
}

function GetColorIndex(dataset) {
    let dataAmount = dataset.length;
    let colorAmount = chartColors.length;
    return dataAmount % colorAmount;
}