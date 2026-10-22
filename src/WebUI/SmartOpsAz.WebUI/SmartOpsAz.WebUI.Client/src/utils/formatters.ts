import { InventoryStatus, DeviceStatus, DeviceConnectivity } from '../types/api.types'

/**
 * Format inventory status to display string
 */
export function formatInventoryStatus(status: InventoryStatus): string {
  const statusMap: Record<InventoryStatus, string> = {
    [InventoryStatus.Available]: 'Available',
    [InventoryStatus.InUse]: 'In Use',
    [InventoryStatus.Maintenance]: 'Maintenance',
    [InventoryStatus.Offline]: 'Offline',
    [InventoryStatus.Retired]: 'Retired'
  }
  return statusMap[status] || 'Unknown'
}

/**
 * Get color for inventory status badge
 */
export function getInventoryStatusColor(status: InventoryStatus): 'success' | 'info' | 'warning' | 'error' | 'default' {
  const colorMap: Record<InventoryStatus, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
    [InventoryStatus.Available]: 'success',
    [InventoryStatus.InUse]: 'info',
    [InventoryStatus.Maintenance]: 'warning',
    [InventoryStatus.Offline]: 'error',
    [InventoryStatus.Retired]: 'default'
  }
  return colorMap[status] || 'default'
}

/**
 * Format device status to display string
 */
export function formatDeviceStatus(status: DeviceStatus): string {
  const statusMap: Record<DeviceStatus, string> = {
    [DeviceStatus.Online]: 'Online',
    [DeviceStatus.Offline]: 'Offline',
    [DeviceStatus.Warning]: 'Warning',
    [DeviceStatus.Critical]: 'Critical',
    [DeviceStatus.Maintenance]: 'Maintenance'
  }
  return statusMap[status] || 'Unknown'
}

/**
 * Get color for device status badge
 */
export function getDeviceStatusColor(status: DeviceStatus): 'success' | 'info' | 'warning' | 'error' | 'default' {
  const colorMap: Record<DeviceStatus, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
    [DeviceStatus.Online]: 'success',
    [DeviceStatus.Offline]: 'error',
    [DeviceStatus.Warning]: 'warning',
    [DeviceStatus.Critical]: 'error',
    [DeviceStatus.Maintenance]: 'info'
  }
  return colorMap[status] || 'default'
}

/**
 * Format date to readable string
 */
export function formatDate(dateString: string): string {
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

/**
 * Format date and time
 */
export function formatDateTime(dateString: string): string {
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

/**
 * Format time ago (e.g., "2 minutes ago")
 */
export function formatTimeAgo(dateString: string): string {
  const date = new Date(dateString)
  const now = new Date()
  const seconds = Math.floor((now.getTime() - date.getTime()) / 1000)

  let interval = seconds / 31536000
  if (interval > 1) return Math.floor(interval) + ' years ago'

  interval = seconds / 2592000
  if (interval > 1) return Math.floor(interval) + ' months ago'

  interval = seconds / 86400
  if (interval > 1) return Math.floor(interval) + ' days ago'

  interval = seconds / 3600
  if (interval > 1) return Math.floor(interval) + ' hours ago'

  interval = seconds / 60
  if (interval > 1) return Math.floor(interval) + ' minutes ago'

  return Math.floor(seconds) + ' seconds ago'
}

/**
 * Format connectivity type
 */
export function formatConnectivity(connectivity: DeviceConnectivity | string): string {
  if (typeof connectivity === 'string') {
    return connectivity.charAt(0).toUpperCase() + connectivity.slice(1)
  }
  return connectivity
}

/**
 * Format RSSI signal strength
 */
export function formatSignalStrength(rssi: number): string {
  if (rssi >= -50) return 'Excellent'
  if (rssi >= -60) return 'Very Good'
  if (rssi >= -70) return 'Good'
  if (rssi >= -80) return 'Fair'
  return 'Poor'
}

/**
 * Get signal strength color
 */
export function getSignalStrengthColor(rssi: number): 'success' | 'warning' | 'error' {
  if (rssi >= -70) return 'success'
  if (rssi >= -80) return 'warning'
  return 'error'
}