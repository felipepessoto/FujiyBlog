FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
#WORKDIR /src
COPY ["/src/FujiyBlog.Web/FujiyBlog.Web.csproj", "FujiyBlog.Web/"]
COPY ["/src/FujiyBlog.Core/FujiyBlog.Core.csproj", "FujiyBlog.Core/"]
RUN dotnet restore "FujiyBlog.Web/FujiyBlog.Web.csproj"
COPY . .
#WORKDIR "/src/FujiyBlog.Web"
RUN dotnet build "/src/FujiyBlog.Web/FujiyBlog.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "/src/FujiyBlog.Web/FujiyBlog.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FujiyBlog.Web.dll"]