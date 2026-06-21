import axios, { type AxiosInstance } from 'axios'
import { API_BASE } from '@/config/services'

const TOKEN_KEY = 'pms_token'

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

export function setToken(token: string | null): void {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token)
  } else {
    localStorage.removeItem(TOKEN_KEY)
  }
}

function createClient(baseURL: string): AxiosInstance {
  const client = axios.create({
    baseURL,
    headers: { 'Content-Type': 'application/json' },
  })

  client.interceptors.request.use((config) => {
    const token = getToken()
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  })

  client.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error.response?.status === 401 && !error.config?.url?.includes('/auth/login')) {
        setToken(null)
        if (window.location.pathname !== '/login') {
          window.location.href = '/login'
        }
      }
      return Promise.reject(error)
    },
  )

  return client
}

export const projectHttp = createClient(API_BASE.project)
export const taskHttp = createClient(API_BASE.task)
export const notifyHttp = createClient(API_BASE.notify)

export function extractErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as { title?: string; message?: string; errors?: Record<string, string[]> } | undefined
    if (data?.message) return data.message
    if (data?.title) return data.title
    if (data?.errors) {
      return Object.values(data.errors).flat().join(', ')
    }
    if (error.message) return error.message
  }
  if (error instanceof Error) return error.message
  return 'Đã xảy ra lỗi không xác định'
}
