### Project Description
A simple Open Source Blog using ASP.NET Core on .NET 10, jQuery, Entity Framework Core and SQL Server.

### Building
Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), then build from the repository root:

```powershell
dotnet build src\FujiyBlog.sln --configuration Release
```

Running the blog also requires SQL Server. Configure `ConnectionStrings:DefaultConnection` in the web application's configuration; the default uses SQL Server Express LocalDB on Windows.

### Testing
Run the MVC validation and SQL Server-backed request regressions from the repository root:

```powershell
dotnet test src\FujiyBlog.sln --configuration Release
```

On Windows, request tests use SQL Server Express LocalDB by default. On other platforms, or to use a dedicated SQL Server test instance, set `FUJIYBLOG_TEST_CONNECTION_STRING` to a connection string with permission to create and drop databases. Each test fixture overrides the database name with a unique `FujiyBlog_FormTests_` name, applies the existing migrations, and drops that database afterward. Use a test instance, not a production server.

The test host uses test authentication, disables telemetry, and rejects unexpected email, SMS, or upload calls; no production credentials or external services are required. To run only the validation tests without SQL Server:

```powershell
dotnet test src\FujiyBlog.Web.Tests --configuration Release --filter FullyQualifiedName~AdminFormValidationTests
```

### Continuous integration
[GitHub Actions](.github/workflows/ci.yml) runs on every pull request and push to `main`. Separate jobs restore, build, test, and publish the web application in Release mode with .NET 10, and build the Docker image. Request tests use an ephemeral SQL Server container with generated credentials, removed when the job finishes.

Publishing only creates deployment-ready files on the CI runner. The workflow does not upload artifacts, push images to a registry, or deploy the application.

### Demo
You can see a live blog at <a href="http://blog.fujiy.net/">http://blog.fujiy.net/</a>.

### Features
* Multi Author Support
* Widgets
* Themes
* Comments (with moderation)
* BlogML Import
* Tags
* Categories
* WebFarm Support (using SQL Server)
* Multi-Language
* OpenID for comments
