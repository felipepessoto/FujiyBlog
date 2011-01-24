using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Services
{
    public class PostCommentResult : ServiceResultBase
    {
        public PostComment Comment
        {
            get;
            private set;
        }

        public PostCommentResult()
            : this(new ValidationResult[0])
        {
        }

        public PostCommentResult(PostComment comment)
            : this()
        {
            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }

            Comment = comment;
        }

        public PostCommentResult(IEnumerable<ValidationResult> validationResults)
            : base(validationResults)
        {
        }
    }
}
