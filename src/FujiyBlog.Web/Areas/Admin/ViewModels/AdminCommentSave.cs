﻿using System.ComponentModel.DataAnnotations;

namespace FujiyBlog.Web.Areas.Admin.ViewModels
{
    public class AdminCommentSave
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string AuthorName { get; set; }

        [StringLength(255), EmailAddress]
        public string AuthorEmail { get; set; }

        [StringLength(200)]
        public string AuthorWebsite { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}