import { createContext } from 'react'
import type { User } from 'oidc-client-ts'

export type AuthContextValue = {
  user: User | null
  isLoading: boolean
  isAuthenticated: boolean
  hasRole: (role: string) => boolean
  permissionsLoading: boolean
  hasPermission: (permission: string) => boolean
  login: (returnUrl?: string) => Promise<void>
  logout: () => Promise<void>
  refreshUser: () => Promise<User | null>
}

export const AuthContext = createContext<AuthContextValue | undefined>(undefined)
