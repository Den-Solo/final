# Web-приложение на стеке технологий ASP.NET MVC

## 2 проекта

 1. само приложение
 2. юнит-тесты

## Основной проект

### Общее описание

Это ASP.NET MVC + WEB Api single page приложение.
Серьезных Js фреймворков (типа Angular) не используется. Для оформления интерфейса  применялся Bootstrap.

#### Зависимости

 1. Aspose.Words - триал-версия библиотек для работы с docx
 2. bootstrap 4.5.3
 3. jQuery 3.5.1
 4. MultipartDataMediaFormatter.V2 - чтобы решить проблему с отправкой файла на сервер  модели через ajax
 5. Microsoft.AspNet. ... (множество библиотек для работы самого Asp.Net)
 6. Другие библиотеки, которыми не пользовался или пользовался косвенно (через зависимые элементы)

#### Содержание проекта

* x1 MVC-контроллер - HomeController
* x1 WebApi-контроллер - EncryptorController
* x2 модели:
  1. InputDataModel
  2. EncryptionResultModel
* x1 представление:
  1. Index.cshtml (для HomeController), к нему также относятся Index.js, Site.css.
  2. Мастер страница _Lasyout.cshtml
* В директории ~\Library имеются 3 вспомогательных класса (не относятся к технологиям ASP.NET):
  1. VigenereEncryptor
  2. OldFileCleaner
  3. Extentions
* В директории ~\App_Data созданы 3 папки:
  1. LoadedFiles - для загружаемых на сервер файлов
  2. ResultFiles - для результатов шифрования
  3. Logs - для логов
* Про существующие по умолчанию в шаблоне ASP.NET + MVC + WEB api подробно не буду:
  1. ~\Content\ - .css файлы (bootsrap + Site.css)
  2. ~\App_Start\ - конфигурация приложения (маршруты web api и др.)
  3. ~\Global.asax - Конфигурация приложения + маппинг виртуальных путей директорий в физические + работа с OldFileCleaner
  4. ~\Scripts\ - js файлы, включая Index.js
  5. ~\Views\ - Представления MVC. Нас интересуют только Home\Index.cshtml и Shared\ _Layout.cshtml

#### Классы и методы

* Модели (директория: ~\Models)
  * InputDataModel  
  Используется для отправки запроса на сервер. Может содержать либо файл(HttpFile), либо текст, введенный с клавиатуры. Обязательно имеет строку - ключевое слово и информацию о том, какое действие с данными произвести - зашифровать или расшифровать.
  * EncryptionResultModel  
  Используется для ответа клиенту.  
  Имеет всего 2 свойства:
    * bool IsError - ответ, была ли ошибка в процессе работыт с данными  
    * string Content - если была ошибка - здесь будет информация о ней, если все успешно - здесь будет результат шифрования(дешифрования) для предпросмотра.
* Контроллеры (директория: ~\Controllers)
  * HomeController  
  Основной mvc-контроллер.  
  Методы:  
    * public ActionResult Index()  
    Возращается представление Index.cshtml.
  * EncryptorController  
  Класс, отвечающий за обработку клиент-серверных запросов. Работает с обоими имеющимися моделями. Своего представления не имеет (использует Index.cshtml).  
    * enum:
      * private enum ErrorMsg  
    Содержит набор возвращаемых методами состояний - ошибок. Каждый элемент имеет тектовое описание через атрибут Description.  
    * Поля:
      * private const string _cookieName - имя сессионного cookie для хранения имени возвращаемого файла с результатом щифрования.
    * Методы-обработчики http-запросов:
      * DoCryptography(InputDataModel model)  
    Получает http запрос с InputDataModel и обрабатывает его. Возвращает ответ в формате EncryptionResultModel. Если обработка успешна, клиенту ставится cookie с именем получившегося файла.
      * GetFile()  
    Проверяет, что у клиента есть необходимый куки (с именем файла) и отправляет этот файл, если есть.
    * Вспомогательные методы:
      * ResultFileNameFromCookie  
    Возвращает имя файла, в который хранится зашифрованный текст, из куки клиента. (Возвращает null, если такого куки нет).
      * TryEncryptFile  
    Выполняет попытку зашифровать файл. Внутри обрабатывает расширение файла и вызывает либо TryEncryptDocx, либо TryEncryptTxt. Обрабатывает ситуации с неверным расширением файла. Возвращает ErrorMsg с результатом операции.
      * TryEncryptRawText  
    Пытается выполнить шифрование текста с обработкой ошибок. Возвращает ErrorMsg с результатом операции.
      * TryEncryptDocx  
    Пытается открыть и обработать .docx документ. Возвращает ErrorMsg с результатом операции.
      * TryEncryptTxt
    Выполняет попытку обработки текстового файла. Возвращает ErrorMsg с результатом операции.
