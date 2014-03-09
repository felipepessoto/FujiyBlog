using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FujiyBlog.Core.Validation;

namespace FujiyBlog.Web.ViewModels
{
    public class ContactForm
    {
        [Display(Name = "Name", ResourceType = typeof(Resources.Labels))]
        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required, StringLength(255), DataType(DataType.EmailAddress), EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Subject", ResourceType = typeof(Resources.Labels))]
        [Required, StringLength(50)]
        public string Subject { get; set; }

        [Display(Name = "Message", ResourceType = typeof(Resources.Labels))]
        [Required, StringLength(5000), AllowHtml, DataType(DataType.MultilineText)]
        public string Body { get; set; }
    }
}