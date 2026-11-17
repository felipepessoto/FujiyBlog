using System.Security.Claims;
using System.Text.Encodings.Web;
using FujiyBlog.Core;
using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Core.Services;
using FujiyBlog.Web.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FujiyBlog.Web.Tests;

public sealed class AdminApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string UserId = "admin-form-tests";
    public const string UserName = "forms@example.invalid";
    private const string AuthenticationScheme = "FormTests";
    private readonly string connectionString = CreateConnectionString();

    private static string CreateConnectionString()
    {
        var configured = Environment.GetEnvironmentVariable("FUJIYBLOG_TEST_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(configured))
        {
            if (!OperatingSystem.IsWindows())
                throw new InvalidOperationException("Set FUJIYBLOG_TEST_CONNECTION_STRING to an isolated SQL Server test instance. See README.md.");
            configured = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=True;TrustServerCertificate=True";
        }

        return new SqlConnectionStringBuilder(configured)
        {
            InitialCatalog = "FujiyBlog_FormTests_" + Guid.NewGuid().ToString("N")
        }.ConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<FujiyBlogDatabase>>();
            services.RemoveAll<IDbContextOptionsConfiguration<FujiyBlogDatabase>>();
            services.AddDbContext<FujiyBlogDatabase>(options => options.UseSqlServer(connectionString));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthenticationScheme;
                options.DefaultChallengeScheme = AuthenticationScheme;
                options.DefaultForbidScheme = AuthenticationScheme;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(AuthenticationScheme, _ => { });

            services.RemoveAll<TelemetryConfiguration>();
            services.AddSingleton(_ =>
            {
                var configuration = TelemetryConfiguration.CreateDefault();
                configuration.DisableTelemetry = true;
                return configuration;
            });
            services.RemoveAll<IEmailSender>();
            services.RemoveAll<ISmsSender>();
            services.RemoveAll<IFileUploadService>();
            services.AddSingleton<IEmailSender, UnexpectedExternalService>();
            services.AddSingleton<ISmsSender, UnexpectedExternalService>();
            services.AddSingleton<IFileUploadService, UnexpectedExternalService>();
        });
    }

    public async Task InitializeAsync()
    {
        await WithDatabaseAsync(async database =>
        {
            await database.Database.MigrateAsync();
            database.Users.Add(new ApplicationUser
            {
                Id = UserId, UserName = UserName, NormalizedUserName = UserName.ToUpperInvariant(),
                Email = UserName, Enabled = true, CreationDate = DateTime.UtcNow
            });
            database.Categories.Add(new Category { Name = "Existing category" });
            database.SaveChanges();
            return 0;
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
        await using var database = new FujiyBlogDatabase(
            new DbContextOptionsBuilder<FujiyBlogDatabase>().UseSqlServer(connectionString).Options);
        await database.Database.EnsureDeletedAsync();
        CacheHelper.ClearCache();
    }

    public HttpClient CreateAdminClient()
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        client.DefaultRequestHeaders.Add("X-Test-User", UserName);
        return client;
    }

    public async Task<T> WithDatabaseAsync<T>(Func<FujiyBlogDatabase, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        return await action(scope.ServiceProvider.GetRequiredService<FujiyBlogDatabase>());
    }

    private sealed class TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers["X-Test-User"] != UserName)
                return Task.FromResult(AuthenticateResult.NoResult());

            var permissions = Request.Headers.TryGetValue("X-Test-Permissions", out var selected)
                ? selected.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)
                : Enum.GetNames<PermissionClaims>();
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, UserId), new(ClaimTypes.Name, UserName)
            };
            claims.AddRange(permissions.Select(permission => new Claim(CustomClaimTypes.Permission, permission)));
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationScheme));
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, AuthenticationScheme)));
        }
    }

    private sealed class UnexpectedExternalService : IEmailSender, ISmsSender, IFileUploadService
    {
        public Task SendEmailAsync(string email, string subject, string message) =>
            throw new InvalidOperationException("Unexpected email during an admin form test.");
        public Task SendEmailAsync(string email, string subject, string message, string replyToEmail, string replyToEmailName) =>
            throw new InvalidOperationException("Unexpected email during an admin form test.");
        public Task SendSmsAsync(string number, string message) =>
            throw new InvalidOperationException("Unexpected SMS during an admin form test.");
        public Task<string> UploadFile(Stream fileStream, string filePath) =>
            throw new InvalidOperationException("Unexpected storage access during an admin form test.");
    }
}
