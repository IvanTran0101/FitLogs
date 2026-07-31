import { NavLink } from 'react-router-dom'

type NavItem = {
  label: string
  icon: string
  to: string
}

const navItems: NavItem[] = [
  { label: 'Home', icon: '⌂', to: '/' },
  { label: 'Food', icon: '🍴', to: '/food' },
  { label: 'Training', icon: '▣', to: '/workout' },
  { label: 'Plan', icon: '☷', to: '/plans' },
  { label: 'Profile', icon: '●', to: '/profile' },
]

export function BottomNav() {
  return (
    <nav className="bottom-nav" aria-label="Main navigation">
      {navItems.map((item) => (
        <NavLink
          key={item.to}
          to={item.to}
          className={({ isActive }) =>
            isActive ? 'bottom-nav-item active' : 'bottom-nav-item'
          }
          end={item.to === '/'}
        >
          <span className="bottom-nav-icon">{item.icon}</span>
          <span className="bottom-nav-label">{item.label}</span>
        </NavLink>
      ))}
    </nav>
  )
}