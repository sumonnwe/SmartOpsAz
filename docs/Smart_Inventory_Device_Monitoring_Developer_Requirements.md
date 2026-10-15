# Smart Inventory & Device Monitoring Platform

## Developer-Ready Software Requirements Specification

**Document Type:** Functional and Technical Requirements Specification  
**Version:** 1.0  
**Project Duration:** 1 Month  
**Target Implementation:** Portfolio-grade enterprise sample  
**Primary Technology Stack:** .NET 8, PostgreSQL, RabbitMQ, MassTransit, Redis, Docker, Azure, GitHub Actions  
**Architecture Style:** Domain-Driven Design, Clean Architecture, Microservices, Event-Driven Architecture  

---

# 1. Purpose

This document converts the Product Manager's business requirements into a developer-ready specification.

It defines:

- What the engineering team must build.
- Which microservices are required.
- The responsibilities and boundaries of each service.
- API endpoints and request/response models.
- Domain entities, business rules, and validation rules.
- Events, message topics, publishers, and subscribers.
- Database, caching, observability, security, testing, deployment, and CI/CD requirements.
- Local and cloud execution steps.
- System evaluation criteria.

The implementation must demonstrate enterprise software engineering practices while keeping the business workflows manageable within a one-month project.

---

# 2. Business Context

ABC Hospital currently manages medical equipment, sensor readings, door access logs, and maintenance information through disconnected and mostly manual processes.

The proposed system will provide a centralized platform for:

- Medical asset inventory. **
- IoT device health monitoring. ** 
- Temperature monitoring.
- Door access monitoring.
- Alert and notification management.
- Operational dashboard and reporting.
- Auditability and future cloud expansion.

The system must support future modules without requiring major redesign.

Future modules include:

- Asset Reservation.
- Barcode Scanner.
- RFID Tracking.
- Azure IoT Hub.
- Digital Twin.
- Power BI Dashboard.
- Mobile Application.
- Multi-Hospital Support.
- Multi-Tenant Architecture.
- Kubernetes Deployment.
- Event Sourcing.
- Saga Pattern.
- Cosmos DB.
- Azure Functions.

---

# 3. Project Scope

## 3.1 In Scope for the One-Month Project

The following capabilities must be implemented with working business logic:

1. Authentication and authorization. ** 
2. Medical asset inventory management.  ** 
3. Department and location assignment. ** 
4. Asset transfer and retirement. ** 
5. IoT device registration and asset linking. ** 
6. Device heartbeat processing. ** 
7. Automatic offline-device detection. ** 
8. Temperature reading ingestion and threshold evaluation.
9. Door access event ingestion and audit logging.
10. Alert creation, acknowledgement, resolution, and escalation state.
11. Notification processing through simulated providers.
12. Dashboard summary and health indicators.
13. API documentation.
14. Docker-based local execution.
15. PostgreSQL persistence.
16. RabbitMQ event-driven communication.
17. Redis caching.
18. Azure deployment.
19. Application Insights and OpenTelemetry.
20. Unit and integration tests.
21. GitHub Actions CI/CD.
22. Architecture documentation and ADRs.

## 3.2 Out of Scope for the One-Month Project

The following are not required as complete production implementations:

- Patient records.
- Billing.
- EMR or HIS integration.
- Appointment management.
- Finance and HR.
- Real SMS gateway.
- Real push notification provider.
- Production Microsoft Teams tenant integration.
- Physical RFID readers.
- Physical barcode scanner integration.
- Real hospital door controller integration.
- Real Azure IoT Hub device provisioning.
- Production-grade predictive maintenance.
- Multi-tenant billing or subscription management.
- Full event sourcing.
- Distributed Saga transactions.
- Kubernetes production cluster.

Interfaces and extension points should be designed so these can be added later.

---

# 4. A) What You Will Build

## 4.1 Required Deliverables

The engineering team must deliver a working repository containing:

1** . **Identity Service**   
   - User authentication. **
   - JWT issuance.**
   - Role-based authorization.

2** . **Inventory Service**
   - Medical asset CRUD. **
   - Asset assignment and transfer. **
   - Asset retirement. **
   - Asset history. **

3** . **Device Monitoring Service**
   - IoT device registration.
   - Asset-device linking.
   - Heartbeat ingestion.
   - Device status management.
   - Offline-device detection.

4. **Environment Monitoring Service**
   - Temperature reading ingestion.
   - Threshold configuration.
   - Warning and critical evaluation.
   - Temperature alert generation.

5. **Access Monitoring Service**
   - Door event ingestion.
   - Immutable access audit log.
   - Unauthorized and forced-open detection.

6. **Alert Service**
   - Central alert lifecycle.
   - Alert acknowledgement.
   - Alert resolution.
   - Escalation state management.

7. **Notification Worker**
   - Consumes alert events.
   - Sends simulated email, Teams, push, and log notifications.
   - Records delivery attempts and status.

8. **Dashboard Query Service**
   - Aggregated system summary.
   - Redis-backed read model.
   - Inventory, device health, alert, temperature, and door-event widgets.

9. **Shared Event Contracts**
   - Versioned integration event contracts.
   - Message metadata.
   - Correlation and causation identifiers.

10. **Infrastructure**
    - Dockerfiles.
    - Docker Compose.
    - PostgreSQL.
    - RabbitMQ.
    - Redis.
    - Azure Container Registry.
    - Azure App Service or Azure Container Apps.
    - Azure Key Vault.
    - Application Insights.

11. **Quality Assets**
    - Unit tests.
    - Integration tests.
    - API documentation.
    - Architecture diagrams.
    - ADRs.
    - CI/CD workflows.

## 4.2 Deliverable Evaluation by Role

### Solution Architect

The solution architect will be evaluated on:

- Clear bounded contexts.
- Correct service responsibilities.
- Appropriate synchronous and asynchronous communication.
- Extensibility.
- Security boundaries.
- Reliability strategy.
- Observability.
- Deployment design.
- Documented trade-offs.

### Senior Software Engineer

The engineer will be evaluated on:

- Correct domain logic.
- Maintainable code.
- Clean Architecture implementation.
- DDD concepts.
- API quality.
- Event publishing and consumption.
- Validation and error handling.
- Test quality.
- Docker and cloud readiness.
- Documentation clarity.

### DevOps / Cloud Engineering Capability

The solution will be evaluated on:

- Reproducible builds.
- Container health.
- Secret management.
- Automated test execution.
- Automated image publishing.
- Environment-specific configuration.
- Cloud deployment.
- Monitoring and tracing.

---

# 5. High-Level Architecture

```text
                            +-----------------------+
                            | Web / API Consumers   |
                            | Swagger / Future UI   |
                            +-----------+-----------+
                                        |
                                  HTTPS / JWT
                                        |
        +-------------------------------+--------------------------------+
        |                               |                                |
+-------v---------+          +----------v----------+          +----------v----------+
| Identity Service|          | Inventory Service  |          | Dashboard Service   |
+-----------------+          +----------+----------+          +----------+----------+
                                        |                                |
                                        | Publish Events                 | Redis Read Model
                                        v                                |
                              +---------+----------+                     |
                              | RabbitMQ / MassTransit|<-----------------+
                              +----+-----------+----+
                                   |           |
               +-------------------+           +-------------------+
               |                                                   |
      +--------v---------+                               +---------v----------+
      | Device Service   |                               | Alert Service      |
      +--------+---------+                               +---------+----------+
               |                                                   |
      +--------v---------+                               +---------v----------+
      | Env. Monitoring  |                               | Notification Worker|
      +------------------+                               +--------------------+
               |
      +--------v---------+
      | Access Monitoring|
      +------------------+
```

## 5.1 Communication Rules

- Client-to-service communication uses REST over HTTPS.
- Internal commands requiring an immediate response may use REST.
- Cross-service state propagation uses RabbitMQ events.
- A service must not directly write to another service's database.
- Each service owns its own schema.
- Eventual consistency is accepted for dashboard and notification updates.
- Critical local state changes must commit before publishing events.
- The Outbox Pattern is recommended for reliable event publication.

---

# 6. Architecture Principles

## 6.1 Domain-Driven Design

The following bounded contexts are defined:

