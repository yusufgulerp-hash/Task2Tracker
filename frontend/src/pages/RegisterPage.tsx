import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link } from 'react-router-dom'
import * as authApi from '../api/auth'
import { getApiErrorMessage } from '../api/client'

export function RegisterPage() {
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setError(null)
    setIsSubmitting(true)

    try {
      const result = await authApi.register({ firstName, lastName, email, password })
      setSuccessMessage(result.message)
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-bg px-4">
      <div className="w-full max-w-sm">
        <div className="mb-8 flex items-center gap-2">
          <span className="h-2.5 w-2.5 rounded-full bg-branch" />
          <span className="font-display text-xl font-semibold tracking-tight text-ink">
            Task2Tracker
          </span>
        </div>

        <div className="rounded-xl border border-border bg-surface p-8 shadow-sm">
          {successMessage ? (
            <>
              <h1 className="mb-1 font-display text-2xl font-semibold text-ink">
                Kayıt alındı
              </h1>
              <p className="mb-6 text-sm text-ink-muted">{successMessage}</p>
              <Link
                to="/login"
                className="inline-block w-full rounded-md bg-branch px-4 py-2 text-center text-sm font-medium text-white transition-colors hover:bg-branch-hover"
              >
                Girişe dön
              </Link>
            </>
          ) : (
            <>
              <h1 className="mb-1 font-display text-2xl font-semibold text-ink">
                Hesap oluştur
              </h1>
              <p className="mb-6 text-sm text-ink-muted">
                Kayıt sonrası admin onayı beklenir.
              </p>

              <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <label className="mb-1.5 block text-sm font-medium text-ink">
                      Ad
                    </label>
                    <input
                      required
                      value={firstName}
                      onChange={(e) => setFirstName(e.target.value)}
                      className="w-full rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
                    />
                  </div>
                  <div>
                    <label className="mb-1.5 block text-sm font-medium text-ink">
                      Soyad
                    </label>
                    <input
                      required
                      value={lastName}
                      onChange={(e) => setLastName(e.target.value)}
                      className="w-full rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
                    />
                  </div>
                </div>

                <div>
                  <label className="mb-1.5 block text-sm font-medium text-ink">
                    E-posta
                  </label>
                  <input
                    type="email"
                    required
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    className="w-full rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
                  />
                </div>

                <div>
                  <label className="mb-1.5 block text-sm font-medium text-ink">
                    Şifre
                  </label>
                  <input
                    type="password"
                    required
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    className="w-full rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
                  />
                </div>

                {error && (
                  <p className="rounded-md bg-red-soft px-3 py-2 text-sm text-red">
                    {error}
                  </p>
                )}

                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="mt-2 w-full rounded-md bg-branch px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-branch-hover disabled:opacity-60"
                >
                  {isSubmitting ? 'Kaydediliyor...' : 'Kayıt ol'}
                </button>
              </form>
            </>
          )}
        </div>

        {!successMessage && (
          <p className="mt-6 text-center text-sm text-ink-muted">
            Zaten hesabın var mı?{' '}
            <Link to="/login" className="font-medium text-branch hover:text-branch-hover">
              Giriş yap
            </Link>
          </p>
        )}
      </div>
    </div>
  )
}
