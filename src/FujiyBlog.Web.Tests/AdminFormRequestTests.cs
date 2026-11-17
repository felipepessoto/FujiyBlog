using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using FujiyBlog.Core.DomainObjects;
using Microsoft.EntityFrameworkCore;

namespace FujiyBlog.Web.Tests;

public sealed class AdminFormRequestTests(AdminApplicationFactory application) : IClassFixture<AdminApplicationFactory>
{
    [Theory]
    [InlineData("Index")]
    [InlineData("Email")]
    [InlineData("Comments")]
    [InlineData("Feed")]
    [InlineData("SocialNetworks")]
    [InlineData("Storage")]
    public async Task Settings_accept_valid_posts_with_blank_optional_fields(string action)
    {
        using var client = application.CreateAdminClient();
        using var response = await client.PostAsync("/Admin/Setting/" + action, new FormUrlEncodedContent(SettingsForm()));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }

    [Theory]
    [InlineData("Index", "PostsPerPage", "0")]
    [InlineData("Index", "TimeZoneId", "not-a-time-zone")]
    [InlineData("Email", "SmtpPort", "0")]
    [InlineData("Comments", "CommentsPerPage", "0")]
    [InlineData("Feed", "ItemsShownInFeed", "-1")]
    [InlineData("Storage", "FileUploadService", "unknown")]
    [InlineData("Storage", "FileUploadService", "AzureStorageFileUploadService")]
    public async Task Invalid_settings_render_submitted_values_without_partial_writes(string action, string field, string value)
    {
        var before = await application.WithDatabaseAsync(database => database.Settings.OrderBy(setting => setting.Id)
            .Select(setting => new { setting.Id, setting.Value }).ToListAsync());
        var form = SettingsForm();
        form[field] = value;
        using var client = application.CreateAdminClient();
        using var response = await client.PostAsync("/Admin/Setting/" + action, new FormUrlEncodedContent(form));
        var html = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("field-validation-error", html);
        if (action == "Index")
        {
            Assert.Contains("value=\"Form regression\"", html);
            Assert.Contains("value=\"UTC\"", html);
            Assert.Contains(">Default</option>", html);
        }
        if (action == "Storage")
            Assert.Contains("LocalFolderFileUploadService</option>", html);
        var after = await application.WithDatabaseAsync(database => database.Settings.OrderBy(setting => setting.Id)
            .Select(setting => new { setting.Id, setting.Value }).ToListAsync());
        Assert.Equal(before, after);
    }

    [Theory]
    [InlineData("Page")]
    [InlineData("Post")]
    public async Task Content_accepts_blank_optional_fields_and_no_categories(string controller)
    {
        var slug = "form-" + Guid.NewGuid().ToString("N");
        using var client = application.CreateAdminClient();
        using var response = await client.PostAsync("/Admin/" + controller + "/Edit",
            new FormUrlEncodedContent(ContentForm(controller, slug)));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        await application.WithDatabaseAsync(async database =>
        {
            if (controller == "Page")
            {
                var page = await database.Pages.SingleAsync(page => page.Slug == slug);
                Assert.Null(page.Description);
                Assert.Null(page.Keywords);
            }
            else
            {
                var post = await database.Posts.Include(post => post.PostCategories).Include(post => post.PostTags)
                    .SingleAsync(post => post.Slug == slug);
                Assert.Null(post.Description);
                Assert.Null(post.ImageUrl);
                Assert.Empty(post.PostCategories);
                Assert.Empty(post.PostTags);
            }
            return 0;
        });
    }

