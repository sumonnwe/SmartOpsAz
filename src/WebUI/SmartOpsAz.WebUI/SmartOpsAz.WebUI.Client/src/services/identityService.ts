import { apiClient } from './apiClient'
import { apiConfig } from '../config/apiConfig'

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

export const identityService = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>(
      'identity',
      apiConfig.services.identity.endpoints.login,
      credentials
    )

    if (response.error) {
      throw response.error
    }

    if (response.data) {
      // Store token and expiry
      apiClient.setAccessToken(response.data.accessToken)
      apiClient.setTokenExpiry(response.data.expiresAtUtc)
    }

    return response.data!
  },

  async logout(): Promise<void> {
    try {
      await apiClient.post(
        'identity',
        apiConfig.services.identity.endpoints.logout
      )
    } finally {
      apiClient.clearAccessToken()
    }
  }
}