using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Services
{
    public class PostService
    {
        private readonly IPostRepository postRepository;
        private readonly IPostCommentRepository postCommentRepository;

        public PostService(IPostRepository postRepository, IPostCommentRepository postCommentRepository)
        {
            this.postRepository = postRepository;
            this.postCommentRepository = postCommentRepository;
        }

        public PostCommentResult AddComment(PostComment comment)
        {
            comment.CreationDate = DateTime.UtcNow;

            postCommentRepository.Add(comment);

            return new PostCommentResult(comment);
        }
    }
}
