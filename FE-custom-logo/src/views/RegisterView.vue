<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'

const router = useRouter()
const authStore = useAuthStore()

const fullName = ref('')
const email = ref('')
const password = ref('')

async function submit() {
  const ok = await authStore.register({
    fullName: fullName.value,
    email: email.value,
    password: password.value,
  })
  if (ok) router.push('/')
}
</script>

<template>
  <div class="auth-page">
    <div class="auth-card card">
      <div class="auth-header">
        <span class="auth-kicker">Tài khoản mới</span>
        <h1>Đăng ký</h1>
        <p>Tài khoản mới mặc định ở quyền Viewer, admin có thể phân quyền sau.</p>
      </div>

      <div v-if="authStore.error" class="error-banner">{{ authStore.error }}</div>

      <form @submit.prevent="submit">
        <div class="form-group">
          <label>Họ tên</label>
          <input v-model="fullName" required placeholder="Nguyễn Văn A" />
        </div>
        <div class="form-group">
          <label>Email</label>
          <input v-model="email" type="email" required placeholder="user@example.com" />
        </div>
        <div class="form-group">
          <label>Mật khẩu</label>
          <input v-model="password" type="password" required minlength="6" placeholder="Tối thiểu 6 ký tự" />
        </div>
        <button class="btn btn-primary auth-submit" type="submit" :disabled="authStore.loading">
          {{ authStore.loading ? 'Đang xử lý...' : 'Tạo tài khoản' }}
        </button>
      </form>

      <p class="auth-footer">
        Đã có tài khoản?
        <RouterLink to="/login">Đăng nhập</RouterLink>
      </p>
    </div>
  </div>
</template>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 1rem;
}

.auth-card {
  width: min(460px, 100%);
  padding: 1.4rem;
  animation: riseIn 0.44s var(--ease) both;
}

.auth-header {
  margin-bottom: 1.35rem;
}

.auth-kicker {
  display: inline-flex;
  padding: 0.34rem 0.68rem;
  border-radius: 999px;
  background: var(--primary-soft);
  color: var(--primary-hover);
  font-weight: 850;
  font-size: 0.78rem;
}

.auth-header h1 {
  margin-top: 0.75rem;
  font-size: 2.2rem;
}

.auth-header p,
.auth-footer {
  color: var(--text-muted);
}

.auth-submit {
  width: 100%;
}

.auth-footer {
  margin-top: 1.2rem;
  text-align: center;
}

.auth-footer a {
  color: var(--primary-hover);
  font-weight: 850;
}
</style>
