import { useEffect, useRef, useState, type ReactNode } from 'react'
import { NavLink, useLocation } from 'react-router-dom'
import { BottomNav } from './BottomNav'

type PageShellProps = {
  title: string
  children: ReactNode
}

const menuItems = [
  { label: 'Trang chủ', to: '/' },
  { label: 'Ăn uống', to: '/food' },
  { label: 'Buổi tập', to: '/workout' },
  { label: 'Kế hoạch', to: '/plans' },
  { label: 'Thư viện bài tập', to: '/exercises' },
  { label: 'Hồ sơ', to: '/profile' },
]

/** Provides the shared mobile frame and an accessible quick-navigation menu. */
export function PageShell({ title, children }: PageShellProps) {
  const location = useLocation()
  const [isMenuOpen, setIsMenuOpen] = useState(false)
  const menuButtonRef = useRef<HTMLButtonElement>(null)
  const firstMenuLinkRef = useRef<HTMLAnchorElement>(null)

  // Closes the menu when a link changes the current route or selected query state.
  useEffect(() => {
    setIsMenuOpen(false)
  }, [location.pathname, location.search])

  // Allows keyboard users to dismiss the menu without needing to tab through every link.
  useEffect(() => {
    if (!isMenuOpen) {
      return
    }

    firstMenuLinkRef.current?.focus()
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        setIsMenuOpen(false)
        menuButtonRef.current?.focus()
      }
    }

    window.addEventListener('keydown', handleKeyDown)
    return () => window.removeEventListener('keydown', handleKeyDown)
  }, [isMenuOpen])

  function closeMenu() {
    setIsMenuOpen(false)
    menuButtonRef.current?.focus()
  }

  return (
    <div className="page-shell">
      <header className="app-header">
        <h1>FitLogs</h1>
        <button
          ref={menuButtonRef}
          className="icon-button"
          type="button"
          aria-label={isMenuOpen ? 'Đóng menu' : 'Mở menu'}
          aria-expanded={isMenuOpen}
          aria-controls="app-menu"
          onClick={() => setIsMenuOpen((isOpen) => !isOpen)}
        >
          ☰
        </button>
      </header>

      {isMenuOpen ? (
        <nav id="app-menu" className="app-menu" aria-label="Điều hướng nhanh">
          <p className="eyebrow">Điều hướng nhanh</p>
          <div className="app-menu-list">
            {menuItems.map((item, index) => (
              <NavLink
                key={item.to}
                ref={index === 0 ? firstMenuLinkRef : undefined}
                className={({ isActive }) =>
                  isActive ? 'app-menu-link active' : 'app-menu-link'
                }
                to={item.to}
                end={item.to === '/'}
                role="menuitem"
                onClick={closeMenu}
              >
                {item.label}
              </NavLink>
            ))}
          </div>
        </nav>
      ) : null}

      <section className="page-title">
        <span>{title}</span>
      </section>

      <main className="page-content">{children}</main>

      <BottomNav />
    </div>
  )
}
