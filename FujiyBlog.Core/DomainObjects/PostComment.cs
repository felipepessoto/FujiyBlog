using System;
using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class PostComment
    {
        public virtual int Id { get; set; }

        [Required, StringLength(50)]
        public virtual string AuthorName { get; set; }

        [StringLength(255), RegularExpression(@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
+ @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
+ @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$")]
        public virtual string AuthorEmail { get; set; }

        [StringLength(200)]
        public virtual string AuthorWebsite { get; set; }

        [Required]
        public virtual string Comment { get; set; }

        [Required, StringLength(45)]
        public virtual string IpAddress { get; set; }

        [StringLength(200)]
        public virtual string Avatar { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual bool IsApproved { get; set; }

        public virtual bool IsSpam { get; set; }

        public virtual bool IsDeleted { get; set; }

        [Required]
        public virtual Post Post { get; set; }

        public virtual PostComment ParentComment { get; set; }

        public virtual User Author { get; set; }

        public virtual User ModeratedBy { get; set; }
    }
}