    [Theory]
    [InlineData("Page")]
    [InlineData("Post")]
    public async Task Invalid_content_preserves_inputs_and_choices_without_creating_records(string controller)
    {
        var slug = "invalid-" + Guid.NewGuid().ToString("N");
        var prefix = controller == "Post" ? "Post." : "";
        var form = ContentForm(controller, slug);
        form[prefix + "AuthorId"] = "";
        form[prefix + "PublicationDate"] = "not-a-date";
        using var client = application.CreateAdminClient();
        using var response = await client.PostAsync("/Admin/" + controller + "/Edit", new FormUrlEncodedContent(form));
        var html = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("value=\"" + slug + "\"", html);
        Assert.Contains(AdminApplicationFactory.UserId, html);
        Assert.Contains("field-validation-error", html);
        if (controller == "Post")
            Assert.Contains("Existing category", html);
        await application.WithDatabaseAsync(async database =>
        {
            Assert.False(controller == "Page"
                ? await database.Pages.AnyAsync(page => page.Slug == slug)
                : await database.Posts.AnyAsync(post => post.Slug == slug));
            return 0;
        });
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Post_categories_and_tags_are_preserved_on_validation_errors_and_saved_when_valid(bool invalid)
    {
        var categoryId = await application.WithDatabaseAsync(database => database.Categories.Select(category => category.Id).FirstAsync());
        var slug = "categories-" + Guid.NewGuid().ToString("N");
        var form = ContentForm("Post", slug);
        form["Post.SelectedCategories"] = categoryId.ToString();
        form["Post.Tags"] = "tag-" + slug;
        if (invalid)
            form["Post.Title"] = new string('x', 201);
        using var client = application.CreateAdminClient();
        using var response = await client.PostAsync("/Admin/Post/Edit", new FormUrlEncodedContent(form));
        if (invalid)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var html = await response.Content.ReadAsStringAsync();
            Assert.Matches(new Regex($"name=\"Post.selectedCategories\"[^>]*value=\"{categoryId}\"[^>]*checked"), html);
            Assert.Contains(form["Post.Tags"], html);
        }
        else
        {
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
        var savedId = await application.WithDatabaseAsync(async database =>
        {
            var post = await database.Posts.Include(post => post.PostTags).ThenInclude(link => link.Tag)
                .Include(post => post.PostCategories).SingleOrDefaultAsync(post => post.Slug == slug);
            if (invalid)
            {
                Assert.Null(post);
                Assert.False(await database.Tags.AnyAsync(tag => tag.Name == form["Post.Tags"]));
            }
            else
            {
                Assert.NotNull(post);
                Assert.Equal(categoryId, Assert.Single(post.PostCategories).CategoryId);
                Assert.Equal(form["Post.Tags"], Assert.Single(post.PostTags).Tag.Name);
            }
            return post?.Id ?? 0;
        });
        if (!invalid)
        {
            using var editResponse = await client.GetAsync("/Admin/Post/Edit/" + savedId);
            editResponse.EnsureSuccessStatusCode();
            Assert.Matches(new Regex($"name=\"Post.selectedCategories\"[^>]*value=\"{categoryId}\"[^>]*checked"),
                await editResponse.Content.ReadAsStringAsync());
        }
    }

    [Theory]
    [InlineData("Page")]
    [InlineData("Post")]
    public async Task Invalid_edits_still_require_the_existing_edit_permission(string controller)
    {
        var slug = "permission-" + Guid.NewGuid().ToString("N");
        var id = await application.WithDatabaseAsync(async database =>
        {
            var author = await database.Users.SingleAsync(user => user.Id == AdminApplicationFactory.UserId);
            if (controller == "Page")
            {
                var page = new Page { Title = "Original", Slug = slug, Author = author };
                database.Pages.Add(page);
                database.SaveChanges();
                return page.Id;
            }
            var post = new Post { Title = "Original", Slug = slug, Author = author };
            database.Posts.Add(post);
            database.SaveChanges();
            return post.Id;
        });
        var prefix = controller == "Post" ? "Post." : "";
        var form = ContentForm(controller, slug);
        form[prefix + "Id"] = id.ToString();
        form[prefix + "AuthorId"] = "";
        using var client = application.CreateAdminClient();
        client.DefaultRequestHeaders.Add("X-Test-Permissions", nameof(PermissionClaims.AccessAdminPages));
        using var response = await client.PostAsync("/Admin/" + controller + "/Edit", new FormUrlEncodedContent(form));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Public_feed_reads_persisted_content_through_the_application()
    {
        var title = "Feed regression " + Guid.NewGuid().ToString("N");
        await application.WithDatabaseAsync(async database =>
        {
            var author = await database.Users.SingleAsync(user => user.Id == AdminApplicationFactory.UserId);
            database.Posts.Add(new Post
            {
                Title = title, Slug = "feed-" + Guid.NewGuid().ToString("N"), Author = author,
                IsPublished = true, PublicationDate = DateTime.UtcNow.AddMinutes(-1), LastModificationDate = DateTime.UtcNow
            });
            database.SaveChanges();
            return 0;
        });
        using var client = application.CreateAdminClient();
        client.DefaultRequestHeaders.Remove("X-Test-User");
        using var response = await client.GetAsync("/Feed/Rss20");
        response.EnsureSuccessStatusCode();
        var feed = XDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Contains(feed.Descendants("item"), item => item.Element("title")?.Value == title);
    }

    private static Dictionary<string, string> SettingsForm() => new()
    {
        ["BlogName"] = "Form regression", ["Theme"] = "Default", ["PostsPerPage"] = "10",
        ["TimeZoneId"] = "UTC", ["Language"] = "en", ["EmailTo"] = "test@example.invalid",
        ["SmtpAddress"] = "smtp.example.invalid", ["SmtpPort"] = "25", ["CommentsPerPage"] = "10",
        ["ItemsShownInFeed"] = "10", ["FileUploadService"] = "LocalFolderFileUploadService",
        ["BlogDescription"] = "", ["ApplicationInsightsInstrumentationKey"] = "", ["CustomCode"] = "",
        ["EmailSubjectPrefix"] = "", ["SmtpUserName"] = "", ["SmtpPassword"] = "",
        ["CommentsAvatar"] = "", ["ReCaptchaPrivateKey"] = "", ["ReCaptchaPublicKey"] = "",
        ["AlternateFeedUrl"] = "", ["FacebookAdminIds"] = "", ["FacebookAppId"] = "",
        ["OpenGraphImageUrl"] = "", ["TwitterBlogAccount"] = "", ["AzureStorageAccountName"] = "",
        ["AzureStorageAccountKey"] = "", ["AzureStorageUploadContainerName"] = ""
    };

    private static Dictionary<string, string> ContentForm(string controller, string slug)
    {
        var prefix = controller == "Post" ? "Post." : "";
        return new Dictionary<string, string>
        {
            [prefix + "Title"] = "Submitted title", [prefix + "Slug"] = slug,
            [prefix + "AuthorId"] = AdminApplicationFactory.UserId,
            [prefix + "PublicationDate"] = "2024-01-02T03:04",
            [prefix + "Description"] = "", [prefix + "Content"] = "",
            [prefix + "Keywords"] = "", [prefix + "ImageUrl"] = "", [prefix + "Tags"] = ""
        };
    }
}
