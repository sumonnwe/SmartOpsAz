namespace SmartOpsAz.IntegrationEvents.Events;

/// <summary>
/// Base class for all integration events published across microservices.
/// Provides standard metadata for event correlation and causation tracking.
/// </summary>
public abstract class IntegrationEventBase
{
    /// <summary>
    /// Unique identifier for this event instance.
    /// </summary>
    public Guid EventId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Fully qualified event type name (e.g., "device.device-offline.v1").
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Event schema version for backward compatibility.
    /// </summary>
    public int EventVersion { get; set; } = 1;

    /// <summary>
    /// UTC timestamp when the event occurred in the source domain.
    /// </summary>
    public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Correlation identifier for tracing related events across services.
    /// </summary>
    public Guid CorrelationId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Causation identifier linking this event to its originating command/event.
    /// </summary>
    public Guid? CausationId { get; set; }

    /// <summary>
    /// Name of the microservice producing this event.
    /// </summary>
    public string Producer { get; set; } = "Unknown";

    /// <summary>
    /// Reserved for future multi-tenant support.
    /// </summary>
    public string? TenantId { get; set; }
}