using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class User
    {
        public User()
        {
            Posts = new List<Post>();
        }

        public int Id { get; set; }
        
        [Required, StringLength(20)]
        public string Username { get; set; }

        [Required, StringLength(255), RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?")]
        public string Email { get; set; }

        [Required, StringLength(50)]
        public string Password { get; set; }

        [StringLength(20)]
        public string DisplayName { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string Location { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        [StringLength(500)]
        public string About { get; set; }

        public DateTime? BirthDate { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public virtual ICollection<PostComment> AuthoredPostComments { get; set; }

        public virtual ICollection<PostComment> ModeratedPostComments { get; set; }
    }
}
