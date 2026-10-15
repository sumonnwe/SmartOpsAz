//csharp src/InventoryService/InventoryService.Infrastructure/Persistence/Configurations/InventoryItemConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryService.Infrastructure.Persistence.Entities;

namespace InventoryService.Infrastructure.Persistence.Configurations;

internal sealed class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItemEntity>
{
    public void Configure(EntityTypeBuilder<InventoryItemEntity> builder)
    {
        builder.ToTable("inventory_items");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();

        builder.Property(x => x.AssetTag).HasColumnName("asset_tag").IsRequired().HasMaxLength(100);
        builder.Property(x => x.SerialNumber).HasColumnName("serial_number").IsRequired().HasMaxLength(100);
        builder.Property(x => x.DeviceName).HasColumnName("device_name").IsRequired().HasMaxLength(200);
        builder.Property(x => x.DeviceType).HasColumnName("device_type").HasMaxLength(100);
        builder.Property(x => x.Department).HasColumnName("department").HasMaxLength(100);
        builder.Property(x => x.Location).HasColumnName("location").HasMaxLength(200);

        builder.Property(x => x.Status).HasColumnName("status").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasIndex(x => x.AssetTag).IsUnique().HasDatabaseName("ux_inventory_asset_tag");
        builder.HasIndex(x => x.SerialNumber).IsUnique().HasDatabaseName("ux_inventory_serial_number");
    }
}