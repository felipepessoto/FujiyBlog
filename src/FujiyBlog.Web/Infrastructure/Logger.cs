using System;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace FujiyBlog.Web.Infrastructure
{
    internal static class Logger
    {
        public static void LogError(Exception ex)
        {
            var code = (ex is HttpException) ? (ex as HttpException).GetHttpCode() : 500;

            if (code != 404)
            {
                CheckRollingTrace();
                Trace.WriteLine("-----------------------------------------------");
                Trace.WriteLine("Date: " + DateTime.UtcNow.ToString("u"));
                Trace.WriteLine(ex.ToString());
                Trace.WriteLine("-----------------------------------------------");
                Trace.WriteLine(string.Empty);
                Trace.Flush();
            }
        }

        public static void RegisterTrace()
        {
            Trace.Listeners.Clear();
            System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("~/App_Data/Logs/"));
            if (!dInfo.Exists)
            {
                dInfo.Create();
            }
            lastTraceLogFile = HttpContext.Current.Server.MapPath("~/App_Data/Logs/FujiyBlog" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".log");
            Trace.Listeners.Add(new TextWriterTraceListener(lastTraceLogFile));
        }

        private static string lastTraceLogFile;
        private static void CheckRollingTrace()
        {
            string logPath = HttpContext.Current.Server.MapPath("~/App_Data/Logs/FujiyBlog" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".log");

            if (lastTraceLogFile != logPath)
            {
                foreach (TraceListener traceListener in Trace.Listeners.OfType<TraceListener>().ToArray())
                {
                    Trace.Listeners.Remove(traceListener);
                    traceListener.Dispose();
                }
                lastTraceLogFile = logPath;
                Trace.Listeners.Add(new TextWriterTraceListener(logPath));
            }
        }
    }
}