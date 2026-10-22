/**
 * Backend API DTOs - Mirror of .NET service contracts
 */

// ============================================
// Authentication Types
// ============================================

export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  tokenType: string
  expiresAtUtc: string
  userId: string
  username: string
  role: string
}

export interface AuthUser {
  userId: string
  username: string
  role: UserRole
  expiresAtUtc: string
}

export type UserRole = 
  | 'HospitalAdministrator'
  | 'BiomedicalEngineer'
  | 'Nurse'
  | 'SecurityOfficer'
  | 'MaintenanceEngineer'
  | 'Auditor'

// ============================================
// Inventory Types - Maps to InventoryService
// ============================================

export enum InventoryStatus {
  Available = 0,
  InUse = 1,
  Maintenance = 2,
  Offline = 3,
  Retired = 4
}

export interface InventoryItem {
  id: string
  assetTag: string
  serialNumber: string
  deviceName: string
  deviceType?: string
  manufacturer?: string
  modelNumber?: string
  purchaseDate?: string
  warrantyExpiration?: string
  department?: string
  location?: string
  status: InventoryStatus
  createdAt: string
  updatedAt: string
  isDeleted: boolean
}

export interface RegisterInventoryRequest {
  assetTag: string
  serialNumber: string
  deviceName: string
  deviceType?: string
  manufacturer?: string
  modelNumber?: string
  purchaseDate?: string
  warrantyExpiration?: string
  department?: string
  location?: string
}

export interface RegisterInventoryResponse {
  inventoryId: string
  assetTag: string
  serialNumber: string
}

export interface UpdateInventoryRequest {
  deviceName: string
  deviceType?: string
  manufacturer?: string
  modelNumber?: string
  purchaseDate?: string
  warrantyExpiration?: string
  department?: string
  location?: string
}

export interface AssignDepartmentRequest {
  department: string
}

export interface TransferLocationRequest {
  location: string
}

export interface SearchInventoryRequest {
  search?: string
  searchTerm?: string
  status?: InventoryStatus
  department?: string
  deviceType?: string
  location?: string
  page?: number
  pageSize?: number
}

export interface SearchInventoryResponse {
  items: InventoryItem[]
  totalCount: number
  page: number
  pageSize: number
}

export interface InventoryMovement {
  movementType: number
  fromDepartment?: string
  toDepartment?: string
  fromLocation?: string
  toLocation?: string
  occurredAt: string
}

export interface AssetHistoryResponse {
  movements: InventoryMovement[]
}

export const InventoryStatusLabel: Record<InventoryStatus, string> = {
  [InventoryStatus.Available]: 'Available',
  [InventoryStatus.InUse]: 'In Use',
  [InventoryStatus.Maintenance]: 'Maintenance',
  [InventoryStatus.Offline]: 'Offline',
  [InventoryStatus.Retired]: 'Retired'
}

export const InventoryStatusColor: Record<InventoryStatus, string> = {
  [InventoryStatus.Available]: '#4caf50',   // Green
  [InventoryStatus.InUse]: '#2196f3',       // Blue
  [InventoryStatus.Maintenance]: '#ff9800', // Orange
  [InventoryStatus.Offline]: '#9e9e9e',     // Gray
  [InventoryStatus.Retired]: '#757575'      // Dark Gray
}

// ============================================
// Device Types - Maps to DeviceService
// ============================================

export enum DeviceStatus {
  Online = 1,
  Offline = 2,
  Warning = 3,
  Critical = 4,
  Maintenance = 5
}

export enum DeviceType {
  TemperatureSensor = 1,
  HumiditySensor = 2,
  Gateway = 3,
  DoorController = 4,
  BatteryDevice = 5,
  NetworkDevice = 6,
  MedicalEquipmentController = 7
}

export enum DeviceConnectivity {
  Cellular = 'Cellular',
  WiFi = 'WiFi',
  Ethernet = 'Ethernet',
  Bluetooth = 'Bluetooth'
}

export interface IoTDevice {
  id: string
  deviceCode: string
  name: string
  deviceType: DeviceType
  status: DeviceStatus
  gatewayId?: string | null
  linkedAssetId?: string | null
  firmwareVersion?: string | null
  batteryPercentage?: number | null
  signalStrengthRssi?: number | null
  cpuUsagePercentage?: number | null
  memoryUsagePercentage?: number | null
  lastHeartbeatAtUtc?: string | null
  registeredAtUtc: string
  updatedAtUtc?: string | null
}

export interface RegisterDeviceRequest {
  deviceCode: string
  name: string
  deviceType: DeviceType
  gatewayId?: string | null
  firmwareVersion?: string | null
}

export interface RegisterDeviceResponse {
  id: string
  deviceCode: string
  name: string
}

export interface UpdateDeviceRequest {
  name: string
  firmwareVersion?: string | null
}

export interface SearchDeviceResponse {
  items: IoTDevice[]
  page: number
  pageSize: number
  totalCount?: number
}

export const DeviceStatusLabel: Record<DeviceStatus, string> = {
  [DeviceStatus.Online]: 'Online',
  [DeviceStatus.Offline]: 'Offline',
  [DeviceStatus.Warning]: 'Warning',
  [DeviceStatus.Critical]: 'Critical',
  [DeviceStatus.Maintenance]: 'Maintenance'
}

export const DeviceStatusColor: Record<DeviceStatus, string> = {
  [DeviceStatus.Online]: '#4caf50',        // Green
  [DeviceStatus.Offline]: '#9e9e9e',       // Gray
  [DeviceStatus.Warning]: '#ff9800',       // Orange
  [DeviceStatus.Critical]: '#f44336',      // Red
  [DeviceStatus.Maintenance]: '#2196f3'    // Blue
}

export const DeviceTypeLabel: Record<DeviceType, string> = {
  [DeviceType.TemperatureSensor]: 'Temperature Sensor',
  [DeviceType.HumiditySensor]: 'Humidity Sensor',
  [DeviceType.Gateway]: 'Gateway',
  [DeviceType.DoorController]: 'Door Controller',
  [DeviceType.BatteryDevice]: 'Battery Device',
  [DeviceType.NetworkDevice]: 'Network Device',
  [DeviceType.MedicalEquipmentController]: 'Medical Equipment Controller'
}

export interface LinkAssetRequest {
  assetId: string
  deviceType?: string
  location?: string
}

export interface HeartbeatRequest {
  batteryPercentage: number
  signalStrength: number
  connectivity: DeviceConnectivity
}

export interface DeviceHeartbeat {
  id: string
  deviceId: string
  batteryPercentage: number
  signalStrength: number
  connectivity: string
  receivedAt: string
}

export interface LinkAssetRequest {
  assetId: string
}

// ============================================
// Dashboard Types
// ============================================

export interface DashboardSummary {
  totalAssets: number
  availableAssets: number
  assetsInUse: number
  assetsUnderMaintenance: number
  totalDevices: number
  onlineDevices: number
  offlineDevices: number
  warningDevices: number
  criticalDevices: number
  lastUpdatedUtc: string
}

// ============================================
// Error Types
// ============================================

export interface ApiError {
  type?: string
  title?: string
  status: number
  message?: string
  errors?: Record<string, string[]>
  code?: string
}

export interface ApiResponse<T> {
  data?: T
  error?: ApiError
  status: number
}

export interface InventoryItem {
    inventoryId: string;

    // Keep all your existing properties
}

export interface SearchInventoryResponse {
    items: InventoryItem[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}