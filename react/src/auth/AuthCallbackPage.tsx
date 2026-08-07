import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { handleLoginCallback } from './authService'
import { ErrorState } from '../components/ErrorState'
import { LoadingState } from '../components/LoadingState'
import { PageShell } from '../components/PageShell'

export function AuthCallbackPage() {
  const navigate = useNavigate()
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    async function completeLogin() {
      try {
        await handleLoginCallback()
        navigate('/')
      } catch (error) {
        setErrorMessage(
          error instanceof Error ? error.message : 'Không thể hoàn tất đăng nhập.',
        )
      }
    }

    void completeLogin()
  }, [navigate])

  return (
    <PageShell title="Đăng nhập">
      {errorMessage ? (
        <ErrorState message={errorMessage} />
      ) : (
        <LoadingState message="Đang hoàn tất đăng nhập..." />
      )}
    </PageShell>
  )
}