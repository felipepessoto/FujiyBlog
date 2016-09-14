using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using FujiyBlog.Core.EntityFramework;

namespace FujiyBlog.Core.Migrations
{
    [DbContext(typeof(FujiyBlogDatabase))]
    [Migration("20160914192431_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("About")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<int>("AccessFailedCount");

                    b.Property<DateTime?>("BirthDate");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("DisplayName")
                        .HasAnnotation("MaxLength", 20);

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("Enabled");

                    b.Property<string>("FullName")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<DateTime?>("LastLoginDate");

                    b.Property<string>("Location")
                        .HasAnnotation("MaxLength", 20);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.Page", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId")
                        .IsRequired();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsFrontPage");

                    b.Property<bool>("IsPublished");

                    b.Property<string>("Keywords")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<DateTime>("LastModificationDate");

                    b.Property<int?>("ParentId");

                    b.Property<DateTime>("PublicationDate");

                    b.Property<bool>("ShowInList");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ParentId");

                    b.ToTable("Pages");
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId")
                        .IsRequired();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("ImageUrl")
                        .HasAnnotation("MaxLength", 255);

                    b.Property<bool>("IsCommentEnabled");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsPublished");

                    b.Property<DateTime>("LastModificationDate");

                    b.Property<DateTime>("PublicationDate");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.PostCategory", b =>
                {
                    b.Property<int>("PostId");

                    b.Property<int>("CategoryId");

                    b.HasKey("PostId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("PostId");

                    b.ToTable("PostCategory");
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.PostComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorEmail")
                        .HasAnnotation("MaxLength", 255);

                    b.Property<string>("AuthorId");

                    b.Property<string>("AuthorName")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("AuthorWebsite")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("Avatar")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("Comment")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 45);

                    b.Property<bool>("IsApproved");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("ModeratedById");

                    b.Property<int?>("ParentCommentId");

                    b.Property<int?>("PostId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ModeratedById");

                    b.HasIndex("ParentCommentId");

                    b.HasIndex("PostId");

                    b.ToTable("PostComments");
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.PostTag", b =>
                {
                    b.Property<int>("PostId");

                    b.Property<int>("TagId");

                    b.HasKey("PostId", "TagId");

                    b.HasIndex("PostId");

                    b.HasIndex("TagId");

                    b.ToTable("PostTag");
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.Setting", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.WidgetSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("Position");

                    b.Property<string>("Settings");

                    b.Property<string>("WidgetZone")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("Id");

                    b.ToTable("WidgetSettings");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.Page", b =>
                {
                    b.HasOne("FujiyBlog.Core.DomainObjects.ApplicationUser", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FujiyBlog.Core.DomainObjects.Page", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.Post", b =>
                {
                    b.HasOne("FujiyBlog.Core.DomainObjects.ApplicationUser", "Author")
                        .WithMany("Posts")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.PostCategory", b =>
                {
                    b.HasOne("FujiyBlog.Core.DomainObjects.Category", "Category")
                        .WithMany("PostCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FujiyBlog.Core.DomainObjects.Post", "Post")
                        .WithMany("PostCategories")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.PostComment", b =>
                {
                    b.HasOne("FujiyBlog.Core.DomainObjects.ApplicationUser", "Author")
                        .WithMany("AuthoredPostComments")
                        .HasForeignKey("AuthorId");

                    b.HasOne("FujiyBlog.Core.DomainObjects.ApplicationUser", "ModeratedBy")
                        .WithMany("ModeratedPostComments")
                        .HasForeignKey("ModeratedById");

                    b.HasOne("FujiyBlog.Core.DomainObjects.PostComment", "ParentComment")
                        .WithMany("NestedComments")
                        .HasForeignKey("ParentCommentId");

                    b.HasOne("FujiyBlog.Core.DomainObjects.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FujiyBlog.Core.DomainObjects.PostTag", b =>
                {
                    b.HasOne("FujiyBlog.Core.DomainObjects.Post", "Post")
                        .WithMany("PostTags")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FujiyBlog.Core.DomainObjects.Tag", "Tag")
                        .WithMany("PostTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("FujiyBlog.Core.DomainObjects.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("FujiyBlog.Core.DomainObjects.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FujiyBlog.Core.DomainObjects.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
