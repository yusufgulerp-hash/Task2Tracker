import { apiClient } from './client'
import type { TaskItem, TaskPriorityValue, TaskStatusValue } from './types'

export interface CreateTaskPayload {
  title: string
  description: string | null
  priority: TaskPriorityValue
  projectId: string
}

export function createTask(payload: CreateTaskPayload) {
  return apiClient.post<string>('/tasks', payload).then((res) => res.data)
}

export interface UpdateTaskPayload {
  title: string
  description: string | null
  priority: TaskPriorityValue
  status: TaskStatusValue
  userId: string | null
  blockerNote: string | null
}

export function updateTask(id: string, payload: UpdateTaskPayload) {
  return apiClient.put(`/tasks/${id}`, payload)
}

export function assignTask(taskId: string, userId: string) {
  return apiClient.put(`/tasks/${taskId}/assign`, null, { params: { userId } })
}

export function unassignTask(taskId: string) {
  return apiClient.delete(`/tasks/${taskId}/assign`)
}

export function deleteTask(id: string) {
  return apiClient.delete(`/tasks/${id}`)
}

export interface MyTasksFilter {
  status?: TaskStatusValue
  priority?: TaskPriorityValue
}

export function getMyTasks(filter: MyTasksFilter = {}) {
  return apiClient
    .get<TaskItem[]>('/tasks/mine', { params: filter })
    .then((res) => res.data)
}
