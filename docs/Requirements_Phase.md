Requirements.md Content
# Smart Inventory & Device Monitoring Platform
## Phase 1 Functional Requirements (1 Month Portfolio)

Purpose: Build an enterprise-style portfolio demonstrating Clean Architecture, DDD, Event-driven Microservices and Azure deployment while keeping business logic intentionally simple.

### Modules
1. Authentication
- Login using JWT  **
- Roles: Admin, Biomedical Engineer, Nurse, Security Officer, Maintenance Engineer  **
- Refresh token not required in Phase 1  

2. Inventory Management
Features:
- Register medical device **
- Update device **
- Delete (soft delete) **
- Search / Filter **
- Assign Department  **
- Transfer Location **
- Retire Device (Biomedical Engineer only) **
- View Asset History  **

Business Rules:
- Asset ID unique
- Serial Number unique  **
- Every inventory movement generates InventoryMoved event.  **
- Retired device cannot be assigned.  **
 
Status:
Available, In Use, Maintenance, Offline, Retired

3. Device Monitoring
- Register IoT Device **
- Link device to Asset **
- Heartbeat every minute (simulated)  **
- Update battery, RSSI and connectivity  **
- Device Online/Offline status   **
- Offline for >5 minutes raises DeviceOffline event.   ??

4. Temperature Monitoring
- Store one-minute sensor readings
- Threshold configurable
- Generate TemperatureAlert event
- Acknowledge alert

5. Door Access
- Receive door events
- Store immutable audit log
- Unauthorized access raises alert

6. Notification
Channels:
- Email (simulation)    **
- Teams webhook (simulation)    **
- Log notification    **

Priority:
Information, Warning, Critical

7. Dashboard
Widgets:
- Total Assets     **
- Online Devices     **
- Offline Devices    **
- Active Alerts   **
- Today's Door Events     ??
- Temperature Warnings   ??

### Weekly Plan

Week1
- Clean Architecture  **
- Inventory CRUD   **
- Authentication   **
- Validation  **
- Swagger   **
- PostgreSQL      **
- Docker **
- GitHub Actions CI     ??

Week2
- Inventory Service     **
- Device Service  **
- RabbitMQ + MassTransit    **
- Publish:  
  InventoryCreated      **
  InventoryTransferred      **
  DeviceOffline   **
  TemperatureAlert  ?? 
- Consumer updates dashboard cache    ??

Week3
- Azure App Service
- Azure Container Registry
- Azure Key Vault
- Redis Cache
- Application Insights
- Deployment Pipeline

Week4
- OpenTelemetry
- Health Checks
- AI Summary endpoint (Azure OpenAI ready abstraction)
- Rate Limiting
- API Versioning
- Unit Tests
- Integration Tests
- Architecture Diagram
- ADR
- Future Roadmap

### Acceptance Criteria

Inventory
- Create/Edit/Delete asset
- Search within 2 sec
- Asset history displayed

Monitoring
- Offline device detected automatically
- Temperature threshold generates alert
- Dashboard updates after event

Notification
- Critical alerts published within 30 sec

### Event List
InventoryCreated
InventoryUpdated
InventoryTransferred
InventoryRetired
DeviceHeartbeat
DeviceOffline
TemperatureAlertRaised
DoorAccessDetected
NotificationSent

### Future Ready Extension Points
- RFID
- Barcode Scanner
- Azure IoT Hub
- Digital Twin
- Cosmos DB
- Event Sourcing
- Saga
- Kubernetes
- Multi Tenant
- Mobile App

============
Incomplete / missing Week‑1 features (based on current code)
•	API layer:
•	No controllers / API endpoints shown in the provided files. No OpenAPI/Swagger configuration shown.
•	Search/Filter + Pagination:
•	No repository/query method for searching/filtering/paging devices.
•	Asset History:
•	No AssetHistory entity, table, repository or append-only persistence for movement history.
•	Uniqueness & DB constraints:
•	No visible EF Core configuration adding unique indexes on Asset ID and Serial Number (DbContext calls ApplyConfigurationsFromAssembly so configs may be elsewhere—if absent, must add).
•	No database migrations or scripts shown.
•	Soft-delete behavior:
•	No global EF query filter for soft-deletes is shown (so ad-hoc filtering is used in repository).
•	Authentication & Authorization:
•	No JWT, RBAC, or role checks (e.g., preventing non‑Biomedical Engineer from retiring).
•	FluentValidation:
•	Not present in the shown files.
•	Authorization for Retire:
•	Not enforced at domain or application layer in shown code (this should be in application/API).