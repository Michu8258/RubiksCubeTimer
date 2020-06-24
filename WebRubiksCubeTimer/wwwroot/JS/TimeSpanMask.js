function VerifyTime(inputID, value) {
    let charArray = value.split("");
    if (charArray.length > 12) {
        $('#NewDuration' + inputID).val($('#NewDuration' + inputID).val().substring(0, 12));
    }
    let ok = true;
    for (var i = 0; i < charArray.length; i++) {
        if (i == 2 || i == 5) {
            ok = CheckIfColon(charArray[i]);
        }
        else if (i == 8) {
            ok = CheckIfDot(charArray[i]);
        }
        else {
            ok = CheckIfNumber(charArray[i]);
        }

        if (ok == false) {
            break;
        }
    }

    if (ok == false) {
        $('#NewDuration' + inputID).addClass('text-danger');
        $('#NewDuration' + inputID).val($('#NewDuration' + inputID).val().slice(0, -1));
    }
    else {
        $('#NewDuration' + inputID).removeClass('text-danger');
    }

    return ok && (charArray.length == 0 || charArray.length == 12);
}

function CheckIfColon(char) {
    if (char == ':') {
        return true;
    }
    return false;
}

function CheckIfDot(char) {
    if (char == '.') {
        return true;
    }
    return false;
}

function CheckIfNumber(char) {
    if (char >= '0' && char <= '9') {
        return true;
    }
    return false;
}

function CheckIfPermittedCharacter(char) {
    let number = CheckIfNumber(char);
    let colon = CheckIfColon(char);
    let dot = CheckIfDot(char);
    return (number == true || colon == true || dot == true);
}