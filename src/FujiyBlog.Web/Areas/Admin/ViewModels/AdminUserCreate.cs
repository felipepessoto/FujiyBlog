using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminUserCreate
    {
        public AdminUserCreate()
        {
            CreationDate = DateTime.UtcNow;
        }

        [Required, StringLength(20)]
        public string Username { get; set; }

        [Required, StringLength(255), RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required, StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [StringLength(20)]
        public string DisplayName { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(20)]
        public string Location { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string About { get; set; }

        public DateTime CreationDate { get; set; }

        public bool Enabled { get; set; }
    }
}