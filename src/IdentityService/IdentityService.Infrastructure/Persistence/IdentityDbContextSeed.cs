using System;
using System.Threading.Tasks;
using IdentityService.Application.Abstractions;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence;

public sealed class IdentityDbContextSeed
{
    public static async Task SeedDevelopmentDataAsync(
        IdentityDbContext context,
        IPasswordHasher passwordHasher,
        ILogger<IdentityDbContextSeed> logger)
    {
        if (context.Users.Any())
        {
            logger.LogInformation("Database already contains users, skipping seed.");
            return;
        }

        var seedPassword = Environment.GetEnvironmentVariable("IdentitySeed__DefaultPassword");
        if (string.IsNullOrWhiteSpace(seedPassword))
        {
            logger.LogWarning("IdentitySeed__DefaultPassword environment variable not configured. Skipping development seed.");
            return;
        }

        var users = new[]
        {
            User.Create("admin", passwordHasher.HashPassword(seedPassword), UserRole.Admin),
            User.Create("biomedical", passwordHasher.HashPassword(seedPassword), UserRole.BiomedicalEngineer),
            User.Create("nurse", passwordHasher.HashPassword(seedPassword), UserRole.Nurse),
            User.Create("security", passwordHasher.HashPassword(seedPassword), UserRole.SecurityOfficer),
            User.Create("maintenance", passwordHasher.HashPassword(seedPassword), UserRole.MaintenanceEngineer)
        };

        foreach (var user in users)
        {
            context.Users.Add(user);
        }

        if (context.Users.Any())
        {
            logger.LogInformation(
                "Database already contains users, skipping seed.");

            return;
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Development users seeded successfully.");
    }
}