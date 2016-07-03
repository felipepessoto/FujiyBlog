using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;
using System.Text;

namespace FujiyBlog.Web.Extensions
{
    public static class HtmlExtensions
    {
        //public static MvcHtmlString FieldIdFor<TModel, TValue>(this HtmlHelper<TModel> html,
        //    Expression<Func<TModel, TValue>> expression)
        //{
        //    string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
        //    string inputFieldId = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName);
        //    return MvcHtmlString.Create(inputFieldId);
        //}

        public static HtmlString Gravatar(this IHtmlHelper url, string emailAddress, int size, string defaultIcon)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return new HtmlString(string.Empty);
            }

            const string baseUrl = "<img src=\"http://www.gravatar.com/avatar/{0}?s={1}&d={2}\">";
            return new HtmlString(string.Format(baseUrl, MD5Hash(emailAddress.ToLower()), size, defaultIcon));
        }

        private static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            byte[] bytes;
            using (var md5Provider = MD5.Create())
            {
                bytes = md5Provider.ComputeHash(new UTF8Encoding().GetBytes(input));
            }
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    } 
}