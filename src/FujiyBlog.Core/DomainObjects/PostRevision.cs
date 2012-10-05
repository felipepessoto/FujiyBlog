//using System;
//using System.ComponentModel.DataAnnotations;

//namespace FujiyBlog.Core.DomainObjects
//{
//    public class PostRevision
//    {
//        public int Id { get; set; }

//        public int RevisionNumber { get; set; }

//        [Required]
//        public Post Post { get; set; }

//        [Required, StringLength(200)]
//        public string Title { get; set; }

//        [StringLength(500)]
//        public string Description { get; set; }

//        [Required, StringLength(200)]
//        public string Slug { get; set; }

//        public string Content { get; set; }

//        [StringLength(255)]
//        public string ImageUrl { get; set; }

//        public DateTime CreationDate { get; set; }

//        [Display(Name = "Publication Date")]
//        public DateTime PublicationDate { get; set; }

//        [Display(Name = "Published")]
//        public bool IsPublished { get; set; }

//        [Display(Name = "Comments Enabled")]
//        public bool IsCommentEnabled { get; set; }

//        [Required]
//        public User Author { get; set; }

//        public string TagsIds { get; set; }

//        public string CategoriesIds { get; set; }
//    }
//}
