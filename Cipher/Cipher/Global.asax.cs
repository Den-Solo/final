using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Hosting;
using MultipartDataMediaFormatter;
using MultipartDataMediaFormatter.Infrastructure;
using Cipher.Library;

namespace Cipher
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static string _LoadedFilesDir { get; private set; }
        public static string _LogPath { get; private set; }
        public static string _ResultFilesDir { get; private set; }
        public static TimeSpan _OutdatingInterval { get; private set; } = new TimeSpan(0, 20, 0);
        public static TimeSpan _CleanInterval { get; private set; } = new TimeSpan(0, 2, 0);

        public static OldFileCleaner _oldFileCleaner;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.Formatters.Add(new FormMultipartEncodedMediaTypeFormatter(new MultipartFormatterSettings()));

            _LoadedFilesDir = HostingEnvironment.MapPath(@"~\App_Data\LoadedFiles\");
            _ResultFilesDir = HostingEnvironment.MapPath(@"~\App_Data\ResultFiles\");
            _LogPath = HostingEnvironment.MapPath(@"~\App_Data\Logs\Log.txt");
            _oldFileCleaner = new OldFileCleaner(_CleanInterval, _OutdatingInterval, _ResultFilesDir, _LogPath);
        }
        protected void Application_End()
        {
            _oldFileCleaner.Abort();
            _oldFileCleaner.ClearAll();
        }
    }
}
