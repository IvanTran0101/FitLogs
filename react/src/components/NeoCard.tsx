import type { HTMLAttributes, ReactNode } from 'react';

type NeoCardProps = {
    children: ReactNode;
    className?: string;
} & HTMLAttributes<HTMLDivElement>;

export function NeoCard({ children, className = '', ...props }: NeoCardProps) {
    return (
        <div className={`neo-card ${className}`} {...props}>
            {children}
        </div>
    )
}