| Bounded Context | Primary Responsibility |
|---|---|
| Identity | User authentication and roles |     **    
| Inventory | Hospital medical assets and movement history |      **     
| Device Monitoring | IoT devices, connectivity, and heartbeat state |      ** 
| Environment Monitoring | Temperature readings and threshold evaluation |
| Access Monitoring | Door events and immutable audit records |
| Alerting | Alert lifecycle and escalation |
| Notification | Delivery through notification channels |
| Dashboard | Read-optimized operational summaries |

## 6.2 Clean Architecture per Service

Each service should follow this project layout:

```text
src/
  Services/
    Inventory/
      Inventory.Api/
      Inventory.Application/
      Inventory.Domain/
      Inventory.Infrastructure/
tests/
  Inventory.UnitTests/
  Inventory.IntegrationTests/
```

### Domain Layer

Contains:

- Entities.
- Value Objects.
- Aggregates.
- Domain Events.
- Domain Services.
- Business rules.
- Domain exceptions.

Must not reference:

- EF Core.
- RabbitMQ.
- ASP.NET Core.
- Redis.
- Azure SDKs.

### Application Layer

Contains:

- Commands.
- Queries.
- DTOs.
- Validators.
- Interfaces.
- Use cases.
- Authorization policies.
- Mapping.

### Infrastructure Layer

Contains:

- EF Core DbContext.
- Repository implementations.
- MassTransit publishers and consumers.
- Redis adapters.
- Email, Teams, and AI provider adapters.
- Key Vault integration.
- OpenTelemetry exporters.

### API Layer

Contains:

- Controllers or Minimal APIs.
- Authentication configuration.
- Dependency injection.
- Middleware.
- Swagger.
- Health checks.
- Rate limiting.
- API versioning.

---

# 7. B) Detailed Service Requirements

# 7.1 Identity Service

## 7.1.1 Responsibility

The Identity Service authenticates users and issues JWT access tokens.

For this one-month project, user administration may be seeded or exposed through a limited Admin endpoint.

## 7.1.2 Roles

- HospitalAdministrator
- BiomedicalEngineer
- Nurse
- SecurityOfficer
- MaintenanceEngineer
- Auditor

## 7.1.3 API Endpoints

| Method | Endpoint | Authorization | Description |
|---|---|---|---|
| POST | `/api/v1/auth/login` | Anonymous | Authenticate a user |
| GET | `/api/v1/auth/me` | Authenticated | Return current user |
| POST | `/api/v1/users` | HospitalAdministrator | Create user |
| GET | `/api/v1/users/{userId}` | HospitalAdministrator | Get user |
| GET | `/api/v1/users` | HospitalAdministrator | List users |
| PATCH | `/api/v1/users/{userId}/status` | HospitalAdministrator | Activate/deactivate user |

## 7.1.4 Models

### LoginRequest

```csharp
public sealed record LoginRequest(
    string Email,
    string Password
);
```

### LoginResponse

```csharp
public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc,
    UserSummary User
);
```

### User

| Field | Type | Required | Rule |
|---|---|---:|---|
| Id | Guid | Yes | Generated by system |
| Email | string | Yes | Unique, valid email |
| PasswordHash | string | Yes | Never returned by API |
| FullName | string | Yes | 2-150 characters |
| Role | UserRole enum | Yes | Must be supported role |
| IsActive | bool | Yes | Default true |
| CreatedAtUtc | DateTimeOffset | Yes | UTC |
| UpdatedAtUtc | DateTimeOffset? | No | UTC |

## 7.1.5 Key Requirements

- Passwords must be hashed.
- JWT must include user ID, email, and role claims.
- Inactive users cannot log in.
- Authorization policies must protect role-specific endpoints.
- JWT signing secret must not be committed to source control.
- Azure Key Vault must store production secrets.

---

# 7.2 Inventory Service

## 7.2.1 Responsibility

The Inventory Service manages hospital medical assets and records every important state change.

## 7.2.2 Supported Asset Types

Examples:

- Wheelchair.
- Infusion Pump.
- ECG Machine.
- Defibrillator.
- Ventilator.
- Ultrasound Machine.
- Portable X-ray.

The model must support adding future types without schema redesign.

## 7.2.3 API Endpoints

| Method | Endpoint | Authorization | Description |
|---|---|---|---|
| POST | `/api/v1/assets` | BiomedicalEngineer, HospitalAdministrator | Register asset |
| GET | `/api/v1/assets/{assetId}` | Authenticated | Get asset |
| GET | `/api/v1/assets` | Authenticated | Search and filter assets |
| PUT | `/api/v1/assets/{assetId}` | BiomedicalEngineer, HospitalAdministrator | Update asset |
| DELETE | `/api/v1/assets/{assetId}` | HospitalAdministrator | Soft delete asset |
| POST | `/api/v1/assets/{assetId}/assign` | BiomedicalEngineer | Assign to department/location |
| POST | `/api/v1/assets/{assetId}/transfer` | BiomedicalEngineer | Transfer asset |
| POST | `/api/v1/assets/{assetId}/retire` | BiomedicalEngineer | Retire asset |
| GET | `/api/v1/assets/{assetId}/history` | Authenticated | Get asset history |
| GET | `/api/v1/departments` | Authenticated | List departments |
| POST | `/api/v1/departments` | HospitalAdministrator | Create department |
| GET | `/api/v1/locations` | Authenticated | List locations |
| POST | `/api/v1/locations` | HospitalAdministrator | Create location |

## 7.2.4 Search Parameters

`GET /api/v1/assets`

Supported query parameters:

- `pageNumber`
- `pageSize`
- `search`
- `assetType`
- `status`
- `departmentId`
- `locationId`
- `maintenanceDueBefore`
- `isDeleted`
- `sortBy`
- `sortDirection`

## 7.2.5 Asset Model

```csharp
public sealed class MedicalAsset
{
    public Guid Id { get; private set; }
    public string AssetCode { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public AssetType Type { get; private set; }
    public string SerialNumber { get; private set; } = default!;
    public string? Manufacturer { get; private set; }
    public string? Model { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public Guid? LocationId { get; private set; }
    public DateOnly? PurchaseDate { get; private set; }
    public DateOnly? WarrantyExpiryDate { get; private set; }
    public DateOnly? LastMaintenanceDate { get; private set; }
    public DateOnly? NextMaintenanceDate { get; private set; }
    public AssetStatus Status { get; private set; }
    public string? VendorName { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = default!;
}
```

## 7.2.6 AssetStatus

```csharp
public enum AssetStatus
{
    Available = 1,
    InUse = 2,
    Maintenance = 3,
    Offline = 4,
    Retired = 5
}
```

## 7.2.7 CreateAssetRequest

```csharp
public sealed record CreateAssetRequest(
    string AssetCode,
    string Name,
    AssetType Type,
    string SerialNumber,
    string? Manufacturer,
    string? Model,
    Guid? DepartmentId,
    Guid? LocationId,
    DateOnly? PurchaseDate,
    DateOnly? WarrantyExpiryDate,
    DateOnly? NextMaintenanceDate,
    string? VendorName
);
```

## 7.2.8 TransferAssetRequest

```csharp
public sealed record TransferAssetRequest(
    Guid ToDepartmentId,
    Guid ToLocationId,
    string Reason
);
```

## 7.2.9 Asset History Model

| Field | Type |
|---|---|
| Id | Guid |
| AssetId | Guid |
| Action | AssetHistoryAction enum |
| FromDepartmentId | Guid? |
| ToDepartmentId | Guid? |
| FromLocationId | Guid? |
| ToLocationId | Guid? |
| PreviousStatus | AssetStatus? |
| NewStatus | AssetStatus? |
| Reason | string? |
| PerformedByUserId | Guid |
| OccurredAtUtc | DateTimeOffset |

## 7.2.10 Business Rules

- AssetCode must be unique.
- SerialNumber must be unique.
- AssetCode cannot be changed after registration.
- Retired assets cannot be assigned or transferred.
- Soft-deleted assets cannot be modified.
- Asset movement must create an AssetHistory record.
- Asset transfer must specify a reason.
- Asset retirement can only be performed by BiomedicalEngineer.
- Retired assets cannot return to Available in Phase 1.
- An asset with overdue maintenance must be visible on the dashboard.
- Updates must use optimistic concurrency.
- Successful creation publishes `inventory.asset-created.v1`.
- Successful transfer publishes `inventory.asset-transferred.v1`.
- Successful retirement publishes `inventory.asset-retired.v1`.

---

# 7.3 Device Monitoring Service

## 7.3.1 Responsibility

