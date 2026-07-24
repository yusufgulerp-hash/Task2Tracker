import { apiClient } from './client'
import type { PendingUser, UserDetail, UserListItem } from './types'

export function getUsers() {
  return apiClient.get<UserListItem[]>('/users').then((res) => res.data)
}

export function searchUsers(text: string) {
  return apiClient
    .get<UserListItem[]>('/users/search', { params: { text } })
    .then((res) => res.data)
}

export function getUserById(id: string) {
  return apiClient.get<UserDetail>(`/users/${id}`).then((res) => res.data)
}

export interface UpdateUserPayload {
  firstName: string
  lastName: string
  email: string
}

export function updateUser(id: string, payload: UpdateUserPayload) {
  return apiClient.put(`/users/${id}`, payload)
}

export function deleteUser(id: string) {
  return apiClient.delete(`/users/${id}`)
}

export function getPendingUsers() {
  return apiClient.get<PendingUser[]>('/users/pending').then((res) => res.data)
}

export function approveUser(id: string) {
  return apiClient.post(`/users/${id}/approve`)
}

export function rejectUser(id: string) {
  return apiClient.post(`/users/${id}/reject`)
}
