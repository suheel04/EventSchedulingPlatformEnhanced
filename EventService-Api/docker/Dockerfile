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
COPY ../src/EventService.Api/EventService.Api.csproj src/EventService.Api/
COPY ../src/EventService.Core/EventService.Core.csproj src/EventService.Core/
COPY ../src/EventService.Infrastructure/EventService.Infrastructure.csproj src/EventService.Infrastructure/

RUN dotnet restore "src/EventService.Api/EventService.Api.csproj"

# Copy everything else
COPY ../src ./src

WORKDIR "/src/src/EventService.Api"
RUN dotnet build "EventService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EventService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventService.Api.dll"]
