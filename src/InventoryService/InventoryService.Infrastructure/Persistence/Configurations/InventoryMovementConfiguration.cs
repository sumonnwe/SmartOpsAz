using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryService.Infrastructure.Persistence.Entities;

namespace InventoryService.Infrastructure.Persistence.Configurations;

public sealed class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovementEntity>
{
    public void Configure(EntityTypeBuilder<InventoryMovementEntity> builder)
    {
        builder.ToTable("inventory_movements");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.InventoryId).HasColumnName("inventory_id").IsRequired();
        builder.Property(x => x.MovementType).HasColumnName("movement_type").IsRequired();
        builder.Property(x => x.FromDepartment).HasColumnName("from_department").HasMaxLength(100);
        builder.Property(x => x.ToDepartment).HasColumnName("to_department").HasMaxLength(100);
        builder.Property(x => x.FromLocation).HasColumnName("from_location").HasMaxLength(200);
        builder.Property(x => x.ToLocation).HasColumnName("to_location").HasMaxLength(200);
        builder.Property(x => x.OccurredAt).HasColumnName("occurred_at").IsRequired();

        builder.HasIndex(x => x.InventoryId).HasDatabaseName("ix_inventory_movements_inventory_id");
        builder.HasIndex(x => new { x.InventoryId, x.OccurredAt }).HasDatabaseName("ix_inventory_movements_inventory_occurred");
    }
}
