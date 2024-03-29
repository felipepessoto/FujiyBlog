﻿using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Core.DomainObjects
{
    public class PostComment : IValidatableObject
    {
        public PostComment()
        {
            NestedComments = new List<PostComment>();
        }

        public int Id { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resources.Names))]
        [StringLength(50)]
        public string? AuthorName { get; set; }

        [Display(Name ="Email")]
        [StringLength(255), EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string? AuthorEmail { get; set; }

        [Display(Name = "Site")]
        [StringLength(200), Url]
        [DataType(DataType.Url)]
        public string? AuthorWebsite { get; set; }

        [Display(Name = "Comment", ResourceType = typeof(Resources.Names))]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [StringLength(45)]
        public string IpAddress { get; set; }

        [StringLength(200)]
        [DataType(DataType.ImageUrl)]
        public string? Avatar { get; set; }

        [Display(Name = "CreationDate", ResourceType = typeof(Resources.Names))]
        public DateTime CreationDate { get; set; }

        [Display(Name="Approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; }

        [Required]
        public Post Post { get; set; }

        public PostComment? ParentComment { get; set; }

        public ICollection<PostComment> NestedComments { get; set; }

        public ApplicationUser? Author { get; set; }

        public ApplicationUser? ModeratedBy { get; set; }


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
