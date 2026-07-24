import { useState } from 'react'
import type { ReactNode } from 'react'
import * as tasksApi from '../api/tasks'
import { TaskPriority, TaskStatus } from '../api/types'
import type { TaskItem, TaskPriorityValue, TaskStatusValue } from '../api/types'
import { PriorityBadge, StatusBadge } from './Badge'
import { getApiErrorMessage } from '../api/client'

const statusOptions: { value: TaskStatusValue; label: string }[] = [
  { value: TaskStatus.ToDo, label: 'Yapılacak' },
  { value: TaskStatus.InProgress, label: 'Sürüyor' },
  { value: TaskStatus.Done, label: 'Tamamlandı' },
]

const priorityOptions: { value: TaskPriorityValue; label: string }[] = [
  { value: TaskPriority.Low, label: 'Düşük' },
  { value: TaskPriority.Medium, label: 'Orta' },
  { value: TaskPriority.High, label: 'Yüksek' },
]

interface TaskDetailCardProps {
  task: TaskItem
  onChanged: () => void
  /** Sahipsiz görevler için: "Ata..." seçim kutusu buraya geçirilir. */
  assignSlot?: ReactNode
}

export function TaskDetailCard({ task, onChanged, assignSlot }: TaskDetailCardProps) {
  const [isExpanded, setIsExpanded] = useState(false)
  const [isBusy, setIsBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const [title, setTitle] = useState(task.title)
  const [description, setDescription] = useState(task.description ?? '')
  const [priority, setPriority] = useState<TaskPriorityValue>(task.priority)
  const [status, setStatus] = useState<TaskStatusValue>(task.status)
  const [blockerNote, setBlockerNote] = useState(task.blockerNote ?? '')

  function resetEdits() {
    setTitle(task.title)
    setDescription(task.description ?? '')
    setPriority(task.priority)
    setStatus(task.status)
    setBlockerNote(task.blockerNote ?? '')
  }

  function toggle() {
    if (isExpanded) {
      resetEdits()
    }
    setIsExpanded((v) => !v)
    setError(null)
  }

  async function handleSave() {
    setIsBusy(true)
    setError(null)
    try {
      await tasksApi.updateTask(task.id, {
        title: title.trim(),
        description: description.trim() || null,
        priority,
        status,
        userId: task.userId,
        blockerNote: blockerNote.trim() || null,
      })
      onChanged()
      setIsExpanded(false)
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setIsBusy(false)
    }
  }

  async function handleUnassign() {
    setIsBusy(true)
    setError(null)
    try {
      await tasksApi.unassignTask(task.id)
      onChanged()
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setIsBusy(false)
    }
  }

  async function handleDelete() {
    setIsBusy(true)
    setError(null)
    try {
      await tasksApi.deleteTask(task.id)
      onChanged()
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setIsBusy(false)
    }
  }

  return (
    <div className="rounded-lg border border-border bg-surface">
      <button
        onClick={toggle}
        className="flex w-full items-center gap-2.5 px-3.5 py-3 text-left"
      >
        <span
          className={`shrink-0 text-ink-faint transition-transform ${isExpanded ? 'rotate-90' : ''}`}
        >
          ›
        </span>
        <span className="min-w-0 flex-1 truncate text-sm font-medium text-ink">
          {task.title}
        </span>
        {task.blockerNote && (
          <span
            title={`Blocker: ${task.blockerNote}`}
            className="shrink-0 rounded-full bg-red-soft px-2 py-0.5 text-xs font-medium text-red"
          >
            Blocked
          </span>
        )}
        <span className="hidden shrink-0 items-center gap-1.5 sm:flex">
          <PriorityBadge priority={task.priority} />
          <StatusBadge status={task.status} />
        </span>
      </button>

      {isExpanded && (
        <div className="border-t border-border px-3.5 py-3.5">
          <label className="mb-1 block text-xs font-medium text-ink-muted">
            Başlık
          </label>
          <input
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className="mb-3 w-full rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
          />

          <label className="mb-1 block text-xs font-medium text-ink-muted">
            Açıklama
          </label>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            rows={4}
            placeholder="Açıklama ekle..."
            className="mb-3 w-full rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-branch focus:ring-1 focus:ring-branch"
          />

          <div className="mb-3 flex flex-wrap gap-3">
            <div>
              <label className="mb-1 block text-xs font-medium text-ink-muted">
                Öncelik
              </label>
              <select
                value={priority}
                onChange={(e) => setPriority(Number(e.target.value) as TaskPriorityValue)}
                className="rounded-md border border-border bg-surface px-2.5 py-1.5 text-sm text-ink outline-none focus:border-branch"
              >
                {priorityOptions.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="mb-1 block text-xs font-medium text-ink-muted">
                Durum
              </label>
              <select
                value={status}
                onChange={(e) => setStatus(Number(e.target.value) as TaskStatusValue)}
                className="rounded-md border border-border bg-surface px-2.5 py-1.5 text-sm text-ink outline-none focus:border-branch"
              >
                {statusOptions.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <label className="mb-1 block text-xs font-medium text-red">
            Blocker
          </label>
          <textarea
            value={blockerNote}
            onChange={(e) => setBlockerNote(e.target.value)}
            rows={2}
            placeholder="Bu görevi ilerletmeni engelleyen bir şey varsa buraya yaz..."
            className="mb-3 w-full rounded-md border border-border bg-surface px-3 py-2 text-sm text-ink outline-none focus:border-red focus:ring-1 focus:ring-red"
          />

          {assignSlot && <div className="mb-3">{assignSlot}</div>}

          {error && (
            <p className="mb-3 rounded-md bg-red-soft px-2.5 py-1.5 text-xs text-red">
              {error}
            </p>
          )}

          <div className="flex items-center justify-between">
            <div className="flex gap-2">
              <button
                onClick={handleSave}
                disabled={isBusy || !title.trim()}
                className="rounded-md bg-branch px-3 py-1.5 text-xs font-medium text-white transition-colors hover:bg-branch-hover disabled:opacity-60"
              >
                Kaydet
              </button>
              <button
                onClick={toggle}
                disabled={isBusy}
                className="rounded-md border border-border px-3 py-1.5 text-xs font-medium text-ink-muted hover:bg-surface-alt"
              >
                İptal
              </button>
            </div>

            <div className="flex items-center gap-3">
              {task.userId && !assignSlot && (
                <button
                  onClick={handleUnassign}
                  disabled={isBusy}
                  className="text-xs font-medium text-ink-faint hover:text-red"
                >
                  Atamayı kaldır
                </button>
              )}
              <button
                onClick={handleDelete}
                disabled={isBusy || task.status !== TaskStatus.Done}
                title={
                  task.status !== TaskStatus.Done
                    ? 'Yalnızca tamamlanmış görevler silinebilir'
                    : 'Görevi sil'
                }
                className="text-xs font-medium text-ink-faint hover:text-red disabled:cursor-not-allowed disabled:opacity-30"
              >
                Sil
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
