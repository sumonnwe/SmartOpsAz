FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Shared/SmartOpsAz.IntegrationEvents/SmartOpsAz.IntegrationEvents.csproj", "src/Shared/SmartOpsAz.IntegrationEvents/"]
COPY ["src/DashboardService/DashboardService.Domain/DashboardService.Domain.csproj", "src/DashboardService/DashboardService.Domain/"]
COPY ["src/DashboardService/DashboardService.Infrastructure/DashboardService.Infrastructure.csproj", "src/DashboardService/DashboardService.Infrastructure/"]
COPY ["src/DashboardService/DashboardService.Application/DashboardService.Application.csproj", "src/DashboardService/DashboardService.Application/"]
COPY ["src/DashboardService/DashboardService.Api/DashboardService.Api.csproj", "src/DashboardService/DashboardService.Api/"]
COPY ["Directory.Packages.props", "."]

RUN dotnet restore "src/DashboardService/DashboardService.Api/DashboardService.Api.csproj"
COPY . .
WORKDIR "/src/src/DashboardService/DashboardService.Api"
RUN dotnet build "DashboardService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DashboardService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DashboardService.Api.dll"]