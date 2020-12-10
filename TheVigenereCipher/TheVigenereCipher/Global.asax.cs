using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TheVigenereCipher.Library;

namespace TheVigenereCipher
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string _LoadedFilesDir { get; private set; }
        public static string _LogPath { get; private set; }
        public static string _ResultFilesDir { get; private set; }
        public static TimeSpan _OutdatingInterval { get; private set; } = new TimeSpan(0, 3, 0);
        public static TimeSpan _CleanInterval { get; private set; } = new TimeSpan(0, 1, 0);

        public OldFileCleaner _oldFileCleaner;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            _LoadedFilesDir = Server.MapPath(@"~\LoadedFiles\");
            _ResultFilesDir = Server.MapPath(@"~\ResultFiles\");
            _LogPath = Server.MapPath(@"~\Logs\Log.txt");
            _oldFileCleaner = new OldFileCleaner(_CleanInterval, _OutdatingInterval, _ResultFilesDir, _LogPath);
        }
        protected void Application_End()
        {
            _oldFileCleaner.Abort();
        }
    }
}
