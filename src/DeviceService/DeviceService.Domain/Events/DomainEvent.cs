namespace DeviceService.Domain.Events;

public abstract class DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAtUtc { get; } = DateTimeOffset.UtcNow;
}