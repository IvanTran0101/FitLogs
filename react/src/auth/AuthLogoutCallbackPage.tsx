import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { handleLogoutCallback } from './authService'
import { LoadingState } from '../components/LoadingState'
import { PageShell } from '../components/PageShell'
import { useAuth } from './useAuth'

export function AuthLogoutCallbackPage() {
  const navigate = useNavigate()
  const { refreshUser } = useAuth()

  useEffect(() => {
    // Completes the provider logout callback and refreshes shared state before returning to the protected home route.
    async function completeLogout() {
      await handleLogoutCallback()
      await refreshUser()
      navigate('/')
    }

    void completeLogout()
  }, [navigate, refreshUser])

  return (
    <PageShell title="Đăng xuất">
      <LoadingState message="Đang hoàn tất đăng xuất..." />
    </PageShell>
  )
}
