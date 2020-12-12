var selectedTab;

$(document).ready(function () {
    /*window.onbeforeunload = HandleOnBeforeUnload;*/
    DisableDownloadBtns();

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        selectedTab = e.target // activated tab
    });

    $('#InputFile').on('change', function () {
        //get the file name
        var fileName = $(this).val();
        //replace the "Choose a file" label
        pathPieces = fileName.split('\\');
        $(this).next('.custom-file-label').html(pathPieces[pathPieces.length - 1]);
    });

    selectedTab = $('#tab_enter_text-tab')
})


function ValidateKeyWord(keyWord) {
    return RegExp('^[А-Яа-яЁё]+$').test(keyWord);
};
function ValidateFileType(fileName) {
    return RegExp('^.*\.(docx|txt)$').test(fileName);
};

function EnableDownloadBtns(fileType) {
    var btns = $('.download_buttons').children();
    btns[0].removeAttribute('disabled');
};
function DisableDownloadBtns() {
    var btns = $('.download_buttons').children();
    btns[0].setAttribute('disabled', 'disabled');
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
        if ($('#InputText').val().length == 0) {
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
        if (isFromFile) {
            data.append("InputFile", files[0]);
        }
        else {
            data.append("InputText", $('#InputText').val());
        }
        data.append("KeyWord", keyWord);
        data.append("ToEncrypt", senderName == 'EncryptButton');
        data.append("FromFile", isFromFile);
        $.ajax({
            type: "POST",
            url: '/api/Encryptor/DoCryptography',
            contentType: false,
            processData: false,
            data: data,
            success: function (result) {
                if (result['IsError'] == false) {
                    document.getElementById('result').textContent = result['Content'];
                    EnableDownloadBtns();
                }
                else {
                    alert(result['Content']);
                    DisableDownloadBtns();
                }
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