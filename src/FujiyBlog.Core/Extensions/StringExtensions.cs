using System.Text;
using System.Text.RegularExpressions;

namespace FujiyBlog.Core.Extensions
{
    public static class StringExtensions
    {
        public static string GenerateSlug(this string text)
        {
            text = RemoveAccent(text).ToLower();

            text = Regex.Replace(text, @"[^a-z0-9\s-]", ""); // invalid chars           
            text = Regex.Replace(text, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            text = text.Substring(0, text.Length <= 200 ? text.Length : 200).Trim(); // cut and trim it   
            text = Regex.Replace(text, @"\s", "-"); // hyphens   

            return text;
        }

        private static string RemoveAccent(string txt)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
