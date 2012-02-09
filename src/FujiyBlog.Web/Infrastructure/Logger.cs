using System;
using System.IO;
using System.Web;

namespace FujiyBlog.Web.Infrastructure
{
    internal static class Logger
    {
        private static readonly object Sync = new object();

        public static void LogError(Exception ex)
        {
            int code = (ex is HttpException) ? (ex as HttpException).GetHttpCode() : 500;

            if (code != 404)
            {
                string messageLog = string.Format(@"-----------------------------------------------
Date: {0}
Exception: {1}
Url: {2}
Refferer: {3}
-----------------------------------------------", DateTime.UtcNow.ToString("u"), ex, HttpContext.Current.Request.RawUrl, HttpContext.Current.Request.UrlReferrer);

                lock (Sync)
                {
                    File.WriteAllText( GetLogPath(), messageLog);
                }
            }
        }

        private static string GetLogPath()
        {
            const string logsDirectory = "~/App_Data/Logs/";
            DirectoryInfo dInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(logsDirectory));
            if (!dInfo.Exists)
            {
                dInfo.Create();
            }

            string fileName = "FujiyBlog" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".log";

            return Path.Combine(dInfo.FullName, fileName);
        }
    }
}