The Device Monitoring Service manages connected IoT devices and evaluates their connectivity state.

## 7.3.2 Supported Device Types

- TemperatureSensor.
- HumiditySensor.
- Gateway.
- DoorController.
- BatteryDevice.
- NetworkDevice.
- MedicalEquipmentController.

## 7.3.3 API Endpoints

| Method | Endpoint | Authorization | Description |
|---|---|---|---|
| POST | `/api/v1/devices` | BiomedicalEngineer, HospitalAdministrator | Register device |
| GET | `/api/v1/devices/{deviceId}` | Authenticated | Get device |
| GET | `/api/v1/devices` | Authenticated | Search devices |
| PUT | `/api/v1/devices/{deviceId}` | BiomedicalEngineer | Update device |
| POST | `/api/v1/devices/{deviceId}/link-asset` | BiomedicalEngineer | Link to medical asset |
| DELETE | `/api/v1/devices/{deviceId}/link-asset` | BiomedicalEngineer | Unlink asset |
| POST | `/api/v1/devices/{deviceId}/heartbeat` | DeviceApiKey or Simulator | Ingest heartbeat |
| GET | `/api/v1/devices/{deviceId}/heartbeats` | BiomedicalEngineer | Get heartbeat history |
| POST | `/api/v1/devices/{deviceId}/maintenance` | BiomedicalEngineer | Put device in maintenance |
| POST | `/api/v1/devices/{deviceId}/activate` | BiomedicalEngineer | Activate device |

## 7.3.4 IoTDevice Model

| Field | Type | Required | Rule |
|---|---|---:|---|
| Id | Guid | Yes | Generated |
| DeviceCode | string | Yes | Unique |
| Name | string | Yes | 2-150 chars |
| DeviceType | DeviceType enum | Yes | Supported value |
| GatewayId | Guid? | No | Required for child sensor |
| LinkedAssetId | Guid? | No | Asset reference |
| FirmwareVersion | string? | No | Max 50 chars |
| Status | DeviceStatus enum | Yes | Default Offline |
| BatteryPercentage | decimal? | No | 0-100 |
| SignalStrengthRssi | int? | No | Expected -120 to 0 |
| CpuUsagePercentage | decimal? | No | 0-100 |
| MemoryUsagePercentage | decimal? | No | 0-100 |
| LastHeartbeatAtUtc | DateTimeOffset? | No | Updated by heartbeat |
| RegisteredAtUtc | DateTimeOffset | Yes | UTC |
| UpdatedAtUtc | DateTimeOffset? | No | UTC |

## 7.3.5 DeviceStatus

```csharp
public enum DeviceStatus
{
    Online = 1,
    Offline = 2,
    Warning = 3,
    Critical = 4,
    Maintenance = 5
}
```

## 7.3.6 HeartbeatRequest

```csharp
public sealed record HeartbeatRequest(
    DateTimeOffset RecordedAtUtc,
    decimal? BatteryPercentage,
    int? SignalStrengthRssi,
    decimal? CpuUsagePercentage,
    decimal? MemoryUsagePercentage,
    string? FirmwareVersion
);
```

## 7.3.7 Key Requirements

- A child IoT device belongs to one gateway.
- A gateway may have many child devices.
- A device may link to zero or one medical asset.
- Heartbeat frequency is expected once per minute.
- Device becomes Online after a valid heartbeat unless it is in Maintenance.
- No heartbeat for more than five minutes changes status to Offline.
- The offline check must run through a HostedService every minute.
- Transition to Offline must create only one active offline incident.
- Repeated checks must not create duplicate alerts.
- A new heartbeat after Offline publishes `device.device-recovered.v1`.
- An offline transition publishes `device.device-offline.v1`.
- Heartbeat events may be published selectively or sampled to avoid message volume.
- Invalid metric ranges must return validation errors.

---

# 7.4 Environment Monitoring Service

## 7.4.1 Responsibility

The Environment Monitoring Service receives temperature readings, checks thresholds, and generates alerts.

## 7.4.2 API Endpoints

| Method | Endpoint | Authorization | Description |
|---|---|---|---|
| POST | `/api/v1/temperature-readings` | DeviceApiKey or Simulator | Submit reading |
| GET | `/api/v1/temperature-readings` | Authenticated | Query readings |
| GET | `/api/v1/temperature-readings/latest` | Authenticated | Latest values by sensor |
| POST | `/api/v1/temperature-thresholds` | BiomedicalEngineer, HospitalAdministrator | Configure threshold |
| GET | `/api/v1/temperature-thresholds` | Authenticated | List thresholds |
| PUT | `/api/v1/temperature-thresholds/{thresholdId}` | BiomedicalEngineer | Update threshold |
| GET | `/api/v1/temperature-excursions` | Authenticated | List excursions |

## 7.4.3 TemperatureReading Model

| Field | Type |
|---|---|
| Id | Guid |
| SensorDeviceId | Guid |
| LocationId | Guid |
| TemperatureCelsius | decimal |
| HumidityPercentage | decimal? |
| RecordedAtUtc | DateTimeOffset |
| ReceivedAtUtc | DateTimeOffset |
| Severity | TemperatureSeverity |
| CorrelationId | Guid |

## 7.4.4 TemperatureThreshold Model

| Field | Type |
|---|---|
| Id | Guid |
| LocationId | Guid |
| SensorDeviceId | Guid? |
| MinimumWarningCelsius | decimal? |
| MaximumWarningCelsius | decimal? |
| MinimumCriticalCelsius | decimal? |
| MaximumCriticalCelsius | decimal? |
| EffectiveFromUtc | DateTimeOffset |
| IsActive | bool |
| CreatedByUserId | Guid |

## 7.4.5 TemperatureSeverity

```csharp
public enum TemperatureSeverity
{
    Normal = 1,
    Warning = 2,
    Critical = 3
}
```

## 7.4.6 SubmitTemperatureReadingRequest

```csharp
public sealed record SubmitTemperatureReadingRequest(
    Guid SensorDeviceId,
    Guid LocationId,
    decimal TemperatureCelsius,
    decimal? HumidityPercentage,
    DateTimeOffset RecordedAtUtc
);
```

## 7.4.7 Key Requirements

- Temperature should be sampled once per minute.
- The service must use the active threshold for the location or sensor.
- Sensor-specific threshold overrides location threshold.
- A reading beyond warning limits is Warning.
- A reading beyond critical limits is Critical.
- A Critical reading must publish `environment.temperature-critical.v1`.
- A Warning reading must publish `environment.temperature-warning.v1`.
- Duplicate readings with the same sensor and timestamp must be rejected or treated idempotently.
- Readings must remain queryable for the project demonstration.
- Alert creation must occur within 30 seconds of ingestion under normal test load.
- Threshold changes must be audited.
- For Phase 1, default sample thresholds may be seeded, but production values must be configurable.

---

# 7.5 Access Monitoring Service

## 7.5.1 Responsibility

The Access Monitoring Service records hospital door access events and detects security incidents.

## 7.5.2 Door Event Types

- DoorOpened.
- DoorClosed.
- ForcedOpen.
- DoorHeldOpen.
- UnauthorizedAccess.
- CardAccessGranted.
- CardAccessDenied.
- EmergencyUnlock.

## 7.5.3 API Endpoints

| Method | Endpoint | Authorization | Description |
|---|---|---|---|
| POST | `/api/v1/door-events` | DoorControllerApiKey or Simulator | Submit door event |
| GET | `/api/v1/door-events` | SecurityOfficer, Auditor, HospitalAdministrator | Search door events |
| GET | `/api/v1/door-events/{eventId}` | SecurityOfficer, Auditor | Get event |
| POST | `/api/v1/doors` | HospitalAdministrator | Register door |
| GET | `/api/v1/doors` | Authenticated | List doors |
| PUT | `/api/v1/doors/{doorId}` | HospitalAdministrator | Update door |
| GET | `/api/v1/doors/{doorId}/status` | SecurityOfficer | Get current door status |

## 7.5.4 DoorAccessEvent Model

| Field | Type |
|---|---|
| Id | Guid |
| ExternalEventId | string |
| DoorId | Guid |
| EventType | DoorEventType |
| CardNumberMasked | string? |
| UserReference | string? |
| DepartmentId | Guid? |
| OccurredAtUtc | DateTimeOffset |
| ReceivedAtUtc | DateTimeOffset |
| DoorOpenDurationSeconds | int? |
| IsAuthorized | bool? |
| RawSource | string? |
| CorrelationId | Guid |

