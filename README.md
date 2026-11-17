### Project Description
A simple Open Source Blog using ASP.NET Core on .NET 10, jQuery, Entity Framework Core and SQL Server.

### Building
Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), then build from the repository root:

```powershell
dotnet build src\FujiyBlog.sln --configuration Release
```

Running the blog also requires SQL Server. Configure `ConnectionStrings:DefaultConnection` in the web application's configuration; the default uses SQL Server Express LocalDB on Windows.

### Continuous integration
[GitHub Actions](.github/workflows/ci.yml) runs on every pull request and push to `main`. Separate jobs restore, build, and publish the web application in Release mode with .NET 10, and build the Docker image.

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
