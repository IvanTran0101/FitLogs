import { NeoCard } from './NeoCard'

type LoadingStateProps = {
  message?: string
}

export function LoadingState({ message = 'Đang tải dữ liệu...' }: LoadingStateProps) {
  return (
    <NeoCard className="state-card loading-state">
      <div className="loading-block" />
      <div className="loading-lines">
        <span />
        <span />
        <span />
      </div>
      <p>{message}</p>
    </NeoCard>
  )
}