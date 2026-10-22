import React, { createContext, useContext, useState, useCallback, useEffect } from 'react'
import { AuthUser, UserRole } from '../../../types/api.types'
import { apiClient } from '../../../services/apiClient'

interface AuthContextType {
  user: AuthUser | null
  isLoading: boolean
  isAuthenticated: boolean
  login: (username: string, password: string) => Promise<void>
  logout: () => void
  hasRole: (role: UserRole | UserRole[]) => boolean
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<AuthUser | null>(null)
  const [isLoading, setIsLoading] = useState(false)

  // Restore user from session on mount
  useEffect(() => {
    const token = apiClient.getAccessToken()
    const storedUser = sessionStorage.getItem('user')
    if (token && storedUser && apiClient.isTokenValid()) {
      try {
        setUser(JSON.parse(storedUser))
      } catch (error) {
        apiClient.clearAccessToken()
      }
    }
  }, [])

  const login = useCallback(async (username: string, password: string) => {
    setIsLoading(true)
    try {
      const response = await apiClient.post('identity','/api/auth/login', {
        username,
        password
      })

      if (response.error || !response.data) {
        throw new Error(response.error?.message || 'Login failed')
      }

      const loginResponse = response.data as any
      const authUser: AuthUser = {
        userId: loginResponse.userId,
        username: loginResponse.username,
        role: loginResponse.role,
        expiresAtUtc: loginResponse.expiresAtUtc
      }

      // Store token and user info
      apiClient.setAccessToken(loginResponse.accessToken)
      apiClient.setTokenExpiry(loginResponse.expiresAtUtc)
      sessionStorage.setItem('user', JSON.stringify(authUser))

      setUser(authUser)
    } finally {
      setIsLoading(false)
    }
  }, [])

  const logout = useCallback(() => {
    apiClient.clearAccessToken()
    sessionStorage.removeItem('user')
    sessionStorage.removeItem('tokenExpiresAt')
    setUser(null)
  }, [])

  const hasRole = useCallback(
    (roles: UserRole | UserRole[]): boolean => {
      if (!user) return false
      const roleArray = Array.isArray(roles) ? roles : [roles]
      return roleArray.includes(user.role)
    },
    [user]
  )

  const value: AuthContextType = {
    user,
    isLoading,
    isAuthenticated: !!user,
    login,
    logout,
    hasRole
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error('useAuth must be used within AuthProvider')
  }
  return context
}