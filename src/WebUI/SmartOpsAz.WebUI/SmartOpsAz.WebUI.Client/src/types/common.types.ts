/**
 * Common application types
 */

export interface PageState {
  page: number
  pageSize: number
}

export interface SortState {
  sortBy?: string
  sortDirection?: 'asc' | 'desc'
}

export interface FilterState {
  search?: string
  status?: number
  department?: string
  location?: string
  deviceType?: string
}

export interface NotificationMessage {
  id: string
  message: string
  type: 'success' | 'error' | 'info' | 'warning'
  duration?: number
}