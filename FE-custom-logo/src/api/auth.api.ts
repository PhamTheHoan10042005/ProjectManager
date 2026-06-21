import { notifyHttp } from './http'
import type { AuthResponse, LoginRequest, RegisterRequest, User } from '@/types'

export const authApi = {
  login(data: LoginRequest) {
    return notifyHttp.post<AuthResponse>('/api/auth/login', data)
  },

  register(data: RegisterRequest) {
    return notifyHttp.post<AuthResponse>('/api/auth/register', data)
  },

  me() {
    return notifyHttp.get<User>('/api/auth/me')
  },

  listUsers() {
    return notifyHttp.get<User[]>('/api/users')
  },

  updateUserRole(userId: string, role: string) {
    return notifyHttp.patch<User>(`/api/users/${userId}/role`, { role })
  },
}
