using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheVigenereCipher.Library;
using TheVigenereCipher.Models;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Xceed.Words.NET;
using Aspose.Words;
using Aspose.Words.Saving;

namespace TheVigenereCipher.Controllers
{
    public class HomeController : Controller
    {
        private const string _cookieName = "resultGUID";
        private static Regex _rusOnly = new Regex("^[А-Яа-яЁё]+$");
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DoCryptography(InputDataModel model)
        {
            DeleteResultFiles(ResultGuidFromCookie());
            string loadedFilePath = null;
            Guid guid = Guid.NewGuid();
            if (model.InputFile != null)
            {
                string fileName = System.IO.Path.GetFileName(model.InputFile.FileName);
                loadedFilePath = MvcApplication._LoadedFilesDir + guid + "_downloaded." + fileName.Split('.').Last();
                model.InputFile.SaveAs(loadedFilePath);
            }
            string res = null;
            if (loadedFilePath != null)
            {
                VigenereEncryptor.Operation op = model.ToEncrypt ? VigenereEncryptor.Operation.Encrypt : VigenereEncryptor.Operation.Decrypt;
                TryEncryptFile(loadedFilePath, model.KeyWord, guid.ToString(), op, out res);
                System.IO.File.Delete(loadedFilePath);
            }
            else
            {
                res = "Problem with file loading";
            }
            HttpContext.Response.Cookies.Add(new HttpCookie(_cookieName, guid.ToString()));
            return Json(res, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public FileResult GetFile(string txt, string docx)
        {
            string guid = ResultGuidFromCookie();

            if (guid == null)
            {
                return null;
            }
            if (txt != null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(MvcApplication._ResultFilesDir + guid + ".txt");
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "result.txt");
            }
            else
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(MvcApplication._ResultFilesDir + guid + ".docx");
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "result.docx");
            }
        }
        [HttpGet]
        public void OnPageClose(string resultGUID)
        {
            DeleteResultFiles(resultGUID);
        }

        private string ResultGuidFromCookie()
        {
            return HttpContext.Request.Cookies.AllKeys.Contains(_cookieName) ? HttpContext.Request.Cookies[_cookieName].Value : null;
        }
        private static void DeleteResultFiles(string guid)
        {
            if (guid != null)
            {
                System.IO.File.Delete(MvcApplication._ResultFilesDir + guid + ".txt");
                System.IO.File.Delete(MvcApplication._ResultFilesDir + guid + ".docx");
            }
        }



        public static bool TryEncryptFile(string filePath, string keyWord, string guid, VigenereEncryptor.Operation op, out string result)
        {
            result = null;
            bool status = true;
            if (_rusOnly.IsMatch(keyWord))
            {
                string extention = filePath.Split('.').Last();
                if (extention == "docx")
                {
                    result = EncryptDocx(filePath, keyWord, guid, op);
                }
                else if (extention == "txt")
                {
                    result = EncryptTxt(filePath, keyWord, guid, op);
                }
                else
                {
                    result = "Wrong file extention";
                    status = false;
                }
            }
            else
            {
                result = "Wrong KeyWord";
                status = false;
            }
            return status;
        }
        public static string EncryptDocx(string filePath, string keyWord, string guid, VigenereEncryptor.Operation op)
        {
            string rawResult = null; //used for preview
            try
            {
                Document docx = new Document(filePath);
                VigenereEncryptor ve = new VigenereEncryptor(keyWord, op);
                for (int i = 1; i < docx.Sections[0].Body.Paragraphs.Count; i++)
                {
                    string ciph = ve.Encrypt(docx.Sections[0].Body.Paragraphs[i].GetText());
                    docx.Sections[0].Body.Paragraphs[i].Runs[0].Text = ciph;
                    rawResult += ciph + "\n";
                }
                OoxmlSaveOptions opt = new OoxmlSaveOptions(SaveFormat.Docx);
                opt.Compliance = OoxmlCompliance.Ecma376_2006;
                docx.Save(MvcApplication._ResultFilesDir + guid + ".docx",opt);
                docx.Save(MvcApplication._ResultFilesDir + guid + ".txt");
            }
            catch(Exception e)
            {

            }
            return rawResult;
        }
        public static string EncryptTxt(string filePath, string keyWord, string guid, VigenereEncryptor.Operation op)
        {
            string result = System.IO.File.ReadAllText(filePath);
            if (result.Contains(Convert.ToChar(0xFFFD)))
            {
                result = System.IO.File.ReadAllText(filePath, Encoding.Default);
            }
            result = VigenereEncryptor.Encrypt(result, keyWord, op);
            System.IO.File.WriteAllText(MvcApplication._ResultFilesDir + guid + ".txt", result);
            return result;
        }
    }
}