## 7.5.5 Key Requirements

- Door access logs are append-only.
- Existing events cannot be updated or deleted through the API.
- ExternalEventId must be unique for idempotency.
- ForcedOpen and UnauthorizedAccess must generate Critical alerts.
- DoorHeldOpen generates Warning or Critical depending on configured duration.
- Card numbers must be masked in API responses.
- EmergencyUnlock must be logged and published.
- Security events must publish `access.security-incident-detected.v1`.
- Normal access events may publish `access.door-event-recorded.v1`.
- Only authorized roles can query full history.

---

# 7.6 Alert Service

## 7.6.1 Responsibility

The Alert Service owns the lifecycle of operational alerts created from device, temperature, access, and maintenance events.

## 7.6.2 API Endpoints

| Method | Endpoint | Authorization | Description |
|---|---|---|---|
| GET | `/api/v1/alerts` | Authenticated | Search alerts |
| GET | `/api/v1/alerts/{alertId}` | Authenticated | Get alert |
| POST | `/api/v1/alerts/{alertId}/acknowledge` | Authorized operational role | Acknowledge alert |
| POST | `/api/v1/alerts/{alertId}/resolve` | Authorized operational role | Resolve alert |
| POST | `/api/v1/alerts/{alertId}/reopen` | HospitalAdministrator | Reopen alert |
| GET | `/api/v1/alerts/{alertId}/history` | Auditor, HospitalAdministrator | Alert history |
| GET | `/api/v1/alert-rules` | Authenticated | List rules |
| PUT | `/api/v1/alert-rules/{ruleId}` | HospitalAdministrator | Update rule |

## 7.6.3 Alert Model

| Field | Type |
|---|---|
| Id | Guid |
| AlertCode | string |
| SourceType | AlertSourceType |
| SourceId | Guid |
| Severity | AlertSeverity |
| Status | AlertStatus |
| Title | string |
| Description | string |
| LocationId | Guid? |
| DepartmentId | Guid? |
| RaisedAtUtc | DateTimeOffset |
| AcknowledgedAtUtc | DateTimeOffset? |
| AcknowledgedByUserId | Guid? |
| ResolvedAtUtc | DateTimeOffset? |
| ResolvedByUserId | Guid? |
| ResolutionNote | string? |
| EscalationLevel | int |
| CorrelationId | Guid |
| DeduplicationKey | string |

## 7.6.4 Enums

```csharp
public enum AlertSeverity
{
    Information = 1,
    Warning = 2,
    Critical = 3,
    Emergency = 4
}

public enum AlertStatus
{
    Open = 1,
    Acknowledged = 2,
    Resolved = 3,
    Reopened = 4
}
```

## 7.6.5 Key Requirements

- Every operational alert must have a unique DeduplicationKey.
- The same unresolved issue must not produce duplicate alerts.
- All alerts require acknowledgement.
- Alert acknowledgement stores user and timestamp.
- Only acknowledged or open alerts may be resolved.
- Resolution requires a note.
- Alert history must record every state transition.
- Critical alerts must trigger notification immediately.
- Escalation intervals are configurable: 1, 5, 10, and 30 minutes.
- A background worker evaluates unacknowledged alerts.
- Alert creation publishes `alert.alert-raised.v1`.
- Alert acknowledgement publishes `alert.alert-acknowledged.v1`.
- Alert resolution publishes `alert.alert-resolved.v1`.
- Escalation publishes `alert.alert-escalated.v1`.

---

# 7.7 Notification Worker

## 7.7.1 Responsibility

The Notification Worker consumes alert events and sends messages through configured providers.

## 7.7.2 Supported Channels in Phase 1

- Log.
- Email simulation.
- Microsoft Teams webhook simulation.
- Push notification simulation.

## 7.7.3 Internal Processing

```text
AlertRaised event
    -> Resolve notification rule
    -> Create notification request
    -> Send through one or more providers
    -> Record delivery attempt
    -> Publish NotificationSent or NotificationFailed
```

## 7.7.4 NotificationDelivery Model

| Field | Type |
|---|---|
| Id | Guid |
| AlertId | Guid |
| Channel | NotificationChannel |
| Recipient | string |
| Subject | string |
| Message | string |
| Status | NotificationStatus |
| AttemptCount | int |
| LastAttemptAtUtc | DateTimeOffset? |
| SentAtUtc | DateTimeOffset? |
| FailureReason | string? |
| CorrelationId | Guid |

## 7.7.5 Key Requirements

- Consumers must be idempotent.
- Failed notifications must retry with exponential backoff.
- A dead-letter queue must capture exhausted messages.
- The provider interface must support future real providers.
- No sensitive secrets may appear in logs.
- Successful delivery publishes `notification.notification-sent.v1`.
- Permanent failure publishes `notification.notification-failed.v1`.

---

# 7.8 Dashboard Query Service

## 7.8.1 Responsibility

The Dashboard Service provides a read-optimized operational view.

It must not query every service database for each request.

It consumes events and updates a Redis-backed read model.

## 7.8.2 API Endpoints

| Method | Endpoint | Authorization | Description |
|---|---|---|---|
| GET | `/api/v1/dashboard/summary` | Authenticated | Main dashboard |
| GET | `/api/v1/dashboard/inventory` | Authenticated | Asset widgets |
| GET | `/api/v1/dashboard/device-health` | Authenticated | Device widgets |
| GET | `/api/v1/dashboard/temperature` | Authenticated | Temperature widgets |
| GET | `/api/v1/dashboard/door-events` | SecurityOfficer, HospitalAdministrator | Door widgets |
| GET | `/api/v1/dashboard/alerts` | Authenticated | Alert widgets |
| GET | `/api/v1/dashboard/maintenance` | BiomedicalEngineer | Maintenance widgets |
| GET | `/api/v1/dashboard/ai-summary` | Authenticated | AI-generated summary abstraction |

## 7.8.3 DashboardSummaryResponse

```csharp
public sealed record DashboardSummaryResponse(
    int TotalAssets,
    int AvailableAssets,
    int AssetsInUse,
    int AssetsInMaintenance,
    int OverdueMaintenanceAssets,
    int TotalDevices,
    int OnlineDevices,
    int OfflineDevices,
    int WarningDevices,
    int CriticalDevices,
    int OpenAlerts,
    int CriticalAlerts,
    int TemperatureWarnings,
    int TemperatureCriticals,
    int TodayDoorEvents,
    int TodaySecurityIncidents,
    DateTimeOffset GeneratedAtUtc
);
```

## 7.8.4 Key Requirements

- Main dashboard response target is below three seconds.
- Redis is used for cached read models.
- Consumers update counters from domain integration events.
- The dashboard must tolerate temporary RabbitMQ or Redis unavailability.
- A rebuild endpoint is not required for public use, but an internal rebuild process should be documented.
- AI summary must use an abstraction such as `IOperationalSummaryGenerator`.
- A deterministic fallback summary must be returned when no AI provider is configured.
- Production AI credentials must be loaded through Key Vault.

---

# 8. Shared API Requirements

## 8.1 Response Format

Successful single-item response:

```json
{
  "data": {},
  "meta": {
    "correlationId": "GUID",
    "timestampUtc": "2026-08-04T08:00:00Z"
  }
}
```

