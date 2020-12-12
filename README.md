# Web-приложение на стеке технологий ASP.NET MVC.

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

#### Содержание

1. 1 MVC-контроллер - HomeController
2. 1 WebApi-контроллер - EncryptorController
3. 2 модели:

  * InputDataModel
  * EncryptionResultModel

4) 1 представление:
	Index.cshtml (для HomeController), к нему также относятся Index.js, Site.css.
	+ Мастер страница _Lasyout.cshtml
В директории ~\Library имеются 3 вспомогательных класса (не относятся к технологиям ASP.NET):
	1. VigenereEncryptor
	2. OldFileCleaner
	3. Extentions
В директории ~\App_Data созданы 3 папки:
	1. LoadedFiles - для загружаемых на сервер файлов
	2. ResultFiles - для результатов шифрования
	3. Logs - для логов 
Про существующие по умолчанию в шаблоне ASP.NET + MVC + WEB api подробно не буду:


	
Классы:
...
Проект юнит-тестов:
...
