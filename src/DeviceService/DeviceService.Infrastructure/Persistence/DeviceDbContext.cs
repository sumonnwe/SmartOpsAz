using DeviceService.Domain.Entities;
using DeviceService.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace DeviceService.Infrastructure.Persistence;

public class DeviceDbContext : DbContext
{
    public DeviceDbContext(DbContextOptions<DeviceDbContext> options) : base(options) { }

    public DbSet<IoTDevice> Devices { get; set; } = null!;
    public DbSet<DeviceHeartbeat> DeviceHeartbeats { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IoTDevice>(b =>
        {
            b.ToTable("iot_devices");
            b.HasKey(x => x.Id);

            b.Property(x => x.DeviceCode)
                .IsRequired()
                .HasMaxLength(200);
            b.HasIndex(x => x.DeviceCode).IsUnique();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            b.Property(x => x.DeviceType)
                .HasConversion<int>()
                .IsRequired();

            b.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();
            b.HasIndex(x => x.Status);

            b.Property(x => x.FirmwareVersion)
                .HasMaxLength(50);

            b.Property(x => x.RegisteredAtUtc).IsRequired();
            b.Property(x => x.LastHeartbeatAtUtc);
            b.Property(x => x.UpdatedAtUtc);

            b.HasIndex(x => x.LastHeartbeatAtUtc);
            b.HasIndex(x => x.GatewayId);
            b.HasIndex(x => x.LinkedAssetId);

            // Explicitly ignore the domain events collection - not persisted to database
            b.Ignore(x => x.Events);
        });

        modelBuilder.Entity<DeviceHeartbeat>(b =>
        {
            b.ToTable("device_heartbeats");
            b.HasKey(x => x.Id);

            b.Property(x => x.DeviceId).IsRequired();
            b.Property(x => x.RecordedAtUtc).IsRequired();
            b.Property(x => x.ReceivedAtUtc).IsRequired();

            b.HasIndex(x => new { x.DeviceId, x.RecordedAtUtc });
            b.HasIndex(x => x.DeviceId);

            b.Property(x => x.FirmwareVersion).HasMaxLength(50);
        });

        // Explicitly configure DomainEvent base type as not mapped
        modelBuilder.Ignore<DomainEvent>();
    }
}