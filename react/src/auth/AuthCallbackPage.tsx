import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { handleLoginCallback } from './authService'
import { ErrorState } from '../components/ErrorState'
import { LoadingState } from '../components/LoadingState'
import { PageShell } from '../components/PageShell'
import { useAuth } from './useAuth'

/** Accepts only an internal SPA path so login state cannot redirect the browser to an external site. */
function getSafeReturnUrl(state: unknown) {
  if (!state || typeof state !== 'object' || !('returnUrl' in state)) {
    return '/'
  }

  const returnUrl = state.returnUrl
  return typeof returnUrl === 'string' && returnUrl.startsWith('/') && !returnUrl.startsWith('//')
    ? returnUrl
    : '/'
}

export function AuthCallbackPage() {
  const navigate = useNavigate()
  const { refreshUser } = useAuth()
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    // Completes the OIDC callback and returns the user to the protected path they originally requested.
    async function completeLogin() {
      try {
        const user = await handleLoginCallback()
        // Silent renew callbacks finish inside an iframe and do not return a new redirect user.
        if (!user) {
          return
        }

        await refreshUser()
        navigate(getSafeReturnUrl(user.state), { replace: true })
      } catch (error) {
        setErrorMessage(
          error instanceof Error ? error.message : 'Không thể hoàn tất đăng nhập.',
        )
      }
    }

    void completeLogin()
  }, [navigate, refreshUser])

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