Validation error response:

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Validation failed",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "instance": "/api/v1/assets",
  "correlationId": "GUID",
  "errors": {
    "serialNumber": [
      "Serial number is required."
    ]
  }
}
```

## 8.2 HTTP Status Rules

- `200 OK` for successful query or action.
- `201 Created` for newly created resource.
- `204 No Content` for successful deletion or command with no body.
- `400 Bad Request` for validation failure.
- `401 Unauthorized` for missing or invalid token.
- `403 Forbidden` for insufficient role.
- `404 Not Found` for missing resource.
- `409 Conflict` for duplicates or invalid state transition.
- `412 Precondition Failed` for concurrency mismatch if ETag is used.
- `429 Too Many Requests` for rate limit.
- `500 Internal Server Error` for unexpected errors.

## 8.3 Cross-Cutting API Requirements

- API version prefix: `/api/v1`.
- Swagger/OpenAPI enabled.
- XML comments or endpoint descriptions required.
- FluentValidation required.
- Global exception middleware required.
- Correlation ID middleware required.
- Structured logging required.
- Rate limiting required.
- Pagination required for list endpoints.
- UTC timestamps required.
- Sensitive values must be masked.
- HTTPS required in cloud.
- Health endpoints must be anonymous but expose minimal information.

---

# 9. C) Event-Driven Communication Design

# 9.1 Event Naming

Use the following convention:

```text
<bounded-context>.<event-name>.v<version>
```

Examples:

- `inventory.asset-created.v1`
- `device.device-offline.v1`
- `environment.temperature-critical.v1`
- `access.security-incident-detected.v1`
- `alert.alert-raised.v1`

## 9.2 Standard Integration Event Envelope

```csharp
public sealed record IntegrationEvent<TPayload>(
    Guid EventId,
    string EventType,
    int EventVersion,
    DateTimeOffset OccurredAtUtc,
    Guid CorrelationId,
    Guid? CausationId,
    string Producer,
    string? TenantId,
    TPayload Data
);
```

## 9.3 Required Metadata

Every event must include:

- EventId.
- EventType.
- EventVersion.
- OccurredAtUtc.
- CorrelationId.
- CausationId where applicable.
- Producer.
- Payload.
- TenantId field reserved for future multi-tenancy.

## 9.4 Required Events

| Topic / Event | Publisher | Subscribers | Purpose |
|---|---|---|---|
| `inventory.asset-created.v1` | Inventory Service | Dashboard | Add asset to summary |
| `inventory.asset-updated.v1` | Inventory Service | Dashboard | Refresh asset read model |
| `inventory.asset-transferred.v1` | Inventory Service | Dashboard, Audit consumers | Update department/location |
| `inventory.asset-retired.v1` | Inventory Service | Dashboard, Device Service | Remove from active inventory |
| `inventory.maintenance-overdue.v1` | Inventory Service worker | Alert Service, Dashboard | Raise maintenance alert |
| `device.heartbeat-received.v1` | Device Service | Dashboard | Update last seen and metrics |
| `device.device-offline.v1` | Device Service | Alert Service, Dashboard | Raise offline alert |
| `device.device-recovered.v1` | Device Service | Alert Service, Dashboard | Resolve or update offline state |
| `environment.temperature-warning.v1` | Environment Service | Alert Service, Dashboard | Raise warning |
| `environment.temperature-critical.v1` | Environment Service | Alert Service, Dashboard | Raise critical alert |
| `access.door-event-recorded.v1` | Access Service | Dashboard | Update door count |
| `access.security-incident-detected.v1` | Access Service | Alert Service, Dashboard | Raise security alert |
| `alert.alert-raised.v1` | Alert Service | Notification Worker, Dashboard | Notify and update widgets |
| `alert.alert-acknowledged.v1` | Alert Service | Dashboard | Update alert state |
| `alert.alert-resolved.v1` | Alert Service | Dashboard | Remove active alert |
| `alert.alert-escalated.v1` | Alert Service | Notification Worker, Dashboard | Send escalated notification |
| `notification.notification-sent.v1` | Notification Worker | Dashboard, Audit | Record delivery |
| `notification.notification-failed.v1` | Notification Worker | Dashboard, Audit | Record failure |

## 9.5 Example Event Payloads

### AssetTransferred

```json
{
  "eventId": "72d1794a-54d0-4d95-a0e6-09fbb879e750",
  "eventType": "inventory.asset-transferred.v1",
  "eventVersion": 1,
  "occurredAtUtc": "2026-08-04T08:10:00Z",
  "correlationId": "5986671f-5ac7-4fbf-804a-949fc2c34cc5",
  "causationId": null,
  "producer": "InventoryService",
  "tenantId": null,
  "data": {
    "assetId": "GUID",
    "assetCode": "DEF-0001",
    "fromDepartmentId": "GUID",
    "toDepartmentId": "GUID",
    "fromLocationId": "GUID",
    "toLocationId": "GUID",
    "reason": "Required by ICU",
    "performedByUserId": "GUID"
  }
}
```

### DeviceOffline

```json
{
  "eventId": "GUID",
  "eventType": "device.device-offline.v1",
  "eventVersion": 1,
  "occurredAtUtc": "2026-08-04T08:15:00Z",
  "correlationId": "GUID",
  "causationId": null,
  "producer": "DeviceMonitoringService",
  "tenantId": null,
  "data": {
    "deviceId": "GUID",
    "deviceCode": "TEMP-ICU-001",
    "linkedAssetId": null,
    "locationId": "GUID",
    "lastHeartbeatAtUtc": "2026-08-04T08:09:00Z",
    "offlineThresholdSeconds": 300
  }
}
```

### TemperatureCritical

```json
{
  "eventId": "GUID",
  "eventType": "environment.temperature-critical.v1",
  "eventVersion": 1,
  "occurredAtUtc": "2026-08-04T08:20:00Z",
  "correlationId": "GUID",
  "causationId": null,
  "producer": "EnvironmentMonitoringService",
  "tenantId": null,
  "data": {
    "readingId": "GUID",
    "sensorDeviceId": "GUID",
    "locationId": "GUID",
    "temperatureCelsius": 9.2,
    "minimumCriticalCelsius": 2.0,
    "maximumCriticalCelsius": 8.0,
    "recordedAtUtc": "2026-08-04T08:19:45Z"
  }
}
```

## 9.6 Message Reliability Requirements

- Use durable queues.
- Use persistent messages.
- Use retry policies.
- Configure dead-letter queues.
- Consumers must be idempotent.
- Store processed EventId values where needed.
- Use Outbox Pattern for important events.
- Do not publish integration events before the local transaction commits.
- Preserve CorrelationId across service boundaries.
- Log every consumer failure with message ID and event type.
- Avoid distributed transactions between services.

---

# 10. Data Persistence Design

## 10.1 Database Ownership

Each service owns a separate PostgreSQL database or schema.

Recommended local layout:

- `identity_db`
- `inventory_db`
- `device_db`
- `environment_db`
- `access_db`
- `alert_db`
- `notification_db`

The Dashboard Service primarily uses Redis and may use PostgreSQL for durable snapshots if needed.

## 10.2 EF Core Requirements

- EF Core 8.
- Code-first migrations.
- One DbContext per service.
- Explicit indexes for unique identifiers and frequent filters.
- UTC timestamps.
- Soft delete query filter for assets.
- Concurrency token for mutable aggregates.
- Transaction boundaries around aggregate updates and outbox writes.
- Seed data for departments, locations, roles, sample devices, and thresholds.

## 10.3 Suggested Indexes

### Inventory

- Unique index on AssetCode.
- Unique index on SerialNumber.
- Index on Status.
- Index on DepartmentId.
- Index on LocationId.
- Index on NextMaintenanceDate.

### Device

- Unique index on DeviceCode.
- Index on Status.
- Index on LastHeartbeatAtUtc.
- Index on GatewayId.
- Index on LinkedAssetId.

### Temperature

- Unique composite index on SensorDeviceId and RecordedAtUtc.
- Index on LocationId and RecordedAtUtc.
- Index on Severity.

### Access

- Unique index on ExternalEventId.
- Index on DoorId and OccurredAtUtc.
- Index on EventType.

### Alert

- Unique filtered index on active DeduplicationKey.
- Index on Status.
- Index on Severity.
- Index on RaisedAtUtc.

---

# 11. D) Infrastructure Requirements

# 11.1 Docker Container Integration

Each service must have a multi-stage Dockerfile.

Example pattern:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Inventory.Api.dll"]
```

## 11.2 Docker Compose Components

The local environment must include:

- Identity API.
- Inventory API.
- Device API.
- Environment API.
- Access API.
- Alert API.
- Notification Worker.
- Dashboard API.
- PostgreSQL.
- RabbitMQ with management UI.
- Redis.
- Optional OpenTelemetry Collector.
- Optional Jaeger or Zipkin for local tracing.

## 11.3 Required Container Configuration

- Services share a private Docker network.
- Each service exposes a distinct host port.
- Health checks are configured.
- Startup dependency does not assume readiness from container start alone.
- Services retry PostgreSQL, RabbitMQ, and Redis connections.
- Environment variables provide connection strings and secrets.
- Persistent volumes store PostgreSQL and RabbitMQ data.
- Local development secrets may use `.env`, excluded from Git.

## 11.4 Suggested Local Ports

