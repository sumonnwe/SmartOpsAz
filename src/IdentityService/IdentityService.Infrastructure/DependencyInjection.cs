using System;
using IdentityService.Application.Abstractions;
using IdentityService.Infrastructure.Authentication;
using IdentityService.Infrastructure.Persistence;
using IdentityService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("IdentityDatabase") ??
                   configuration["IdentityDatabase:ConnectionString"] ??
                   throw new InvalidOperationException("Connection string 'IdentityDatabase' not configured.");

        services.AddDbContext<IdentityDbContext>(opts =>
        {
            opts.UseNpgsql(conn, b => b.EnableRetryOnFailure());
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        return services;
    }
}