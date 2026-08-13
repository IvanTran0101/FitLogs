import type { ReactNode } from 'react'
import {NeoCard} from './NeoCard'

type EmptyStateProps = {
    title: string
    message: string
    action?: ReactNode
}
export function EmptyState({ title, message, action }: EmptyStateProps) {
    return (
        <NeoCard className="state-card empty-state" role="status" aria-live="polite">
            <div className="state-content">□</div>
            <h2>{title}</h2>
            <p>{message}</p>
            {action ? <div className="state-action">{action}</div> : null}
        </NeoCard>
    )
}
