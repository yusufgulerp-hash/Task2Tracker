import { TaskPriority, TaskStatus } from '../api/types'
import type { TaskPriorityValue, TaskStatusValue } from '../api/types'

const statusConfig: Record<TaskStatusValue, { label: string; className: string }> = {
  [TaskStatus.ToDo]: {
    label: 'Yapılacak',
    className: 'bg-surface-alt text-ink-muted',
  },
  [TaskStatus.InProgress]: {
    label: 'Sürüyor',
    className: 'bg-blue-soft text-blue',
  },
  [TaskStatus.Done]: {
    label: 'Tamamlandı',
    className: 'bg-branch-soft text-branch',
  },
}

const priorityConfig: Record<TaskPriorityValue, { label: string; className: string }> = {
  [TaskPriority.Low]: {
    label: 'Düşük',
    className: 'bg-surface-alt text-ink-muted',
  },
  [TaskPriority.Medium]: {
    label: 'Orta',
    className: 'bg-amber-soft text-amber',
  },
  [TaskPriority.High]: {
    label: 'Yüksek',
    className: 'bg-red-soft text-red',
  },
}

function BaseBadge({ label, className }: { label: string; className: string }) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${className}`}
    >
      {label}
    </span>
  )
}

export function StatusBadge({ status }: { status: TaskStatusValue }) {
  const config = statusConfig[status]
  return <BaseBadge label={config.label} className={config.className} />
}

export function PriorityBadge({ priority }: { priority: TaskPriorityValue }) {
  const config = priorityConfig[priority]
  return <BaseBadge label={config.label} className={config.className} />
}
