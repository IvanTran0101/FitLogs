import type { ReactNode } from 'react';
import { BottomNav } from './BottomNav'

type PageSellProps = {
    title:string;
    children: ReactNode;
};

export function PageShell({title, children}: PageSellProps) {
    return (
        <div className="page-shell">
            <header className="app-header">
                <h1>FitLogs</h1>
                <button className="icon-button" type ="button" aria-label="Open menu"> ☰ </button>
            </header>

            <section className="page-title">
                <span>{title}</span>
            </section>

            <main className="page-content">{children}</main>

            <BottomNav/>
        </div>
    )
}