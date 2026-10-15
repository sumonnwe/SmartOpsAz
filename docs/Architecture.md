# SmartOps — Architecture Overview

Version: 1.0  
Date: 2026-08-03  
Author: Senior Software Architect

## Purpose

This document describes the architecture of SmartOps — an enterprise-grade Smart Inventory and Device Monitoring Platform. It covers high-level goals and constraints, domain decomposition, technical choices, component interactions, deployment and operational patterns, data and messaging design, security and compliance, and recommended next steps.

## Goals & Non-Goals

- Goals
  - Provide reliable, scalable device telemetry ingestion, storage, analytics, alerting, and device control.
  - Maintain strong domain boundaries (DDD) and clear separation of concerns using Clean Architecture.
  - Enable independent teams to develop, deploy, and operate microservices.
  - Support multi-tenant enterprise operations, observability, and secure device management.
- Non-goals
  - Not intended to be a one-size-fits-all IoT platform; extensions for domain-specific analytics are expected.
  - Does not mandate a single vendor for managed components; recommendations favor Azure-managed services where appropriate.

## Key Requirements & Constraints

- High ingest throughput for telemetry (millions of messages/day).
- Low-latency real-time UI updates for operators (sub-second where practical).
- Strong eventual consistency across microservices for inventory and status.
- Multi-tenant isolation and per-tenant data partitioning.
- Deployable to Azure (AKS and managed services) using Docker/Kubernetes.
- Observability using Application Insights plus Kubernetes-native tooling.
- Support both modern cloud devices (HTTPS/WebSockets/MQTT) and constrained devices (MQTT).

## Technology Stack (selected)

