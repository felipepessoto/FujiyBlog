using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FujiyBlog.Core.DomainObjects
{
    public class Tag
    {
        public virtual int Id { get; set; }

        [Required, StringLength(50)]
        public virtual string Name { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
