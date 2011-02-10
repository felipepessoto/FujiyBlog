﻿using System.Collections.Generic;
using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Repositories
{
    public interface IPostRepository : IRepository<Post>
    {
        IEnumerable<Post> GetRecentPosts(bool isPublic, int skip, int take);
        int GetTotal(bool isPublic);
        Post GetPost(string slug);
        Post GetPost(int id);
    }
}
