# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY ["nuget.config", ""]
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Directory.Packages.props", "src/"]
COPY ["src/shared/Broadcasting.Contracts/Broadcasting.Contracts.csproj", "src/shared/Broadcasting.Contracts/"]
COPY ["src/shared/Broadcasting.ServiceDefaults/Broadcasting.ServiceDefaults.csproj", "src/shared/Broadcasting.ServiceDefaults/"]
COPY ["src/backend/Broadcasting.SharedKernel/Broadcasting.SharedKernel.csproj", "src/backend/Broadcasting.SharedKernel/"]
COPY ["src/backend/modules/Broadcasting.Chat/Broadcasting.Chat.csproj", "src/backend/modules/Broadcasting.Chat/"]
COPY ["src/backend/modules/Broadcasting.Streaming/Broadcasting.Streaming.csproj", "src/backend/modules/Broadcasting.Streaming/"]
COPY ["src/backend/Broadcasting.Web/Broadcasting.Web.csproj", "src/backend/Broadcasting.Web/"]
RUN dotnet restore "src/backend/Broadcasting.Web/Broadcasting.Web.csproj"

# copy everything else and build app
COPY ["src/shared/", "src/shared/"]
COPY ["src/backend/", "src/backend/"]

RUN apt-get update && apt-get install -y tree
RUN tree /source

WORKDIR /source/src/backend/Broadcasting.Web
RUN dotnet publish -c release -o /app/publish --no-restore /p:UseAppHost=false

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Broadcasting.Web.dll"]