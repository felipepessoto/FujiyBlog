#Workaround to https://github.com/docker/for-mac/issues/1922
FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
#WORKDIR /src
COPY ["/src/FujiyBlog.Web/FujiyBlog.Web.csproj", "FujiyBlog.Web/"]
COPY ["/src/FujiyBlog.Core/FujiyBlog.Core.csproj", "FujiyBlog.Core/"]
RUN dotnet restore "FujiyBlog.Web/FujiyBlog.Web.csproj"
COPY . .
#WORKDIR "/src/FujiyBlog.Web"
RUN dotnet build "/src/FujiyBlog.Web/FujiyBlog.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "/src/FujiyBlog.Web/FujiyBlog.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FujiyBlog.Web.dll"]