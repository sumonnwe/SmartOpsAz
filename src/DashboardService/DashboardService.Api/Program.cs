using DashboardService.Application.Consumers;
using DashboardService.Application.Handlers;
using DashboardService.Infrastructure;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// MediatR for CQRS
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetDashboardSummaryHandler>());

// Infrastructure: Redis and consumers
builder.Services.AddDashboardInfrastructure(builder.Configuration);
builder.Services.AddDashboardConsumers();

// MassTransit with RabbitMQ
var rabbitMQEnabled = builder.Configuration.GetValue<bool>("RabbitMQ:Enabled", true);
if (rabbitMQEnabled)
{
    builder.Services.AddMassTransit(x =>
    {
        var rabbitHost = builder.Configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost";
        var rabbitPort = builder.Configuration.GetValue<ushort>("RabbitMQ:Port", 5672);
        var rabbitUser = builder.Configuration.GetValue<string>("RabbitMQ:Username") ?? "guest";
        var rabbitPassword = builder.Configuration.GetValue<string>("RabbitMQ:Password") ?? "guest";
        var rabbitVhost = builder.Configuration.GetValue<string>("RabbitMQ:VirtualHost") ?? "/";

        x.AddConsumer<DeviceOfflineConsumer>();
        x.AddConsumer<DeviceRecoveredConsumer>();
        x.AddConsumer<DeviceRegisteredConsumer>();
        x.AddConsumer<DeviceHeartbeatReceivedConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(rabbitHost, rabbitPort, rabbitVhost, h =>
            {
                h.Username(rabbitUser);
                h.Password(rabbitPassword);
            });

            // Configure all consumers
            cfg.ConfigureEndpoints(context);

            // Retry policy: exponential backoff
            cfg.UseMessageRetry(r =>
            {
                r.Exponential(
                    retryLimit: 3,
                    minInterval: TimeSpan.FromMilliseconds(100),
                    maxInterval: TimeSpan.FromSeconds(10),
                    intervalDelta: TimeSpan.FromMilliseconds(50));
            });

            // In-memory outbox for reliable consuming
            cfg.UseInMemoryOutbox();

            // Structured logging
            //cfg.ConnectEndpointConfigurationObserver(new EndpointConfigurationObserver());
        });
    });
}
 

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dashboard Service API",
        Version = "v1",
        Description = "Real-time operational dashboard for inventory and device monitoring"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT access token issued by IdentityService"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
var signingKey = jwtSection["SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            RoleClaimType = "role",
            NameClaimType = "unique_name",
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "DashboardService API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();

/// <summary>
/// Observer for RabbitMQ endpoint configuration events (used for logging).
/// </summary>
/*
internal sealed class EndpointConfigurationObserver : IEndpointConfigurationObserver
{
    public void EndpointConfigured<T>(IEndpointConfigurator configurator) where T : class
    {
        // Log or perform custom actions on endpoint configuration
    }

    public void EndpointConfigured<T>(T configurator) where T : IReceiveEndpointConfigurator
    {
        throw new NotImplementedException();
    }
}
*/