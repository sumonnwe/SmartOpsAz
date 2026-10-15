namespace DeviceService.Domain.Events;

public sealed class DeviceRecoveredEvent : DomainEvent
{
    public Guid DeviceId { get; }
    public string DeviceCode { get; }
    public DateTimeOffset LastHeartbeatAtUtc { get; }

    public DeviceRecoveredEvent(Guid deviceId, string deviceCode, DateTimeOffset lastHeartbeatAtUtc)
    {
        DeviceId = deviceId;
        DeviceCode = deviceCode;
        LastHeartbeatAtUtc = lastHeartbeatAtUtc;
    }
}