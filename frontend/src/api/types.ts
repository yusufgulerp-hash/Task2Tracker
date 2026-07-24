// Backend enum'larıyla birebir aynı sayısal değerler
// (Task2Tracker.Domain.Enums.TaskProgressStatus / TaskPriority)
export const TaskStatus = {
  ToDo: 1,
  InProgress: 2,
  Done: 3,
} as const
export type TaskStatusValue = (typeof TaskStatus)[keyof typeof TaskStatus]

export const TaskPriority = {
  Low: 1,
  Medium: 2,
  High: 3,
} as const
export type TaskPriorityValue = (typeof TaskPriority)[keyof typeof TaskPriority]

export type UserRole = 'User' | 'Admin'

export interface AuthResponse {
  accessToken: string
  userId: string
  email: string
  role: UserRole
}

export interface UserListItem {
  id: string
  firstName: string
  lastName: string
  email: string
}

export interface UserDetail {
  id: string
  firstName: string
  lastName: string
  email: string
}

export interface PendingUser {
  id: string
  firstName: string
  lastName: string
  email: string
  createdAt: string
}

export interface TaskItem {
  id: string
  title: string
  description: string | null
  blockerNote: string | null
  status: TaskStatusValue
  priority: TaskPriorityValue
  projectId: string
  userId: string | null
}

export interface ProjectListItem {
  id: string
  name: string
}

export interface ProjectDetail {
  id: string
  name: string
  createdAt: string
  memberCount: number
  taskCount: number
}

export interface ProjectMember {
  userId: string
  firstName: string
  lastName: string
  email: string
  joinedAt: string
}

export interface ProjectMemberWithTasks {
  userId: string
  firstName: string
  lastName: string
  email: string
  tasks: TaskItem[]
}

export interface ProjectDashboard {
  projectId: string
  projectName: string
  members: ProjectMemberWithTasks[]
  unassignedTasks: TaskItem[]
}

export interface ApiErrorBody {
  statusCode: number
  message: string
  errors?: Record<string, string[]>
}
