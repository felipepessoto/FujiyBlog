namespace FujiyBlog.Core.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class v03 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "PostComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AuthorName = c.String(maxLength: 50, unicode: false),
                        AuthorEmail = c.String(maxLength: 255, unicode: false),
                        AuthorWebsite = c.String(maxLength: 200, unicode: false),
                        Comment = c.String(nullable: false, unicode: false),
                        IpAddress = c.String(nullable: false, maxLength: 45, unicode: false),
                        Avatar = c.String(maxLength: 200, unicode: false),
                        CreationDate = c.DateTime(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Author_Id = c.Int(),
                        ModeratedBy_Id = c.Int(),
                        Post_Id = c.Int(nullable: false),
                        ParentComment_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Users", t => t.Author_Id)
                .ForeignKey("Users", t => t.ModeratedBy_Id)
                .ForeignKey("Posts", t => t.Post_Id)
                .ForeignKey("PostComments", t => t.ParentComment_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.ModeratedBy_Id)
                .Index(t => t.Post_Id)
                .Index(t => t.ParentComment_Id);
            
            CreateTable(
                "Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 200, unicode: false),
                        Description = c.String(maxLength: 500, unicode: false),
                        Slug = c.String(nullable: false, maxLength: 200, unicode: false),
                        Content = c.String(unicode: false),
                        ImageUrl = c.String(maxLength: 255, unicode: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                        PublicationDate = c.DateTime(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                        IsCommentEnabled = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Author_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Users", t => t.Author_Id)
                .Index(t => t.Author_Id);
            
            CreateTable(
                "Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 20, unicode: false),
                        Email = c.String(nullable: false, maxLength: 255, unicode: false),
                        Password = c.String(nullable: false, maxLength: 50, unicode: false),
                        DisplayName = c.String(maxLength: 20, unicode: false),
                        FullName = c.String(maxLength: 100, unicode: false),
                        Location = c.String(maxLength: 20, unicode: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastLoginDate = c.DateTime(),
                        About = c.String(maxLength: 500, unicode: false),
                        BirthDate = c.DateTime(),
                        Enabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "RoleGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        AssignedRoles = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Settings",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 500, unicode: false),
                        Value = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "WidgetSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        WidgetZone = c.String(nullable: false, maxLength: 50, unicode: false),
                        Position = c.Int(nullable: false),
                        Settings = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 200, unicode: false),
                        Description = c.String(maxLength: 500, unicode: false),
                        Slug = c.String(nullable: false, maxLength: 200, unicode: false),
                        Content = c.String(unicode: false),
                        Keywords = c.String(maxLength: 500, unicode: false),
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
                .ForeignKey("Pages", t => t.ParentId)
                .ForeignKey("Users", t => t.Author_Id)
                .Index(t => t.ParentId)
                .Index(t => t.Author_Id);
            
            CreateTable(
                "RoleGroupUsers",
                c => new
                    {
                        RoleGroup_Id = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleGroup_Id, t.User_Id })
                .ForeignKey("RoleGroups", t => t.RoleGroup_Id, cascadeDelete: true)
                .ForeignKey("Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.RoleGroup_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "TagPosts",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Post_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Post_Id })
                .ForeignKey("Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("Posts", t => t.Post_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Post_Id);
            
            CreateTable(
                "CategoryPosts",
                c => new
                    {
                        Category_Id = c.Int(nullable: false),
                        Post_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Category_Id, t.Post_Id })
                .ForeignKey("Categories", t => t.Category_Id, cascadeDelete: true)
                .ForeignKey("Posts", t => t.Post_Id, cascadeDelete: true)
                .Index(t => t.Category_Id)
                .Index(t => t.Post_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("CategoryPosts", new[] { "Post_Id" });
            DropIndex("CategoryPosts", new[] { "Category_Id" });
            DropIndex("TagPosts", new[] { "Post_Id" });
            DropIndex("TagPosts", new[] { "Tag_Id" });
            DropIndex("RoleGroupUsers", new[] { "User_Id" });
            DropIndex("RoleGroupUsers", new[] { "RoleGroup_Id" });
            DropIndex("Pages", new[] { "Author_Id" });
            DropIndex("Pages", new[] { "ParentId" });
            DropIndex("Posts", new[] { "Author_Id" });
            DropIndex("PostComments", new[] { "ParentComment_Id" });
            DropIndex("PostComments", new[] { "Post_Id" });
            DropIndex("PostComments", new[] { "ModeratedBy_Id" });
            DropIndex("PostComments", new[] { "Author_Id" });
            DropForeignKey("CategoryPosts", "Post_Id", "Posts");
            DropForeignKey("CategoryPosts", "Category_Id", "Categories");
            DropForeignKey("TagPosts", "Post_Id", "Posts");
            DropForeignKey("TagPosts", "Tag_Id", "Tags");
            DropForeignKey("RoleGroupUsers", "User_Id", "Users");
            DropForeignKey("RoleGroupUsers", "RoleGroup_Id", "RoleGroups");
            DropForeignKey("Pages", "Author_Id", "Users");
            DropForeignKey("Pages", "ParentId", "Pages");
            DropForeignKey("Posts", "Author_Id", "Users");
            DropForeignKey("PostComments", "ParentComment_Id", "PostComments");
            DropForeignKey("PostComments", "Post_Id", "Posts");
            DropForeignKey("PostComments", "ModeratedBy_Id", "Users");
            DropForeignKey("PostComments", "Author_Id", "Users");
            DropTable("CategoryPosts");
            DropTable("TagPosts");
            DropTable("RoleGroupUsers");
            DropTable("Pages");
            DropTable("WidgetSettings");
            DropTable("Settings");
            DropTable("Categories");
            DropTable("Tags");
            DropTable("RoleGroups");
            DropTable("Users");
            DropTable("Posts");
            DropTable("PostComments");
        }
    }
}
