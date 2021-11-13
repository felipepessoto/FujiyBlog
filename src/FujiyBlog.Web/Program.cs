using FujiyBlog.Core;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Services;
using FujiyBlog.Web.Infrastructure;
using FujiyBlog.Web.Models;
using FujiyBlog.Web.Services;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;

var builder = WebApplication.CreateBuilder(args);

//using (var scope = webHost.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var db = services.GetRequiredService<FujiyBlogDatabase>();

//    try
//    {
//        db.Database.Migrate();
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "An error occurred while migrating the database.");
//    }
//}



builder.Services.AddApplicationInsightsTelemetry();
builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FujiyBlogDatabase>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)//true
//    .AddEntityFrameworkStores<FujiyBlogDatabase>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<FujiyBlogDatabase>();

builder.Services.AddControllersWithViews();

// Add application services.
builder.Services.AddTransient<IEmailSender, AuthMessageSender>();
builder.Services.AddTransient<ISmsSender, AuthMessageSender>();

//Custom
builder.Services.AddScoped<SettingRepository>();
builder.Services.AddScoped<DateTimeUtil>();
builder.Services.AddScoped<PostRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<WidgetSettingRepository>();
builder.Services.AddScoped<FeedRepository>();
builder.Services.AddScoped<FeedGenerator>();

//File upload
builder.Services.AddScoped<AzureStorageFileUploadService>();
builder.Services.AddScoped<LocalFolderFileUploadService>();
builder.Services.AddScoped(FileUploadServiceFactory.Build);

builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationExpanders.Add(new ThemeViewLocationExpander());//TODO confirmar
});

builder.Services.AddAuthorization(options =>
{
    foreach (var role in Enum.GetNames(typeof(PermissionClaims)))
    {
        options.AddPolicy(role, policy => policy.RequireClaim(CustomClaimTypes.Permission, role));
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");//app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(7)
        };
    }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("Error", "Error", new { controller = "Post", action = "Error" });
app.MapControllerRoute("PostDetailId", "postid/{Id}", new { controller = "Post", action = "DetailsById" });
app.MapControllerRoute("PostDetail", "post/{*PostSlug}", new { controller = "Post", action = "Details" });
app.MapControllerRoute("PageById", "pageid/{Id}", new { controller = "Page", action = "DetailsById" });
app.MapControllerRoute("Page", "page/{*PageSlug}", new { controller = "Page", action = "Details" });
app.MapControllerRoute("TagHome", "tag/{tag}", new { controller = "Post", action = "Tag" });
app.MapControllerRoute("CategoryHome", "category/{category}", new { controller = "Post", action = "Category" });
app.MapControllerRoute("AuthorHome", "author/{author}", new { controller = "Post", action = "Author" });
app.MapControllerRoute("Archive", "archive", new { controller = "Post", action = "Archive" });
app.MapControllerRoute("ArchiveByMonth", "archive/{year:int:range(0,9999)}/{month:int:range(1,12)}", new { controller = "Post", action = "ArchiveDate" });
app.MapControllerRoute("UploadedFiles", "upload/{*filePath}", new { controller = "Upload", action = "Details" }, new { controller = new UploadServiceConfiguredConstraint() });
app.MapControllerRoute("areaRoute", "{area:exists}/{controller}/{action}/{id?}");
app.MapControllerRoute("FirstUsage", "", new { controller = "Account", action = "Register" }, new { controller = new FirstUsageConstraint() });
app.MapControllerRoute("HomePosts", "", new { controller = "Post", action = "Index" }, new { controller = new HomeConstraint() });
app.MapControllerRoute("BlogHome", "blog", new { controller = "Post", action = "Index" });
app.MapControllerRoute("DefaultPage", "", new { controller = "Page", action = "Index" }, new { controller = new HomeConstraint() });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.MapHealthChecks("/health");

//app.MapRazorPages();


//       var supportedCultures = new[]
//{
//     new CultureInfo("en"),
//     new CultureInfo("pt-BR"),
// };

//       app.UseRequestLocalization(new RequestLocalizationOptions
//       {
//           DefaultRequestCulture = new RequestCulture("en-US"),
//           // Formatting numbers, dates, etc.
//           SupportedCultures = supportedCultures,
//           // UI strings that we have localized.
//           SupportedUICultures = supportedCultures
//       });

//TelemetryConfiguration configuration = ...
//configuration.DisableTelemetry = string.IsNullOrWhiteSpace(settingRepository.ApplicationInsightsInstrumentationKey);

//if (configuration.DisableTelemetry == false)
//{
//    configuration.InstrumentationKey = settingRepository.ApplicationInsightsInstrumentationKey;
//}

app.Run();





//public override string GetVaryByCustomString(HttpContext context, string arg)
//{
//    string result = string.Empty;

//    foreach (string s in arg.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
//    {
//        if (s.ToLower() == "user")
//        {
//            result += "user:" + context.User.Identity.Name;
//        }
//        if (s.ToLower() == "lastcache")
//        {
//            result += "lastcache:" + DependencyResolver.Current.GetService<FujiyBlogDatabase>().LastDatabaseChange;
//        }
//    }

//    if (!string.IsNullOrEmpty(result))
//    {
//        return result;
//    }

//    return base.GetVaryByCustomString(context, arg);
//}