# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY ["nuget.config", ""]
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Directory.Packages.props", "src/"]
COPY ["src/shared/Broadcasting.Contracts/Broadcasting.Contracts.csproj", "src/shared/Broadcasting.Contracts/"]
COPY ["src/shared/Broadcasting.ServiceDefaults/Broadcasting.ServiceDefaults.csproj", "src/shared/Broadcasting.ServiceDefaults/"]
COPY ["src/frontend/web/Broadcasting.Blazor/Broadcasting.Blazor.csproj", "src/frontend/web/Broadcasting.Blazor/"]
COPY ["src/frontend/web/Broadcasting.Blazor.Client/Broadcasting.Blazor.Client.csproj", "src/frontend/web/Broadcasting.Blazor.Client/"]
RUN dotnet restore "src/frontend/web/Broadcasting.Blazor/Broadcasting.Blazor.csproj"

# copy everything else and build app
COPY ["src/shared/", "src/shared/"]
COPY ["src/frontend/web/", "src/frontend/web/"]

# RUN apt-get update && apt-get install -y tree
# RUN tree /source

WORKDIR /source/src/frontend/web/Broadcasting.Blazor
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Broadcasting.Blazor.dll"]