import { useContext } from 'react'
import { ToastContext } from './toastContext'

/** Reads the shared toast actions and fails early when a page is mounted outside the app provider. */
export function useToast() {
  const context = useContext(ToastContext)
  if (!context) {
    throw new Error('useToast must be used inside ToastProvider')
  }

  return context
}
