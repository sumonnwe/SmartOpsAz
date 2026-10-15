using MediatR;
using FluentValidation;
using FluentValidation.AspNetCore;
using InventoryService.Application.Inventory.RegisterInventory;
using InventoryService.Infrastructure;
using InventoryService.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; 
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "InventoryService.Api",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT access token"
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
            Array.Empty<string>()
        }
    }); 
});

// Register MediatR handlers from application assembly
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterInventoryHandler).Assembly));

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<RegisterInventoryValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddInfrastructure(builder.Configuration);

// Add JWT Bearer authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
var signingKey = jwtSection["SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BiomedicalEngineerOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("BiomedicalEngineer");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Inventory.Api v1");
    });
}

// Exception middleware should be early in the pipeline
app.UseInventoryExceptionHandler();

app.UseHttpsRedirection();

// Authentication MUST execute before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
