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
            services.Configure<CookiePolicyOptions>(options =>
            {
                //options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.AddDbContext<FujiyBlogDatabase>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"))
                    .ConfigureWarnings(warnings => warnings.Throw(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.QueryClientEvaluationWarning)));

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                //config.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<FujiyBlogDatabase>().AddDefaultTokenProviders();

            //services.AddDefaultIdentity<IdentityUser>()
            //    .AddEntityFrameworkStores<FujiyBlogDatabase>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, SettingRepository settingRepository)
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
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("Error", "Error", new { controller = "Post", action = "Error" });
                routes.MapRoute("PostDetailId", "postid/{Id}", new { controller = "Post", action = "DetailsById" });
                routes.MapRoute("PostDetail", "post/{*PostSlug}", new { controller = "Post", action = "Details" });
                routes.MapRoute("PageById", "pageid/{Id}", new { controller = "Page", action = "DetailsById" });
                routes.MapRoute("Page", "page/{*PageSlug}", new { controller = "Page", action = "Details" });
                routes.MapRoute("TagHome", "tag/{tag}", new { controller = "Post", action = "Tag" });
                routes.MapRoute("CategoryHome", "category/{category}", new { controller = "Post", action = "Category" });
                routes.MapRoute("AuthorHome", "author/{author}", new { controller = "Post", action = "Author" });
                routes.MapRoute("Archive", "archive", new { controller = "Post", action = "Archive" });
                routes.MapRoute("ArchiveByMonth", "archive/{year:int:range(0,9999)}/{month:int:range(1,12)}", new { controller = "Post", action = "ArchiveDate" });
                routes.MapRoute("UploadedFiles", "upload/{*filePath}", new { controller = "Upload", action = "Details" }, new { controller = new UploadServiceConfiguredConstraint() });

                routes.MapRoute(name: "areaRoute", template: "{area:exists}/{controller}/{action}/{id?}");

                routes.MapRoute("FirstUsage", "", new { controller = "Account", action = "Register" }, new { controller = new FirstUsageConstraint() });

                routes.MapRoute("HomePosts", "", new { controller = "Post", action = "Index" }, new { controller = new HomeConstraint() });

                routes.MapRoute("BlogHome", "blog", new { controller = "Post", action = "Index" });

                routes.MapRoute("DefaultPage", "", new { controller = "Page", action = "Index" }, new { controller = new HomeConstraint() });

                routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}");
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
