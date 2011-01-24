using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.EntityFramework.Configuration;
using FujiyBlog.Core.Infrastructure;

namespace FujiyBlog.EntityFramework
{
    public class FujiyBlogDatabase : DbContext, IUnitOfWork
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Configurations.Add(new PostCommentConfiguration());
            builder.Configurations.Add(new PostConfiguration());
            builder.Configurations.Add(new UserConfiguration());
        }

        //public string Script()
        //{
        //    return ObjectContext.CreateDatabaseScript();
        //}

        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }

        void IUnitOfWork.SaveChanges()
        {
            SaveChanges();
        } 
    }
}
