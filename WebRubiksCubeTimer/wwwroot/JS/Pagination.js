let amountOfPages;
let currentPage;
let pageSize;
let pageRecordsAmount;

function InitializaPagination() {
    let currentPageNumber = GetPageNumber();
    let routeData = GetRouteData();
    GetPaginationData(currentPageNumber, routeData);
}

function GetPageNumber() {
    let fullURL = document.URL;
    let pageNumber = fullURL.substring(fullURL.lastIndexOf('/') + 1);
    return pageNumber;
}

function GetRouteData() {
    var routes = window.location.pathname.split("/");
    var controller = routes[1];
    var action = routes[2];
    var identity = '';
    if (routes.length > 3) {
        identity = routes[3];
    }
    return [controller, action, identity];
}

function GetPaginationData(currentPageNumber, routeData) {
    $.ajax({
        url: '/' + routeData[0] + '/' + routeData[1] + 'Pagination',
        type: 'GET',
        data: 'currentPageNumber=' + currentPageNumber,
        success: function (jsonData) {
            GeneratePagesLinks(jsonData, routeData[2]);
        }
    })
}

function GeneratePagesLinks(paginationData, identity) {
    let routeData = GetRouteData();
    if (paginationData.amountOfPages > 1) {
        if (paginationData.amountOfPages >= 10) {
            if (paginationData.currentPage <= 4) {
                for (var i = 0; i < 5; i++) {
                    AddPaginationButton(routeData, i + 1, i + 1 == paginationData.currentPage, identity);
                }
                AddDots();
                AddPaginationButton(routeData, paginationData.amountOfPages, false, identity);
            } else if (paginationData.currentPage >= paginationData.amountOfPages - 3) {

                AddPaginationButton(routeData, 1, false, identity);
                AddDots();
                for (var i = paginationData.amountOfPages - 5; i < paginationData.amountOfPages; i++) {
                    AddPaginationButton(routeData, i + 1, i + 1 == paginationData.currentPage, identity);
                }
            } else {
                AddPaginationButton(routeData, 1, false, identity);
                AddDots();
                for (var i = paginationData.currentPage - 3; i < paginationData.currentPage +2; i++) {
                    AddPaginationButton(routeData, i + 1, i + 1 == paginationData.currentPage, identity);
                }
                AddDots();
                AddPaginationButton(routeData, paginationData.amountOfPages, false, identity);
            }
        }
        else {
            for (var i = 0; i < paginationData.amountOfPages; i++) {
                AddPaginationButton(routeData, i + 1, i + 1 == paginationData.currentPage, identity);
            }
        }
    }
}

function AddPaginationButton(routeData, pageNumber, selected, identity) {
    let aTag = document.createElement('a');
    if (identity == 'Page' || identity == '') {
        aTag.setAttribute('href', '/' + routeData[0] + '/' + routeData[1] + '/Page/' + pageNumber)
    }
    else {
        aTag.setAttribute('href', '/' + routeData[0] + '/' + routeData[1] + '/' + identity + '/Page/' + pageNumber)
    }
    aTag.classList.add('btn', 'm-1');
    if (selected == true) {
        aTag.classList.add('btn-dark');
    }
    else {
        aTag.classList.add('btn-secondary');
    }
    aTag.innerHTML = pageNumber;
    document.getElementById("PaginationButtons").appendChild(aTag);
}

function AddDots() {
    let spanTag = document.createElement('span');
    spanTag.innerHTML = '...';
    spanTag.classList.add('m-1', 'p-2', 'text-dark');
    document.getElementById("PaginationButtons").appendChild(spanTag);
}

function CreatePagination() {
    $('#PaginationButtons').empty();
    if (amountOfPages > 1) {
        GeneratePagesLinksFilter();
    }
}

function GeneratePagesLinksFilter() {
    if (amountOfPages >= 10) {
        if (currentPage <= 4) {
            for (var i = 0; i < 5; i++) {
                HandlePaginationButton(i + 1, i + 1 == currentPage);
            }
            AddDots();
            HandlePaginationButton(amountOfPages, false);
        } else if (currentPage >= amountOfPages - 3) {

            HandlePaginationButton(1, false);
            AddDots();
            for (var i = amountOfPages - 5; i < amountOfPages; i++) {
                HandlePaginationButton(i + 1, i + 1 == currentPage);
            }
        } else {
            HandlePaginationButton(1, false, identity);
            AddDots();
            for (var i = currentPage - 3; i < currentPage + 2; i++) {
                HandlePaginationButton(i + 1, i + 1 == currentPage);
            }
            AddDots();
            HandlePaginationButton(amountOfPages, false);
        }
    }
    else {
        for (var i = 0; i < amountOfPages; i++) {
            HandlePaginationButton(i + 1, i + 1 == currentPage);
        }
    }
}

function HandlePaginationButton(pageNumber, selected) {
    if (selected == true) {
        $('#PaginationButtons').append('<button class="btn btn-dark m-1" onclick="ChangePage(\'' + pageNumber + '\')">' + pageNumber + '</button>');
    }
    else {
        $('#PaginationButtons').append('<button class="btn btn-secondary m-1" onclick="ChangePage(\'' + pageNumber + '\')">' + pageNumber + '</button>');
    }
}