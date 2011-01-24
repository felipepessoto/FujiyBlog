using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using FujiyBlog.Core.Repositories;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.EntityFramework
{
    public class PostCommentRepository : RepositoryBase<PostComment>, IPostCommentRepository
    {
        public PostCommentRepository(FujiyBlogDatabase database)
            : base(database)
        {
        }
    }
}