* Дополнительные классы (директория: ~\Library)
  * VigenereEncryptor  
  Занимается непосредственно шифрованием/дешифрованием текста.
    * Методы
      * конструктор VigenereEncryptor(string keyWord, Operation op)  
  устанваливает тип обработки (зашифровать/расшифровать) и кодовое слово.
      * нестатичный Encrypt(in string text)  
  шифрует фрагмент текста. Сохраняет следующий индекс в массиве нидексов символов кодового слова. Позволяет работать с раздробленным на форагменты текстом, например, с абзацами в .docx. В случае неправльного кодового слова возвращает null.
      * статичный Encrypt(in string text, in string keyWord, Operation op)  
  Шифрует текст целиком. Не подходит для раздробленного текста. В случае неправльного кодового слова возвращает null.
      * статичный EncryptChar(char c, short kwCharIdx, out char result)  
  Шифрует один символ. Фактически, реализует проверку: является ли символ кириллицей и, если да, выполняет формулу шифрования. Так как шифрование от дешифрования отличается слабо, а именно все отличие можно свести к индексам кодового слова, формула нужна только одна. Возвращает true, если символ кириллица и был зашифрован, false - если символ не кириллица.
      * статичный StrToIdxs(in string s)  
  Возвращает массив индексов от строки из кириллических символов. Если символ НЕ из кириллицы - возвращает null.
      * статичный GetKeyWordIdxs(in string keyWord, Operation op)  
  Преобразует ключевое слово в массив индексов, учитывая операцию: шифрование/дешифрование. Если символ НЕ из кириллицы - возвращает null.
      * bool ValidateKeyWord(string KeyWord)  
  Выполняет проверку на соотвествие кодового слова заданным требованиям.
  * OldFileCleaner  
    Асинхронно через заданные интервалы времени удалаяет из заданной директории файлы, существующие дольше какого-то интервала времени. Используется для очистки файлов в ~\App_Data\ResultFiles.  
    Объект класса создается в методе Application_Start класса WebApiApplication (~\Global.asax).  
    В методе Application_End того же класса выполняется остановка потока очистки.  
    Методы:
    * Конструктор OldFileCleaner
    Задается значения полей и запускает поток очистки.
    * Clean()  
    В бесконечном цикле через время _cleanInterval запускает проверку директории на наличие файлов старше _outdatingInterval и удаляет их.
    * ClearAll()  
    Удаляет все файлы в заднной директории.
    * Abort()  
    Останавливает поток очистки.
  * Extentions  
    Содержит метод расширения GetDescription для извлечения в строковом виде информации из тега Description (System.ComponentModel). Используется для enum ErrorMsg в классе EncryptorController
* ~\Global.asax  
Кроме стандартных действие по конфигурации приложения на запуске, раотает с экземпляром класса OldFileCleaner. Задает ему интервал очистки и время устаревания файлов.

#### Представления

 В директории ~\Views находятся представления.

* Shared\ _Layout.cshtml - глобальное оформление
* Home\Index.cshtml (совместно с ~\Content\Site.css и ~\Scripts.Index.js)  
Это наш фронтэнд.  
Index.cshtml содержит раметку страницы.  
Index.js содерджит следующие компоненты:
  * Глобальная переменная selectedTab, хранящая объект-ссылку на текущую вкладку. Вкладок всего две. Первая - для ввода текста, вторая - для загрузки файла. В зависимости от выбранной вкладки будет отличаться содержание InputDataModel, отправляемого на сервер.
  * Функции:
    * $(document).ready()  
    Выполняет инииацлизацию после полной загрузки страницы. Присваивает начальное значение переменной selectedTab и задает обработчкик для событий переключения вкладок (текст или файл) и выбора файла.
    * function ValidateKeyWord(keyWord)  
    Проверяет, что в ключевом слове только русский символы.
    * function ValidateFileType(fileName)  
    Проверяет, что тип файла .txt или .docx
    * function EnableDownloadBtns(fileType)  
    Включает возможность нажатия кнопки "Скачать результат"
    * function DisableDownloadBtns()  
    Отключает возможность нажатия кнопки "Скачать результат"
    * function SendData(senderName)  
    При нажатии на кнопку "Зашифровать" или "Расшифровать" запускается эта функция. Она проверяет валидность ключевого слова и расширения файла, затем формирует ajax запрос к методу EncryptionController\DoCryptography. Запрос формируется в формате, соответвующем InputDataModel.  
      * В случае успеха запроса: проверяется ответ (в формате EncryptionResultModel). Если ответ сообщает об ошибке обработки (IsError == true) - показывает ее содердимое через alert. Если обработка была успешна (IsError == false) - выводит текст в окно предпросмотра и активирует кнопку скачивания результата.
      * В случае ошибки - сообщает в alert, что с сервером проблемы.

## Проект юнит-тестов

### EncryptorControllerTest

Директория: ~\Controllers  
Выполняет тестирование методов класса EncryptorController.  
Имена методов совпадают с тестируемыми методами класса EncryptorController.

### HomeControllerTest

Директория: ~\Controllers  
Тестирует единственный метод Home контроллера - метод Index на возвращение корректного (непустого)представления. (View).

### VigenerEncryptorTest

Директория: ~\Library  
Выполняет тестирование методов класса VigenereEncryptor.
