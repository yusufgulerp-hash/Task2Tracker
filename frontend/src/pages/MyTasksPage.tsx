import { useEffect, useState } from 'react'
import * as tasksApi from '../api/tasks'
import { TaskPriority, TaskStatus } from '../api/types'
import type { TaskItem, TaskPriorityValue, TaskStatusValue } from '../api/types'
import { TaskDetailCard } from '../components/TaskDetailCard'
import { getApiErrorMessage } from '../api/client'

export function MyTasksPage() {
  const [tasks, setTasks] = useState<TaskItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const [statusFilter, setStatusFilter] = useState<TaskStatusValue | ''>('')
  const [priorityFilter, setPriorityFilter] = useState<TaskPriorityValue | ''>('')
  const [titleFilter, setTitleFilter] = useState('')

  function loadTasks() {
    tasksApi
      .getMyTasks({
        status: statusFilter || undefined,
        priority: priorityFilter || undefined,
      })
      .then(setTasks)
      .catch((err) => setError(getApiErrorMessage(err)))
      .finally(() => setIsLoading(false))
  }

  useEffect(() => {
    loadTasks()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [statusFilter, priorityFilter])

  const visibleTasks = tasks.filter((task) =>
    task.title.toLowerCase().includes(titleFilter.trim().toLowerCase()),
  )

  return (
    <div>
      <h1 className="font-display text-2xl font-semibold text-ink">Görevlerim</h1>

      <div className="mt-5 flex flex-wrap items-center gap-2">
        <input
          value={titleFilter}
          onChange={(e) => setTitleFilter(e.target.value)}
          placeholder="Başlıkta ara..."
          className="w-full max-w-xs rounded-md border border-border bg-surface px-3 py-1.5 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
        />

        <select
          value={statusFilter}
          onChange={(e) =>
            setStatusFilter(e.target.value ? (Number(e.target.value) as TaskStatusValue) : '')
          }
          className="rounded-md border border-border bg-surface px-2.5 py-1.5 text-sm text-ink outline-none focus:border-branch"
        >
          <option value="">Tüm durumlar</option>
          <option value={TaskStatus.ToDo}>Yapılacak</option>
          <option value={TaskStatus.InProgress}>Sürüyor</option>
          <option value={TaskStatus.Done}>Tamamlandı</option>
        </select>

        <select
          value={priorityFilter}
          onChange={(e) =>
            setPriorityFilter(e.target.value ? (Number(e.target.value) as TaskPriorityValue) : '')
          }
          className="rounded-md border border-border bg-surface px-2.5 py-1.5 text-sm text-ink outline-none focus:border-branch"
        >
          <option value="">Tüm öncelikler</option>
          <option value={TaskPriority.Low}>Düşük</option>
          <option value={TaskPriority.Medium}>Orta</option>
          <option value={TaskPriority.High}>Yüksek</option>
        </select>
      </div>

      {error && (
        <p className="mt-4 rounded-md bg-red-soft px-3 py-2 text-sm text-red">{error}</p>
      )}

      <div className="mt-5">
        {isLoading ? (
          <p className="text-sm text-ink-muted">Yükleniyor...</p>
        ) : visibleTasks.length === 0 ? (
          <div className="rounded-xl border border-dashed border-border px-6 py-12 text-center">
            <p className="text-sm text-ink-muted">
              {tasks.length === 0
                ? 'Sana atanmış bir görev yok.'
                : 'Filtrelerle eşleşen bir görev yok.'}
            </p>
          </div>
        ) : (
          <div className="flex flex-col gap-2">
            {visibleTasks.map((task) => (
              <TaskDetailCard key={task.id} task={task} onChanged={loadTasks} />
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
