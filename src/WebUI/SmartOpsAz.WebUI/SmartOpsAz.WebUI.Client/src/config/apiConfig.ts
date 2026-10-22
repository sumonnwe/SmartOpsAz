/**
 * API Configuration for service-specific routing
 * Defines base URLs for each microservice
 */

// Service base URLs - can be overridden by environment variables
const IDENTITY_API_URL = import.meta.env.VITE_IDENTITY_API_URL || '/identity-api'
const INVENTORY_API_URL = import.meta.env.VITE_INVENTORY_API_URL || '/inventory-api'
const DEVICE_API_URL = import.meta.env.VITE_DEVICE_API_URL || '/device-api'
const DASHBOARD_API_URL = import.meta.env.VITE_DASHBOARD_API_URL || '/dashboard-api'

export const apiConfig = {
  services: {
    identity: {
      baseUrl: IDENTITY_API_URL,
      endpoints: {
        login: '/api/auth/login',
        logout: '/api/auth/logout'
      }
    },
    inventory: {
      baseUrl: INVENTORY_API_URL,
      endpoints: {
        register: '/api/inventory',
        getById: (id: string) => `/api/inventory/${id}`,
        update: (id: string) => `/api/inventory/${id}`,
        delete: (id: string) => `/api/inventory/${id}`,
        search: '/api/inventory',
        assignDepartment: (id: string) => `/api/inventory/${id}/department`,
        transferLocation: (id: string) => `/api/inventory/${id}/location`,
        retire: (id: string) => `/api/inventory/${id}/retire`,
        history: (id: string) => `/api/inventory/${id}/history`
      }
    },
    device: {
      baseUrl: DEVICE_API_URL,
      endpoints: {
        register: '/api/devices',
        getAll: '/api/devices',
        getById: (id: string) => `/api/devices/${id}`,
        update: (id: string) => `/api/devices/${id}`,
        linkAsset: (id: string) => `/api/devices/${id}/link-asset`,
        heartbeat: (id: string) => `/api/devices/${id}/heartbeat`,
        getHeartbeats: (id: string) => `/api/devices/${id}/heartbeats`
      }
    },
    dashboard: {
      baseUrl: DASHBOARD_API_URL,
      endpoints: {
        summary: '/api/v1/dashboard/summary'
      }
    }
  }
}

export type ApiService = keyof typeof apiConfig.services