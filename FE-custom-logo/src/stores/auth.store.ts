import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/api/auth.api'
import { getToken, setToken, extractErrorMessage } from '@/api/http'
import type { LoginRequest, RegisterRequest, User } from '@/types'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => !!getToken() && !!user.value)
  const isAdmin = computed(() => user.value?.role === 'Admin')
  const isProjectManager = computed(() =>
    user.value?.role === 'Admin' || user.value?.role === 'ProjectManager',
  )

  async function fetchMe() {
    if (!getToken()) return
    loading.value = true
    error.value = null
    try {
      const { data } = await authApi.me()
      user.value = data
    } catch (e) {
      setToken(null)
      user.value = null
      error.value = extractErrorMessage(e)
    } finally {
      loading.value = false
    }
  }

  async function login(payload: LoginRequest) {
    loading.value = true
    error.value = null
    try {
      const { data } = await authApi.login(payload)
      setToken(data.token)
      user.value = data.user
      return true
    } catch (e) {
      error.value = extractErrorMessage(e)
      return false
    } finally {
      loading.value = false
    }
  }

  async function register(payload: RegisterRequest) {
    loading.value = true
    error.value = null
    try {
      const { data } = await authApi.register(payload)
      setToken(data.token)
      user.value = data.user
      return true
    } catch (e) {
      error.value = extractErrorMessage(e)
      return false
    } finally {
      loading.value = false
    }
  }

  function logout() {
    setToken(null)
    user.value = null
  }

  return {
    user,
    loading,
    error,
    isAuthenticated,
    isAdmin,
    isProjectManager,
    fetchMe,
    login,
    register,
    logout,
  }
})
