using System.Text;
using System.Text.RegularExpressions;

namespace FujiyBlog.Core.Extensions
{
    public static class StringExtensions
    {
        public static string GenerateSlug(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            text = RemoveAccent(text).ToLower();

            text = Regex.Replace(text, @"[^a-z0-9\s-]", ""); // invalid chars           
            text = Regex.Replace(text, @"[\s-]+", "-").Trim(); // convert multiple spaces or hyphens into one hyphen   
            //text = Regex.Replace(text, @"\s", "-"); // hyphens
            text = text.Substring(0, text.Length <= 200 ? text.Length : 200).Trim(); // cut and trim it   
            

            return text;
        }

        private static string RemoveAccent(string txt)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }

        private static readonly Regex RegexStripHtml = new Regex("<[^>]*>", RegexOptions.Compiled);
        public static string StripHtml(this string html)
        {
            return string.IsNullOrWhiteSpace(html) ? string.Empty : RegexStripHtml.Replace(html, string.Empty).Trim();
        }
    }
}
