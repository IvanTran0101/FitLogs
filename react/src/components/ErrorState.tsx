import type { ReactNode } from 'react'
import { NeoCard } from './NeoCard'

type ErrorStateProps = {
  title?: string
  message: string
  action?: ReactNode
}

export function ErrorState({
  title = 'Có lỗi xảy ra',
  message,
  action,
}: ErrorStateProps) {
  return (
    <NeoCard className="state-card error-state" role="alert" aria-live="assertive">
      <div className="state-icon">!</div>
      <h2>{title}</h2>
      <p>{message}</p>
      {action ? <div className="state-action">{action}</div> : null}
    </NeoCard>
  )
}
