using FujiyBlog.Core;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Services;
using FujiyBlog.Web.Infrastructure;
using FujiyBlog.Web.Models;
using FujiyBlog.Web.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;

namespace FujiyBlog.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<FujiyBlogDatabase>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<FujiyBlogDatabase>();

            services.AddControllersWithViews();
            //services.AddRazorPages();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();//TODO necessario ainda?

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            //Custom
            services.AddScoped<SettingRepository>();
            services.AddScoped<DateTimeUtil>();
            services.AddScoped<PostRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<WidgetSettingRepository>();
            services.AddScoped<FeedRepository>();
            services.AddScoped<FeedGenerator>();
            services.AddScoped<cloudscribe.Syndication.Models.Rss.IChannelProvider, FeedGenerator>();//to enable cloudscribe.Syndication RssController

            //File upload
            services.AddScoped<AzureStorageFileUploadService>();
            services.AddScoped<LocalFolderFileUploadService>();
            services.AddScoped(FileUploadServiceFactory.Build);

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ThemeViewLocationExpander());//TODO confirmar
            });

            services.AddAuthorization(options =>
            {
                foreach (var role in Enum.GetNames(typeof(PermissionClaims)))
                {
                    options.AddPolicy(role, policy => policy.RequireClaim(CustomClaimTypes.Permission, role));
                }
            });

            services.Configure<MvcOptions>(options =>
            {
                options.CacheProfiles.Add("RssCacheProfile",
                     new CacheProfile
                     {
                         Duration = 100
                     });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SettingRepository settingRepository)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("Error", "Error", new { controller = "Post", action = "Error" });
                endpoints.MapControllerRoute("PostDetailId", "postid/{Id}", new { controller = "Post", action = "DetailsById" });
                endpoints.MapControllerRoute("PostDetail", "post/{*PostSlug}", new { controller = "Post", action = "Details" });
                endpoints.MapControllerRoute("PageById", "pageid/{Id}", new { controller = "Page", action = "DetailsById" });
                endpoints.MapControllerRoute("Page", "page/{*PageSlug}", new { controller = "Page", action = "Details" });
                endpoints.MapControllerRoute("TagHome", "tag/{tag}", new { controller = "Post", action = "Tag" });
                endpoints.MapControllerRoute("CategoryHome", "category/{category}", new { controller = "Post", action = "Category" });
                endpoints.MapControllerRoute("AuthorHome", "author/{author}", new { controller = "Post", action = "Author" });
                endpoints.MapControllerRoute("Archive", "archive", new { controller = "Post", action = "Archive" });
                endpoints.MapControllerRoute("ArchiveByMonth", "archive/{year:int:range(0,9999)}/{month:int:range(1,12)}", new { controller = "Post", action = "ArchiveDate" });
                endpoints.MapControllerRoute("UploadedFiles", "upload/{*filePath}", new { controller = "Upload", action = "Details" }, new { controller = new UploadServiceConfiguredConstraint() });
                endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller}/{action}/{id?}");
                endpoints.MapControllerRoute("FirstUsage", "", new { controller = "Account", action = "Register" }, new { controller = new FirstUsageConstraint() });
                endpoints.MapControllerRoute("HomePosts", "", new { controller = "Post", action = "Index" }, new { controller = new HomeConstraint() });
                endpoints.MapControllerRoute("BlogHome", "blog", new { controller = "Post", action = "Index" });
                endpoints.MapControllerRoute("DefaultPage", "", new { controller = "Page", action = "Index" }, new { controller = new HomeConstraint() });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                
                //endpoints.MapHealthChecks("/health");
            });

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

            TelemetryConfiguration.Active.DisableTelemetry = string.IsNullOrWhiteSpace(settingRepository.ApplicationInsightsInstrumentationKey);

            if (TelemetryConfiguration.Active.DisableTelemetry == false)
            {
                TelemetryConfiguration.Active.InstrumentationKey = settingRepository.ApplicationInsightsInstrumentationKey;
            }
        }

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
    }
}
