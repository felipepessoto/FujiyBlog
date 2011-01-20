﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.ViewModel;

namespace FujiyBlog.Web.Controllers
{
    public partial class BlogController : Controller
    {
        private readonly IPostRepository postRepository;

        public BlogController(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public virtual ActionResult Index(int? skip)
        {
            IEnumerable<PostDetails> recentPosts = postRepository.GetRecentPosts(skip.GetValueOrDefault(), 10);

            return View(recentPosts);
        }
    }
}
