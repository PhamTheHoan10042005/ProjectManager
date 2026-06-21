<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { authApi } from '@/api/auth.api'
import { extractErrorMessage } from '@/api/http'
import { GLOBAL_ROLE_LABELS, GLOBAL_ROLES } from '@/utils/constants'
import type { User } from '@/types'

const users = ref<User[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
const success = ref<string | null>(null)

async function loadUsers() {
  loading.value = true
  error.value = null
  try {
    const { data } = await authApi.listUsers()
    users.value = data
  } catch (e) {
    error.value = extractErrorMessage(e)
  } finally {
    loading.value = false
  }
}

async function changeRole(user: User, role: string) {
  error.value = null
  success.value = null
  try {
    await authApi.updateUserRole(user.id, role)
    user.role = role
    success.value = `Đã cập nhật role cho ${user.email}`
  } catch (e) {
    error.value = extractErrorMessage(e)
  }
}

onMounted(loadUsers)
</script>

<template>
  <div>
    <div class="page-header">
      <div>
        <span class="eyebrow">Admin Console</span>
        <h1 class="page-title">Quản trị người dùng</h1>
        <p class="page-subtitle">Quản lý tài khoản và phân quyền hệ thống.</p>
      </div>
    </div>

    <div class="admin-note card">
      <div>
        <strong>Tài khoản Admin cố định</strong>
        <p><code>admin@gmail.com</code> / <code>Admin123@</code></p>
      </div>
      <span class="note-muted">Tự động khôi phục mỗi khi Notify Service khởi động.</span>
    </div>

    <div v-if="error" class="error-banner">{{ error }}</div>
    <div v-if="success" class="success-banner">{{ success }}</div>

    <div v-if="loading" class="loading-center"><div class="spinner" /></div>

    <div v-else class="card table-wrap">
      <table class="user-table">
        <thead>
          <tr>
            <th>Họ tên</th>
            <th>Email</th>
            <th>Role hệ thống</th>
            <th>Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td>{{ user.fullName }}</td>
            <td>{{ user.email }}</td>
            <td>{{ GLOBAL_ROLE_LABELS[user.role || 'Viewer'] || user.role }}</td>
            <td>
              <select
                :value="user.role"
                class="role-select"
                :disabled="user.email === 'admin@gmail.com'"
                @change="changeRole(user, ($event.target as HTMLSelectElement).value)"
              >
                <option v-for="role in GLOBAL_ROLES" :key="role" :value="role">
                  {{ GLOBAL_ROLE_LABELS[role] }}
                </option>
              </select>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.page-header {
  margin-bottom: 1rem;
}

.eyebrow {
  display: inline-block;
  margin-bottom: 0.6rem;
  color: var(--primary-hover);
  font-size: 0.78rem;
  font-weight: 850;
  text-transform: uppercase;
}

.admin-note {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1rem;
}

.admin-note p {
  margin-top: 0.25rem;
  color: var(--text-muted);
}

.admin-note code {
  padding: 0.15rem 0.4rem;
  border-radius: 6px;
  background: var(--bg-soft);
}

.note-muted {
  color: var(--text-muted);
  font-size: 0.86rem;
}

.table-wrap {
  overflow-x: auto;
  padding: 0;
}

.user-table {
  width: 100%;
  border-collapse: collapse;
}

.user-table th,
.user-table td {
  padding: 0.95rem 1rem;
  text-align: left;
  border-bottom: 1px solid var(--border);
}

.user-table th {
  color: var(--text-muted);
  font-size: 0.8rem;
  font-weight: 850;
  text-transform: uppercase;
}

.user-table tr {
  transition: background 0.2s var(--ease);
}

.user-table tbody tr:hover {
  background: var(--primary-soft);
}

.role-select {
  min-width: 180px;
  padding: 0.52rem 0.7rem;
  border-radius: 8px;
  border: 1px solid var(--border);
  background: var(--surface-solid);
  color: var(--text);
}

@media (max-width: 720px) {
  .admin-note {
    align-items: flex-start;
    flex-direction: column;
  }
}
</style>
