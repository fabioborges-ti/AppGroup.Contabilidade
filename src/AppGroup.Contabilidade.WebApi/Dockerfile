# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/AppGroup.Contabilidade.WebApi/AppGroup.Contabilidade.WebApi.csproj", "src/AppGroup.Contabilidade.WebApi/"]
COPY ["src/AppGroup.Contabilidade.Application/AppGroup.Contabilidade.Application.csproj", "src/AppGroup.Contabilidade.Application/"]
COPY ["src/AppGroup.Contabilidade.Domain/AppGroup.Contabilidade.Domain.csproj", "src/AppGroup.Contabilidade.Domain/"]
COPY ["src/AppGroup.Contabilidade.Logging/AppGroup.Contabilidade.Logging.csproj", "src/AppGroup.Contabilidade.Logging/"]
COPY ["src/AppGroup.Contabilidade.Infrastructure.Database/AppGroup.Contabilidade.Infrastructure.Database.csproj", "src/AppGroup.Contabilidade.Infrastructure.Database/"]
RUN dotnet restore "./src/AppGroup.Contabilidade.WebApi/AppGroup.Contabilidade.WebApi.csproj"
COPY . .
WORKDIR "/src/src/AppGroup.Contabilidade.WebApi"
RUN dotnet build "./AppGroup.Contabilidade.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AppGroup.Contabilidade.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppGroup.Contabilidade.WebApi.dll"]