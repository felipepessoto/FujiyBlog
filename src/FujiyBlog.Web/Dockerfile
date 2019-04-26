FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["FujiyBlog.Web/FujiyBlog.Web.csproj", "FujiyBlog.Web/"]
COPY ["FujiyBlog.Core/FujiyBlog.Core.csproj", "FujiyBlog.Core/"]
RUN dotnet restore "FujiyBlog.Web/FujiyBlog.Web.csproj"
COPY . .
WORKDIR "/src/FujiyBlog.Web"
RUN dotnet build "FujiyBlog.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FujiyBlog.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FujiyBlog.Web.dll"]