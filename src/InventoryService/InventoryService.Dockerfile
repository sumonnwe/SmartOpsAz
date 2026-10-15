
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
COPY ["src/InventoryService/InventoryService.Domain/InventoryService.Domain.csproj", "src/InventoryService/InventoryService.Domain/"]
COPY ["src/InventoryService/InventoryService.Application/InventoryService.Application.csproj", "src/InventoryService/InventoryService.Application/"]
COPY ["src/InventoryService/InventoryService.Infrastructure/InventoryService.Infrastructure.csproj", "src/InventoryService/InventoryService.Infrastructure/"]
COPY ["src/InventoryService/InventoryService.Contracts/InventoryService.Contracts.csproj", "src/InventoryService/InventoryService.Contracts/"]
COPY ["src/InventoryService/InventoryService.Api/InventoryService.Api.csproj", "src/InventoryService/InventoryService.Api/"]

# Shared integration event contracts
COPY ["src/Shared/SmartOpsAz.IntegrationEvents/SmartOpsAz.IntegrationEvents.csproj", "src/Shared/SmartOpsAz.IntegrationEvents/"]

# Restore NuGet packages
RUN dotnet restore "src/InventoryService/InventoryService.Api/InventoryService.Api.csproj"

# Copy remaining source files
COPY . .

WORKDIR /src/src/InventoryService/InventoryService.Api

# Build API
RUN dotnet build "InventoryService.Api.csproj" -c Release -o /app/build --no-restore


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

RUN dotnet publish "InventoryService.Api.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false


# ==========================================
# Stage 5: Final runtime image
# ==========================================
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "InventoryService.Api.dll"]
