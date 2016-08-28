using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FujiyBlog.Web.Models;
using FujiyBlog.Web.Services;
using FujiyBlog.Core.EntityFramework;
using Microsoft.Extensions.Caching.Memory;
using FujiyBlog.Core.Caching;
using Microsoft.AspNetCore.Mvc.Razor;
using FujiyBlog.Core;
using FujiyBlog.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication.Google;
using FujiyBlog.Core.Services;
using FujiyBlog.Core.DomainObjects;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace FujiyBlog.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<FujiyBlogDatabase>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));//TODO confirmar

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<FujiyBlogDatabase>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddApplicationInsightsTelemetry(Configuration);

            //Custom
            services.AddScoped<SettingRepository>();
            services.AddScoped<DateTimeUtil>();
            services.AddScoped<PostRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<WidgetSettingRepository>();
            services.AddScoped<FeedRepository>();
            services.AddScoped<FeedGenerator>();

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Add Application Insights monitoring to the request pipeline as a very first middleware.
            app.UseApplicationInsightsRequestTelemetry();

            if (false && env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            // Add Application Insights exceptions handling to the request pipeline.
            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

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

                routes.MapRoute(name: "areaRoute", template: "{area:exists}/{controller}/{action}/{id?}");

                routes.MapRoute("FirstUsage", "", new { controller = "Account", action = "Register" }, new { controller = new FirstUsageConstraint() });

                routes.MapRoute("HomePosts", "", new { controller = "Post", action = "Index" }, new { controller = new HomeConstraint() });

                routes.MapRoute("BlogHome", "blog", new { controller = "Post", action = "Index" });

                routes.MapRoute("DefaultPage", "", new { controller = "Page", action = "Index" });

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


            //TODO
            //TelemetryConfiguration.Active.DisableTelemetry = string.IsNullOrWhiteSpace(Settings.SettingRepository.ApplicationInsightsInstrumentationKey);

            //if (TelemetryConfiguration.Active.DisableTelemetry == false)
            //{
            //    TelemetryConfiguration.Active.InstrumentationKey = Settings.SettingRepository.ApplicationInsightsInstrumentationKey;
            //}

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
