using Cipher;
using Cipher.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using System.Reflection;
using Cipher.Library;
using Aspose.Words;
using System.IO;

namespace Cipher.Tests.Controllers
{
    [TestClass]
    public class EncryptorControllerTest
    {
        string projectDir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
        string plainText = "Привет мир 123 Hello\nЭто тест!";
        string cipherText = "Ябсдйе ьщщ 123 Hello\nЯчб вхъф!";
        string keyWord = "Привет";
        [TestMethod]
        public void DoCryptography()
        {
            EncryptorController controller = new EncryptorController();
            controller.Request = new HttpRequestMessage();
            controller.Request.SetConfiguration(new HttpConfiguration());
            // Wrong KeyWord 1
            {
                Models.InputDataModel inp = new Models.InputDataModel()
                {
                    InputFile = null,
                    InputText = plainText,
                    FromFile = false,
                    KeyWord = "qwe",
                    ToEncrypt = true
                };
                var response = controller.DoCryptography(inp);
                Models.EncryptionResultModel result = JsonConvert.DeserializeObject<Models.EncryptionResultModel>(response.Content.ReadAsStringAsync().Result);
                // Утверждение
                Assert.IsTrue(result.IsError);
                Assert.AreEqual(EncryptorController.ErrorMsg.InvalidKeyWord.GetDescription(), result.Content);
            }
            // Wrong KeyWord 2
            {
                Models.InputDataModel inp = new Models.InputDataModel()
                {
                    InputFile = null,
                    InputText = plainText,
                    FromFile = false,
                    KeyWord = "привет мир",
                    ToEncrypt = true
                };
                var response = controller.DoCryptography(inp);
                Models.EncryptionResultModel result = JsonConvert.DeserializeObject<Models.EncryptionResultModel>(response.Content.ReadAsStringAsync().Result);
                // Утверждение
                Assert.IsTrue(result.IsError);
                Assert.AreEqual(EncryptorController.ErrorMsg.InvalidKeyWord.GetDescription(), result.Content);
            }
            // Wrong file extention
            {
                Models.InputDataModel inp = new Models.InputDataModel()
                {
                    InputFile = new MultipartDataMediaFormatter.Infrastructure.HttpFile("fileName.wrongFType", "application/octet-stream", Encoding.UTF8.GetBytes(plainText)),
                    InputText = null,
                    FromFile = true,
                    KeyWord = keyWord,
                    ToEncrypt = true
                };
                var response = controller.DoCryptography(inp);
                Models.EncryptionResultModel result = JsonConvert.DeserializeObject<Models.EncryptionResultModel>(response.Content.ReadAsStringAsync().Result);
                // Утверждение
                Assert.IsTrue(result.IsError);
                Assert.AreEqual(EncryptorController.ErrorMsg.InvalidFileExtention.GetDescription(), result.Content);
            }
            // Correct encoding from rawText (textarea)
            {
                Models.InputDataModel inp = new Models.InputDataModel()
                {
                    InputFile = null,
                    InputText = plainText,
                    FromFile = false,
                    KeyWord = "привет",
                    ToEncrypt = true
                };
                var response = controller.DoCryptography(inp);
                Models.EncryptionResultModel result = JsonConvert.DeserializeObject<Models.EncryptionResultModel>(response.Content.ReadAsStringAsync().Result);
                // Утверждение
                Assert.IsFalse(result.IsError);
                Assert.AreEqual(cipherText, result.Content);
            }
            // Correct decoding from rawText (textarea)
            {
                Models.InputDataModel inp = new Models.InputDataModel()
                {
                    InputFile = null,
                    InputText = cipherText,
                    FromFile = false,
                    KeyWord = keyWord,
                    ToEncrypt = false
                };
                var response = controller.DoCryptography(inp);
                Models.EncryptionResultModel result = JsonConvert.DeserializeObject<Models.EncryptionResultModel>(response.Content.ReadAsStringAsync().Result);
                // Утверждение
                Assert.IsFalse(result.IsError);
                Assert.AreEqual(plainText, result.Content);
            }
            // Correct encoding from file.txt
            {
                Models.InputDataModel inp = new Models.InputDataModel()
                {
                    InputFile = new MultipartDataMediaFormatter.Infrastructure.HttpFile("fileName.txt", "application/octet-stream", Encoding.UTF8.GetBytes(plainText)),
                    InputText = null,
                    FromFile = true,
                    KeyWord = keyWord,
                    ToEncrypt = true
                };
                var response = controller.DoCryptography(inp);
                Models.EncryptionResultModel result = JsonConvert.DeserializeObject<Models.EncryptionResultModel>(response.Content.ReadAsStringAsync().Result);
                // Утверждение
                Assert.IsFalse(result.IsError);
                Assert.AreEqual(cipherText, result.Content);
            }
            // Correct decoding from file.txt
            {
                Models.InputDataModel inp = new Models.InputDataModel()
                {
                    InputFile = new MultipartDataMediaFormatter.Infrastructure.HttpFile("fileName.txt", "application/octet-stream", Encoding.UTF8.GetBytes(cipherText)),
                    InputText = null,
                    FromFile = true,
                    KeyWord = keyWord,
                    ToEncrypt = false
                };
                var response = controller.DoCryptography(inp);
                Models.EncryptionResultModel result = JsonConvert.DeserializeObject<Models.EncryptionResultModel>(response.Content.ReadAsStringAsync().Result);
                // Утверждение
                Assert.IsFalse(result.IsError);
                Assert.AreEqual(plainText, result.Content);
            }
            // Correct encoding from file.docx
            {
                string pathTest = projectDir + "/TestFiles/Test_1.docx";
                string pathTestResult = projectDir + "/TestFiles/Test_1_result.docx";
                Models.InputDataModel inp = new Models.InputDataModel()
                {
                    InputFile = new MultipartDataMediaFormatter.Infrastructure.HttpFile("fileName.docx", "application/octet-stream",
                        System.IO.File.ReadAllBytes(pathTest)),
                    InputText = null,
                    FromFile = true,
                    KeyWord = keyWord,
                    ToEncrypt = true
                };
                var response = controller.DoCryptography(inp);
                Models.EncryptionResultModel result = JsonConvert.DeserializeObject<Models.EncryptionResultModel>(response.Content.ReadAsStringAsync().Result);
                // Утверждение
                Assert.IsFalse(result.IsError);
                Assert.AreEqual(cipherText, result.Content.Replace("\f","").Replace("\r","").Replace("", "").TrimEnd()); //raw text
            }
        }
        [TestMethod]
        public void TryEncryptFile()
        {
            var tryEncryptTxt = typeof(EncryptorController).GetMethod("TryEncryptFile", BindingFlags.NonPublic | BindingFlags.Static);
            Guid guid = Guid.NewGuid();
            {
                object[] parameters = new object[] { projectDir + "/TestFiles/Test_1.docx", keyWord, guid.ToString(), VigenereEncryptor.Operation.Encrypt, null,null };
                var response = (EncryptorController.ErrorMsg)tryEncryptTxt.Invoke(null, parameters);
                Assert.AreEqual(EncryptorController.ErrorMsg.Ok, response);
                Assert.AreEqual("docx", parameters[parameters.Length -1]);
                Assert.AreEqual(cipherText, parameters[parameters.Length - 2].ToString().Replace("\r", "").Replace("\f", "").TrimEnd());
            }
            {
                object[] parameters = new object[] { projectDir + "/TestFiles/Test_1.txt", keyWord, guid.ToString(), VigenereEncryptor.Operation.Encrypt, null, null };
                var response = (EncryptorController.ErrorMsg)tryEncryptTxt.Invoke(null, parameters);
                Assert.AreEqual(EncryptorController.ErrorMsg.Ok, response);
                Assert.AreEqual("txt", parameters[parameters.Length - 1]);
                Assert.AreEqual(cipherText, parameters[parameters.Length - 2].ToString().Replace("\r", "").Replace("\f", "").TrimEnd());
            }
        }
        [TestMethod]
        public void TryEncryptRawText()
        {
            var tryEncryptTxt = typeof(EncryptorController).GetMethod("TryEncryptRawText", BindingFlags.NonPublic | BindingFlags.Static);
            Guid guid = Guid.NewGuid();
            {
                object[] parameters = new object[] { plainText, keyWord, guid.ToString(), VigenereEncryptor.Operation.Encrypt, null };
                var obj = tryEncryptTxt.Invoke(null, parameters);
                Assert.AreEqual(cipherText, parameters.Last().ToString().Replace("\r", "").Replace("\f", "").TrimEnd());
            }
        }
        [TestMethod]
        public void TryEncryptDocx()
        {
            var tryEncryptTxt = typeof(EncryptorController).GetMethod("TryEncryptDocx", BindingFlags.NonPublic | BindingFlags.Static);
            Guid guid = Guid.NewGuid();
            {
                object[] parameters = new object[] { projectDir + "/TestFiles/Test_1.docx", keyWord, guid.ToString(), VigenereEncryptor.Operation.Encrypt, null };
                var obj = tryEncryptTxt.Invoke(null, parameters);
                Assert.AreEqual(cipherText, parameters.Last().ToString().Replace("\r", "").Replace("\f","").TrimEnd());
            }
        }
        [TestMethod]
        public void TryEncryptTxt()
        {
            var tryEncryptTxt = typeof(EncryptorController).GetMethod("TryEncryptTxt", BindingFlags.NonPublic | BindingFlags.Static);
            Guid guid = Guid.NewGuid();
            {
                object[] parameters = new object[] { projectDir + "/TestFiles/Test_1.txt", keyWord, guid.ToString(), VigenereEncryptor.Operation.Encrypt, null };
                var obj = tryEncryptTxt.Invoke(null, parameters);
                Assert.AreEqual(cipherText, parameters.Last().ToString().Replace("\r", ""));
            }
            {
                object[] parameters = new object[] { projectDir + "/TestFiles/Test_1_result.txt", keyWord, guid.ToString(), VigenereEncryptor.Operation.Decrypt, null };
                var obj = tryEncryptTxt.Invoke(null, parameters);
                Assert.AreEqual(plainText, parameters.Last().ToString().Replace("\r", ""));
            }
        }

        private bool CompareDocx(string path1, string path2)
        {
            bool isOk = false;
            Aspose.Words.Document docx1 = new Document(path1);
            Aspose.Words.Document docx2 = new Document(path2);
            isOk = isOk && docx1.Sections[0].Body.Paragraphs.Count == docx2.Sections[0].Body.Paragraphs.Count;
            for (int i = 1; i < docx1.Sections[0].Body.Paragraphs.Count; i++)
            {
                isOk = isOk && docx1.Sections[0].Body.Paragraphs[i].Runs[0].GetText() == docx2.Sections[0].Body.Paragraphs[i].Runs[0].GetText();
            }
            return isOk;
        }
    }
}
