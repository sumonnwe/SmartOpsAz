/**
 /**
 * Inventory Service API Client
 * Handles inventory-related requests to InventoryService (Port 5102)
 * 
 * Service Base URL (Dev): /inventory-api → http://localhost:5102
 * Service Base URL (Prod): Configured via VITE_INVENTORY_API_URL
 */

import { apiClient } from './apiClient'
import { apiConfig } from '../config/apiConfig'
import type {
  InventoryItem,
  SearchInventoryResponse,
  SearchInventoryRequest,
  RegisterInventoryRequest,
  RegisterInventoryResponse,
  UpdateInventoryRequest,
  AssignDepartmentRequest,
  TransferLocationRequest
} from '../types/api.types'

export const inventoryService = { 
  /**
   * Map inventory DTO to frontend interface
   * @param item - Raw inventory item from API
   * @returns Mapped inventory item
   */
  mapInventoryItem(item: unknown): InventoryItem {
    return item as InventoryItem
  },

  /**
   * Search and list inventory with pagination and filtering
   * Routes to: GET /api/inventory/search on InventoryService
   * 
   * Supports:
   * - Text search across asset tag, serial number, device name
   * - Filter by status (Available, InUse, Maintenance, Offline, Retired)
   * - Filter by department
   * - Filter by location
   * - Filter by device type
   * - Pagination (page, pageSize)
   * 
   * @param params - Search parameters
   * @returns Paginated search results
   * @throws ApiError if request fails
   */
  async search(params: SearchInventoryRequest = {}): Promise<SearchInventoryResponse> {
    // Build query string from parameters
    const queryParams = new URLSearchParams()
    
    if (params.search) {
      queryParams.append('search', params.search)
    }
    if (params.status !== undefined) {
      queryParams.append('status', params.status.toString())
    }
    if (params.department) {
      queryParams.append('department', params.department)
    }
    if (params.location) {
      queryParams.append('location', params.location)
    }
    if (params.deviceType) {
      queryParams.append('deviceType', params.deviceType)
    }
    
    // Pagination defaults: page 1, 20 items per page
    queryParams.append('page', (params.page || 1).toString())
    queryParams.append('pageSize', (params.pageSize || 20).toString())

    const response = await apiClient.get<any>(
      'inventory',
      `/api/inventory?${queryParams.toString()}`
    )
    
    if (response.error) {
      throw response.error
    }
    
    // Map DTOs to frontend interface
    return {
      items: response.data!.items.map((item: unknown) => this.mapInventoryItem(item)),
      page: response.data!.page,
      pageSize: response.data!.pageSize,
      totalCount: response.data!.totalCount,
      totalPages: response.data!.totalPages
    }
  },

  /**
   * Register new inventory item
   * Routes to: POST /api/inventory on InventoryService
   * 
   * @param data - Inventory registration data
   * @returns Registered inventory with ID
   * @throws ApiError if request fails
   */
  async register(data: RegisterInventoryRequest): Promise<RegisterInventoryResponse> {
    const response = await apiClient.post<RegisterInventoryResponse>(
      'inventory',
      apiConfig.services.inventory.endpoints.register,
      data
    )

    if (response.error) {
      throw response.error
    }

    return response.data!
  },

  /**
   * Get single inventory by ID
   * Routes to: GET /api/inventory/{id} on InventoryService
   * 
   * @param id - Inventory ID (GUID)
   * @returns Full inventory item details
   * @throws ApiError if request fails or item not found
   */
  async getById(id: string): Promise<InventoryItem> {
    const response = await apiClient.get<InventoryItem>(
      'inventory',
      apiConfig.services.inventory.endpoints.getById(id)
    )

    if (response.error) {
      throw response.error
    }

    return response.data!
  },

  /**
   * Update inventory details
   * Routes to: PUT /api/inventory/{id} on InventoryService
   * 
   * @param id - Inventory ID (GUID)
   * @param data - Updated inventory data
   * @returns Updated inventory item
   * @throws ApiError if request fails or access denied
   */
  async update(id: string, data: UpdateInventoryRequest): Promise<InventoryItem> {
    const response = await apiClient.put<InventoryItem>(
      'inventory',
      apiConfig.services.inventory.endpoints.update(id),
      data
    )

    if (response.error) {
      throw response.error
    }

    return response.data!
  },

  /**
   * Soft delete (retire) inventory item
   * Routes to: DELETE /api/inventory/{id} on InventoryService
   * 
   * Requires: BiomedicalEngineer role or higher
   * 
   * @param id - Inventory ID (GUID)
   * @throws ApiError if request fails or access denied (403)
   */
  async retire(id: string): Promise<void> {
    const response = await apiClient.delete(
      'inventory',
      apiConfig.services.inventory.endpoints.retire(id)
    )

    if (response.error) {
      throw response.error
    }
  },

  /**
   * Assign inventory to department
   * Routes to: PUT /api/inventory/{id}/department on InventoryService
   * 
   * @param id - Inventory ID (GUID)
   * @param data - Assignment request with department name
   * @returns Updated inventory item
   * @throws ApiError if request fails
   */
  async assignDepartment(id: string, data: AssignDepartmentRequest): Promise<InventoryItem> {
    const response = await apiClient.put<InventoryItem>(
      'inventory',
      apiConfig.services.inventory.endpoints.assignDepartment(id),
      data
    )

    if (response.error) {
      throw response.error
    }

    return response.data!
  },

  /**
   * Transfer inventory to new location
   * Routes to: PUT /api/inventory/{id}/location on InventoryService
   * 
   * @param id - Inventory ID (GUID)
   * @param data - Transfer request with location name
   * @returns Updated inventory item
   * @throws ApiError if request fails
   */
  async transferLocation(id: string, data: TransferLocationRequest): Promise<InventoryItem> {
    const response = await apiClient.put<InventoryItem>(
      'inventory',
      apiConfig.services.inventory.endpoints.transferLocation(id),
      data
    )

    if (response.error) {
      throw response.error
    }

    return response.data!
  },

  /**
   * Get inventory movement history
   * Routes to: GET /api/inventory/{id}/history on InventoryService
   * 
   * @param id - Inventory ID (GUID)
   * @returns Array of inventory history entries
   * @throws ApiError if request fails
   */
  async getHistory(id: string): Promise<InventoryItem[]> {
    const response = await apiClient.get<InventoryItem[]>(
      'inventory',
      apiConfig.services.inventory.endpoints.history(id)
    )

    if (response.error) {
      throw response.error
    }

    return response.data!
  }

}