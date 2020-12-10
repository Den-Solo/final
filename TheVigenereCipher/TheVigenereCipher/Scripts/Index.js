var selectedTab = $('#myTab li.active');

$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    selectedTab = e.target // activated tab
})
function ValidateKeyWord(keyWord) {
    return RegExp('^[А-Яа-яЁё]+$').test(keyWord);
};
function ValidateFileType(fileName) {
    return RegExp('^.*\.(docx|txt)$').test(fileName);
};

function EnableDownloadBtns() {
    var btns = $('.download_buttons').children();
    btns[0].removeAttribute('disabled');
    btns[1].removeAttribute('disabled');
};
function DisableDownloadBtns() {
    var btns = $('.download_buttons').children();
    btns[0].setAttribute('disabled', 'disabled');
    btns[1].setAttribute('disabled', 'disabled');
};


function SendData(senderName) {
    var files = document.getElementById('InputFile').files;
    var keyWord = document.getElementById('KeyWord').value;
    var isFromFile = selectedTab.id == "tab_upload_file-tab";
    if (isFromFile) {
        if (files == null || files.length == 0) {
            alert("файл не выбран");
            return;
        }
        if (ValidateFileType(files[0].name) == false) {
            alert("Поддерживаются только .txt и .docx");
            return;
        }
    } else {
        if ($('#InputText').text.length == 0) {
            alert("Текстовое поле пустое");
            return;
        }
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
        data.append("ToEncrypt", senderName == 'EncryptButton')
        $.ajax({
            type: "POST",
            url: '/Home/DoCryptography',
            contentType: false,
            processData: false,
            data: data,
            success: function (result) {
                document.getElementById('result').textContent = result;
                EnableDownloadBtns();
            },
            error: function (xhr, status, p3) {
                document.getElementById('result').textContent = xhr.responseText;
                DisableDownloadBtns();
            }
        });
    } else {
        alert("Браузер не поддерживает загрузку файлов HTML5!");
    }

};

$('#InputFile').on('change', function () {
    //get the file name
    var fileName = $(this).val();
    //replace the "Choose a file" label
    pathPieces = fileName.split('\\');
    $(this).next('.custom-file-label').html(pathPieces[pathPieces.length - 1]);
});

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
};

function HandleOnBeforeUnload()
{
    var data = new FormData();
    data.append("resultGUID", getCookie("resultGUID"));
    $.ajax({
        type: "Get",
        url: '/Home/OnPageClose',
        contentType: false,
        processData: false,
        data: data,
        success: function (result) {},
        error: function (xhr, status, p3) {}
    });
}