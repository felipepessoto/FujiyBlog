using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Validation;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminUserSave
    {
        public int Id { get; set; }

        [Required, StringLength(20), RegularExpression(User.UsernameRegex)]
        public string Username { get; set; }

        [Required, StringLength(255), EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

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

        [Display(Name = "Role Groups")]
        public IEnumerable<int> SelectedRoleGroups { get; set; }

        public IEnumerable<RoleGroup> AllRoleGroups { get; set; }
    }
}