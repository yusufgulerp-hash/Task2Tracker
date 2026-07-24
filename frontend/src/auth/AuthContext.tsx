import { createContext, useContext, useMemo, useState } from 'react'
import type { ReactNode } from 'react'
import * as authApi from '../api/auth'
import { TOKEN_STORAGE_KEY } from '../api/client'
import type { UserRole } from '../api/types'

const USER_STORAGE_KEY = 'task2tracker_user'

interface CurrentUser {
  userId: string
  email: string
  role: UserRole
}

interface AuthContextValue {
  user: CurrentUser | null
  isAuthenticated: boolean
  isAdmin: boolean
  isLoading: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

// localStorage senkron okunabildiği için, önceki oturumu bir useEffect
// yerine doğrudan useState'in lazy initializer'ında geri yüklüyoruz.
// Bu hem bir render/effect döngüsü daha az demek, hem de "önce
// giriş yapılmamış gibi görünüp sonra flaş bir şekilde giriş yapılmışa
// dönme" sorununu baştan ortadan kaldırıyor.
function readStoredUser(): CurrentUser | null {
  const rawUser = localStorage.getItem(USER_STORAGE_KEY)
  const token = localStorage.getItem(TOKEN_STORAGE_KEY)

  if (rawUser && token) {
    return JSON.parse(rawUser)
  }

  return null
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<CurrentUser | null>(readStoredUser)

  async function login(email: string, password: string) {
    const response = await authApi.login(email, password)

    const currentUser: CurrentUser = {
      userId: response.userId,
      email: response.email,
      role: response.role,
    }

    localStorage.setItem(TOKEN_STORAGE_KEY, response.accessToken)
    localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(currentUser))

    setUser(currentUser)
  }

  function logout() {
    localStorage.removeItem(TOKEN_STORAGE_KEY)
    localStorage.removeItem(USER_STORAGE_KEY)
    setUser(null)
  }

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: user !== null,
      isAdmin: user?.role === 'Admin',
      isLoading: false,
      login,
      logout,
    }),
    [user],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth, AuthProvider içinde kullanılmalı.')
  }
  return context
}
