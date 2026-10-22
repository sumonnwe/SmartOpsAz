/**
 * Device Service API Client
 * Handles device-related requests to DeviceService (Port 5103)
 * 
 * Service Base URL (Dev): /device-api → http://localhost:5103
 * Service Base URL (Prod): Configured via VITE_DEVICE_API_URL
 */

import type {
  IoTDevice,
  SearchDeviceResponse,
  RegisterDeviceRequest,
  RegisterDeviceResponse,
  UpdateDeviceRequest
} from '../types/api.types'
import { apiClient } from './apiClient'

export const deviceService = {
  /**
   * Map device DTO to frontend interface
   * @param item - Raw device item from API
   * @returns Mapped device item
   */
  mapDeviceItem(item: unknown): IoTDevice {
    return item as IoTDevice
  },

  /**
   * Search and list devices with pagination
   * Routes to: GET /api/v1/devices on DeviceService
   * 
   * Supports:
   * - Pagination (page, pageSize)
   * 
   * @param page - Page number (1-based)
   * @param pageSize - Items per page
   * @returns Paginated search results
   * @throws ApiError if request fails
   */
  async search(page: number = 1, pageSize: number = 20): Promise<SearchDeviceResponse> {
    // Validate pagination parameters
    if (page < 1) page = 1
    if (pageSize < 1 || pageSize > 100) pageSize = 20

    const response = await apiClient.get<any>(
      'device',
      `/api/v1/devices?page=${page}&pageSize=${pageSize}`
    )

    if (response.error) {
      throw response.error
    }

    // Map DTOs to frontend interface
    return {
      items: response.data!.items.map((item: unknown) => this.mapDeviceItem(item)),
      page: response.data!.page,
      pageSize: response.data!.pageSize,
      totalCount: response.data!.totalCount || response.data!.items.length
    }
  },

  /**
   * Get device by ID
   * Routes to: GET /api/v1/devices/{deviceId} on DeviceService
   * 
   * @param id - Device ID (UUID)
   * @returns Device details
   * @throws ApiError if request fails
   */
  async getById(id: string): Promise<IoTDevice> {
    const response = await apiClient.get<IoTDevice>(
      'device',
      `/api/v1/devices/${id}`
    )

    if (response.error) {
      throw response.error
    }

    return this.mapDeviceItem(response.data!)
  },

  /**
   * Register new device
   * Routes to: POST /api/v1/devices on DeviceService
   * 
   * @param data - Device registration data
   * @returns Registered device with ID
   * @throws ApiError if request fails
   */
  async register(data: RegisterDeviceRequest): Promise<RegisterDeviceResponse> {
    const response = await apiClient.post<RegisterDeviceResponse>(
      'device',
      '/api/v1/devices',
      data
    )

    if (response.error) {
      throw response.error
    }

    return response.data!
  },

  /**
   * Update device details
   * Routes to: PUT /api/v1/devices/{deviceId} on DeviceService
   * 
   * @param id - Device ID
   * @param data - Update data
   * @returns void
   * @throws ApiError if request fails
   */
  async update(id: string, data: UpdateDeviceRequest): Promise<void> {
    const response = await apiClient.put<void>(
      'device',
      `/api/v1/devices/${id}`,
      data
    )

    if (response.error) {
      throw response.error
    }
  },

  /**
   * Link device to inventory asset
   * Routes to: POST /api/v1/devices/{deviceId}/link-asset on DeviceService
   * 
   * @param deviceId - Device ID
   * @param assetId - Inventory Asset ID
   * @returns void
   * @throws ApiError if request fails
   */
  async linkAsset(deviceId: string, assetId: string): Promise<void> {
    const response = await apiClient.post<void>(
      'device',
      `/api/v1/devices/${deviceId}/link-asset`,
      { assetId }
    )

    if (response.error) {
      throw response.error
    }
  },

  /**
   * Unlink device from inventory asset
   * Routes to: DELETE /api/v1/devices/{deviceId}/link-asset on DeviceService
   * 
   * @param deviceId - Device ID
   * @returns void
   * @throws ApiError if request fails
   */
  async unlinkAsset(deviceId: string): Promise<void> {
    const response = await apiClient.delete<void>(
      'device',
      `/api/v1/devices/${deviceId}/link-asset`
    )

    if (response.error) {
      throw response.error
    }
  },

  /**
   * Delete device (retire)
   * Routes to: DELETE /api/v1/devices/{deviceId} on DeviceService
   * 
   * @param id - Device ID
   * @returns void
   * @throws ApiError if request fails
   */
  async delete(id: string): Promise<void> {
    const response = await apiClient.delete<void>(
      'device',
      `/api/v1/devices/${id}`
    )

    if (response.error) {
      throw response.error
    }
  },

  /**
   * Search devices with filtering
   * Routes to: GET /api/v1/devices/search on DeviceService
   * 
   * @param filters - Search filters (deviceCode, status, gatewayId, page, pageSize)
   * @returns Filtered paginated results
   * @throws ApiError if request fails
   */
  async advancedSearch(filters: {
    deviceCode?: string
    status?: number
    gatewayId?: string
    page?: number
    pageSize?: number
  }): Promise<SearchDeviceResponse> {
    const params = new URLSearchParams()
    if (filters.deviceCode) params.append('deviceCode', filters.deviceCode)
    if (filters.status !== undefined) params.append('status', filters.status.toString())
    if (filters.gatewayId) params.append('gatewayId', filters.gatewayId)
    params.append('page', (filters.page || 1).toString())
    params.append('pageSize', (filters.pageSize || 20).toString())

    const response = await apiClient.get<any>(
      'device',
      `/api/v1/devices/search?${params.toString()}`
    )

    if (response.error) {
      throw response.error
    }

    return {
      items: response.data!.items.map((item: unknown) => this.mapDeviceItem(item)),
      page: response.data!.page,
      pageSize: response.data!.pageSize,
      totalCount: response.data!.totalCount || response.data!.items.length
    }
  },

  /**
   * Set device to maintenance mode
   * Routes to: POST /api/v1/devices/{deviceId}/maintenance on DeviceService
   * 
   * @param deviceId - Device ID
   * @returns void
   * @throws ApiError if request fails
   */
  async setMaintenance(deviceId: string): Promise<void> {
    const response = await apiClient.post<void>(
      'device',
      `/api/v1/devices/${deviceId}/maintenance`,
      {}
    )

    if (response.error) {
      throw response.error
    }
  },

  /**
   * Activate device (from maintenance)
   * Routes to: POST /api/v1/devices/{deviceId}/activate on DeviceService
   * 
   * @param deviceId - Device ID
   * @returns void
   * @throws ApiError if request fails
   */
  async activate(deviceId: string): Promise<void> {
    const response = await apiClient.post<void>(
      'device',
      `/api/v1/devices/${deviceId}/activate`,
      {}
    )

    if (response.error) {
      throw response.error
    }
  },

  /**
   * Get device heartbeat history
   * Routes to: GET /api/v1/devices/{deviceId}/heartbeats on DeviceService
   * 
   * @param deviceId - Device ID
   * @param page - Page number
   * @param pageSize - Items per page
   * @returns Heartbeat history
   * @throws ApiError if request fails
   */
  async getHeartbeats(deviceId: string, page: number = 1, pageSize: number = 50): Promise<any> {
    const response = await apiClient.get<any>(
      'device',
      `/api/v1/devices/${deviceId}/heartbeats?page=${page}&pageSize=${pageSize}`
    )

    if (response.error) {
      throw response.error
    }

    return response.data
  }
}