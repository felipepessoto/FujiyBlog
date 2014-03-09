namespace FujiyBlog.Core.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UnicodeSupport : DbMigration
    {
        public override void Up()
        {
            AlterColumn("PostComments", "AuthorName", c => c.String(maxLength: 50));
            AlterColumn("PostComments", "AuthorEmail", c => c.String(maxLength: 255));
            AlterColumn("PostComments", "AuthorWebsite", c => c.String(maxLength: 200));
            AlterColumn("PostComments", "Comment", c => c.String(nullable: false));
            AlterColumn("PostComments", "IpAddress", c => c.String(nullable: false, maxLength: 45));
            AlterColumn("PostComments", "Avatar", c => c.String(maxLength: 200));
            AlterColumn("Posts", "Title", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("Posts", "Description", c => c.String(maxLength: 500));
            AlterColumn("Posts", "Slug", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("Posts", "Content", c => c.String());
            AlterColumn("Posts", "ImageUrl", c => c.String(maxLength: 255));
            AlterColumn("Users", "Username", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("Users", "Email", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("Users", "Password", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("Users", "DisplayName", c => c.String(maxLength: 20));
            AlterColumn("Users", "FullName", c => c.String(maxLength: 100));
            AlterColumn("Users", "Location", c => c.String(maxLength: 20));
            AlterColumn("Users", "About", c => c.String(maxLength: 500));
            AlterColumn("RoleGroups", "Name", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("RoleGroups", "AssignedRoles", c => c.String());
            AlterColumn("Tags", "Name", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("Categories", "Name", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("Settings", "Description", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("Settings", "Value", c => c.String());
            AlterColumn("WidgetSettings", "Name", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("WidgetSettings", "WidgetZone", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("WidgetSettings", "Settings", c => c.String());
            AlterColumn("Pages", "Title", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("Pages", "Description", c => c.String(maxLength: 500));
            AlterColumn("Pages", "Slug", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("Pages", "Content", c => c.String());
            AlterColumn("Pages", "Keywords", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("Pages", "Keywords", c => c.String(maxLength: 500, unicode: false));
            AlterColumn("Pages", "Content", c => c.String(unicode: false));
            AlterColumn("Pages", "Slug", c => c.String(nullable: false, maxLength: 200, unicode: false));
            AlterColumn("Pages", "Description", c => c.String(maxLength: 500, unicode: false));
            AlterColumn("Pages", "Title", c => c.String(nullable: false, maxLength: 200, unicode: false));
            AlterColumn("WidgetSettings", "Settings", c => c.String(unicode: false));
            AlterColumn("WidgetSettings", "WidgetZone", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AlterColumn("WidgetSettings", "Name", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AlterColumn("Settings", "Value", c => c.String(unicode: false));
            AlterColumn("Settings", "Description", c => c.String(nullable: false, maxLength: 500, unicode: false));
            AlterColumn("Categories", "Name", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AlterColumn("Tags", "Name", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AlterColumn("RoleGroups", "AssignedRoles", c => c.String(unicode: false));
            AlterColumn("RoleGroups", "Name", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AlterColumn("Users", "About", c => c.String(maxLength: 500, unicode: false));
            AlterColumn("Users", "Location", c => c.String(maxLength: 20, unicode: false));
            AlterColumn("Users", "FullName", c => c.String(maxLength: 100, unicode: false));
            AlterColumn("Users", "DisplayName", c => c.String(maxLength: 20, unicode: false));
            AlterColumn("Users", "Password", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AlterColumn("Users", "Email", c => c.String(nullable: false, maxLength: 255, unicode: false));
            AlterColumn("Users", "Username", c => c.String(nullable: false, maxLength: 20, unicode: false));
            AlterColumn("Posts", "ImageUrl", c => c.String(maxLength: 255, unicode: false));
            AlterColumn("Posts", "Content", c => c.String(unicode: false));
            AlterColumn("Posts", "Slug", c => c.String(nullable: false, maxLength: 200, unicode: false));
            AlterColumn("Posts", "Description", c => c.String(maxLength: 500, unicode: false));
            AlterColumn("Posts", "Title", c => c.String(nullable: false, maxLength: 200, unicode: false));
            AlterColumn("PostComments", "Avatar", c => c.String(maxLength: 200, unicode: false));
            AlterColumn("PostComments", "IpAddress", c => c.String(nullable: false, maxLength: 45, unicode: false));
            AlterColumn("PostComments", "Comment", c => c.String(nullable: false, unicode: false));
            AlterColumn("PostComments", "AuthorWebsite", c => c.String(maxLength: 200, unicode: false));
            AlterColumn("PostComments", "AuthorEmail", c => c.String(maxLength: 255, unicode: false));
            AlterColumn("PostComments", "AuthorName", c => c.String(maxLength: 50, unicode: false));
        }
    }
}
