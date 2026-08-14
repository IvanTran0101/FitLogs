import { createContext } from 'react'

export type ToastTone = 'success' | 'error'

export type ToastContextValue = {
  showToast: (message: string, tone?: ToastTone) => void
  dismissToast: () => void
}

export const ToastContext = createContext<ToastContextValue | undefined>(undefined)
