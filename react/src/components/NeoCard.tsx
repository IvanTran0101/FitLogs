import type { ReactNode } from 'react';

type NeoCardProps = {
    children: ReactNode;
    className?: string;
};

export function NeoCard({ children, className = '' }: NeoCardProps) {
    return (
        <div className={`neo-card ${className}`}>
            {children}
        </div>
    )
}
