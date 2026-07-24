import { useEffect, useRef, useState } from 'react'
import type { FormEvent } from 'react'
import { useParams } from 'react-router-dom'
import * as projectsApi from '../api/projects'
import * as tasksApi from '../api/tasks'
import * as usersApi from '../api/users'
import { TaskPriority } from '../api/types'
import type {
  ProjectDashboard,
  ProjectMemberWithTasks,
  TaskItem,
  TaskPriorityValue,
  UserListItem,
} from '../api/types'
import { TaskDetailCard } from '../components/TaskDetailCard'
import { getApiErrorMessage } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import { useDebouncedValue } from '../hooks/useDebouncedValue'

function initials(firstName: string, lastName: string) {
  return `${firstName[0] ?? ''}${lastName[0] ?? ''}`.toUpperCase()
}

const UNASSIGNED_KEY = '__unassigned__'

function MemberRow({
  memberKey,
  displayName,
  subtitle,
  tasks,
  isExpanded,
  onToggle,
  onRemove,
  onChanged,
  assignOptions,
  emptyLabel,
}: {
  memberKey: string
  displayName: string
  subtitle: string
  tasks: TaskItem[]
  isExpanded: boolean
  onToggle: () => void
  onRemove?: () => void
  onChanged: () => void
  assignOptions?: ProjectMemberWithTasks[]
  emptyLabel: string
}) {
  const [first, last] = displayName.replace(' (sen)', '').split(' ')

  return (
    <div className="rounded-xl border border-border bg-surface">
      <div className="flex items-center gap-3 px-4 py-3">
        <button onClick={onToggle} className="flex flex-1 items-center gap-3 text-left">
          <span
            className={`text-ink-faint transition-transform ${isExpanded ? 'rotate-90' : ''}`}
          >
            ›
          </span>
          <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-branch-soft text-xs font-semibold text-branch">
            {memberKey === UNASSIGNED_KEY ? '—' : initials(first ?? '', last ?? '')}
          </span>
          <div className="min-w-0">
            <p className="truncate text-sm font-medium text-ink">{displayName}</p>
            <p className="text-xs text-ink-faint">{subtitle}</p>
          </div>
        </button>
        {onRemove && (
          <button
            onClick={onRemove}
            title="Üyeyi projeden çıkar"
            className="shrink-0 text-xs text-ink-faint hover:text-red"
          >
            Üyeyi çıkar
          </button>
        )}
      </div>

      {isExpanded && (
        <div className="flex flex-col gap-2 border-t border-border px-4 py-3">
          {tasks.length === 0 ? (
            <p className="text-xs text-ink-faint">{emptyLabel}</p>
          ) : (
            tasks.map((task) => (
              <TaskDetailCard
                key={task.id}
                task={task}
                onChanged={onChanged}
                assignSlot={
                  memberKey === UNASSIGNED_KEY && assignOptions ? (
                    <AssignSelect taskId={task.id} members={assignOptions} onChanged={onChanged} />
                  ) : undefined
                }
              />
            ))
          )}
        </div>
      )}
    </div>
  )
}

function AssignSelect({
  taskId,
  members,
  onChanged,
}: {
  taskId: string
  members: ProjectMemberWithTasks[]
  onChanged: () => void
}) {
  async function handleAssign(userId: string) {
    if (!userId) return
    try {
      await tasksApi.assignTask(taskId, userId)
      onChanged()
    } catch {
      // Hata burada TaskDetailCard'ın kendi save akışının dışında oluştuğu
      // için sessizce geçiyoruz; kullanıcı seçim kutusundan tekrar deneyebilir.
    }
  }

  return (
    <div>
      <label className="mb-1 block text-xs font-medium text-ink-muted">Ata</label>
      <select
        defaultValue=""
        onChange={(e) => handleAssign(e.target.value)}
        className="w-full rounded-md border border-border bg-surface px-2.5 py-1.5 text-sm text-ink outline-none focus:border-branch"
      >
        <option value="">Kullanıcı seç...</option>
        {members.map((m) => (
          <option key={m.userId} value={m.userId}>
            {m.firstName} {m.lastName}
          </option>
        ))}
      </select>
    </div>
  )
}

