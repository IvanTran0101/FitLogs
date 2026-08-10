import { useContext } from 'react'
import { AuthContext } from './authContext'

/** Returns shared authentication state and fails clearly when called outside the provider. */
export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used inside AuthProvider')
  }

  return context
}
