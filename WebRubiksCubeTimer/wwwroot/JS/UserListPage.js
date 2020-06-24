$(document).ready(function () {
    GetUsersData(1);
});

function GetUserRolesData(userId, userName) {
    ReadUserRoles(userId, userName);
}

function ReadUserRoles(userId, userName) {
    $.ajax({
        url: '/RolesAdmin/GetUserRolesUserInfo',
        type: 'GET',
        data: 'id=' + userId,
        error: function () {
            alert("Error while trying to read " + userName + " role aissignmets.");
        },
        success: function (jsonData) {
            OpenRoleAssignmentModal(jsonData, userName);
        }
    });
}

function OpenRoleAssignmentModal(jsonData, userName) {
    if (jsonData.validUser) {
        $('#RolesModalHeader').html('Change roles assignments of user ' + userName);
        $('#RolesModalBody').html('');
        for (var i = 0; i < jsonData.roles.length; i++) {
            let roleOption = $('<span>');
            roleOption.append('<p class="paragraph-nomargin text-dark">' + jsonData.roles[i].roleName + '</p>');
            if (jsonData.roles[i].userBelongs) {
                roleOption.append('<input type="checkbox" id="RoleID_' + jsonData.roles[i].roleName + '" checked>');
            }
            else {
                roleOption.append('<input type="checkbox" id="RoleID_' + jsonData.roles[i].roleName + '">');
            }
            roleOption.append('</span>');
            $('#RolesModalBody').append(roleOption);
        }
        $('#RolesModalBody').append('<input type="hidden" id="RoleUserID" value="' + jsonData.userId + '"/>')
        $('#ModifyRolesModal').modal('show');
    };
}

function SaveRoleAssignmentsChanges() {
    let rolesData = CollectRolesData();
    $('#ModifyRolesModal').modal('hide');
    UpdateUserRoles(rolesData);
}

function CollectRolesData() {
    let userID = document.getElementById('RoleUserID').value;
    let roles = $('[id^=RoleID_]');
    let rolesCollection = [];
    for (var i = 0; i < roles.length; i++) {
        let role = roles[i].id.substring(7);
        let assigned = roles[i].checked;
        let roleData = {};
        roleData.roleName = role;
        roleData.userBelongs = assigned;
        rolesCollection.push(roleData);
    }
    let rolesModificationData = {};
    rolesModificationData.userID = userID;
    rolesModificationData.roles = rolesCollection;
    return rolesModificationData;
}

function UpdateUserRoles(jsonData) {
    $.ajax({
        url: '/RolesAdmin/UpdateUserRoles',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(jsonData),
        error: function () {
            alert("Error while trying to update user roles.");
        },
        complete: function (jsonData) {
            DisplayMessages(jsonData);
        }
    })
}

function EditUser(id) {
    document.location = '/UserAdmin/EditUser?id=' + id + '&returnUrl=' + window.location.pathname;
}

function DeleteUser(id) {
    document.location = '/UserAdmin/ConfirmUserDeletion?id=' + id + '&returnUrl=' + window.location.pathname;
}

function PinUser(id, userName) {
    $.post('/UserAdmin/PinUser/', { id: id, userName: userName },
        function (response) {
            let data = {};
            data.responseText = JSON.stringify(response);
            DisplayMessages(data);
            if (response.isFaulted == false) {
                AssignPinnedUser();
            }
    });
}

$('#FilterButton').click(function () {
    $('#FilteringModal').modal('show');
})

$('#UpdateFiltersButton').click(function () {
    $('#FilteringModal').modal('hide');
    GetUsersData(1);
})

$('#ResetFiltersButton').click(function () {
    $('#FilterUserName').val('');
    $('#FilterEmail').val('');
    $('#FilterPhoneNumber').val('');
    $('input[name=RolesRadio]').prop('checked', false);
    $('#FilteringModal').modal('hide');
    GetUsersData(1);
})

function ChangePage(newPage) {
    GetUsersData(newPage);
}

function GetUsersData(pageNumber) {
    let userName = $('#FilterUserName').val();
    let email = $('#FilterEmail').val();
    let phone = $('#FilterPhoneNumber').val();
    let roleValue = $('input[name=RolesRadio]:checked').val();
    $.get('/UserAdmin/GetUsersData',
        {
            username: userName, email: email, phoneNumber: phone,
            role: roleValue, page: pageNumber
        },
        function (response) {
            ResponseCheck(response, HandleUsersData);
        }
    )
}

function HandleUsersData(jsonData) {
    amountOfPages = jsonData.pagination.amountOfPages;
    currentPage = jsonData.pagination.currentPage;
    pageSize = jsonData.pagination.pageSize;
    pageRecordsAmount = jsonData.users.length;
    PopulateUsersTable(jsonData.users);
}

function PopulateUsersTable(usersData) {
    $('#UsersCollection').empty();
    if (usersData.length < 1) {
        AddNoResultsRow();
    }
    else {
        for (var i = 0; i < usersData.length; i++) {
            AddSingleUserRecordToTable(usersData[i], i + 1);
        }
    }
    CreatePagination();
}

function AddNoResultsRow() {
    $('#UsersCollection').append(
        '<tr>' +
        '<td class="align-middle text-center" colspan="10">No data to display</td>' +
        '</tr>'
    );
}

function AddSingleUserRecordToTable(userData, index) {
    $('#UsersCollection').append(
        '<tr>' +
            '<td class="align-middle">' + GetRecordNumber(index) + '</td>' +
            '<td class="align-middle"><button type="button" class="btn badge badge-primary" data-toggle="tooltip" title="User management options" onclick="PinUser(\'' +
            userData.id + '\', \'' + userData.userName + '\')">' + userData.id + '</button></td> ' +
            '<td class="align-middle">' + GetLastLoginStamp(userData.lastLoginTimeSpan) + '</td>' +
            '<td class="align-middle">' + userData.userName + '</td>' +
            '<td class="align-middle">' + userData.email + '</td>' +
            '<td class="align-middle">' + PhoneNumber(userData.phoneNumber) + '</td>' +
            '<td class="align-middle">' +
            '<a class="btn btn-sm btn-danger text-white text-nowrap" onclick="DeleteUser(\'' + userData.id + '\')">Delete</a>' +
            '</td>' +
            '<td class="align-middle">' +
            '<a class="btn btn-sm btn-secondary text-white text-nowrap" onclick="EditUser(\'' + userData.id + '\')">Edit</a>' +
            '</td>' +
            '<td class="align-middle">' +
            '<a class="btn btn-sm btn-secondary text-white text-nowrap" onclick="GetUserRolesData(\'' + userData.id + '\', \'' + userData.userName + '\')">Assign Roles</a>' +
            '</td>' +
            '<td class="align-middle">' +
            '<a class="btn btn-sm btn-secondary text-white text-nowrap" onclick="OpenEmailModal(\'' + userData.id + '\', \'' + userData.userName + '\', \'' + userData.email + '\')">Send email</a>' +
            '</td>' +
        '</tr>'
    );
}

function GetLastLoginStamp(stamp) {
    if (stamp.substring(0, 4) < 2000) {
        return 'Before year of 2000';
    }
    else {
        return stamp.substring(0, 19).replace('T', ' ')
    }
}

function PhoneNumber(number) {
    if (number == null) {
        return '---------';
    }
    return number;
}

function GetRecordNumber(index) {
    return ((currentPage - 1) * pageSize + index);
}