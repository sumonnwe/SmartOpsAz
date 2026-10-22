import axios, {
  AxiosInstance,
  AxiosRequestConfig,
  AxiosError,
  InternalAxiosRequestConfig
} from 'axios'
import { ApiError, ApiResponse } from '../types/api.types'
import { apiConfig, ApiService } from '../config/apiConfig'

class ApiClient {
  private clients: Map<ApiService, AxiosInstance> = new Map()

  constructor() {
    // Initialize a client for each service
    const services: ApiService[] = ['identity', 'inventory', 'device', 'dashboard']

    services.forEach(service => {
      const config = apiConfig.services[service]
      const client = axios.create({
        baseURL: config.baseUrl,
        timeout: 30000,
        headers: {
          'Content-Type': 'application/json'
        }
      })

      // Request interceptor: attach JWT token (except for login)
      client.interceptors.request.use(
        (config: InternalAxiosRequestConfig) => {
          // Don't attach token to login requests
          if (config.url !== apiConfig.services.identity.endpoints.login) {
            const token = this.getAccessToken()
            if (token) {
              config.headers.Authorization = `Bearer ${token}`
            }
          }
          return config
        },
        (error) => Promise.reject(error)
      )

      // Response interceptor: handle errors
      client.interceptors.response.use(
        (response) => response,
        (error: AxiosError) => {
          if (error.response?.status === 401) {
            // Token expired or invalid - redirect to login
            this.clearAccessToken()
            window.location.href = '/login'
          }
          // Note: 403 is NOT handled here - preserve authorization errors
          return Promise.reject(error)
        }
      )

      this.clients.set(service, client)
    })
  }

  /**
   * Get the appropriate client for a service
   */
  private getClient(service: ApiService): AxiosInstance {
    const client = this.clients.get(service)
    if (!client) {
      throw new Error(`API client for service '${service}' not initialized`)
    }
    return client
  }

  /**
   * Generic GET request
   */
  async get<T>(
    service: ApiService,
    url: string,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    try {
      const response = await this.getClient(service).get<T>(url, config)
      return {
        data: response.data,
        status: response.status
      }
    } catch (error) {
      return this.handleError(error)
    }
  }

  /**
   * Generic POST request
   */
  async post<T, D = unknown>(
    service: ApiService,
    url: string,
    data?: D,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    try {
      const response = await this.getClient(service).post<T>(url, data, config)
      return {
        data: response.data,
        status: response.status
      }
    } catch (error) {
      return this.handleError<T>(error)
    }
  }

  /**
   * Generic PUT request
   */
  async put<T, D = unknown>(
    service: ApiService,
    url: string,
    data?: D,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    try {
      const response = await this.getClient(service).put<T>(url, data, config)
      return {
        data: response.data,
        status: response.status
      }
    } catch (error) {
      return this.handleError<T>(error)
    }
  }

  /**
   * Generic DELETE request
   */
  async delete<T>(
    service: ApiService,
    url: string,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    try {
      const response = await this.getClient(service).delete<T>(url, config)
      return {
        data: response.data,
        status: response.status
      }
    } catch (error) {
      return this.handleError<T>(error)
    }
  }

  /**
   * Handle API errors consistently
   */
  private handleError<T>(error: unknown): ApiResponse<T> {
    if (axios.isAxiosError(error)) {
      const status = error.response?.status ?? 500
      const data = error.response?.data as
        | Record<string, unknown>
        | undefined

      const apiError: ApiError = {
        status,
        message:
          typeof data?.message === 'string'
            ? data.message
            : error.message || 'An error occurred',
        errors:
          typeof data?.errors === 'object' &&
          data.errors !== null
            ? (data.errors as Record<string, string[]>)
            : undefined,
        code:
          typeof data?.code === 'string'
            ? data.code
            : undefined
      }

      return {
        status,
        error: apiError
      }
    }

    return {
      status: 500,
      error: {
        status: 500,
        message:
          error instanceof Error
            ? error.message
            : 'An unexpected error occurred'
      }
    }
  }

  /**
   * Token management
   */
  setAccessToken(token: string): void {
    sessionStorage.setItem('accessToken', token)
  }

  getAccessToken(): string | null {
    return sessionStorage.getItem('accessToken')
  }

  clearAccessToken(): void {
    sessionStorage.removeItem('accessToken')
    sessionStorage.removeItem('tokenExpiresAt')
  }

  isTokenValid(): boolean {
    const token = this.getAccessToken()
    const expiresAt = sessionStorage.getItem('tokenExpiresAt')
    if (!token || !expiresAt) return false
    return new Date(expiresAt) > new Date()
  }

  setTokenExpiry(expiresAtUtc: string): void {
    sessionStorage.setItem('tokenExpiresAt', expiresAtUtc)
  }
}

export const apiClient = new ApiClient()