- Backend: .NET 8 (C#), Clean Architecture, Domain-Driven Design (DDD)
- Frontend: React (TypeScript), SignalR (real-time UI)
- Microservices: Independent services per bounded context, containerized
- Messaging & Streaming: Kafka (event backbone) + RabbitMQ (command/task queues)
- Device Protocol: MQTT (device telemetry/command) / TLS
- Data Storage:
  - PostgreSQL (primary relational persistence)
  - Redis (caching, leader election, short-lived state)
- Infrastructure: Docker, Kubernetes (AKS), Helm
- Cloud: Microsoft Azure (AKS, Azure Database for PostgreSQL, Azure Cache for Redis, Azure Container Registry, Application Insights, Key Vault)
- Observability: Application Insights, Azure Monitor, Prometheus + Grafana (optional)
- AI/Assist: Azure OpenAI for enhanced diagnostic insights / operator assistance
- Container Registry & CI/CD: Azure Container Registry / GitHub Actions
- Other: OAuth2 / OpenID Connect (Azure AD), TLS everywhere

Note: Kafka may be hosted via Confluent Cloud, Azure Event Hubs (Kafka-compatible), or self-managed Strimzi on AKS depending on operational preference.

## Architecture Principles

1. Domain-First: Model behavior and state with DDD. Bounded contexts drive service boundaries.
2. Clean Architecture: Each microservice follows layers — Domain → Application (use cases) → Infrastructure → API/Adapters.
3. Asynchronous by default: Use events for eventual consistency; commands for point-to-point actions.
4. Resilience & Observability: Fail-fast, circuit breakers, retries with back-off, idempotency, and exhaustive telemetry.
5. Automation: repeatable CI/CD, infra-as-code, and automated testing for services and contracts.
6. Security-first: Least-privilege, managed secrets, device authentication, and encrypted transport.

## Domain & Bounded Contexts

Suggested Top-Level Bounded Contexts / Microservices:

- Identity & Access
  - Responsibilities: Authentication, authorization, RBAC, tenant management (use Azure AD + custom tenant service).
  - Data store: PostgreSQL (tenant metadata).

- Device Registry / Inventory
  - Responsibilities: Device metadata, provisioning status, firmware versions, capabilities.
  - Aggregates: Device, DeviceModel, Location
  - Interfaces: REST + internal events.

- Device Connectivity & Telemetry Ingress
  - Responsibilities: Device connections (MQTT broker), protocol adapters, initial validation, enrichment, and forwarding to Kafka.
  - Notes: Use a dedicated MQTT cluster (or managed service). This component is critical for scale and resiliency.

- Telemetry Stream Processor / Ingestion
  - Responsibilities: Consume raw telemetry (Kafka), perform normalization, telemetry partitioning, enrichment, and store recent state in Redis and persisted events in PostgreSQL or a time-series store.
  - Patterns: Use stream processing (Kafka Streams or .NET consumer) for enrichment, sampling, and derivation.

- Alerting & Rules Engine
  - Responsibilities: Evaluate telemetry against rules, generate alerts, escalate, handle silences and schedules.
  - Notes: Stateless rules engine instances that read rule definitions from DB / config; emits alert events to Kafka; integrates with notification delivery.

- Command & Control (Device Commands)
  - Responsibilities: Accept operator commands (API/SignalR), translate to device commands, send through RabbitMQ or MQTT depending on routing architecture, track command lifecycle.
  - Patterns: Use command queue (RabbitMQ) for delivery workflows, persistent command history in Postgres.

- Analytics & Reporting
  - Responsibilities: Aggregations, trend computation, long-term analytics (data warehouse).
  - Data: Bulk export to data lake / analytical DB (e.g., Synapse / managed Data Warehouse).

- UI / Operator Console
  - Frontend: React + SignalR for real-time updates. Communication with Backend APIs (GraphQL/REST).
  - Responsibilities: Inventory management, live telemetry view, command issuing, alert management, reporting.

- Integration & Connectors
  - Responsibilities: Connectors to external systems (ERP, ticketing), webhook delivery, and 3rd-party integrations.

- Orchestration / Saga Coordinator
  - Responsibilities: Coordinate long-running workflows across services (firmware update flows, bulk provisioning).
  - Patterns: Saga orchestration using state machine (or choreography via events) with durable store.

## Clean Architecture & Project Layout (per microservice)

Each microservice should follow a consistent structure:

- src/
  - ServiceName.Api (controllers, minimal logic, mapping)
  - ServiceName.Application (use-cases, commands/queries, DTOs)
  - ServiceName.Domain (entities, value objects, domain events, aggregates)
  - ServiceName.Infrastructure (persistence, repositories, messaging, integrations)
  - ServiceName.Contracts (event/message schemas)
  - ServiceName.Tests (unit/integration tests)

Notes:
- Domain has no external dependencies.
- Application implements use-cases and depends on Domain abstractions.
- Infrastructure implements interfaces for repositories, message producers/consumers, external clients.
- Avoid direct dependencies from API → Infrastructure.

## Messaging & Integration Patterns

- Kafka: The primary event stream for domain events and telemetry events. Use for high-throughput, immutable event logs and streaming processing.
  - Topics: follow `tenant.{tenantId}.context.eventType` or `context.eventType` with tenant in message header for multi-tenant architectures.
  - Schema management: Use a schema registry (Avro/JSON Schema) to manage compatibility.
  - Retention: Telemetry topics with normal retention; compacted topics for latest-state.

- RabbitMQ: Use as a command/task queue where ordered, transactional, and direct point-to-point deliver semantics are required (e.g., device command delivery, firmware tasks).
  - Prefer persistent queues, dead-lettering, and consumer retry policies.

- MQTT: Device ingress protocol; broker accepts device connections and publishes device telemetry to internal topics. Bridge MQTT to Kafka for telemetry stream (or have an ingestion gateway forward MQTT messages to Kafka).
  - Use MQTT over TLS with client certificates or token-based auth.

- SignalR: Real-time UI updates for operators. Backend services push events to SignalR hubs (or a dedicated real-time gateway).

- Event patterns:
  - Event-Carried State Transfer for eventual consistency.
  - Choreography for simple flows, orchestration (Saga) for complex multi-step business processes.
  - Use idempotency keys, deduplication, and sequence numbers on cross-service commands.

## Data & Storage Strategy

- PostgreSQL
  - Use as the primary OLTP relational store per microservice (one DB per service or logical schemas for tenant isolation).
  - Use strong schema migrations (Flyway/EF Core Migrations).
  - Prefer managed Azure Database for PostgreSQL for operational simplicity.

- Redis
  - Use for caching hot device state, rate-limiting counters, leader election, short-lived locks, and session state for SignalR scale-out.
  - Prefer Azure Cache for Redis (Managed).

- Time-series / Analytical Store
  - For high cardinality telemetry and time-series analysis, evaluate dedicated TSDB or cloud services (e.g., Azure Data Explorer). Alternatively, export to a data lake and use Spark/Synapse for heavy analytics.

- Backups & DR
  - Automated backups for Postgres, retention aligned with RTO/RPO.
  - Kafka topic replication or cloud-managed durability.
  - Disaster recovery plan: cross-region read replicas, failover runbooks.

## API Design

- Expose clear REST/GraphQL APIs per bounded context, with strong contract definitions.
- Use OpenAPI/Swagger for service APIs and publish them in the developer portal.
- Versioning: route-based or header-based versioning, keep backward compatibility.
- Rate-limiting and throttling via API gateway (Azure API Management or ingress + policies).

## Security & Identity

- Authentication: OAuth2 / OpenID Connect with Azure AD (or AD B2C for external customers).
- Authorization: RBAC and attribute-based access control inside services.
- Device Authentication: use X.509 certificates, JWT tokens, or SAS tokens for devices. Rotate device credentials and store keys securely in Key Vault.
- Secrets: Azure Key Vault for secrets and certificates.
- Network: TLS everywhere; private AKS clusters and secure VNet peering for managed services.
- Multi-tenant data separation: use logical separation (tenantId) + row-level security or separate schemas/databases for stricter isolation.
- Security Scanning: Static analysis, dependency scanning, container image scanning, and regular pen-tests.

## Observability & Telemetry

- Telemetry:
  - Application Insights for .NET services (distributed tracing, metrics, logs).
  - Correlate traces across services using W3C trace context.
  - Instrumentation: log structured JSON, include tenantId, requestId, correlationId, deviceId in logs and metrics.

- Monitoring:
  - Metrics: Prometheus + Grafana (optional) plus Application Insights KPI dashboards.
  - Alerts: Define SLOs and set alerts (PI availability, message lag, consumer group lag, error rates, CPU/memory).
  - Kafka monitoring: consumer lag monitoring, broker health.
  - Kubernetes: use metrics-server, HPA, pod metrics, node autoscaling.

- Log retention and sampling:
  - Sample high-volume telemetry logs but keep critical event logs un-sampled.
  - Export long-term logs to a central store (Log Analytics).

## Deployment & Infra

- Packaging: Docker images for all services; minimal base images using .NET 8 runtime.
- Container registry: Azure Container Registry (ACR).
- Orchestration: AKS with Helm charts; use namespaces per environment and per-team if needed.
- Stateful services: use managed services (Azure Database for PostgreSQL, Azure Cache for Redis) where possible to reduce ops overhead.
- Kafka: Either use managed Confluent or Event Hubs (Kafka-compatible) or Strimzi on AKS. Use Zookeeper-less Kafka if available. Ensure multi-AZ replication.
- RabbitMQ: Managed or deployed via Helm (with persistence on Azure disks).
- Helm charts & GitOps: Use Helm + Flux/ArgoCD for GitOps, or GitHub Actions + Helm for CD pipelines.
- Resource sizing: define initial resource requests/limits; use HPA/VPA to scale services.
- Secrets: mount Key Vault secrets via CSI driver or use external-secrets for GitOps.

## CI/CD

- CI: GitHub Actions for builds, unit tests, and contract tests.
- Build matrix: run multi-target builds (e.g., linux/amd64).
- Container image build and push to ACR; use immutable tags (sha).
- CD: promote images through environments (dev → staging → prod) via GitOps or pipelines; require approvals for production.
- Infrastructure as Code: ARM templates / Bicep / Terraform for Azure resources. Keep infra code in a separate repo or subdirectory.

## Resilience, Reliability & Patterns

- Idempotency: Commands/events must be idempotent; use request IDs and dedup records.
- Retry & backoff policies: Use Polly for .NET transient fault handling.
- Circuit breakers and bulkheads: to isolate failing dependencies.
- Backpressure: Support throttling and queue length monitoring for telemetry ingestion.
- Canary & Blue-Green: deploy new versions using canary or blue-green patterns.

## Data Contracts & Schema Management

- Events and messages must have well-defined contracts:
  - Schema registry for Kafka (Avro/JSON Schema).
  - Contracts repository (ServiceName.Contracts) published as NuGet packages or OpenAPI/Event schema registry.
- Topic naming and versioning:
  - Use hierarchical topic naming: `telemetry.device.{deviceType}` or `domain.inventory.device.created.v1`.
  - Version events with v1/v2 and maintain compatibility rules.

## Dev & QA Strategy

- Local development: docker-compose for local dependencies (Postgres, Redis, RabbitMQ, Kafka) and sample device simulator.
- Contract testing between services (Pact or similar).
- Integration tests that run against ephemeral AKS (kind/minikube) or test clusters in CI.
- Load testing: JMeter or k6 for telemetry ingestion and API load.

## Operational Runbook (High-level)

- On telemetry ingestion degradation:
  - Inspect Kafka producer error rates and broker health.
  - Check MQTT gateway or ingress for connection saturation.
  - Check consumer groups & lag and scale consumers.
- When device commands fail:
  - Check RabbitMQ queue health, dead-letter queue entries, and device connectivity (MQTT sessions).
- Incident onboarding:
  - page on-call engineers, provide correlationId for traces, and escalate via incident runbooks.
- Recovery:
  - Use backups and point-in-time restores for Postgres.
  - Restore Kafka topics via broker replication or cloud provider's retention + reprocessing from raw events if stored.

## Example Events & Topics (samples)

- Topic: `telemetry.raw`
  - Message: { tenantId, deviceId, timestamp, payloadType, payload, seq, traceId }
- Topic: `device.status`
  - Message: { tenantId, deviceId, status: "online"|"offline", lastSeen, metadata }
- Topic: `inventory.device.created.v1`
  - Message: { deviceId, tenantId, model, createdAt, attributes }

## Real-Time UI (React + SignalR)

- UI uses React + TypeScript.
- SignalR hubs in backend expose real-time channels per tenant and per operator session.
- Use hub authentication via access tokens; scale using Redis backplane or Azure SignalR Service for large scale.

## Azure OpenAI & Assistive Features

- Use Azure OpenAI for operator assistance: automated diagnostics summaries, suggested remediation steps, and natural language query across logs/telemetry.
- Limit PII and tenant-sensitive data exposure; ensure prompts and caching are sanitized and governed by policy.

## Example Security & Compliance Notes

- Data residency: use region-specific Azure resources per tenant if required.
- PII handling: encryption at rest and in transit; anonymization for analytics.
- Auditing: write immutable audit events for critical actions and store in a secure, tamper-evident store.

## Scalability & Performance Considerations

- Horizontal scale for stateless services; stateful services use managed scaling where possible.
- Partition Kafka topics by tenant or deviceId hashing for even distribution.
- Use HPA on CPU/memory or custom metrics (consumer lag) to scale consumers.
- Pre-warm caching tiers for busy tenants.

## Cost Considerations & Trade-offs

- Managed services reduce ops burden but increase fixed OPEX.
- Self-managed Kafka on AKS gives control but requires operations expertise.
- Azure Event Hubs (Kafka-compatible) is a good balance for many enterprises.

## Roadmap & Next Steps

1. Finalize bounded contexts and a first MVP service list (suggest: Device Registry, Telemetry Ingest, Telemetry Processor, Alerting, UI).
2. Implement core contracts and an initial schema registry.
3. Build device simulator and local dev environment (docker-compose).
4. Implement CI pipeline for build/test and CD to dev cluster.
5. Provision managed Azure services (Postgres, Redis, Key Vault).
6. Pilot with a single tenant and a limited device fleet; measure telemetry and tune scaling.

## Appendix

- Recommended libraries & frameworks:
  - .NET 8: MediatR (mediator pattern), EF Core (persistence), MassTransit (if using RabbitMQ integration), Confluent.Kafka (.NET client)
  - Telemetry: OpenTelemetry for tracing + Application Insights exporter
  - Security: IdentityServer / Azure AD integration libraries
- Useful patterns: Saga (Durable Functions or custom orchestrator), Outbox pattern (reliable event publishing), CQRS for complex read/write separation.

---
Document prepared for the SmartOps platform. If you want, I can:
- Commit this Architecture.md into sumonnwe/SmartOps (create branch / PR), or
- Expand sections into individual design docs (e.g., "Telemetry Ingestion Design", "Device Provisioning Flow", "Alerting Workflow") or
- Generate Helm chart skeletons, Kubernetes manifests, or a starter .NET microservice scaffold.
