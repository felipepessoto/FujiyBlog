using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class User
    {
        public virtual int Id { get; set; }
        
        [Required, StringLength(20)]
        public virtual string Login { get; set; }

        [Required, StringLength(255), RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?")]
        public virtual string Email { get; set; }

        [Required, StringLength(50)]
        public virtual string Password { get; set; }

        [StringLength(20)]
        public virtual string DisplayName { get; set; }

        [StringLength(100)]
        public virtual string FullName { get; set; }

        [Required, StringLength(20)]
        public virtual string Location { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual DateTime LastLoginDate { get; set; }

        [StringLength(500)]
        public virtual string About { get; set; }

        public virtual DateTime BirthDate { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
