using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FujiyBlog.Core.DomainObjects
{
    public class PostComment
    {
        public int Id { get; set; }
        public virtual Post Post { get; set; }
        public virtual PostComment ParentComment { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual User Author { get; set; }

        public virtual string AuthorName { get; set; }
        public virtual string AuthorEmail { get; set; }
        public virtual string AuthorWebsite { get; set; }

        public virtual string Comment { get; set; }
        public virtual string Ip { get; set; }
        public virtual bool IsApproved { get; set; }
        public virtual User ModeratedBy { get; set; }
        public virtual string Avatar { get; set; }
        public virtual bool IsSpam { get; set; }
        public virtual bool IsDeleted { get; set; }
    }
}
