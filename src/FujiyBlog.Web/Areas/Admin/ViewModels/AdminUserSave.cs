using FujiyBlog.Core.DomainObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminUserSave
    {
        public AdminUserSave()
        {
            SelectedRoles =new List<string>(Enumerable.Empty<string>());
        }

         public AdminUserSave(ApplicationUser user)
         {
             Id = user.Id;
             Username = user.UserName;
             Email = user.Email;
             DisplayName = user.DisplayName;
             FullName = user.FullName;
             BirthDate = user.BirthDate;
             Location = user.Location;
             About = user.About;
             SelectedRoles = user.Roles.Select(x => x.RoleId).ToList();
         }

        public string Id { get; set; }

        [Required]
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

        [Display(Name = "Roles")]
        public List<string> SelectedRoles { get; set; }

        public IEnumerable<IdentityRole> AllRoles { get; set; }
    }
}