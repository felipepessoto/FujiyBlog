namespace FujiyBlog.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v05 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 500),
                        Slug = c.String(nullable: false, maxLength: 200),
                        Content = c.String(),
                        ImageUrl = c.String(maxLength: 255),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        PublicationDate = c.DateTime(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                        IsCommentEnabled = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Author_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Author_Id)
                .Index(t => t.Author_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 20),
                        Email = c.String(nullable: false, maxLength: 255),
                        Password = c.String(nullable: false, maxLength: 50),
                        DisplayName = c.String(maxLength: 20),
                        FullName = c.String(maxLength: 100),
                        Location = c.String(maxLength: 20),
                        CreationDate = c.DateTime(nullable: false),
                        LastLoginDate = c.DateTime(),
                        About = c.String(maxLength: 500),
                        BirthDate = c.DateTime(),
                        Enabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PostComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthorName = c.String(maxLength: 50),
                        AuthorEmail = c.String(maxLength: 255),
                        AuthorWebsite = c.String(maxLength: 200),
                        Comment = c.String(nullable: false),
                        IpAddress = c.String(nullable: false, maxLength: 45),
                        Avatar = c.String(maxLength: 200),
                        CreationDate = c.DateTime(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        ParentComment_Id = c.Int(),
                        Post_Id = c.Int(nullable: false),
                        Author_Id = c.Int(),
                        ModeratedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PostComments", t => t.ParentComment_Id)
                .ForeignKey("dbo.Posts", t => t.Post_Id)
                .ForeignKey("dbo.Users", t => t.Author_Id)
                .ForeignKey("dbo.Users", t => t.ModeratedBy_Id)
                .Index(t => t.ParentComment_Id)
                .Index(t => t.Post_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.ModeratedBy_Id);
            
            CreateTable(
                "dbo.RoleGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        AssignedRoles = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 500),
                        Slug = c.String(nullable: false, maxLength: 200),
                        Content = c.String(),
                        Keywords = c.String(maxLength: 500),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        PublicationDate = c.DateTime(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                        IsFrontPage = c.Boolean(nullable: false),
                        ParentId = c.Int(),
                        ShowInList = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Author_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Author_Id)
                .ForeignKey("dbo.Pages", t => t.ParentId)
                .Index(t => t.ParentId)
                .Index(t => t.Author_Id);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 500),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WidgetSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        WidgetZone = c.String(nullable: false, maxLength: 50),
                        Position = c.Int(nullable: false),
                        Settings = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoleGroupUsers",
                c => new
                    {
                        RoleGroup_Id = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleGroup_Id, t.User_Id })
                .ForeignKey("dbo.RoleGroups", t => t.RoleGroup_Id, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.RoleGroup_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.PostCategories",
                c => new
                    {
                        Post_Id = c.Int(nullable: false),
                        Category_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Post_Id, t.Category_Id })
                .ForeignKey("dbo.Posts", t => t.Post_Id, cascadeDelete: true)
                .ForeignKey("dbo.Categories", t => t.Category_Id, cascadeDelete: true)
                .Index(t => t.Post_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.TagPosts",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Post_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Post_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Posts", t => t.Post_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Post_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pages", "ParentId", "dbo.Pages");
            DropForeignKey("dbo.Pages", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.TagPosts", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.TagPosts", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.PostCategories", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.PostCategories", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.Posts", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.RoleGroupUsers", "User_Id", "dbo.Users");
            DropForeignKey("dbo.RoleGroupUsers", "RoleGroup_Id", "dbo.RoleGroups");
            DropForeignKey("dbo.PostComments", "ModeratedBy_Id", "dbo.Users");
            DropForeignKey("dbo.PostComments", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.PostComments", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.PostComments", "ParentComment_Id", "dbo.PostComments");
            DropIndex("dbo.TagPosts", new[] { "Post_Id" });
            DropIndex("dbo.TagPosts", new[] { "Tag_Id" });
            DropIndex("dbo.PostCategories", new[] { "Category_Id" });
            DropIndex("dbo.PostCategories", new[] { "Post_Id" });
            DropIndex("dbo.RoleGroupUsers", new[] { "User_Id" });
            DropIndex("dbo.RoleGroupUsers", new[] { "RoleGroup_Id" });
            DropIndex("dbo.Pages", new[] { "Author_Id" });
            DropIndex("dbo.Pages", new[] { "ParentId" });
            DropIndex("dbo.PostComments", new[] { "ModeratedBy_Id" });
            DropIndex("dbo.PostComments", new[] { "Author_Id" });
            DropIndex("dbo.PostComments", new[] { "Post_Id" });
            DropIndex("dbo.PostComments", new[] { "ParentComment_Id" });
            DropIndex("dbo.Posts", new[] { "Author_Id" });
            DropTable("dbo.TagPosts");
            DropTable("dbo.PostCategories");
            DropTable("dbo.RoleGroupUsers");
            DropTable("dbo.WidgetSettings");
            DropTable("dbo.Settings");
            DropTable("dbo.Pages");
            DropTable("dbo.Tags");
            DropTable("dbo.RoleGroups");
            DropTable("dbo.PostComments");
            DropTable("dbo.Users");
            DropTable("dbo.Posts");
            DropTable("dbo.Categories");
        }
    }
}
