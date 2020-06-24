$('#AddScrambleButton').click(function () {
    AddScramble($('#CategoryId').val());
})

$('#ChangeCategoryButton').click(function () {
    $('#AllCategoriesModal').modal('show');
})

$('#CategoriesWithNoScrambleButton').click(function () {
    $('#CategoriesNoScrambleModal').modal('show');
})

function AddScramble(categoryId) {
    document.location = '/Categories/AddScramble/' + categoryId;
}

function ChangeCategory(categoryId) {
    document.location = '/Categories/Scramble/' + categoryId;
}

function ModifyDefinition(scrambleId) {
    document.location = '/Categories/ModifyScramble/' + scrambleId;
}

function SetAsDefault(categoryId, scrambleId) {
    $.post('/Categories/SetScrambleAsDefault', { categoryId: categoryId, scrambleId: scrambleId },
        function (response) {
            ResponseCheck(response, HandleSetDefaultData);
        })
}

function HandleSetDefaultData(ok) {
    if (ok === true) {
        document.location.reload();
    }
}

function ConfirmScrambleDeletion(scrambleId, scrambleName, categoryId, categoryname) {
    $('#DeleteScrambleId').val(scrambleId);
    $('#ScrName').text(scrambleName);
    $('#CatName').text(categoryname);
    $('#DeleteScrambleModal').modal('show');
}

$('#ScrambleDeletionConfirmedButton').click(function () {
    let scrambleId = $('#DeleteScrambleId').val();
    $.post('/Categories/DeleteScrambleDefinition', { scrambleId: scrambleId },
        function (response) {
            ResponseCheck(response, HandleSetDefaultData);
        })
})

function EditMoves(scrambleId) {
    $.get('/Categories/GetScrambleDefinition/', { scrambleId: scrambleId },
        function (response) {
            ResponseCheck(response, ShowMovesModal);
        })
}

function ShowMovesModal(jsonData) {
    $('#ModifiedMovesScrambleId').val(jsonData.scramble.identity);
    $('#MoveModificationErrorMessage').text('');
    $('#ScrNameM').text(jsonData.scramble.name);
    $('#CatNameM').text(jsonData.scramble.category.name);
    $('#MovesList').val(GetMovesString(jsonData.moves));
    $('#MovesModificationModal').modal('show');
}

function GetMovesString(moves) {
    if (moves.length < 1) {
        return '';
    }
    else {
        let output = '';
        for (var i = 0; i < moves.length; i++) {
            output += moves[i].move;
            if (i < moves.length - 1) {
                output += ' ';
            }
        }
        return output;
    }
}

$('#MovesModificationConfirmButton').click(function () {
    let moves = $('#MovesList').val();
    let scrambleId = $('#ModifiedMovesScrambleId').val();
    $.post('/Categories/ModifyMoves/', { scrambleId: scrambleId, moves: moves },
        function (response) {
            ResponseCheck(response, HandleMovesResponse);
        })
})

function HandleMovesResponse(jsonData) {
    if (jsonData == '') {
        $('#MovesModificationModal').modal('hide');
        document.location.reload();
    }
    else {
        $('#MoveModificationErrorMessage').text(jsonData);
    }
}