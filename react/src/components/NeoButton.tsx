import type {ButtonHTMLAttributes, ReactNode} from 'react';

type NeoButtonProps = {
    children: ReactNode;
} & ButtonHTMLAttributes<HTMLButtonElement>;

export function NeoButton({ children, className = '' ,type = 'button', ...props }: NeoButtonProps) {
    return (
        <button 
        className={`neo-button ${className}`}
        type={type}
        {...props}
        >
            {children}
        </button>
    )
}
