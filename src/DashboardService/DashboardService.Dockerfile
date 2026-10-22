
# ==========================================
# Stage 1: ASP.NET Core runtime
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
	
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
COPY ["src/DashboardService/DashboardService.Domain/DashboardService.Domain.csproj", "src/DashboardService/DashboardService.Domain/"]
COPY ["src/DashboardService/DashboardService.Application/DashboardService.Application.csproj", "src/DashboardService/DashboardService.Application/"]
COPY ["src/DashboardService/DashboardService.Infrastructure/DashboardService.Infrastructure.csproj", "src/DashboardService/DashboardService.Infrastructure/"] 
COPY ["src/DashboardService/DashboardService.Api/DashboardService.Api.csproj", "src/DashboardService/DashboardService.Api/"]

# Shared integration event contracts
COPY ["src/Shared/SmartOpsAz.IntegrationEvents/SmartOpsAz.IntegrationEvents.csproj", "src/Shared/SmartOpsAz.IntegrationEvents/"]

# Restore NuGet packages
RUN dotnet restore "src/DashboardService/DashboardService.Api/DashboardService.Api.csproj" 

# Copy remaining source files
COPY . .

WORKDIR /src/src/DashboardService/DashboardService.Api

# Build API
RUN dotnet build "DashboardService.Api.csproj" -c Release -o /app/build --no-restore /p:EnableSourceControlManagerQueries=false /p:EnableSourceLink=false


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

RUN dotnet publish "DashboardService.Api.csproj" -c Release -o /app/publish --no-restore /p:EnableSourceControlManagerQueries=false /p:EnableSourceLink=false


# ==========================================
# Stage 5: Final runtime image
# ==========================================
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "DashboardService.Api.dll"]
