# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/API/SocialService.API.csproj", "src/API/"]
COPY ["src/Application/SocialService.Application.csproj", "src/Application/"]
COPY ["src/Domain/SocialService.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/SocialService.Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "./src/API/SocialService.API.csproj"
COPY . .
WORKDIR "/src/src/API"
RUN dotnet build "./SocialService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SocialService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialService.API.dll"]
