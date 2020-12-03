function EncryptButtonClick() {
    SendData('EncryptButton');
};
function DecryptButtonClick() {
    SendData('DecryptButton');
};
function ValidateKeyWord(keyWord) {
    return RegExp('^[А-Яа-яЁё]+$').test(keyWord);
};

function SendData(senderName) {
    var files = document.getElementById('InputFile').files;
    var keyWord = document.getElementById('KeyWord').value;
    if (files == null || files.length == 0) {
        alert("файл не выбран");
        return;
    }
    if (keyWord.length < 1) {
        alert("Ключевое слово должно состоять хотя бы из одной буквы");
        return;
    }
    if (ValidateKeyWord(keyWord) == false) {
        alert("Ключевое слово должно состоять только из русских букв");
        return;
    }

    if (window.FormData !== undefined) {
        var data = new FormData();
        data.append("InputFile", files[0]);
        data.append("KeyWord", keyWord);
        data.append(senderName, 'ButtonPressed')
        $.ajax({
            type: "POST",
            url: '/Home/DoCryptography',
            contentType: false,
            processData: false,
            data: data,
            success: function (result) {
                document.getElementById('result').textContent = "Success\n" + result;
            },
            error: function (xhr, status, p3) {
                document.getElementById('result').textContent = "Error\n" + xhr.responseText;
            }
        });
    } else {
        alert("Браузер не поддерживает загрузку файлов HTML5!");
    }

};
