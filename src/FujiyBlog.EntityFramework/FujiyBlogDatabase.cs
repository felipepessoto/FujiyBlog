﻿using System.Data.Entity;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.EntityFramework.Configuration;
using FujiyBlog.Core.Infrastructure;

namespace FujiyBlog.EntityFramework
{
    public class FujiyBlogDatabase : DbContext, IUnitOfWork
    {
        public FujiyBlogDatabase()
        {
            Configuration.LazyLoadingEnabled = false;
            //Database.SetInitializer<FujiyBlogDatabase>(null);
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Configurations.Add(new PostCommentConfiguration());
            builder.Configurations.Add(new PostConfiguration());
            builder.Configurations.Add(new UserConfiguration());
            builder.Configurations.Add(new SettingConfiguration());
            builder.Configurations.Add(new TagConfiguration());
            builder.Configurations.Add(new CategoryConfiguration());
            builder.Configurations.Add(new WidgetSettingConfiguration());
        }

        //public string Script()
        //{
        //    return ((System.Data.Entity.Infrastructure.IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        //}

        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<WidgetSetting> WidgetSettings { get; set; }

        void IUnitOfWork.SaveChanges()
        {
            SaveChanges();
        } 
    }
}