| Component | Port |
|---|---:|
| Identity API | 5101 |
| Inventory API | 5102 |
| Device API | 5103 |
| Environment API | 5104 |
| Access API | 5105 |
| Alert API | 5106 |
| Dashboard API | 5107 |
| RabbitMQ AMQP | 5672 |
| RabbitMQ Management | 15672 |
| PostgreSQL | 5432 |
| Redis | 6379 |
| Jaeger UI | 16686 |
| OpenTelemetry Collector | 4317 / 4318 |

---

# 12. Azure Deployment Design

## 12.1 Required Azure Resources

- Resource Group.
- Azure Container Registry.
- Azure App Service for Containers or Azure Container Apps.
- Azure Database for PostgreSQL Flexible Server.
- Azure Cache for Redis.
- Azure Key Vault.
- Application Insights.
- Log Analytics Workspace.
- Storage Account if required for diagnostics.
- Optional Azure OpenAI resource for AI summary.
- RabbitMQ hosted container or external managed broker for the portfolio.

## 12.2 Deployment Steps

1. Create a resource group.
2. Create Azure Container Registry.
3. Create PostgreSQL Flexible Server and databases.
4. Create Redis.
5. Create Key Vault.
6. Add secrets:
   - JWT signing key.
   - PostgreSQL connection strings.
   - RabbitMQ credentials.
   - Redis connection string.
   - Teams webhook if used.
   - AI provider credentials if used.
7. Create Application Insights.
8. Create App Service plans and container apps.
9. Configure managed identity.
10. Grant managed identity access to Key Vault.
11. Configure container environment variables.
12. Configure ingress and HTTPS.
13. Push images to ACR.
14. Deploy each service image.
15. Apply EF Core migrations through a controlled deployment step.
16. Verify health endpoints.
17. Execute smoke tests.
18. Confirm logs, metrics, and traces in Application Insights.

## 12.3 Cloud Configuration Requirements

- Secrets must be read from Key Vault.
- Production containers must not use local default passwords.
- HTTPS must be enabled.
- Public endpoints should be limited to required APIs.
- RabbitMQ and PostgreSQL should not be publicly exposed without restrictions.
- CORS must be configured explicitly.
- Application Insights connection string must be injected through configuration.
- Environment name must be set to Production.
- Swagger exposure in production must be configurable.

---

# 13. Monitoring and Observability

## 13.1 Health Checks

Each API must expose:

- `/health/live`
- `/health/ready`

Readiness checks should verify relevant dependencies:

- PostgreSQL.
- RabbitMQ.
- Redis where used.

Liveness should not fail only because an external dependency is temporarily unavailable.

## 13.2 Logging

Use structured logging.

Every log must include when applicable:

- Timestamp.
- Log level.
- Service name.
- CorrelationId.
- TraceId.
- UserId.
- Endpoint.
- EventId.
- EventType.
- EntityId.
- Duration.
- Outcome.

Do not log:

- Passwords.
- JWT tokens.
- Full card numbers.
- Secrets.
- Sensitive connection strings.

## 13.3 OpenTelemetry

Instrument:

- ASP.NET Core requests.
- HttpClient calls.
- EF Core database calls.
- MassTransit publish and consume operations.
- Redis operations where supported.
- Background worker executions.

Export to:

- Console in local development.
- Jaeger or Zipkin locally.
- Application Insights in Azure.

## 13.4 Required Metrics

- Request count.
- Request duration.
- Error count.
- Message publish count.
- Message consume count.
- Consumer failure count.
- Dead-letter count.
- Notification success/failure count.
- Offline device count.
- Active critical alert count.
- Temperature critical event count.
- Database dependency duration.

---

# 14. CI/CD Pipeline

# 14.1 Pull Request Pipeline

Trigger:

- Pull request to `main`.

Steps:

1. Checkout repository.
2. Setup .NET 8.
3. Restore dependencies.
4. Build with warnings treated according to project policy.
5. Run unit tests.
6. Run integration tests.
7. Collect code coverage.
8. Validate formatting.
9. Run static analysis.
10. Build Docker images.
11. Optionally scan container images.
12. Publish test results.

A pull request must fail if:

- Build fails.
- Required tests fail.
- Migration validation fails.
- Docker build fails.

## 14.2 Main Branch Deployment Pipeline

Trigger:

- Merge to `main`.
- Manual production approval recommended.

Steps:

1. Execute all CI checks.
2. Authenticate to Azure using federated credentials.
3. Build versioned Docker images.
4. Tag images with:
   - Commit SHA.
   - Semantic version where available.
   - `latest` for non-production convenience only.
5. Push images to ACR.
6. Deploy to Azure.
7. Apply database migrations.
8. Wait for readiness.
9. Execute smoke tests.
10. Roll back or stop deployment on failure.
11. Publish deployment summary.

## 14.3 Environment Strategy

Recommended:

- Local.
- Development.
- Staging.
- Production demonstration.

At minimum, the one-month project must support Local and Azure Development.

---

# 15. Test Automation Requirements

# 15.1 Unit Tests

Each domain must test:

### Inventory

- Create asset with valid data.
- Reject duplicate AssetCode.
- Reject duplicate SerialNumber.
- Transfer active asset.
- Reject transfer of retired asset.
- Retire asset.
- Create history entry.

### Device

- Heartbeat changes Offline to Online.
- Missing heartbeat changes device to Offline.
- Maintenance device is not automatically changed to Online.
- Duplicate offline event is not created.
- Invalid battery range is rejected.

### Temperature

- Normal reading creates no alert event.
- Warning reading creates warning event.
- Critical reading creates critical event.
- Sensor-specific threshold overrides location threshold.
- Duplicate timestamp is idempotent.

### Access

- Normal access is stored.
- Unauthorized access creates security event.
- Forced open creates security event.
- Existing event cannot be updated.
- Duplicate ExternalEventId is rejected.

### Alert

- New source event creates alert.
- Duplicate source event does not create duplicate alert.
- Alert can be acknowledged.
- Alert resolution requires note.
- Resolved alert cannot be acknowledged again.
- Escalation increases level.

### Notification

- Provider is selected correctly.
- Retry occurs on transient failure.
- Permanent failure publishes failure event.
- Duplicate message is ignored.

# 15.2 Integration Tests

Use:

- `WebApplicationFactory`.
- Testcontainers for PostgreSQL.
- Testcontainers for RabbitMQ.
- Testcontainers for Redis where required.

Required scenarios:

1. Create asset through API and verify database.
2. Transfer asset and verify history.
3. Publish asset event and verify dashboard consumer.
4. Submit heartbeat and verify device status.
5. Simulate timeout and verify DeviceOffline event.
6. Submit critical temperature and verify alert creation.
7. Submit unauthorized door event and verify alert creation.
8. Verify alert event triggers notification.
9. Verify correlation ID is preserved.
10. Verify unauthorized role receives 403.

# 15.3 End-to-End Smoke Tests

After Docker Compose or Azure deployment:

- Login succeeds.
- Inventory API returns 200.
- Asset creation returns 201.
- Heartbeat endpoint returns 202 or 200.
- Critical temperature creates alert.
- Unauthorized access creates alert.
- Dashboard reflects new counts.
- Health endpoints are healthy.
- RabbitMQ queues have active consumers.
- Traces appear in observability tool.

---

# 16. E) API Documentation Requirements

Each service must expose OpenAPI documentation.

## 16.1 Swagger Requirements

Swagger must include:

- Service name and version.
- Endpoint description.
- Authentication configuration.
- Request examples.
- Response examples.
- Response status codes.
- Validation constraints.
- Enum descriptions.
- Pagination parameters.
- Correlation ID header.
- Deprecation information for future versions.

## 16.2 Required API Documentation Files

Recommended repository structure:

```text
docs/
  api/
    identity-api.md
    inventory-api.md
    device-api.md
    environment-api.md
    access-api.md
    alert-api.md
    dashboard-api.md
  events/
    event-catalog.md
  architecture/
    system-context.md
    container-diagram.md
    component-diagrams.md
    sequence-diagrams.md
    adr/
```

## 16.3 Per-Endpoint Documentation Template

Every endpoint should document:

1. Purpose.
2. Authorization.
3. Request headers.
4. Route parameters.
5. Query parameters.
6. Request body.
7. Validation rules.
8. Success response.
9. Error responses.
10. Published events.
11. Side effects.
12. Idempotency behavior.
13. Example cURL command.

