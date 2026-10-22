import { apiClient } from './apiClient'
import { apiConfig } from '../config/apiConfig'

export interface DashboardSummary {
  totalAssets: number
  totalDevices: number
  onlineDevices: number
  offlineDevices: number
}

export const dashboardService = {
  async getSummary(): Promise<DashboardSummary> {
    const response = await apiClient.get<DashboardSummary>(
      'dashboard',
      apiConfig.services.dashboard.endpoints.summary
    )

    if (response.error) {
      throw response.error
    }

    return response.data!
  }
}