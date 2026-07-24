import type { ReactNode } from 'react'
import { Link, NavLink } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'

const navItems = [
  { to: '/projects', label: 'Projeler' },
  { to: '/my-tasks', label: 'Görevlerim' },
]

export function Layout({ children }: { children: ReactNode }) {
  const { user, isAdmin, logout } = useAuth()

  return (
    <div className="flex min-h-screen bg-bg">
      <aside className="flex w-60 shrink-0 flex-col border-r border-border bg-surface">
        <div className="px-6 py-6">
          <div className="flex items-center gap-2">
            <span className="h-2 w-2 rounded-full bg-branch" />
            <span className="font-display text-lg font-semibold tracking-tight text-ink">
              Task2Tracker
            </span>
          </div>
        </div>

        <nav className="flex flex-1 flex-col gap-1 px-3">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                [
                  'relative rounded-md px-3 py-2 text-sm font-medium transition-colors',
                  isActive
                    ? 'bg-branch-soft text-branch before:absolute before:left-0 before:top-1.5 before:bottom-1.5 before:w-0.5 before:rounded-full before:bg-branch before:content-[""]'
                    : 'text-ink-muted hover:bg-surface-alt hover:text-ink',
                ].join(' ')
              }
            >
              {item.label}
            </NavLink>
          ))}
        </nav>

        <div className="border-t border-border px-4 py-4">
          <Link
            to="/profile"
            className="mb-1 block rounded-md px-2 py-1.5 transition-colors hover:bg-surface-alt"
          >
            <p className="truncate text-sm font-medium text-ink">{user?.email}</p>
            <p className="text-xs text-ink-faint">
              {isAdmin ? 'Admin · Profili görüntüle' : 'Üye · Profili görüntüle'}
            </p>
          </Link>
          <button
            onClick={logout}
            className="w-full rounded-md px-3 py-2 text-left text-sm font-medium text-ink-muted transition-colors hover:bg-surface-alt hover:text-ink"
          >
            Çıkış yap
          </button>
        </div>
      </aside>

      <main className="flex-1 overflow-y-auto">
        <div className="mx-auto max-w-5xl px-8 py-10">{children}</div>
      </main>
    </div>
  )
}
