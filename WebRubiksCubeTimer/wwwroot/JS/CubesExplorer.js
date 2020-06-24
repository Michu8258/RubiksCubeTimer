$(document).ready(function () {
    GetData(1);
})

function ResetFilters() {
    $('#CategoryFilter').val('');
    $('#ManufacturerFilter').val('');
    $('#ModelNameFilter').val('');
}

function EraseCubesTableContent() {
    $('#CubesTable').empty();
}

function GetData(newPage) {
    let catFilter = $('#CategoryFilter').val();
    let manuFilter = $('#ManufacturerFilter').val();
    let modelFilter = $('#ModelNameFilter').val();
    $.get('/Home/GetCubesData/',
        {
            category: catFilter, manufacturer: manuFilter,
            modelName: modelFilter, page: newPage
        },
        function (response) {
            ResponseCheck(response, HandleCubesResponse)
        }
    )
}

function HandleCubesResponse(jsonData) {
    amountOfPages = jsonData.pagination.amountOfPages;
    currentPage = jsonData.pagination.currentPage;
    pageSize = jsonData.pagination.pageSize;
    pageRecordsAmount = jsonData.cubes.length;
    PopulateCubesTable(jsonData.cubes);
}

function PopulateCubesTable(data) {
    EraseCubesTableContent();
    if (data.length < 1) {
        AddNoResultsRow();
    }
    else {
        for (var i = 0; i < data.length; i++) {
            AddSingleCubeRecord(data[i], i + 1);
        }
    }
    CreatePagination();
}

function AddNoResultsRow() {
    $('#CubesTable').append(
        '<tr>' +
        '<td class="align-middle text-center" colspan="8">No data to display</td>' +
        '</tr>'
    );
}

function AddSingleCubeRecord(data, index) {
    $('#CubesTable').append(
        '<tr>' +
        '<td class="align-middle">' + GetRecordNumber(index) + '</td>' +
        '<td class="align-middle">' + data.category.name + '</td>' +
        '<td class="align-middle">' + data.manufacturer.name + '</td>' +
        '<td class="align-middle">' + data.modelName + '</td>' +
        '<td class="align-middle">' + data.plasticColor.name + '</td>' +
        '<td class="align-middle">' + data.rating + '</td>' +
        '<td class="align-middle">' + data.releaseYear + '</td>' +
        '<td class="align-middle">' + BoolToYN(data.wcaPermission) + '</td>' +
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

$('#OpenFiltersButton').click(function () {
    $('#CubesFilterModal').modal('show');
})

$('#UpdateFiltersButton').click(function () {
    GetData(1);
    $('#CubesFilterModal').modal('hide');
})

$('#ResetFiltersButton').click(function () {
    ResetFilters();
    GetData(1);
    $('#CubesFilterModal').modal('hide');
})

function ChangePage(newPage) {
    GetData(newPage);
}