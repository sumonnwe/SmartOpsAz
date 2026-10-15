//csharp src/InventoryService/InventoryService.Infrastructure/Persistence/InventoryDbContext.cs
using Microsoft.EntityFrameworkCore;
using InventoryService.Infrastructure.Persistence.Entities;

namespace InventoryService.Infrastructure.Persistence;

public sealed class InventoryDbContext : DbContext
{
    public DbSet<InventoryItemEntity> InventoryItems
    {
        get
        {
            return Set<InventoryItemEntity>();
        }
    }

    public DbSet<InventoryMovementEntity> InventoryMovements
    {
        get
        {
            return Set<InventoryMovementEntity>();
        }
    }

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        // Global query filter: exclude soft-deleted items from normal queries
        modelBuilder.Entity<InventoryItemEntity>()
            .HasQueryFilter(x => !x.IsDeleted);
    }
}