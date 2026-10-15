# Smart Inventory & Device Monitoring Platform — Architecture Diagram Pack

This pack translates the developer requirements into precise technical diagrams.

## Diagram Index

1. [System Context](01-system-context.svg)
2. [Event-Driven Integration Topology](02-event-topology.svg)
3. [Identity Service](03-identity-service.svg)
4. [Inventory Service](04-inventory-service.svg)
5. [Device Monitoring Service](05-device-service.svg)
6. [Environment Monitoring Service](06-environment-service.svg)
7. [Access Monitoring Service](07-access-service.svg)
8. [Alert Service](08-alert-service.svg)
9. [Notification Worker](09-notification-worker.svg)
10. [Dashboard Query Service](10-dashboard-service.svg)
11. [Azure Cloud Deployment](11-azure-deployment.svg)
12. [CI/CD and Test Automation](12-cicd-pipeline.svg)

## Architecture Conventions

- Each service follows Clean Architecture: API → Application → Domain, with Infrastructure implementing external ports.
- Each service owns its PostgreSQL database/schema.
- RabbitMQ and MassTransit carry versioned integration events.
- The Alert Service subscribes to operational source events.
- The Notification Worker subscribes to alert lifecycle events.
- The Dashboard Service consumes events into a Redis read model.
- Azure Key Vault stores secrets; managed identity grants access.
- OpenTelemetry exports logs, metrics, and traces to Application Insights.
- Docker Compose runs the complete local stack.
- GitHub Actions builds, tests, scans, publishes images to ACR, deploys, migrates databases, and runs smoke tests.

## Recommended Repository Placement

```text
docs/
  architecture/
    diagrams/
      01-system-context.svg
      02-event-topology.svg
      03-identity-service.svg
      04-inventory-service.svg
      05-device-service.svg
      06-environment-service.svg
      07-access-service.svg
      08-alert-service.svg
      09-notification-worker.svg
      10-dashboard-service.svg
      11-azure-deployment.svg
      12-cicd-pipeline.svg
```

Both SVG and PNG versions are included. DOT source files are included so the diagrams remain editable.