---

# 17. Non-Functional Requirements

## 17.1 Performance

- Dashboard response below three seconds under demonstration load.
- Asset search below two seconds under demonstration data volume.
- Critical notification event published within 30 seconds.
- Device offline detection within one worker cycle after five-minute threshold.
- APIs should avoid unbounded result sets.

## 17.2 Availability

The business target is 99.9%.

The portfolio implementation demonstrates readiness through:

- Health checks.
- Stateless APIs.
- Container restart support.
- Retry policies.
- Durable messaging.
- Database backups.
- Observability.

## 17.3 Scalability

The business target mentions up to 100,000 IoT devices.

The one-month implementation does not need to load-test 100,000 physical devices, but it must:

- Avoid in-memory-only device state.
- Use indexed queries.
- Use asynchronous processing.
- Support horizontal API scaling.
- Avoid direct service database coupling.
- Document partitioning and IoT Hub options for future scale.

## 17.4 Security

- JWT authentication.
- RBAC.
- HTTPS.
- Secret management.
- Audit logs.
- Input validation.
- Rate limiting.
- Least-privilege cloud access.
- No secrets in repository.
- Dependency and container scanning recommended.

## 17.5 Backup and Disaster Recovery

For Azure PostgreSQL:

- Automated backups enabled.
- Point-in-time restore configured where available.
- Restore procedure documented.

For RabbitMQ and Redis:

- Message durability and queue persistence configured.
- Redis is treated as rebuildable cache, not system of record.

## 17.6 Auditability

Audit records are required for:

- User status changes.
- Asset creation, update, transfer, and retirement.
- Threshold changes.
- Door events.
- Alert acknowledgement and resolution.
- Notification delivery.
- Administrative configuration changes.

---

# 18. One-Month Delivery Plan

# Week 1 — Foundation

## Deliverables

- Repository and solution structure.
- Clean Architecture projects.
- Identity Service.
- Inventory Service.
- PostgreSQL integration.
- EF Core migrations.
- FluentValidation.
- JWT.
- Swagger.
- Dockerfiles.
- Docker Compose foundation.
- GitHub Actions build and unit tests.

## Definition of Done

- User can log in.
- Authorized user can create, read, update, and soft-delete assets.
- Asset validation works.
- Asset data persists in PostgreSQL.
- Swagger is usable.
- Services run in Docker.
- CI build is green.

# Week 2 — Microservices and Messaging

## Deliverables

- Device Monitoring Service.
- Environment Monitoring Service.
- Access Monitoring Service.
- Alert Service foundation.
- RabbitMQ.
- MassTransit.
- HostedService workers.
- Event contracts.
- Event publishing and consuming.
- Dashboard read model foundation.

## Definition of Done

- Heartbeat updates device.
- Missing heartbeat raises offline event.
- Critical temperature raises event.
- Unauthorized access raises event.
- Alert Service consumes source events.
- Dashboard consumer receives events.

# Week 3 — Azure and DevOps

## Deliverables

- Azure resource provisioning.
- ACR.
- App Service or Container Apps.
- PostgreSQL.
- Redis.
- Key Vault.
- Application Insights.
- Deployment workflow.
- Cloud configuration.
- Smoke test workflow.

## Definition of Done

- Images are pushed to ACR.
- Services are deployed.
- Secrets come from Key Vault.
- Cloud health endpoints pass.
- Logs and traces appear in Application Insights.
- Dashboard uses Redis.

# Week 4 — Enterprise Features and Documentation

## Deliverables

- OpenTelemetry.
- Health checks.
- AI summary abstraction and fallback.
- Rate limiting.
- API versioning.
- Expanded unit tests.
- Integration tests.
- Architecture documentation.
- ADRs.
- Event catalog.
- Future roadmap.
- Demo script.

## Definition of Done

- Required test scenarios pass.
- Trace follows an event across services.
- API versioning and rate limiting work.
- Documentation enables another engineer to run and understand the project.
- Demo proves end-to-end business flows.

---

# 19. F) System Evaluation Steps

# 19.1 Architecture Evaluation

| Criterion | Pass Condition |
|---|---|
| Service boundaries | Each service owns a clear business capability |
| Database ownership | No service writes another service database |
| Clean Architecture | Domain has no infrastructure dependency |
| DDD | Aggregates enforce business rules |
| Event-driven communication | Cross-service state changes propagate through RabbitMQ |
| Extensibility | Future integrations use interfaces and events |
| Reliability | Retry, idempotency, outbox, and DLQ strategy documented |
| Security | JWT, RBAC, Key Vault, and HTTPS implemented |
| Observability | Logs, metrics, traces, and health checks available |

# 19.2 Functional Evaluation

| Scenario | Pass Condition |
|---|---|
| Asset registration | Asset created with unique code and serial number |
| Asset transfer | Current location changes and history is recorded |
| Asset retirement | Only BiomedicalEngineer can retire |
| Device heartbeat | Device becomes Online |
| Device offline | Alert created after missing heartbeat threshold |
| Temperature warning | Warning event and alert created |
| Temperature critical | Critical alert and notification created |
| Door unauthorized access | Immutable event and Critical alert created |
| Alert acknowledgement | User and timestamp recorded |
| Alert resolution | Resolution note recorded |
| Dashboard | Counters reflect events |
| Notification | Delivery record created |

# 19.3 Technical Evaluation

| Area | Pass Condition |
|---|---|
| Build | `dotnet build` succeeds |
| Unit tests | Required domain tests pass |
| Integration tests | PostgreSQL/RabbitMQ/Redis scenarios pass |
| Docker | `docker compose up --build` starts all required components |
| Health | Required readiness endpoints report healthy |
| Swagger | All APIs documented and testable |
| CI | Pull request workflow succeeds |
| CD | Main workflow deploys to Azure |
| Monitoring | Traces and logs visible |
| Security | Unauthorized requests rejected correctly |

# 19.4 Code Quality Evaluation

- Meaningful names.
- Small focused methods.
- No business logic inside controllers.
- No direct infrastructure dependency in Domain.
- Consistent error handling.
- CancellationToken support for async operations.
- Async database and messaging APIs.
- Nullable reference types enabled.
- Warnings reviewed.
- Tests readable and deterministic.
- Architecture decisions documented.

---

# 20. Running and Testing the Application

# 20.1 Prerequisites

Install:

- Git.
- .NET 8 SDK.
- Docker Desktop.
- Optional Azure CLI.
- Optional PostgreSQL client.
- Optional REST client such as Postman.

# 20.2 Step 1 — Clone and Navigate

```bash
git clone <repository-url>
cd smart-inventory-device-monitoring
```

Copy environment configuration:

```bash
cp .env.example .env
```

Review local values:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=local_password
RABBITMQ_DEFAULT_USER=guest
RABBITMQ_DEFAULT_PASS=guest
JWT_SIGNING_KEY=local-development-key
REDIS_CONNECTION=redis:6379
```

## Evaluation Criteria

- Repository clones successfully.
- Solution file exists.
- `.env.example` documents required variables.
- No real secret is committed.
- README identifies project structure.

# 20.3 Step 2 — Start Everything with Docker Compose

```bash
docker compose up --build -d
```

Check status:

```bash
docker compose ps
```

View logs:

```bash
docker compose logs -f
```

Apply migrations if not automatic:

```bash
docker compose exec inventory-api dotnet Inventory.Api.dll --migrate
```

## Evaluation Criteria

- All required containers start.
- PostgreSQL is healthy.
- RabbitMQ is healthy.
- Redis is healthy.
- APIs reach Ready state.
- Notification worker has active consumers.
- No container enters a crash loop.
- Persistent data survives container restart.

# 20.4 Step 3 — Cloud Services Activation

Authenticate:

```bash
az login
```

Select subscription:

```bash
az account set --subscription "<subscription-id>"
```

Provision or deploy infrastructure:

```bash
az deployment group create \
  --resource-group <resource-group> \
  --template-file infra/main.bicep \
  --parameters environmentName=dev
