import type { ReactNode } from 'react'
import { LoadingState } from './LoadingState'
import { useAuth } from '../auth/useAuth'

type PermissionGateProps = {
  permission: string
  children: ReactNode
  fallback?: ReactNode
  loadingFallback?: ReactNode
}

/** Shows a control only when ABP has granted its permission; the backend remains the real security boundary. */
export function PermissionGate({
  permission,
  children,
  fallback = null,
  loadingFallback = null,
}: PermissionGateProps) {
  const { hasPermission, isAuthenticated, permissionsLoading } = useAuth()

  if (isAuthenticated && permissionsLoading) {
    return <>{loadingFallback}</>
  }

  return hasPermission(permission) ? <>{children}</> : <>{fallback}</>
}

/** Provides a compact loading placeholder for permission-gated page actions when desired. */
export function PermissionLoadingState() {
  return <LoadingState message="Đang kiểm tra quyền..." />
}
