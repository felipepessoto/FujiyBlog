using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminUserSave
    {
        public AdminUserSave()
        {
            SelectedRoleGroups = Enumerable.Empty<int>();
        }

         public AdminUserSave(User user)
         {
             Id = user.Id;
             Username = user.Username;
             Email = user.Email;
             DisplayName = user.DisplayName;
             FullName = user.FullName;
             BirthDate = user.BirthDate;
             Location = user.Location;
             About = user.About;
             SelectedRoleGroups = user.RoleGroups.Select(x => x.Id);
         }

        public int Id { get; set; }

        [Required, StringLength(20), RegularExpression(User.UsernameRegex)]
        public string Username { get; set; }

        [Required, StringLength(255), EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(20)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        [StringLength(20)]
        public string Location { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string About { get; set; }

        [Display(Name = "Role Groups")]
        public IEnumerable<int> SelectedRoleGroups { get; set; }

        public IEnumerable<RoleGroup> AllRoleGroups { get; set; }

        public void FillUser(User user)
        {
            user.Id = Id;
            user.Email = Email;
            user.DisplayName = DisplayName;
            user.FullName = FullName;
            user.BirthDate = BirthDate;
            user.Location = Location;
            user.About = About;
        }
    }
}