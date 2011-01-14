using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FujiyBlog.Core.DomainObjects
{
    public class User
    {
        public virtual int Id { get; set; }
        public virtual string Login { get; set; }
        public virtual string Password { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual DateTime LastLoginDate { get; set; }
        public virtual UserProfile Profile { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
