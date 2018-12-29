using FujiyBlog.Core.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FujiyBlog.Web.Infrastructure
{
    public class UploadServiceConfiguredConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var settings = httpContext.RequestServices.GetRequiredService<SettingRepository>();

            if (string.IsNullOrEmpty(settings.AzureStorageAccountName) || string.IsNullOrEmpty(settings.AzureStorageUploadContainerName))
            {
                return false;
            }

            return true;
        }
    }
}
