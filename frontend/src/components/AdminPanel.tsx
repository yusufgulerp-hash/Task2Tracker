import { useEffect, useState } from 'react'
import type { FormEvent, ReactNode } from 'react'
import * as usersApi from '../api/users'
import * as projectsApi from '../api/projects'
import type { PendingUser, ProjectListItem, UserListItem } from '../api/types'
import { getApiErrorMessage } from '../api/client'
import { useAuth } from '../auth/AuthContext'

function Section({ title, children }: { title: string; children: ReactNode }) {
  return (
    <div className="rounded-xl border border-border bg-surface p-5">
      <h2 className="mb-4 font-display text-lg font-semibold text-ink">{title}</h2>
      {children}
    </div>
  )
}

function PendingUsersSection() {
  const [pending, setPending] = useState<PendingUser[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  function load() {
    usersApi
      .getPendingUsers()
      .then(setPending)
      .catch((err) => setError(getApiErrorMessage(err)))
      .finally(() => setIsLoading(false))
  }

  useEffect(() => {
    load()
  }, [])

  async function handleApprove(id: string) {
    setError(null)
    try {
      await usersApi.approveUser(id)
      load()
    } catch (err) {
      setError(getApiErrorMessage(err))
    }
  }

  async function handleReject(id: string) {
    setError(null)
    try {
      await usersApi.rejectUser(id)
      load()
    } catch (err) {
      setError(getApiErrorMessage(err))
    }
  }

  return (
    <Section title="Onay bekleyen kullanıcılar">
      {error && (
        <p className="mb-3 rounded-md bg-red-soft px-3 py-2 text-sm text-red">{error}</p>
      )}
      {isLoading ? (
        <p className="text-sm text-ink-muted">Yükleniyor...</p>
      ) : pending.length === 0 ? (
        <p className="text-sm text-ink-faint">Onay bekleyen kullanıcı yok.</p>
      ) : (
        <div className="flex flex-col gap-2">
          {pending.map((u) => (
            <div
              key={u.id}
              className="flex items-center justify-between rounded-lg border border-border px-3 py-2"
            >
              <div>
                <p className="text-sm font-medium text-ink">
                  {u.firstName} {u.lastName}
                </p>
                <p className="text-xs text-ink-faint">{u.email}</p>
              </div>
              <div className="flex gap-2">
                <button
                  onClick={() => handleApprove(u.id)}
                  className="rounded-md bg-branch px-3 py-1.5 text-xs font-medium text-white hover:bg-branch-hover"
                >
                  Onayla
                </button>
                <button
                  onClick={() => handleReject(u.id)}
                  className="rounded-md border border-border px-3 py-1.5 text-xs font-medium text-ink-muted hover:bg-surface-alt hover:text-red"
                >
                  Reddet
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </Section>
  )
}

function AllUsersSection() {
  const { user: currentUser } = useAuth()
  const [users, setUsers] = useState<UserListItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [editingId, setEditingId] = useState<string | null>(null)
  const [editFirstName, setEditFirstName] = useState('')
  const [editLastName, setEditLastName] = useState('')
  const [editEmail, setEditEmail] = useState('')
  const [isSaving, setIsSaving] = useState(false)

  function load() {
    usersApi
      .getUsers()
      .then(setUsers)
      .catch((err) => setError(getApiErrorMessage(err)))
      .finally(() => setIsLoading(false))
  }

  useEffect(() => {
    load()
  }, [])

  function startEditing(u: UserListItem) {
    setEditingId(u.id)
    setEditFirstName(u.firstName)
    setEditLastName(u.lastName)
    setEditEmail(u.email)
  }

  async function handleSaveEdit(id: string) {
    if (!editFirstName.trim() || !editLastName.trim() || !editEmail.trim()) return
    setIsSaving(true)
    setError(null)
    try {
      await usersApi.updateUser(id, {
        firstName: editFirstName.trim(),
        lastName: editLastName.trim(),
        email: editEmail.trim(),
      })
      setEditingId(null)
      load()
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setIsSaving(false)
    }
  }

  async function handleDelete(id: string) {
    if (!window.confirm('Bu kullanıcıyı kalıcı olarak silmek istediğine emin misin?')) {
      return
    }
    setError(null)
    try {
      await usersApi.deleteUser(id)
      load()
    } catch (err) {
      setError(getApiErrorMessage(err))
    }
  }

  return (
    <Section title="Tüm kullanıcılar">
      {error && (
        <p className="mb-3 rounded-md bg-red-soft px-3 py-2 text-sm text-red">{error}</p>
      )}
      {isLoading ? (
        <p className="text-sm text-ink-muted">Yükleniyor...</p>
      ) : (
        <div className="flex flex-col gap-2">
          {users.map((u) =>
            editingId === u.id ? (
              <div
                key={u.id}
                className="flex flex-col gap-2 rounded-lg border border-branch bg-branch-soft/30 px-3 py-2"
              >
                <div className="grid grid-cols-2 gap-2">
                  <input
                    value={editFirstName}
                    onChange={(e) => setEditFirstName(e.target.value)}
                    placeholder="Ad"
                    className="rounded-md border border-border bg-surface px-2 py-1 text-sm text-ink outline-none focus:border-branch"
                  />
                  <input
                    value={editLastName}
                    onChange={(e) => setEditLastName(e.target.value)}
                    placeholder="Soyad"
                    className="rounded-md border border-border bg-surface px-2 py-1 text-sm text-ink outline-none focus:border-branch"
                  />
                </div>
                <input
                  type="email"
                  value={editEmail}
                  onChange={(e) => setEditEmail(e.target.value)}
                  placeholder="E-posta"
                  className="rounded-md border border-border bg-surface px-2 py-1 text-sm text-ink outline-none focus:border-branch"
                />
                <div className="flex gap-3">
                  <button
                    onClick={() => handleSaveEdit(u.id)}
                    disabled={isSaving}
                    className="text-xs font-medium text-branch hover:text-branch-hover"
                  >
                    Kaydet
                  </button>
                  <button
                    onClick={() => setEditingId(null)}
                    disabled={isSaving}
                    className="text-xs font-medium text-ink-faint hover:text-ink"
                  >
                    İptal
                  </button>
                </div>
              </div>
            ) : (
              <div
                key={u.id}
                className="flex items-center justify-between rounded-lg border border-border px-3 py-2"
              >
                <div>
                  <p className="text-sm font-medium text-ink">
                    {u.firstName} {u.lastName}
                  </p>
                  <p className="text-xs text-ink-faint">{u.email}</p>
                </div>
                <div className="flex shrink-0 gap-3">
                  <button
                    onClick={() => startEditing(u)}
                    className="text-xs font-medium text-ink-faint hover:text-ink"
                  >
                    Düzenle
                  </button>
                  <button
                    onClick={() => handleDelete(u.id)}
                    disabled={u.id === currentUser?.userId}
                    title={u.id === currentUser?.userId ? 'Kendi hesabını silemezsin' : 'Kullanıcıyı sil'}
                    className="text-xs font-medium text-ink-faint hover:text-red disabled:cursor-not-allowed disabled:opacity-30"
                  >
                    Sil
                  </button>
                </div>
              </div>
            ),
          )}
        </div>
      )}
    </Section>
  )
}

function AllProjectsSection() {
  const [projects, setProjects] = useState<ProjectListItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [newProjectName, setNewProjectName] = useState('')
  const [isCreating, setIsCreating] = useState(false)

  const [editingId, setEditingId] = useState<string | null>(null)
  const [editName, setEditName] = useState('')
  const [isSaving, setIsSaving] = useState(false)

  function load() {
    projectsApi
      .getProjects()
      .then(setProjects)
      .catch((err) => setError(getApiErrorMessage(err)))
      .finally(() => setIsLoading(false))
  }

  useEffect(() => {
    load()
  }, [])

  async function handleCreate(e: FormEvent) {
    e.preventDefault()
    if (!newProjectName.trim()) return
    setIsCreating(true)
    setError(null)
    try {
      await projectsApi.createProject(newProjectName.trim())
      setNewProjectName('')
      load()
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setIsCreating(false)
    }
  }

  function startEditing(p: ProjectListItem) {
    setEditingId(p.id)
    setEditName(p.name)
  }

  async function handleRename(id: string) {
    if (!editName.trim()) return
    setIsSaving(true)
    setError(null)
    try {
      await projectsApi.updateProject(id, editName.trim())
      setEditingId(null)
      load()
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setIsSaving(false)
    }
  }

  async function handleDelete(id: string) {
    if (!window.confirm('Bu projeyi kalıcı olarak silmek istediğine emin misin?')) {
      return
    }
    setError(null)
    try {
      await projectsApi.deleteProject(id)
      load()
    } catch (err) {
      setError(getApiErrorMessage(err))
    }
  }

  return (
    <Section title="Tüm projeler">
      <form onSubmit={handleCreate} className="mb-4 flex gap-2">
        <input
          value={newProjectName}
          onChange={(e) => setNewProjectName(e.target.value)}
          placeholder="Yeni proje adı"
          className="flex-1 rounded-md border border-border bg-surface px-3 py-1.5 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
        />
        <button
          type="submit"
          disabled={isCreating || !newProjectName.trim()}
          className="rounded-md bg-branch px-3 py-1.5 text-sm font-medium text-white hover:bg-branch-hover disabled:opacity-60"
        >
          {isCreating ? 'Oluşturuluyor...' : 'Oluştur'}
        </button>
      </form>

      {error && (
        <p className="mb-3 rounded-md bg-red-soft px-3 py-2 text-sm text-red">{error}</p>
      )}
      {isLoading ? (
        <p className="text-sm text-ink-muted">Yükleniyor...</p>
      ) : (
        <div className="flex flex-col gap-2">
          {projects.map((p) => (
            <div
              key={p.id}
              className="flex items-center justify-between gap-2 rounded-lg border border-border px-3 py-2"
            >
              {editingId === p.id ? (
                <>
                  <input
                    value={editName}
                    onChange={(e) => setEditName(e.target.value)}
                    autoFocus
                    className="flex-1 rounded-md border border-border bg-surface px-2 py-1 text-sm text-ink outline-none focus:border-branch"
                  />
                  <button
                    onClick={() => handleRename(p.id)}
                    disabled={isSaving}
                    className="text-xs font-medium text-branch hover:text-branch-hover"
                  >
                    Kaydet
                  </button>
                  <button
                    onClick={() => setEditingId(null)}
                    disabled={isSaving}
                    className="text-xs font-medium text-ink-faint hover:text-ink"
                  >
                    İptal
                  </button>
                </>
              ) : (
                <>
                  <p className="text-sm font-medium text-ink">{p.name}</p>
                  <div className="flex shrink-0 gap-3">
                    <button
                      onClick={() => startEditing(p)}
                      className="text-xs font-medium text-ink-faint hover:text-ink"
                    >
                      Düzenle
                    </button>
                    <button
                      onClick={() => handleDelete(p.id)}
                      className="text-xs font-medium text-ink-faint hover:text-red"
                    >
                      Sil
                    </button>
                  </div>
                </>
              )}
            </div>
          ))}
        </div>
      )}
    </Section>
  )
}

export function AdminPanel() {
  return (
    <div className="flex flex-col gap-5">
      <PendingUsersSection />
      <AllUsersSection />
      <AllProjectsSection />
    </div>
  )
}
