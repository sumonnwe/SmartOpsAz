using DeviceService.Api.Controllers;
using DeviceService.Application.DTOs;
using DeviceService.Application.Interfaces;
using DeviceService.Application.Validators;
using DeviceService.Infrastructure;
using DeviceService.Infrastructure.Messaging;
using DeviceService.Infrastructure.Persistence;
using DeviceService.Infrastructure.Repositories;
using DeviceService.Infrastructure.Workers;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; 
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();

builder.Services.AddHostedService<DeviceOfflineDetectionWorker>();

// Swagger configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DeviceService API",
        Version = "v1",
        Description = "IoT Device Management and Monitoring Service"
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

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDeviceRequestValidator>();

// Infrastructure
builder.Services.AddDeviceInfrastructure(builder.Configuration);
var rabbitEnabled =
           builder.Configuration.GetValue<bool>("RabbitMQ:Enabled", false);

if (rabbitEnabled)
{
    builder.Services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            var host = builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq";
            var port = builder.Configuration.GetValue<ushort>("RabbitMQ:Port", 5672);
            var virtualHost = builder.Configuration["RabbitMQ:VirtualHost"] ?? "/";
            var username = builder.Configuration["RabbitMQ:Username"] ?? "guest";
            var password = builder.Configuration["RabbitMQ:Password"] ?? "guest";

            cfg.Host(host, port, virtualHost, h =>
            {
                h.Username(username);
                h.Password(password);
            });
        });
    });

    builder.Services.AddScoped<IEventPublisher, RabbitMQEventPublisher>();
}
else
{
    builder.Services.AddScoped<IEventPublisher, NoOpEventPublisher>();
}


// JWT authentication (reuse same configuration as InventoryService)
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BiomedicalEngineerOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("BIOMEDICAL");
    });
});

// Health checks
builder.Services.AddHealthChecks().AddDbContextCheck<DeviceDbContext>(name: "DeviceDB");

var app = builder.Build();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DeviceDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "DeviceService.Api v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoints
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();
