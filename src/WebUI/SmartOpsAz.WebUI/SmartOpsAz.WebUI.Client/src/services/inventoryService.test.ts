/**
 * Inventory Service Tests
 * Verifies API integration with InventoryService backend
 */

import { describe, it, expect, beforeEach, vi } from 'vitest'
import { inventoryService } from './inventoryService'
import { apiClient } from './apiClient'
import { InventoryStatus, SearchInventoryResponse, InventoryItem } from '../types/api.types'

// Mock apiClient
vi.mock('./apiClient', () => ({
    apiClient: {
        get: vi.fn(),
        post: vi.fn(),
        put: vi.fn(),
        delete: vi.fn(),
        getAccessToken: vi.fn(() => 'test-token'),
        setAccessToken: vi.fn(),
        setTokenExpiry: vi.fn(),
        clearAccessToken: vi.fn(),
        isTokenValid: vi.fn(() => true)
    }
}))

describe('inventoryService', () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    describe('search()', () => {
        it('should fetch inventory with default parameters', async () => {
            const mockResponse: SearchInventoryResponse = {
                items: [
                    {
                        inventoryId: '123e4567-e89b-12d3-a456-426614174000',
                        assetTag: 'INV-001',
                        serialNumber: 'SN-12345',
                        deviceName: 'Vital Signs Monitor',
                        deviceType: 'Monitor',
                        department: 'ICU',
                        location: 'Ward A',
                        status: InventoryStatus.InUse,
                        createdAt: '2024-01-01T00:00:00Z',
                        updatedAt: '2024-01-02T00:00:00Z',
                        isDeleted: false,
                        id: ''
                    }
                ],
                page: 1,
                pageSize: 20,
                totalCount: 1,
                totalPages: 1
            }

            vi.mocked(apiClient.get).mockResolvedValueOnce({
                data: mockResponse,
                status: 200
            })

            const result = await inventoryService.search()

            expect(apiClient.get).toHaveBeenCalledWith(
                'inventory',
                expect.stringContaining('/api/inventory/search')
            )
            expect(result.items).toHaveLength(1)
            expect(result.items[0].assetTag).toBe('INV-001')
        })

        it('should include search parameters in query string', async () => {
            const mockResponse: SearchInventoryResponse = {
                items: [],
                page: 1,
                pageSize: 10,
                totalCount: 0,
                totalPages: 0
            }

            vi.mocked(apiClient.get).mockResolvedValueOnce({
                data: mockResponse,
                status: 200
            })

            await inventoryService.search({
                search: 'monitor',
                status: InventoryStatus.InUse,
                department: 'ICU',
                location: 'Ward A',
                page: 2,
                pageSize: 10
            })

            const callUrl = vi.mocked(apiClient.get).mock.calls[0][1]
            expect(callUrl).toContain('search=monitor')
            expect(callUrl).toContain('status=1')
            expect(callUrl).toContain('department=ICU')
            expect(callUrl).toContain('location=Ward%20A')
            expect(callUrl).toContain('page=2')
            expect(callUrl).toContain('pageSize=10')
        })

        it('should throw error if API request fails', async () => {
            const mockError = {
                status: 500,
                message: 'Internal Server Error'
            }

            vi.mocked(apiClient.get).mockResolvedValueOnce({
                status: 500,
                error: mockError
            })

            await expect(inventoryService.search()).rejects.toThrow()
        })

        it('should handle 401 unauthorized error', async () => {
            const mockError = {
                status: 401,
                message: 'Unauthorized'
            }

            vi.mocked(apiClient.get).mockResolvedValueOnce({
                status: 401,
                error: mockError
            })

            await expect(inventoryService.search()).rejects.toThrow()
        })

        it('should handle 403 forbidden error', async () => {
            const mockError = {
                status: 403,
                message: 'Insufficient permissions'
            }

            vi.mocked(apiClient.get).mockResolvedValueOnce({
                status: 403,
                error: mockError
            })

            await expect(inventoryService.search()).rejects.toThrow()
        })
    })

    describe('getById()', () => {
        it('should fetch inventory by ID', async () => {
            const mockItem: InventoryItem = {
                inventoryId: '123e4567-e89b-12d3-a456-426614174000',
                assetTag: 'INV-001',
                serialNumber: 'SN-12345',
                deviceName: 'Vital Signs Monitor',
                deviceType: 'Monitor',
                department: 'ICU',
                location: 'Ward A',
                status: InventoryStatus.InUse,
                createdAt: '2024-01-01T00:00:00Z',
                updatedAt: '2024-01-02T00:00:00Z',
                id: '',
                isDeleted: false
            }

            vi.mocked(apiClient.get).mockResolvedValueOnce({
                data: mockItem,
                status: 200
            })

            const result = await inventoryService.getById('123e4567-e89b-12d3-a456-426614174000')

            expect(apiClient.get).toHaveBeenCalledWith(
                'inventory',
                expect.stringContaining('/api/inventory/123e4567-e89b-12d3-a456-426614174000')
            )
            expect(result.assetTag).toBe('INV-001')
        })
    })

    describe('register()', () => {
        it('should register new inventory', async () => {
            const mockResponse = {
                inventoryId: '123e4567-e89b-12d3-a456-426614174000',
                assetTag: 'INV-001',
                serialNumber: 'SN-12345'
            }

            vi.mocked(apiClient.post).mockResolvedValueOnce({
                data: mockResponse,
                status: 201
            })

            const result = await inventoryService.register({
                assetTag: 'INV-001',
                serialNumber: 'SN-12345',
                deviceName: 'Vital Signs Monitor',
                deviceType: 'Monitor'
            })

            expect(apiClient.post).toHaveBeenCalledWith(
                'inventory',
                expect.stringContaining('/api/inventory'),
                expect.any(Object)
            )
            expect(result.inventoryId).toBe('123e4567-e89b-12d3-a456-426614174000')
        })
    })

    describe('retire()', () => {
        it('should retire inventory', async () => {
            vi.mocked(apiClient.delete).mockResolvedValueOnce({
                status: 200
            })

            await inventoryService.retire('123e4567-e89b-12d3-a456-426614174000')

            expect(apiClient.delete).toHaveBeenCalledWith(
                'inventory',
                expect.stringContaining('/api/inventory/123e4567-e89b-12d3-a456-426614174000')
            )
        })

        it('should throw 403 error if user lacks permissions', async () => {
            const mockError = {
                status: 403,
                message: 'Only BiomedicalEngineers can retire inventory'
            }

            vi.mocked(apiClient.delete).mockResolvedValueOnce({
                status: 403,
                error: mockError
            })

            await expect(
                inventoryService.retire('123e4567-e89b-12d3-a456-426614174000')
            ).rejects.toThrow()
        })
    })

    describe('getHistory()', () => {
        it('should fetch inventory history', async () => {
            const mockHistory: InventoryItem[] = [
                {
                    inventoryId: '123e4567-e89b-12d3-a456-426614174000',
                    assetTag: 'INV-001',
                    serialNumber: 'SN-12345',
                    deviceName: 'Vital Signs Monitor',
                    deviceType: 'Monitor',
                    department: 'ICU',
                    location: 'Ward A',
                    status: InventoryStatus.InUse,
                    createdAt: '2024-01-01T00:00:00Z',
                    updatedAt: '2024-01-02T00:00:00Z',
                    id: '',
                    isDeleted: false
                }
            ]

            vi.mocked(apiClient.get).mockResolvedValueOnce({
                data: mockHistory,
                status: 200
            })

            const result = await inventoryService.getHistory('123e4567-e89b-12d3-a456-426614174000')

            expect(apiClient.get).toHaveBeenCalledWith(
                'inventory',
                expect.stringContaining('/api/inventory/123e4567-e89b-12d3-a456-426614174000/history')
            )
            expect(result).toHaveLength(1)
        })
    })
})