import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import * as usersApi from '../api/users'
import { getApiErrorMessage } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import { AdminPanel } from '../components/AdminPanel'

export function ProfilePage() {
  const { user, isAdmin } = useAuth()

  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)
  const [isSaving, setIsSaving] = useState(false)

  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [email, setEmail] = useState('')

  useEffect(() => {
    if (!user) return
    usersApi
      .getUserById(user.userId)
      .then((detail) => {
        setFirstName(detail.firstName)
        setLastName(detail.lastName)
        setEmail(detail.email)
      })
      .catch((err) => setError(getApiErrorMessage(err)))
      .finally(() => setIsLoading(false))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user?.userId])

  async function handleSave(e: FormEvent) {
    e.preventDefault()
    if (!user) return
    setIsSaving(true)
    setError(null)
    setSuccessMessage(null)
    try {
      await usersApi.updateUser(user.userId, { firstName, lastName, email })
      setSuccessMessage('Bilgilerin güncellendi.')
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <div className="flex flex-col gap-8">
      <div>
        <h1 className="font-display text-2xl font-semibold text-ink">Profilim</h1>
        <p className="mt-1 text-sm text-ink-muted">
          Hesap bilgilerini görüntüle ve düzenle.
        </p>
      </div>

      <div className="max-w-md rounded-xl border border-border bg-surface p-5">
        {isLoading ? (
          <p className="text-sm text-ink-muted">Yükleniyor...</p>
        ) : (
          <form onSubmit={handleSave} className="flex flex-col gap-4">
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="mb-1.5 block text-sm font-medium text-ink">Ad</label>
                <input
                  required
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                  className="w-full rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
                />
              </div>
              <div>
                <label className="mb-1.5 block text-sm font-medium text-ink">Soyad</label>
                <input
                  required
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                  className="w-full rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
                />
              </div>
            </div>

            <div>
              <label className="mb-1.5 block text-sm font-medium text-ink">E-posta</label>
              <input
                type="email"
                required
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
              />
            </div>

            <div>
              <label className="mb-1.5 block text-sm font-medium text-ink">Rol</label>
              <p className="text-sm text-ink-muted">{isAdmin ? 'Admin' : 'Üye'}</p>
            </div>

            {error && (
              <p className="rounded-md bg-red-soft px-3 py-2 text-sm text-red">{error}</p>
            )}
            {successMessage && (
              <p className="rounded-md bg-branch-soft px-3 py-2 text-sm text-branch">
                {successMessage}
              </p>
            )}

            <button
              type="submit"
              disabled={isSaving}
              className="w-fit rounded-md bg-branch px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-branch-hover disabled:opacity-60"
            >
              {isSaving ? 'Kaydediliyor...' : 'Kaydet'}
            </button>
          </form>
        )}
      </div>

      {isAdmin && (
        <div>
          <h2 className="mb-4 font-display text-xl font-semibold text-ink">
            Admin Paneli
          </h2>
          <AdminPanel />
        </div>
      )}
    </div>
  )
}
