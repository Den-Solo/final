using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cipher.Library;
using System.Web.Hosting;
using System.IO;

namespace Cipher.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            ViewBag.Title = "Encryptor";
            return View();
        }
    }
}
