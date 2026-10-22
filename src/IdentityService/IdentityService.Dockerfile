
# ==========================================
# Stage 1: ASP.NET Core runtime
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app

EXPOSE 8080
EXPOSE 8443


# ==========================================
# Stage 2: Build
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Central package management
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]

# Copy project files for restore caching
COPY ["src/IdentityService/IdentityService.Domain/IdentityService.Domain.csproj", "src/IdentityService/IdentityService.Domain/"]
COPY ["src/IdentityService/IdentityService.Application/IdentityService.Application.csproj", "src/IdentityService/IdentityService.Application/"]
COPY ["src/IdentityService/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj", "src/IdentityService/IdentityService.Infrastructure/"]
COPY ["src/IdentityService/IdentityService.Contracts/IdentityService.Contracts.csproj", "src/IdentityService/IdentityService.Contracts/"]
COPY ["src/IdentityService/IdentityService.Api/IdentityService.Api.csproj", "src/IdentityService/IdentityService.Api/"]

# Shared integration event contracts
COPY ["src/Shared/SmartOpsAz.IntegrationEvents/SmartOpsAz.IntegrationEvents.csproj", "src/Shared/SmartOpsAz.IntegrationEvents/"]

# Restore NuGet packages
RUN dotnet restore "src/IdentityService/IdentityService.Api/IdentityService.Api.csproj"

# Copy remaining source files
COPY . .

WORKDIR /src/src/IdentityService/IdentityService.Api

# Build API
RUN dotnet build "IdentityService.Api.csproj" -c Release -o /app/build --no-restore /p:EnableSourceControlManagerQueries=false /p:EnableSourceLink=false


# ==========================================
# Stage 3: EF Core migrations
# ==========================================
FROM build AS migration

# Install EF Core CLI compatible with EF Core 9
RUN dotnet tool install --global dotnet-ef --version 9.0.0

# Make dotnet-ef accessible
ENV PATH="/root/.dotnet/tools:${PATH}"

# Set working directory to solution root
WORKDIR /src

# Verify installation
RUN dotnet ef --version


# ==========================================
# Stage 4: Publish
# ==========================================
FROM build AS publish

RUN dotnet publish "IdentityService.Api.csproj" -c Release -o /app/publish --no-restore /p:EnableSourceControlManagerQueries=false /p:EnableSourceLink=false


# ==========================================
# Stage 5: Final runtime image
# ==========================================
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "IdentityService.Api.dll"]
