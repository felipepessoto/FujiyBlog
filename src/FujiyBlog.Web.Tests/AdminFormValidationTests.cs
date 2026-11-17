using FujiyBlog.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FujiyBlog.Web.Tests;

public sealed class AdminFormValidationTests
{
    [Theory]
    [InlineData("basic")]
    [InlineData("page")]
    [InlineData("post")]
    [InlineData("storage")]
    [InlineData("comments")]
    [InlineData("email")]
    [InlineData("feed")]
    [InlineData("social")]
    public void Optional_inputs_and_unposted_display_data_are_not_required(string form)
    {
        object model = form switch
        {
            "basic" => new AdminBasicSettings { BlogName = "Blog", Theme = "Default", PostsPerPage = 10, TimeZoneId = "UTC", Language = "en" },
            "page" => new AdminPageSave { Title = "Page", Slug = "page", AuthorId = "author" },
            "post" => new AdminPostSave { Title = "Post", Slug = "post", AuthorId = "author" },
            "storage" => new AdminStorageSettings { FileUploadService = "LocalFolderFileUploadService" },
            "comments" => new AdminCommentsSettings { CommentsPerPage = 10 },
            "email" => new AdminEmailSettings { EmailTo = "test@example.invalid", SmtpAddress = "localhost", SmtpPort = 25 },
            "feed" => new AdminFeedSettings { ItemsShownInFeed = 0 },
            "social" => new AdminSettingsSocialNetworks(),
            _ => throw new ArgumentOutOfRangeException(nameof(form))
        };
        var state = Validate(model);
        Assert.True(state.IsValid, string.Join("; ", state.SelectMany(field =>
            field.Value!.Errors.Select(error => $"{field.Key}: {error.ErrorMessage}"))));
    }

    [Theory]
    [InlineData("posts", 0, "PostsPerPage")]
    [InlineData("comments", 0, "CommentsPerPage")]
    [InlineData("days", -1, "CloseCommentsAfterDays")]
    [InlineData("feed", -1, "ItemsShownInFeed")]
    [InlineData("smtp", 0, "SmtpPort")]
    [InlineData("smtp", 65536, "SmtpPort")]
    public void Invalid_numeric_values_have_field_errors(string form, int value, string field)
    {
        object model = form switch
        {
            "posts" => new AdminBasicSettings { PostsPerPage = value },
            "comments" => new AdminCommentsSettings { CommentsPerPage = value },
            "days" => new AdminCommentsSettings { CommentsPerPage = 10, CloseCommentsAfterDays = value },
            "feed" => new AdminFeedSettings { ItemsShownInFeed = value },
            "smtp" => new AdminEmailSettings { SmtpPort = value },
            _ => throw new ArgumentOutOfRangeException(nameof(form))
        };
        Assert.NotEmpty(Validate(model)[field]!.Errors);
    }

    private static ModelStateDictionary Validate(object model)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddControllersWithViews();
        using var provider = services.BuildServiceProvider();
        var context = new ActionContext(new DefaultHttpContext { RequestServices = provider },
            new RouteData(), new ActionDescriptor(), new ModelStateDictionary());
        provider.GetRequiredService<IObjectModelValidator>().Validate(context, null, "", model);
        return context.ModelState;
    }
}
