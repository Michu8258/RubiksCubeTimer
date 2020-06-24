$('#Password').blur(function () {
    CheckPasswords();
});

$('#Password2').blur(function () {
    CheckPasswords();
});

$('#Email').blur(function () {
    CheckEmail();
});

$('#UserName').blur(function () {
    CheckUserName();
});

$('#CreateForm').submit(function (e) {
    Validate(e);
});

function CheckPasswords() {
    let pass1 = $('#Password').val();
    let pass2 = $('#Password2').val();
    let ok = (pass1 === pass2) && pass1.length >= 8;
    if (!ok) {
        document.getElementById("PasswordNotEqual").classList.remove('d-none');
    }
    else {
        document.getElementById("PasswordNotEqual").classList.add('d-none');
    }
    return ok;
}

function CheckEmail() {
    let mail = document.getElementById("Email").value;
    let ok = mail.includes('@') && mail.includes('.');
    if (!ok) {
        document.getElementById("BadEmail").classList.remove('d-none');
    }
    else {
        document.getElementById("BadEmail").classList.add('d-none');
    }
    return ok;
}

function CheckUserName() {
    let userName = document.getElementById("UserName").value;
    let ok = userName.length >= 8;
    if (!ok) {
        document.getElementById("BadUserName").classList.remove('d-none');
    }
    else {
        document.getElementById("BadUserName").classList.add('d-none');
    }
    return ok;
}

function Validate(e) {
    let passwordOK = CheckPasswords();
    let emailOK = CheckEmail();
    let userNameOK = CheckUserName();
    if (!(passwordOK && emailOK && userNameOK)) {
        e.preventDefault(e);
    }
}
