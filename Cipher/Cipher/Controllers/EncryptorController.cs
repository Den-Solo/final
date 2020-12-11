using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Cipher.Models;
using Cipher.Library;
using Aspose.Words;
using Aspose.Words.Saving;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore;
using System.IO;
using MultipartDataMediaFormatter;
using MultipartDataMediaFormatter.Infrastructure;
using System.Globalization;
using System.ComponentModel;
using System.Reflection;

namespace Cipher.Controllers
{
    public class EncryptorController : ApiController
    {
        private const string _cookieName = "resultFileName";
        

        private enum ErrorMsg
        {
            [Description("Model loading error")]
            ModelNotLoaded,
            [Description("File loaded with error")]
            FileLoadingError,
            [Description("Keyword either empty or containts non rus chars")]
            InvalidKeyWord,
            [Description("File was corrupted")] //if docx opened or edited with exception
            InvalidFileContent,
            [Description("File extention is different from .txt or .docx")]
            InvalidFileExtention,
            [Description("Everything fine")]
            Ok
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage DoCryptography(InputDataModel model)
        {
            ErrorMsg status = ErrorMsg.Ok;
            string resultContent = null;
            string fileExtention = null;
            Guid guid = Guid.NewGuid();

            if (model == null)
            {
                status = ErrorMsg.ModelNotLoaded;
            }
            else //some checks in case js in frontend was abused
            {
                VigenereEncryptor.Operation op = model.ToEncrypt ? VigenereEncryptor.Operation.Encrypt : VigenereEncryptor.Operation.Decrypt;
                if (model.FromFile)
                {
                    if (model.InputFile == null)
                    {
                        status = ErrorMsg.FileLoadingError;
                    }
                    else
                    {
                        string fileName = System.IO.Path.GetFileName(model.InputFile.FileName);
                        string loadedFilePath = HomeController._LoadedFilesDir + guid + '.' + fileName.Split('.').Last();
                        System.IO.File.WriteAllBytes(loadedFilePath, model.InputFile.Buffer);

                        status = TryEncryptFile(loadedFilePath, model.KeyWord, guid.ToString(), op, out resultContent, out fileExtention);
                        System.IO.File.Delete(loadedFilePath); //we don't need uploaded file anymore
                    }
                }
                else //from text area input
                {
                    status = TryEncryptRawText(model.InputText, model.KeyWord, guid.ToString(), op, out resultContent);
                    fileExtention = "txt";
                }
            }
            if (status != ErrorMsg.Ok)
            {
                return Request.CreateResponse(HttpStatusCode.OK, 
                    new EncryptionResultModel() { IsError = true, Content = status.GetDescription()});
            }
            var response = Request.CreateResponse(HttpStatusCode.OK, new EncryptionResultModel() { IsError = false, Content = resultContent });
            response.Headers.AddCookies(new CookieHeaderValue[] { new CookieHeaderValue(_cookieName, guid.ToString() + '.' + fileExtention) });
            return response;
        }

        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage GetFile()
        {
            string fName = ResultFileNameFromCookie();
            if (fName == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            byte[] fileBytes = null;
            string path = HomeController._ResultFilesDir + fName;
            string extention = fName.Split('.').Last();
            if (File.Exists(path))
            {
                fileBytes = System.IO.File.ReadAllBytes(path);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.Gone);
            }
            var dataStream = new MemoryStream(fileBytes);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(dataStream);
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "result."+extention;
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            return response;
        }

        private string ResultFileNameFromCookie()
        {
            CookieHeaderValue cookie = Request.Headers.GetCookies(_cookieName).FirstOrDefault();
            if (cookie != null)
            {
                return cookie[_cookieName].Value;
            }
            return null;
        }


        private static void DeleteResultFiles(string resultFileName)
        {
            if (resultFileName != null)
            {
                System.IO.File.Delete(HomeController._ResultFilesDir + resultFileName);
            }
        }
        private static ErrorMsg TryEncryptFile(string filePath, string keyWord, string guid, VigenereEncryptor.Operation op, out string result, out string fileExtention)
        {
            result = null;
            fileExtention = null;
            ErrorMsg status = ErrorMsg.Ok;
            if (VigenereEncryptor.ValidateKeyWord(keyWord))
            {
                fileExtention = filePath.Split('.').Last();
                if (fileExtention == "docx")
                {
                    status = TryEncryptDocx(filePath, keyWord, guid, op, out result);

                }
                else if (fileExtention == "txt")
                {
                    status = TryEncryptTxt(filePath, keyWord, guid, op, out result);
                }
                else
                {
                    status = ErrorMsg.InvalidFileExtention;
                }
            }
            else
            {
                status = ErrorMsg.InvalidKeyWord;
            }
            return status;
        }
        private static ErrorMsg TryEncryptRawText(string text,string keyWord, string guid, VigenereEncryptor.Operation op, out string rawResult)
        {
            rawResult = VigenereEncryptor.Encrypt(text, keyWord, op);
            var status =  rawResult == null ? ErrorMsg.InvalidKeyWord : ErrorMsg.Ok;
            if (status == ErrorMsg.Ok)
            {
                System.IO.File.WriteAllText(HomeController._ResultFilesDir + guid + ".txt", rawResult);
            }
            return status;
        }
        private static ErrorMsg TryEncryptDocx(string filePath, string keyWord, string guid, VigenereEncryptor.Operation op, out string rawResult)
        {
            rawResult = null; //used for preview
            try
            {
                Document docx = new Document(filePath);
                VigenereEncryptor ve = new VigenereEncryptor(keyWord, op);
                for (int i = 1; i < docx.Sections[0].Body.Paragraphs.Count; i++)
                {
                    string ciph = ve.Encrypt(docx.Sections[0].Body.Paragraphs[i].GetText());
                    docx.Sections[0].Body.Paragraphs[i].Runs[0].Text = ciph;
                    if (ciph == null)
                    {
                        return ErrorMsg.InvalidKeyWord;
                    }
                    rawResult += ciph + "\n";
                }
                OoxmlSaveOptions opt = new OoxmlSaveOptions(SaveFormat.Docx);
                opt.Compliance = OoxmlCompliance.Ecma376_2006;
                docx.Save(HomeController._ResultFilesDir + guid + ".docx", opt);
            }
            catch (Exception)
            {
                return ErrorMsg.InvalidFileContent;
            }
            return ErrorMsg.Ok;
        }
        private static ErrorMsg TryEncryptTxt(string filePath, string keyWord, string guid, VigenereEncryptor.Operation op, out string result)
        {
            result = System.IO.File.ReadAllText(filePath);
            if (result.Contains(Convert.ToChar(0xFFFD)))
            {
                result = System.IO.File.ReadAllText(filePath, Encoding.Default);
            }
            result = VigenereEncryptor.Encrypt(result, keyWord, op);
            if (result == null)
            {
                return ErrorMsg.InvalidKeyWord;
            }
            System.IO.File.WriteAllText(HomeController._ResultFilesDir + guid + ".txt", result);
            return ErrorMsg.Ok;
        }
    }

    public static class Extensions
    {
        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }
    }

}
