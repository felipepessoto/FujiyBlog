using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FujiyBlog.Core.DomainObjects
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Posts = new List<Post>();
            AuthoredPostComments = new List<PostComment>();
            ModeratedPostComments = new List<PostComment>();
        }

        [StringLength(20)]
        public string DisplayName { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string Location { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string About { get; set; }

        public DateTime? BirthDate { get; set; }

        public bool Enabled { get; set; }

        public ICollection<Post> Posts { get; set; }

        public ICollection<PostComment> AuthoredPostComments { get; set; }

        public ICollection<PostComment> ModeratedPostComments { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();

    }
}
