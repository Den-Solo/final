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
        public static string _LoadedFilesDir { get; private set; }
        public static string _LogPath { get; private set; }
        public static string _ResultFilesDir { get; private set; }
        public static TimeSpan _OutdatingInterval { get; private set; } = new TimeSpan(0, 10, 0);
        public static TimeSpan _CleanInterval { get; private set; } = new TimeSpan(0, 2, 0);

        public static OldFileCleaner _oldFileCleaner;
        static HomeController()
        {
            _LoadedFilesDir = HostingEnvironment.MapPath(@"~\App_Data\LoadedFiles\");
            _ResultFilesDir = HostingEnvironment.MapPath(@"~\App_Data\ResultFiles\");
            _LogPath = HostingEnvironment.MapPath(@"~\App_Data\Logs\Log.txt");
            _oldFileCleaner = new OldFileCleaner(_CleanInterval, _OutdatingInterval, _ResultFilesDir, _LogPath);

            System.IO.File.WriteAllText(_LogPath, "static HomeController()\n");
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Encryptor";
            return View();
        }
    }
}
