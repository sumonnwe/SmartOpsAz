/**
 * Form validation utilities
 */

export const validators = {
  /**
   * Validate asset tag format
   */
  assetTag: (value: string): string | undefined => {
    if (!value) return 'Asset tag is required'
    if (value.length > 50) return 'Asset tag must be 50 characters or less'
    if (!/^[a-zA-Z0-9\-_]+$/.test(value))
      return 'Asset tag can only contain letters, numbers, hyphens, and underscores'
    return undefined
  },

  /**
   * Validate serial number format
   */
  serialNumber: (value: string): string | undefined => {
    if (!value) return 'Serial number is required'
    if (value.length > 50) return 'Serial number must be 50 characters or less'
    return undefined
  },

  /**
   * Validate device name
   */
  deviceName: (value: string): string | undefined => {
    if (!value) return 'Device name is required'
    if (value.length > 100) return 'Device name must be 100 characters or less'
    return undefined
  },

  /**
   * Validate device code
   */
  deviceCode: (value: string): string | undefined => {
    if (!value) return 'Device code is required'
    if (!/^[a-zA-Z0-9\-_]+$/.test(value))
      return 'Device code can only contain letters, numbers, hyphens, and underscores'
    return undefined
  },

  /**
   * Validate battery percentage
   */
  batteryPercentage: (value: number): string | undefined => {
    if (value < 0 || value > 100) return 'Battery must be between 0 and 100'
    return undefined
  },

  /**
   * Validate signal strength (RSSI)
   */
  signalStrength: (value: number): string | undefined => {
    if (value < -120 || value > 0) return 'Signal strength must be between -120 and 0'
    return undefined
  },

  /**
   * Validate username
   */
  username: (value: string): string | undefined => {
    if (!value) return 'Username is required'
    if (value.length < 3) return 'Username must be at least 3 characters'
    return undefined
  },

  /**
   * Validate password
   */
  password: (value: string): string | undefined => {
    if (!value) return 'Password is required'
    if (value.length < 6) return 'Password must be at least 6 characters'
    return undefined
  }
}

export function getValidationError(
  errors: Record<string, string[]> | undefined,
  fieldName: string
): string | undefined {
  if (!errors || !errors[fieldName]) return undefined
  return errors[fieldName][0]
}