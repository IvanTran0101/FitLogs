import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { handleLogoutCallback } from './authService'
import { LoadingState } from '../components/LoadingState'
import { PageShell } from '../components/PageShell'

export function AuthLogoutCallbackPage() {
  const navigate = useNavigate()

  useEffect(() => {
    async function completeLogout() {
      await handleLogoutCallback()
      navigate('/')
    }

    void completeLogout()
  }, [navigate])

  return (
    <PageShell title="Đăng xuất">
      <LoadingState message="Đang hoàn tất đăng xuất..." />
    </PageShell>
  )
}