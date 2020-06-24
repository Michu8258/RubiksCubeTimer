function Autohide(groupName) {
    var accordions = $('[id^="Options"]');
    for (var i = 0; i < accordions.length; i++) {
        let fullId = accordions[i].id;
        if (fullId != 'Options' + groupName) {
            document.getElementById(fullId).classList.remove('show');
        }
    }
    Arrows(groupName);
}

function Arrows(groupName) {
    var arrows = $('[id^="Arrow"]');
    for (var i = 0; i < arrows.length; i++) {
        if (arrows[i].id == 'Arrow' + groupName) {
            if (arrows[i].classList.contains('fa-angle-down')) {
                arrows[i].classList.remove('fa-angle-down');
                arrows[i].classList.add('fa-angle-up');
            }
            else {
                arrows[i].classList.remove('fa-angle-up');
                arrows[i].classList.add('fa-angle-down');
            }
        }
        else {
            arrows[i].classList.remove('fa-angle-up');
            arrows[i].classList.add('fa-angle-down');
        }
    }
}

function RefreshRequestsSidebar() {
    $.get('/Requests/UpdateRequestsAmount',
        function (response) {
            EraseRequestSpans();
            CreateNewRequestSpans(response);
        })
}

function EraseRequestSpans() {
    $('#AmountOfRequestsUserSpan').empty();
    if ($('#AmountOfRequestsAdminSpan').length != 0) {
        $('#AmountOfRequestsAdminSpan').empty();
    }
}

function CreateNewRequestSpans(data) {
    if (data.amountOfNewStatesUser > 0) {
        $('#AmountOfRequestsUserSpan').append('<i class="badge badge-danger" id="AmountOfRequestsUser">' + data.amountOfNewStatesUser + '</i>');
    }
    if ($('#AmountOfRequestsAdminSpan').length != 0) {
        if (data.amountOfNewStatesAdmin > 0) {
            $('#AmountOfRequestsAdminSpan').append('<i class="badge badge-danger" id="AmountOfRequestsAdmin">' + data.amountOfNewStatesAdmin + '</i>');
        }
    }
}

$('#Pinned_UserToolsButton').click(function () {
    OpenPinnedUserModal();
})

$('#UnpinUserButton').click(function () {
    UnpinUser();
    $('#UserManagementModal').modal('hide');
})

function OpenPinnedUserModal() {
    $('#UserManagementModal').modal('show');
}

function AssignPinnedUser() {
    if (!$('#ToolsHeader').length) {
        CreateTools();
    }
    else if ($('#ToolsHeader').length && !$('#Pinned_UserToolsButton').length) {
        AppendPinnedUserButton();
    }
}

function UnpinUser() {
    $.post('/UserAdmin/UnpinUser/', {},
        function (response) {
            if (response.isFaulted == false) {
                DeletePinnedUser();
            }
        })
}

function DeletePinnedUser() {
    if (CountTools() < 2) {
        ClearTools();
    }
    else {
        DeletePinnedUserTool();
    }
}

function CreateTools() {
    ClearTools();
    $('#ToolsContainer').append(CreateHeader());
    $('#ToolsContainer').append(CreatePinnedUserButton());
}

function ClearTools() {
    $('#ToolsContainer').empty();
}

function AppendPinnedUserButton() {
    $('#ToolsContainer').append(CreatePinnedUserButton());
}

function CreateHeader() {
    return '<h5 class="mt-3 mb-1" id="ToolsHeader">Tools</h5>'
}

function CreatePinnedUserButton() {
    return '<button class="btn btn-sm bg-light d-flex flex-column my-1 py-1 text-left" ' +
        ' id="Pinned_UserToolsButton" onclick="OpenPinnedUserModal()">Pinned User</button>'
}

function DeletePinnedUserTool() {
    $('#Pinned_UserToolsButton').remove();
}

function CountTools() {
    return $("#ToolsContainer > button").length;
}