```

Build and push images:

```bash
az acr login --name <acr-name>
docker compose build
docker tag inventory-api:latest <acr-name>.azurecr.io/inventory-api:<version>
docker push <acr-name>.azurecr.io/inventory-api:<version>
```

Deploy services through GitHub Actions or Azure CLI.

Configure Key Vault and managed identity.

## Evaluation Criteria

- Azure resources are created.
- Images exist in ACR.
- Services use cloud configuration.
- Key Vault secrets are resolvable.
- PostgreSQL connections succeed.
- Redis connections succeed.
- Application Insights receives telemetry.
- HTTPS endpoints are reachable.
- Health checks pass.

# 20.5 Step 4 — Verify Services

Example checks:

```bash
curl http://localhost:5101/health/ready
curl http://localhost:5102/health/ready
curl http://localhost:5103/health/ready
curl http://localhost:5104/health/ready
curl http://localhost:5105/health/ready
curl http://localhost:5106/health/ready
curl http://localhost:5107/health/ready
```

Open:

- Swagger for each API.
- RabbitMQ Management UI.
- Jaeger UI or Application Insights.
- Dashboard endpoint.

## Evaluation Criteria

- Every service returns a successful health response.
- Swagger loads.
- RabbitMQ shows expected exchanges and queues.
- Consumers are connected.
- Traces include HTTP and messaging spans.
- Dashboard responds within target.

---

# 21. End-to-End Demonstration

# 21.1 Authenticate

Request:

```http
POST /api/v1/auth/login
```

Expected:

- HTTP 200.
- JWT returned.
- Role claim present.

Evaluation:

- Invalid password returns 401.
- Inactive user returns 401.
- Protected endpoint requires token.

# 21.2 Register an Asset

Request:

```http
POST /api/v1/assets
Authorization: Bearer <token>
```

Example:

```json
{
  "assetCode": "INF-PUMP-0001",
  "name": "Infusion Pump 1",
  "type": "InfusionPump",
  "serialNumber": "SN-100001",
  "manufacturer": "Demo Manufacturer",
  "model": "IP-200",
  "departmentId": "GUID",
  "locationId": "GUID",
  "purchaseDate": "2025-01-10",
  "warrantyExpiryDate": "2028-01-10",
  "nextMaintenanceDate": "2026-09-01",
  "vendorName": "Demo Medical Supplier"
}
```

Expected:

- HTTP 201.
- Asset saved.
- AssetCreated event published.
- Dashboard total increments.

Evaluation:

- Duplicate AssetCode returns 409.
- Duplicate SerialNumber returns 409.
- Invalid date or missing field returns 400.

# 21.3 Transfer the Asset

```http
POST /api/v1/assets/{assetId}/transfer
```

```json
{
  "toDepartmentId": "GUID",
  "toLocationId": "GUID",
  "reason": "Required for ICU patient care"
}
```

Expected:

- Asset location updated.
- History entry added.
- AssetTransferred event published.
- Dashboard reflects new department.

Evaluation:

- Empty reason returns 400.
- Retired asset returns 409.
- Unauthorized role returns 403.

# 21.4 Register an IoT Device

```http
POST /api/v1/devices
```

Expected:

- Device registered as Offline.
- Device may be linked to asset.
- Dashboard total device count increments.

# 21.5 Send Device Heartbeat

```http
POST /api/v1/devices/{deviceId}/heartbeat
```

```json
{
  "recordedAtUtc": "2026-08-04T08:30:00Z",
  "batteryPercentage": 88.5,
  "signalStrengthRssi": -62,
  "cpuUsagePercentage": 22.4,
  "memoryUsagePercentage": 41.8,
  "firmwareVersion": "1.0.3"
}
```

Expected:

- Device becomes Online.
- Last heartbeat updates.
- Metrics are stored.
- Dashboard online count increments.

Evaluation:

- Battery over 100 returns 400.
- Unknown device returns 404.

# 21.6 Demonstrate Offline Detection

Stop sending heartbeat for more than five minutes, or use a test clock/simulator.

Expected:

- HostedService detects timeout.
- Device status becomes Offline.
- DeviceOffline event is published.
- Alert Service creates one Critical or Warning alert according to rule.
- Notification Worker sends configured notifications.
- Dashboard offline and alert counts update.

Evaluation:

- Detection occurs within one worker interval.
- Duplicate alerts are not created on repeated scans.
- Correlation ID appears across logs and traces.

# 21.7 Submit Temperature Reading

```http
POST /api/v1/temperature-readings
```

```json
{
  "sensorDeviceId": "GUID",
  "locationId": "GUID",
  "temperatureCelsius": 9.2,
  "humidityPercentage": 45.0,
  "recordedAtUtc": "2026-08-04T08:35:00Z"
}
```

Expected:

- Reading stored.
- Critical severity assigned.
- TemperatureCritical event published.
- Alert created.
- Notification sent.
- Dashboard critical temperature count updates.

Evaluation:

- Same sensor and timestamp does not create duplicate processing.
- Reading within normal range creates no alert.
- Critical flow completes within 30 seconds under demonstration load.

# 21.8 Submit Unauthorized Door Access

```http
POST /api/v1/door-events
```

```json
{
  "externalEventId": "DOOR-EVT-1001",
  "doorId": "GUID",
  "eventType": "UnauthorizedAccess",
  "cardNumber": "1234567890",
  "userReference": "Unknown",
  "departmentId": "GUID",
  "occurredAtUtc": "2026-08-04T08:40:00Z"
}
```

Expected:

- Event stored as immutable record.
- Card number masked in output.
- Security incident event published.
- Critical alert created.
- Security notification sent.
- Dashboard security count updates.

Evaluation:

- Duplicate ExternalEventId is idempotent or returns 409.
- No update/delete endpoint exists.
- Unauthorized user cannot query restricted history.

# 21.9 Acknowledge and Resolve Alert

Acknowledge:

```http
POST /api/v1/alerts/{alertId}/acknowledge
```

Resolve:

```http
POST /api/v1/alerts/{alertId}/resolve
```

```json
{
  "resolutionNote": "Sensor checked and refrigerator door closed."
}
```

Expected:

- Alert state changes.
- User and timestamp recorded.
- State-change events published.
- Dashboard active alert count decreases after resolution.

Evaluation:

- Resolution without note returns 400.
- Invalid state transition returns 409.
- History shows all transitions.

# 21.10 Verify Dashboard

```http
GET /api/v1/dashboard/summary
```

Expected:

- Asset count includes created asset.
- Device status reflects latest heartbeat state.
- Temperature counts reflect readings.
- Door incident count reflects event.
- Alert counts reflect lifecycle.
- GeneratedAtUtc is current.

Evaluation:

- Response is below three seconds.
- Redis contains updated read model.
- Temporary missing event handling is documented.

# 21.11 Verify Observability

Inspect:

- Application logs.
- RabbitMQ queues.
- Distributed trace.
- Application Insights failures.
- Health status.
- Dead-letter queues.

Expected:

- One trace can follow request -> publish -> consume -> alert -> notification.
- Failed message includes diagnostic metadata.
- No secret values appear in logs.

---

# 22. Acceptance Criteria Summary

The project is accepted when:

1. All required services build and run.
2. Docker Compose starts the complete local platform.
3. JWT and role authorization work.
4. Inventory workflows work.
5. Device heartbeat and offline detection work.
6. Temperature thresholds generate alerts.
7. Door security events generate alerts.
8. Alert acknowledgement and resolution work.
9. Notifications are processed and recorded.
10. Dashboard is updated through events.
11. PostgreSQL, RabbitMQ, and Redis are integrated.
12. Swagger documents each service.
13. Unit and integration tests pass.
14. GitHub Actions CI is green.
15. Azure deployment is demonstrated.
16. Key Vault and Application Insights are integrated.
17. Health checks and OpenTelemetry are available.
18. Architecture and event documentation are complete.
19. Future extension points are documented.
20. Another software engineer can clone, run, test, and understand the project using this repository alone.

---

# 23. Future Architecture Roadmap

## Phase 2

- Azure IoT Hub.
- Device provisioning.
- Azure Functions.
- Predictive maintenance.
- Anomaly detection.
- Power consumption analytics.

## Phase 3

- RFID.
- BLE beacon.
- Barcode scanner.
- Indoor navigation.
- Digital Twin.
- Mobile application.
- Offline synchronization.

## Phase 4

- Multi-hospital support.
- Tenant isolation.
- Regional dashboard.
- Cross-hospital asset sharing.
- Kubernetes.
- Event Sourcing.
- Saga orchestration.
- Cosmos DB.
- Power BI.

The Phase 1 implementation must preserve these extension points through service boundaries, integration events, provider interfaces, and environment-independent infrastructure configuration.
