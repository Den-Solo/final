using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheVigenereCipher.Library;
using TheVigenereCipher.Models;
using System.IO;

namespace TheVigenereCipher.Controllers
{
    public class HomeController : Controller
    {
        VigenereEncryptor _encryptor = new VigenereEncryptor();
        string lastFileName = "";
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DoCryptography(InputDataModel model, string encryptButton, string decryptButton)
        {
            string path = null;
            if (model.InputFile != null)
            {
                string fileName = System.IO.Path.GetFileName(model.InputFile.FileName);
                // сохраняем файл в папку Files в проекте
                path = "~/LoadedFiles/" + Guid.NewGuid() + fileName.Split('.').Last();
                lastFileName = path;
                model.InputFile.SaveAs(Server.MapPath(path));
            }
            string res = "Empty file";
            if (path != null)
                res = System.IO.File.ReadAllText(Server.MapPath(path));
            if (encryptButton != null)
            {
                res = "Encrypt" + res + model.KeyWord;
            }
            else
            {
                res = "Decrypt" + res + model.KeyWord;
            }
            return Json(res, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public FileResult GetFile(string txt, string docx)
        {
            //if (lastFileName == "") { return null; }
            if (txt != null)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/LoadedFiles/text.txt"));
                string fileName = lastFileName.Split('\\').Last();
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "text.txt");
            }
            else
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/LoadedFiles/text.txt"));
                string fileName = lastFileName.Split('\\').Last();
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "text.docx");
            }
        }
    }
}