function AddMemberPanel({
  existingMemberIds,
  onAdd,
}: {
  existingMemberIds: Set<string>
  onAdd: (userId: string) => Promise<void>
}) {
  const [searchText, setSearchText] = useState('')
  const [results, setResults] = useState<UserListItem[]>([])
  const debouncedSearch = useDebouncedValue(searchText, 250)

  useEffect(() => {
    const trimmed = debouncedSearch.trim()
    if (!trimmed) {
      return
    }
    usersApi
      .searchUsers(trimmed)
      .then(setResults)
      .catch(() => setResults([]))
  }, [debouncedSearch])

  const candidates = results.filter((u) => !existingMemberIds.has(u.id))

  return (
    <div className="mb-6 rounded-lg border border-border bg-surface p-3">
      <input
        value={searchText}
        onChange={(e) => setSearchText(e.target.value)}
        placeholder="İsim veya e-posta ile kullanıcı ara..."
        className="w-full rounded-md border border-border bg-surface px-3 py-1.5 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
      />

      {searchText.trim() && (
        <div className="mt-2 flex flex-col gap-1">
          {candidates.length === 0 ? (
            <p className="px-1 py-1 text-xs text-ink-faint">Eşleşen kullanıcı yok.</p>
          ) : (
            candidates.map((u) => (
              <div
                key={u.id}
                className="flex items-center justify-between rounded-md px-2 py-1.5 hover:bg-surface-alt"
              >
                <span className="text-sm text-ink">
                  {u.firstName} {u.lastName}{' '}
                  <span className="text-ink-faint">({u.email})</span>
                </span>
                <button
                  onClick={() => onAdd(u.id)}
                  className="rounded-md bg-branch px-2.5 py-1 text-xs font-medium text-white hover:bg-branch-hover"
                >
                  Ekle
                </button>
              </div>
            ))
          )}
        </div>
      )}
    </div>
  )
}

