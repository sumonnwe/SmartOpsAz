using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDeviceCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "device_heartbeats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BatteryPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    SignalStrengthRssi = table.Column<int>(type: "integer", nullable: true),
                    CpuUsagePercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    MemoryUsagePercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    FirmwareVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReceivedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_heartbeats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "iot_devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DeviceType = table.Column<int>(type: "integer", nullable: false),
                    GatewayId = table.Column<Guid>(type: "uuid", nullable: true),
                    LinkedAssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    FirmwareVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    BatteryPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    SignalStrengthRssi = table.Column<int>(type: "integer", nullable: true),
                    CpuUsagePercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    MemoryUsagePercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    LastHeartbeatAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RegisteredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    HasPublishedOfflineEvent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_iot_devices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_device_heartbeats_DeviceId",
                table: "device_heartbeats",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_device_heartbeats_DeviceId_RecordedAtUtc",
                table: "device_heartbeats",
                columns: new[] { "DeviceId", "RecordedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_iot_devices_DeviceCode",
                table: "iot_devices",
                column: "DeviceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_iot_devices_GatewayId",
                table: "iot_devices",
                column: "GatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_iot_devices_LastHeartbeatAtUtc",
                table: "iot_devices",
                column: "LastHeartbeatAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_iot_devices_LinkedAssetId",
                table: "iot_devices",
                column: "LinkedAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_iot_devices_Status",
                table: "iot_devices",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device_heartbeats");

            migrationBuilder.DropTable(
                name: "iot_devices");
        }
    }
}
