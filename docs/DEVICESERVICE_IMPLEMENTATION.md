# DeviceService Implementation Guide

## Overview

DeviceService Phase 1 implements IoT device registration, heartbeat processing, automatic offline detection, and event publishing for the SmartOps platform.

### Features Implemented

✅ Register IoT Devices  
✅ Link Devices to Inventory Assets  
✅ Submit and Store Heartbeats  
✅ Update Device Metrics (Battery, RSSI, CPU, Memory)  
✅ Automatic Offline Detection (>5 minutes no heartbeat)  
✅ Device Recovery Detection  
✅ Simulated Heartbeat Generator (optional, dev-only)  
✅ JWT Authentication (IdentityService integration)  
✅ Role-based Authorization (BiomedicalEngineer, HospitalAdministrator)  
✅ Swagger/OpenAPI Documentation  
✅ Health Check Endpoints  
✅ Comprehensive Unit and Integration Tests  

---

## Quick Start

### Prerequisites

- .NET 8 SDK
- PostgreSQL 16
- Docker & Docker Compose (optional, for containerized setup)
- Git

### Local Development Setup (Without Docker)

#### 1. Clone and Navigate
