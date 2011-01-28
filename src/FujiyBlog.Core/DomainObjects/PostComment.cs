﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class PostComment : IValidatableObject
    {
        public virtual int Id { get; set; }

        [StringLength(50)]
        public virtual string AuthorName { get; set; }

        [StringLength(255), RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?")]
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


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Author == null)
            {
                if (AuthorName == null)
                {
                    yield return new ValidationResult("You should enter your name", new[] { "AuthorName" });
                }
            }
        }
    }
}