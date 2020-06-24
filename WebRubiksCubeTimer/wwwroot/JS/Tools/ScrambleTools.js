let minLength;
let maxLength;
let defaultLength;
let scrambleId;
let eliminateDuplicates;
let moves = [];
let scrambles = {};
let allowRegenerate;
let userInterfaceLocked = false;
let previosMovesAmount;

$(document).ready(function () {
    if ($('#ScrambleToolsButton').length && $('#ScramblesModal').length) {
        CollectDefaultScrambleValues();
        CreateScrambleContent();
        GetScrambles();
    }
})

function LockInterface() {
    userInterfaceLocked = true;
}

function UnlockInterface() {
    userInterfaceLocked = false;
}

function CollectDefaultScrambleValues() {
    defaultLength = $('#CurrentScramble_Scramble_DefaultScrambleLength').val();
    scrambleId = $('#SelectedScrambleId').val();
    eliminateDuplicates = $('#CurrentScramble_Scramble_EliminateDuplicates').is(':checked');
    minLength = $('#MinMovesAmount').text();
    maxLength = $('#MaxMovesAmount').text();
}

function RestoreDefaults() {
    $('#CurrentScramble_Scramble_DefaultScrambleLength').val(defaultLength);
    $('#SelectedScrambleId').val(scrambleId);
    $('#CurrentScramble_Scramble_EliminateDuplicates').prop('checked', eliminateDuplicates);
    $('#CurrentScramble_Scramble_EliminateDuplicates').val(eliminateDuplicates);
    $('#MinMovesAmount').text(minLength);
    $('#MaxMovesAmount').text(maxLength);
}

function GetScrambles() {
    let catId = $('#CategoryID').val();
    $.get('/Timer/GetScrambles/', { categoryId: catId },
        function (response) {
            scrambles = response;
            for (var i = 0; i < response.length; i++) {
                if (response[i].scramble.identity == scrambleId) {
                    GetMoves(response[i].moves);
                    allowRegenerate = response[i].scramble.allowRegenerate;
                    if (allowRegenerate) {
                        AddRegenerateButton();
                    }
                    Scramble();
                }
            }
        })
}

function GetMoves(movesCollection) {
    moves = [];
    for (var i = 0; i < movesCollection.length; i++) {
        moves.push(movesCollection[i].move);
    }
}

$('#ScrambleToolsButton').click(function () {
    if (userInterfaceLocked === false) {
        previosMovesAmount = $('#CurrentScramble_Scramble_DefaultScrambleLength').val();
        $('#ScramblesModal').modal('show');
    }
})

$('#ScrambleCancelButton').click(function () {
    if (userInterfaceLocked === false) {
        RestoreDefaults();
    }
})

$('#SelectedScrambleId').change(function () {
    if (userInterfaceLocked === false) {
        let scrambleId = $('#SelectedScrambleId').val();
        LoadScramble(scrambleId);
        EnableModalInputs(allowRegenerate);
        previosMovesAmount = defaultLength;
    }
})

$('#ScrambleConfirmButton').click(function () {
    if (userInterfaceLocked === false) {
        if (CheckScrambleLength() == true) {
            CollectDefaultScrambleValues();
            if ($('#CurrentScramble_Scramble_DefaultScrambleLength').val() != previosMovesAmount) {
                Scramble();
            }
            $('#ScramblesModal').modal('hide');
        }
        else {
            $('#CurrentScramble_Scramble_DefaultScrambleLength').val(defaultLength);
        }
    }
})

function CheckScrambleLength() {
    let min = parseInt($('#MinMovesAmount').text());
    let max = parseInt($('#MaxMovesAmount').text());
    let curr = $('#CurrentScramble_Scramble_DefaultScrambleLength').val();
    if (curr >= min && curr <= max) {
        return true;
    }
    return false;
}

function LoadScramble(id) {
    for (var i = 0; i < scrambles.length; i++) {
        if (scrambles[i].scramble.identity == id) {
            minLength = scrambles[i].scramble.minimumScrambleLength;
            maxLength = scrambles[i].scramble.maximumScrambleLength;
            defaultLength = scrambles[i].scramble.defaultScrambleLength;
            scrambleId = scrambles[i].scramble.identity;
            eliminateDuplicates = scrambles[i].scramble.eliminateDuplicates;
            allowRegenerate = scrambles[i].scramble.allowRegenerate;
            $('#TopWallColor').text(scrambles[i].scramble.topColor);
            $('#FrontWallColor').text(scrambles[i].scramble.frontColor);
            GetMoves(scrambles[i].moves);
            RestoreDefaults();
            RemoveRegenerateButton();
            if (scrambles[i].scramble.allowRegenerate) {
                AddRegenerateButton();
            }
            Scramble();
            break;
        }
    }
}

function CreateScrambleContent() {
    $('#ScrambleContainer').append(
        '<div class="row row-cols-2 bg-response rounded mx-1 py-1">' +
        '<div class="col-10">' +
        '<label class="d-flex justify-content-center align-items-center my-2 py-0" id="ScrambleLabel"></label>' +
        '</div>' +
        '<div class="col-2 d-flex justify-content-center align-items-center" id="RegenerateDiv">' +
        '</div>' +
        '</div>'
    );
}

function DeleteScrambleContent() {
    $('#ScrambleContainer').empty();
}

function AddRegenerateButton() {
    $('#RegenerateDiv').append(
        '<button type="button" class="btn btn-sm btn-dark text-white" id="RegenerateButton" onclick="Scramble()">Re-genereate</button>');
}

function RemoveRegenerateButton() {
    $('#RegenerateDiv').empty();
}

function GenerateScramble(amount, preventDuplicates) {
    var output = '';
    var lastMove = '';
    if (amount < 1) {
        return output;
    }
    if (moves.length < 1) {
        return 'No moves';
    }
    if (moves.length == 1) {
        for (var i = 0; i < amount; i++) {
            output += moves[0] + ' ';
        }
        output.slice(0, -1);
        return output;
    }
    for (var i = 0; i < amount; i++) {
        let error = true;
        while (error) {
            let randomMove = moves[Math.floor(Math.random() * moves.length)];
            if (preventDuplicates) {
                if (randomMove === lastMove) {
                    error = true;
                }
                else {
                    error = false;
                    output += randomMove + ' ';
                    lastMove = randomMove;
                }
            }
            else {
                error = false;
                output += randomMove + ' ';
                lastMove = randomMove;
            }
        }
    }
    output.slice(0, -1);
    return output;
}

function Scramble() {
    if (userInterfaceLocked === false) {
        if (scrambles.length > 0 && moves.length > 0) {
            let newScramble = GenerateScramble(defaultLength, eliminateDuplicates);
            $('#ScrambleLabel').text(newScramble);
        }
    }
}

function GetCurrentScramble() {
    if ($('#ScrambleLabel').length) {
        return $('#ScrambleLabel').text();
    }
    return '';
}

function EnableModalInputs(allowRegenerate) {
    if (allowRegenerate == true) {
        $('#CurrentScramble_Scramble_DefaultScrambleLength').prop('readonly', false);
    }
    else {
        $('#CurrentScramble_Scramble_DefaultScrambleLength').prop('readonly', true);
    }
}