using System.ComponentModel.DataAnnotations;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminUserCreate : AdminUserSave
    {
        [Required, StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public new void FillUser(User user)
        {
            user.Username = Username;
            user.Password = Password;
            base.FillUser(user);
        }
    }
}