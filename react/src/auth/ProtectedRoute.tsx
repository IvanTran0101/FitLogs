import type { ReactNode } from 'react'
import { useLocation } from 'react-router-dom'
import { ErrorState } from '../components/ErrorState'
import { LoadingState } from '../components/LoadingState'
import { NeoButton } from '../components/NeoButton'
import { PageShell } from '../components/PageShell'
import { useAuth } from './useAuth'

type ProtectedRouteProps = {
  children: ReactNode
}

/** Shows private content only after auth is ready, otherwise provides a login action for the requested path. */
export function ProtectedRoute({ children }: ProtectedRouteProps) {
  const { isAuthenticated, isLoading, login } = useAuth()
  const location = useLocation()
  const returnUrl = `${location.pathname}${location.search}${location.hash}`

  if (isLoading) {
    return (
      <PageShell title="Đăng nhập">
        <LoadingState message="Đang kiểm tra phiên đăng nhập..." />
      </PageShell>
    )
  }

  if (!isAuthenticated) {
    return (
      <PageShell title="Đăng nhập">
        <ErrorState
          title="Cần đăng nhập"
          message="Bạn cần đăng nhập để xem khu vực cá nhân này."
          action={
            <NeoButton onClick={() => void login(returnUrl)}>
              Đăng nhập
            </NeoButton>
          }
        />
      </PageShell>
    )
  }

  return <>{children}</>
}
