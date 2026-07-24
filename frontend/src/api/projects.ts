import { apiClient } from './client'
import type {
  ProjectDashboard,
  ProjectDetail,
  ProjectListItem,
  ProjectMember,
} from './types'

export function getProjects() {
  return apiClient.get<ProjectListItem[]>('/projects').then((res) => res.data)
}

export function searchProjects(text: string) {
  return apiClient
    .get<ProjectListItem[]>('/projects/search', { params: { text } })
    .then((res) => res.data)
}

export function getProjectById(id: string) {
  return apiClient.get<ProjectDetail>(`/projects/${id}`).then((res) => res.data)
}

export function getProjectDashboard(id: string) {
  return apiClient
    .get<ProjectDashboard>(`/projects/${id}/dashboard`)
    .then((res) => res.data)
}

export function getProjectMembers(id: string) {
  return apiClient
    .get<ProjectMember[]>(`/projects/${id}/members`)
    .then((res) => res.data)
}

export function createProject(name: string) {
  return apiClient.post<string>('/projects', { name }).then((res) => res.data)
}

export function updateProject(id: string, name: string) {
  return apiClient.put(`/projects/${id}`, { name })
}

export function deleteProject(id: string) {
  return apiClient.delete(`/projects/${id}`)
}

export function addProjectMember(projectId: string, userId: string) {
  return apiClient.post(`/projects/${projectId}/members`, { userId })
}

export function removeProjectMember(projectId: string, userId: string) {
  return apiClient.delete(`/projects/${projectId}/members/${userId}`)
}
