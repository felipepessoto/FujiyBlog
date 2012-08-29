namespace FujiyBlog.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostRevision : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PostRevisions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RevisionNumber = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 500),
                        Slug = c.String(nullable: false, maxLength: 200),
                        Content = c.String(),
                        ImageUrl = c.String(maxLength: 255),
                        CreationDate = c.DateTime(nullable: false),
                        PublicationDate = c.DateTime(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                        IsCommentEnabled = c.Boolean(nullable: false),
                        TagsIds = c.String(),
                        CategoriesIds = c.String(),
                        Post_Id = c.Int(nullable: false),
                        Author_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.Post_Id)
                .ForeignKey("dbo.Users", t => t.Author_Id)
                .Index(t => t.Post_Id)
                .Index(t => t.Author_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.PostRevisions", new[] { "Author_Id" });
            DropIndex("dbo.PostRevisions", new[] { "Post_Id" });
            DropForeignKey("dbo.PostRevisions", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.PostRevisions", "Post_Id", "dbo.Posts");
            DropTable("dbo.PostRevisions");
        }
    }
}
