import { apiClient } from './client'
import type { AuthResponse } from './types'

export function login(email: string, password: string) {
  return apiClient
    .post<AuthResponse>('/auth/login', { email, password })
    .then((res) => res.data)
}

export interface RegisterPayload {
  firstName: string
  lastName: string
  email: string
  password: string
}

export interface RegisterResponse {
  userId: string
  email: string
  status: string
  message: string
}

export function register(payload: RegisterPayload) {
  return apiClient
    .post<RegisterResponse>('/auth/register', payload)
    .then((res) => res.data)
}
