namespace DeviceService.Domain.Events;

public sealed class DeviceRegisteredEvent : DomainEvent
{
    public Guid DeviceId { get; }
    public string DeviceCode { get; }
    public string Name { get; }

    public DeviceRegisteredEvent(Guid deviceId, string deviceCode, string name)
    {
        DeviceId = deviceId;
        DeviceCode = deviceCode;
        Name = name;
    }
}