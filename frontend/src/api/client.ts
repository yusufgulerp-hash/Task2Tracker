import axios from 'axios'
import type { ApiErrorBody } from './types'

export const TOKEN_STORAGE_KEY = 'task2tracker_token'

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:8080/api',
})

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY)
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// 401 aldığımızda oturum artık geçersiz demektir (token süresi dolmuş
// veya geçersiz) — kullanıcıyı login'e geri gönderiyoruz.
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem(TOKEN_STORAGE_KEY)
      if (window.location.pathname !== '/login') {
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  },
)

export function getApiErrorMessage(error: unknown): string {
  if (axios.isAxiosError<ApiErrorBody>(error)) {
    return error.response?.data?.message ?? 'Beklenmeyen bir hata oluştu.'
  }
  return 'Beklenmeyen bir hata oluştu.'
}