export function ProjectDashboardPage() {
  const { id } = useParams<{ id: string }>()
  const { isAdmin, user } = useAuth()

  const [dashboard, setDashboard] = useState<ProjectDashboard | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [expanded, setExpanded] = useState<Set<string>>(new Set())
  const [taskSearchText, setTaskSearchText] = useState('')
  const hasInitializedExpanded = useRef(false)

  const [showAddMember, setShowAddMember] = useState(false)

  const [showNewTask, setShowNewTask] = useState(false)
  const [newTaskTitle, setNewTaskTitle] = useState('')
  const [newTaskDescription, setNewTaskDescription] = useState('')
  const [newTaskPriority, setNewTaskPriority] = useState<TaskPriorityValue>(TaskPriority.Medium)
  const [isSubmitting, setIsSubmitting] = useState(false)

  function loadDashboard() {
    if (!id) return
    projectsApi
      .getProjectDashboard(id)
      .then(setDashboard)
      .catch((err) => setError(getApiErrorMessage(err)))
      .finally(() => setIsLoading(false))
  }

  useEffect(() => {
    hasInitializedExpanded.current = false
    loadDashboard()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id])

  // Proje ilk yüklendiğinde tüm satırlar (üyeler + sahipsiz) varsayılan
  // olarak açık gelsin — kullanıcı isterse küçük ok butonuyla tek tek
  // kapatabilir. Bu sadece projeye ilk girişte bir kez çalışır; sonraki
  // veri yenilemelerinde (task oluşturma vb.) kullanıcının kapattığı
  // satırları tekrar açmaz.
  useEffect(() => {
    if (dashboard && !hasInitializedExpanded.current) {
      hasInitializedExpanded.current = true
      const all = new Set(dashboard.members.map((m) => m.userId))
      all.add(UNASSIGNED_KEY)
      setExpanded(all)
    }
  }, [dashboard])

  function toggle(key: string) {
    setExpanded((prev) => {
      const next = new Set(prev)
      if (next.has(key)) {
        next.delete(key)
      } else {
        next.add(key)
      }
      return next
    })
  }

  async function handleAddMember(userId: string) {
    if (!id) return
    setError(null)
    try {
      await projectsApi.addProjectMember(id, userId)
      setShowAddMember(false)
      loadDashboard()
    } catch (err) {
      setError(getApiErrorMessage(err))
    }
  }

  async function handleRemoveMember(userId: string) {
    if (!id) return
    setError(null)
    try {
      await projectsApi.removeProjectMember(id, userId)
      loadDashboard()
    } catch (err) {
      setError(getApiErrorMessage(err))
    }
  }

  async function handleCreateTask(e: FormEvent) {
    e.preventDefault()
    if (!id || !newTaskTitle.trim()) return
    setIsSubmitting(true)
    setError(null)
    try {
      await tasksApi.createTask({
        title: newTaskTitle.trim(),
        description: newTaskDescription.trim() || null,
        priority: newTaskPriority,
        projectId: id,
      })
      setNewTaskTitle('')
      setNewTaskDescription('')
      setNewTaskPriority(TaskPriority.Medium)
      setShowNewTask(false)
      loadDashboard()
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setIsSubmitting(false)
    }
  }

  if (isLoading) {
    return <p className="text-sm text-ink-muted">Yükleniyor...</p>
  }

  if (!dashboard) {
    return (
      <p className="rounded-md bg-red-soft px-3 py-2 text-sm text-red">
        {error ?? 'Proje bulunamadı.'}
      </p>
    )
  }

  const normalizedTaskSearch = taskSearchText.trim().toLowerCase()
  const matchesSearch = (task: TaskItem) =>
    !normalizedTaskSearch || task.title.toLowerCase().includes(normalizedTaskSearch)

  const orderedMembers = [...dashboard.members].sort((a, b) => {
    if (a.userId === user?.userId) return -1
    if (b.userId === user?.userId) return 1
    return 0
  })

  const existingMemberIds = new Set(dashboard.members.map((m) => m.userId))

  return (
    <div>
      <div className="mb-8 flex flex-wrap items-start justify-between gap-4">
        <div>
          <div className="flex items-center gap-2">
            <span className="h-2 w-2 rounded-full bg-branch" />
            <h1 className="font-display text-2xl font-semibold text-ink">
              {dashboard.projectName}
            </h1>
          </div>
          <p className="mt-1 text-sm text-ink-muted">
            {dashboard.members.length} üye ·{' '}
            {dashboard.members.reduce((sum, m) => sum + m.tasks.length, 0) +
              dashboard.unassignedTasks.length}{' '}
            görev
          </p>
        </div>

        <div className="flex gap-2">
          {isAdmin && (
            <button
              onClick={() => setShowAddMember((v) => !v)}
              className="rounded-md border border-border bg-surface px-3 py-1.5 text-sm font-medium text-ink transition-colors hover:border-branch"
            >
              + Üye ekle
            </button>
          )}
          <button
            onClick={() => setShowNewTask((v) => !v)}
            className="rounded-md bg-branch px-3 py-1.5 text-sm font-medium text-white transition-colors hover:bg-branch-hover"
          >
            + Görev oluştur
          </button>
        </div>
      </div>

      {error && (
        <p className="mb-4 rounded-md bg-red-soft px-3 py-2 text-sm text-red">{error}</p>
      )}

      {showAddMember && (
        <AddMemberPanel existingMemberIds={existingMemberIds} onAdd={handleAddMember} />
      )}

      {showNewTask && (
        <form
          onSubmit={handleCreateTask}
          className="mb-6 flex flex-col gap-3 rounded-lg border border-border bg-surface p-4"
        >
          <input
            required
            value={newTaskTitle}
            onChange={(e) => setNewTaskTitle(e.target.value)}
            placeholder="Görev başlığı"
            className="rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-branch"
          />
          <textarea
            value={newTaskDescription}
            onChange={(e) => setNewTaskDescription(e.target.value)}
            placeholder="Açıklama (opsiyonel)"
            rows={2}
            className="rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-branch"
          />
          <div className="flex items-center gap-2">
            <select
              value={newTaskPriority}
              onChange={(e) => setNewTaskPriority(Number(e.target.value) as TaskPriorityValue)}
              className="rounded-md border border-border bg-surface px-3 py-1.5 text-sm text-ink outline-none focus:border-branch"
            >
              <option value={TaskPriority.Low}>Düşük öncelik</option>
              <option value={TaskPriority.Medium}>Orta öncelik</option>
              <option value={TaskPriority.High}>Yüksek öncelik</option>
            </select>
            <button
              type="submit"
              disabled={isSubmitting}
              className="ml-auto rounded-md bg-branch px-3 py-1.5 text-sm font-medium text-white hover:bg-branch-hover disabled:opacity-60"
            >
              Oluştur
            </button>
          </div>
        </form>
      )}

      <div className="mb-4">
        <input
          value={taskSearchText}
          onChange={(e) => setTaskSearchText(e.target.value)}
          placeholder="Görev başlığında ara..."
          className="w-full max-w-xs rounded-md border border-border bg-surface px-3 py-1.5 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
        />
      </div>

      <div className="flex flex-col gap-3">
        {orderedMembers.map((member) => {
          const filteredTasks = member.tasks.filter(matchesSearch)
          return (
            <MemberRow
              key={member.userId}
              memberKey={member.userId}
              displayName={
                member.userId === user?.userId
                  ? `${member.firstName} ${member.lastName} (sen)`
                  : `${member.firstName} ${member.lastName}`
              }
              subtitle={`${member.tasks.length} görev`}
              tasks={filteredTasks}
              isExpanded={expanded.has(member.userId)}
              onToggle={() => toggle(member.userId)}
              onRemove={isAdmin ? () => handleRemoveMember(member.userId) : undefined}
              onChanged={loadDashboard}
              emptyLabel={
                normalizedTaskSearch ? 'Aramayla eşleşen görev yok.' : 'Atanmış görev yok.'
              }
            />
          )
        })}

        <MemberRow
          memberKey={UNASSIGNED_KEY}
          displayName="Sahipsiz"
          subtitle={`${dashboard.unassignedTasks.length} görev`}
          tasks={dashboard.unassignedTasks.filter(matchesSearch)}
          isExpanded={expanded.has(UNASSIGNED_KEY)}
          onToggle={() => toggle(UNASSIGNED_KEY)}
          onChanged={loadDashboard}
          assignOptions={dashboard.members}
          emptyLabel={normalizedTaskSearch ? 'Aramayla eşleşen görev yok.' : 'Sahipsiz görev yok.'}
        />
      </div>
    </div>
  )
}
