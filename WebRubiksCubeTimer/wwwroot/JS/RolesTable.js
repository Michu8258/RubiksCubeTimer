$(document).ready(function () {
    EraseTableContent();
    PopulateRolesTable();
});

function EraseTableContent() {
    let table = document.getElementById("RolesTable");
    let amount = table.rows.length - 1;
    if (amount > 0) {
        for (var i = 0; i < amount; i++) {
            table.deleteRow(1);
        }
    }
}

function PopulateRolesTable() {
    $.getJSON("GetAllRoles",
        function (json) {
            if (json.length == 0) {
                tableRow = $('<tr>');
                tableRow.append('<td colspan="3" class="align-middle text-center text-dark">No roles defined</td>');
                tableRow.append('</tr>');
                $('#RolesTable').append(tableRow);
            }
            else {
                let tableRow;
                for (var i = 0; i < json.length; i++) {
                    if (i % 2 == 1) {
                        tableRow = $('<tr class="bg-light">');
                    }
                    else {
                        tableRow = $('<tr>');
                    }
                    tableRow.append('<td class="align-middle">' + json[i].id + "</td>");
                    tableRow.append('<td class="align-middle">' + json[i].name + "</td>");
                    tableRow.append('<td><a class="align-middle btn btn-sm btn-danger text-white my-1" onclick="ConfirmModal(\'' + json[i].id + '\', \'' + json[i].name +  '\')">Delete</a></td>');
                    tableRow.append('</tr>');
                    $('#RolesTable').append(tableRow);
                }
            }
        });
}

function ConfirmModal(identity, roleName) {
    $('#DeleteModalHeader').html('Please confirm');
    $('#DeleteModalBody').html('');
    let body = document.getElementById("DeleteModalBody")
    let div1 = document.createElement('div');
    div1.innerHTML = 'Are you sure you want to fully delete role "' + roleName + '"?';
    let div2 = document.createElement('div');
    div2.innerHTML = 'This operation cannot be reverted.';
    let hidInput = document.createElement('input');
    hidInput.setAttribute('type', "hidden");
    hidInput.setAttribute('id', "DeleteRoleID");
    hidInput.setAttribute('value', identity);
    body.append(div1);
    body.append(div2);
    body.append(hidInput);
    $('#DeleteRolesModal').modal('show');
}

function RoleDeletionConfirmed() {
    let roleID = document.getElementById('DeleteRoleID').value;
    DeleteRole(roleID, function () {
        $('#DeleteRolesModal').modal('hide');
        EraseTableContent();
        PopulateRolesTable();
    });
}

function DeleteRole(roleId, handleData) {
    $.ajax({
        url: '/RolesAdmin/DeleteRole',
        contentType: 'application/json',
        type: 'POST',
        data: JSON.stringify(roleId),
        error: function () {
            alert("Error while trying to delete role.");
        },
        success: function () {
            handleData();
        }
    });
}