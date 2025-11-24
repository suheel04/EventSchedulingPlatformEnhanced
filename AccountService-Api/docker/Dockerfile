# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Update path because Dockerfile moved to /docker folder
COPY ../src/AccountService.Api/AccountService.Api.csproj src/AccountService.Api/
COPY ../src/AccountService.Core/AccountService.Core.csproj src/AccountService.Core/
COPY ../src/AccountService.Infrastructure/AccountService.Infrastructure.csproj src/AccountService.Infrastructure/

RUN dotnet restore "src/AccountService.Api/AccountService.Api.csproj"

# Copy everything else
COPY ../src ./src

WORKDIR "/src/src/AccountService.Api"
RUN dotnet build "AccountService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AccountService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AccountService.Api.dll"]
