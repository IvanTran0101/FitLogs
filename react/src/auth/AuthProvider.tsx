import {
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import type { User } from 'oidc-client-ts'
import {
  getCurrentUser,
  login as startLogin,
  logout as startLogout,
  renewUserSession,
  userManager,
} from './authService'
import { getGrantedPolicies, type GrantedPolicies } from '../api/permissionsApi'
import { AuthContext, type AuthContextValue } from './authContext'

/** Provides shared OIDC user state while keeping token storage and requests in the auth/API layers. */
export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthContextValue['user']>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [permissions, setPermissions] = useState<GrantedPolicies>({})
  const [permissionsLoading, setPermissionsLoading] = useState(true)

  // Loads server-calculated permissions and fails closed when the configuration request is unavailable.
  const loadPermissions = useCallback(async (currentUser: User | null) => {
    if (!currentUser || currentUser.expired) {
      setPermissions({})
      setPermissionsLoading(false)
      return
    }

    setPermissionsLoading(true)
    try {
      setPermissions(await getGrantedPolicies())
    } catch {
      setPermissions({})
    } finally {
      setPermissionsLoading(false)
    }
  }, [])

  // Re-reads the persisted OIDC user so pages can refresh auth state after a callback or session change.
  const refreshUser = useCallback(async () => {
    setIsLoading(true)
    try {
      const currentUser = await getCurrentUser()
      const activeUser = currentUser?.expired
        ? await renewUserSession()
        : currentUser
      setUser(activeUser)
      await loadPermissions(activeUser)
      return activeUser
    } finally {
      setIsLoading(false)
    }
  }, [loadPermissions])

  useEffect(() => {
    let isMounted = true

    // Loads the stored user once and subscribes to OIDC events that change authentication state.
    async function synchronizeUser() {
      setIsLoading(true)
      try {
        const currentUser = await getCurrentUser()
        const activeUser = currentUser?.expired
          ? await renewUserSession()
          : currentUser
        if (isMounted) {
          setUser(activeUser)
        }
        await loadPermissions(activeUser)
      } catch {
        if (isMounted) {
          setUser(null)
          setPermissions({})
          setPermissionsLoading(false)
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
        void loadPermissions(nextUser)
      }
    })
    const unsubscribeUserUnloaded = userManager.events.addUserUnloaded(() => {
      if (isMounted) {
        setUser(null)
        setPermissions({})
        setPermissionsLoading(false)
        setIsLoading(false)
      }
    })
    const unsubscribeUserSignedOut = userManager.events.addUserSignedOut(() => {
      if (isMounted) {
        setUser(null)
        setPermissions({})
        setPermissionsLoading(false)
        setIsLoading(false)
      }
    })
    const unsubscribeAccessTokenExpired = userManager.events.addAccessTokenExpired(() => {
      if (isMounted) {
        // Attempts one final centralized renewal before protected routes fall back to login.
        void renewUserSession().then((renewedUser) => {
          if (isMounted) {
            setUser(renewedUser)
            void loadPermissions(renewedUser)
          }
        })
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
  }, [loadPermissions])

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isLoading,
      isAuthenticated: user !== null && !user.expired,
      // Role claims are UI hints only; backend permissions remain the security boundary.
      hasRole: (role: string) => {
        const claims = user?.profile as Record<string, unknown> | undefined
        const roleClaim = claims?.role ?? claims?.roles
        if (typeof roleClaim === 'string') {
          return roleClaim === role
        }

        return Array.isArray(roleClaim) && roleClaim.some((value) => value === role)
      },
      permissionsLoading,
      // Unknown permissions are denied so a failed configuration request never exposes a privileged control.
      hasPermission: (permission: string) => permissions[permission] === true,
      login: startLogin,
      logout: startLogout,
      refreshUser,
    }),
    [isLoading, permissions, permissionsLoading, refreshUser, user],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
