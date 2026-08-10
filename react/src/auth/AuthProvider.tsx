import {
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import {
  getCurrentUser,
  login as startLogin,
  logout as startLogout,
  userManager,
} from './authService'
import { AuthContext, type AuthContextValue } from './authContext'

/** Provides shared OIDC user state while keeping token storage and requests in the auth/API layers. */
export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthContextValue['user']>(null)
  const [isLoading, setIsLoading] = useState(true)

  // Re-reads the persisted OIDC user so pages can refresh auth state after a callback or session change.
  const refreshUser = useCallback(async () => {
    setIsLoading(true)
    try {
      const currentUser = await getCurrentUser()
      setUser(currentUser)
      return currentUser
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    let isMounted = true

    // Loads the stored user once and subscribes to OIDC events that change authentication state.
    async function synchronizeUser() {
      setIsLoading(true)
      try {
        const currentUser = await getCurrentUser()
        if (isMounted) {
          setUser(currentUser)
        }
      } catch {
        if (isMounted) {
          setUser(null)
        }
      } finally {
        if (isMounted) {
          setIsLoading(false)
        }
      }
    }

    const unsubscribeUserLoaded = userManager.events.addUserLoaded((nextUser) => {
      if (isMounted) {
        setUser(nextUser)
        setIsLoading(false)
      }
    })
    const unsubscribeUserUnloaded = userManager.events.addUserUnloaded(() => {
      if (isMounted) {
        setUser(null)
        setIsLoading(false)
      }
    })
    const unsubscribeUserSignedOut = userManager.events.addUserSignedOut(() => {
      if (isMounted) {
        setUser(null)
        setIsLoading(false)
      }
    })
    const unsubscribeAccessTokenExpired = userManager.events.addAccessTokenExpired(() => {
      if (isMounted) {
        setUser(null)
      }
    })

    void synchronizeUser()

    return () => {
      isMounted = false
      unsubscribeUserLoaded()
      unsubscribeUserUnloaded()
      unsubscribeUserSignedOut()
      unsubscribeAccessTokenExpired()
    }
  }, [])

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isLoading,
      isAuthenticated: user !== null && !user.expired,
      login: startLogin,
      logout: startLogout,
      refreshUser,
    }),
    [isLoading, refreshUser, user],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
