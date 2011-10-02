using System;
using System.Web;

namespace FujiyBlog.Web.Extensions
{
    public static class HttpResponseExtensions
    {
        public static void SendToUnauthorized(this HttpResponse response)
        {
            response.StatusCode = 401;
            response.WriteFile("~/errors/401.htm");
            response.End();
            throw new Exception("The user has no permission");
        }

        public static void SendToUnauthorized(this HttpResponseBase response)
        {
            response.StatusCode = 401;
            response.WriteFile("~/errors/401.htm");
            response.End();
            throw new Exception("The user has no permission");
        }
    }
}