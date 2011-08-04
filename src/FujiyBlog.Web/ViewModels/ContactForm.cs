using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Web.ViewModels
{
    public class ContactForm
    {
        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required, StringLength(255), DataType(DataType.EmailAddress), RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?")]
        public string Email { get; set; }

        [Required, StringLength(50)]
        public string Subject { get; set; }

        [Required, StringLength(5000), DataType(DataType.MultilineText)]
        public string Body { get; set; }
    }
}