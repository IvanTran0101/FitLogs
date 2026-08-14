import {
  useCallback,
  useEffect,
  useRef,
  useState,
  type ReactNode,
} from 'react'
import { ToastContext, type ToastTone } from './toastContext'

type ToastMessage = {
  id: number
  message: string
  tone: ToastTone
}

const TOAST_DURATION_MS = 3500

/** Provides short-lived mutation feedback without replacing page-level error states or navigation. */
export function ToastProvider({ children }: { children: ReactNode }) {
  const [toast, setToast] = useState<ToastMessage | null>(null)
  const nextToastId = useRef(0)
  const timeoutId = useRef<number | null>(null)

  // Clears the previous timer whenever a new notification replaces the visible one.
  const dismissToast = useCallback(() => {
    setToast(null)
    if (timeoutId.current !== null) {
      window.clearTimeout(timeoutId.current)
      timeoutId.current = null
    }
  }, [])

  // Shows one accessible notification and automatically removes it after a short delay.
  const showToast = useCallback(
    (message: string, tone: ToastTone = 'success') => {
      if (timeoutId.current !== null) {
        window.clearTimeout(timeoutId.current)
      }

      const id = nextToastId.current + 1
      nextToastId.current = id
      setToast({ id, message, tone })
      timeoutId.current = window.setTimeout(() => {
        setToast((currentToast) =>
          currentToast?.id === id ? null : currentToast,
        )
        timeoutId.current = null
      }, TOAST_DURATION_MS)
    },
    [],
  )

  // Prevents a timer from surviving if the app shell is unmounted.
  useEffect(() => {
    return () => {
      if (timeoutId.current !== null) {
        window.clearTimeout(timeoutId.current)
      }
    }
  }, [])

  return (
    <ToastContext.Provider value={{ showToast, dismissToast }}>
      {children}
      {toast ? (
        <div
          className="toast-viewport"
          aria-live={toast.tone === 'error' ? 'assertive' : 'polite'}
          aria-atomic="true"
        >
          <div className={`toast toast-${toast.tone}`} role="status">
            <span>{toast.message}</span>
            <button
              className="toast-dismiss"
              type="button"
              aria-label="Đóng thông báo"
              onClick={dismissToast}
            >
              ×
            </button>
          </div>
        </div>
      ) : null}
    </ToastContext.Provider>
  )
}
