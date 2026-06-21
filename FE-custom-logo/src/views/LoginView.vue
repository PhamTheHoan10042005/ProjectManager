<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')

function fillAdmin() {
  email.value = 'admin@gmail.com'
  password.value = 'Admin123@'
}

async function submit() {
  const ok = await authStore.login({ email: email.value, password: password.value })
  if (ok) {
    const redirect = (route.query.redirect as string) || '/'
    router.push(redirect)
  }
}
</script>

<template>
  <div class="auth-page">
    <section class="auth-shell">
      <div class="auth-art">
        <span class="brand-chip">
          <img src="/brand-logo.png" alt="" />
          MixiPM
        </span>
        <h1>Ship tasks like deployments.</h1>
        <p>
          Một dashboard quản lý dự án theo phong cách hosting console: rõ trạng thái,
          gọn workflow, sẵn sàng cho sprint và Kanban.
        </p>
        <div class="login-shelf card">
          <span class="login-book teal"><b>N1</b></span>
          <span class="login-book orange tall"><b>PROJECT</b></span>
          <span class="login-book green"><b>TASK</b></span>
          <span class="login-book blue tall"><b>KANBAN</b></span>
          <span class="login-book ochre"><b>SPRINT</b></span>
          <span class="login-book pink tall"><b>NOTIFY</b></span>
        </div>
      </div>

      <div class="auth-card card">
        <div class="auth-header">
          <span class="auth-kicker">Login</span>
          <h2>Welcome back</h2>
          <p>Đề tài 06 - Quản lý dự án và phân công công việc.</p>
        </div>

        <div v-if="authStore.error" class="error-banner">{{ authStore.error }}</div>

        <form @submit.prevent="submit">
          <div class="form-group">
            <label>Email</label>
            <input v-model="email" type="email" required placeholder="admin@gmail.com" />
          </div>
          <div class="form-group">
            <label>Mật khẩu</label>
            <input v-model="password" type="password" required placeholder="Nhập mật khẩu" />
          </div>
          <button class="btn btn-primary auth-submit" type="submit" :disabled="authStore.loading">
            {{ authStore.loading ? 'Đang đăng nhập...' : 'Đăng nhập' }}
          </button>
        </form>

        <div class="demo-box">
          <div>
            <strong>Tài khoản Admin</strong>
            <p><code>admin@gmail.com</code> / <code>Admin123@</code></p>
          </div>
          <button type="button" class="btn btn-secondary btn-sm" @click="fillAdmin">Điền nhanh</button>
        </div>

        <p class="auth-footer">
          Chưa có tài khoản?
          <RouterLink to="/register">Đăng ký</RouterLink>
        </p>
      </div>
    </section>
  </div>
</template>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 1rem;
}

.auth-shell {
  width: min(1060px, 100%);
  display: grid;
  grid-template-columns: minmax(0, 1.1fr) minmax(360px, 420px);
  gap: 1rem;
}

.auth-art {
  min-height: 600px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding: clamp(1.5rem, 4vw, 3rem);
  border: 1px solid var(--border);
  border-radius: 28px;
  background:
    radial-gradient(circle at 86% 20%, rgba(207, 124, 7, 0.15), transparent 18rem),
    linear-gradient(135deg, #e8f5e9, #faf9f6 48%, #f3ead6);
  box-shadow: var(--shadow);
}

.brand-chip,
.auth-kicker {
  display: inline-flex;
  align-items: center;
  gap: 0.45rem;
  width: fit-content;
  padding: 0.32rem 0.68rem;
  border: 1px solid var(--border);
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.65);
  color: var(--primary-hover);
  font-size: 0.82rem;
  font-weight: 900;
}

.brand-chip img {
  width: 24px;
  height: 24px;
  border-radius: 7px;
}

.auth-art h1 {
  max-width: 620px;
  margin-top: 1.2rem;
  color: var(--primary);
  font-family: Outfit, Inter, sans-serif;
  font-size: clamp(3rem, 7vw, 6rem);
  font-weight: 900;
  letter-spacing: -0.06em;
  line-height: 0.92;
}

.auth-art p {
  max-width: 560px;
  margin-top: 1rem;
  color: var(--text-muted);
  font-size: 1rem;
}

.login-shelf {
  width: min(560px, 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.45rem;
  min-height: 180px;
  padding: 1.1rem 1rem 0.8rem;
  overflow: visible;
  background:
    linear-gradient(transparent calc(100% - 16px), #7b4f2e calc(100% - 16px), #5c3820 100%),
    rgba(255, 255, 255, 0.58);
}

.login-book {
  position: relative;
  display: grid;
  place-items: center;
  width: 36px;
  height: 110px;
  overflow: hidden;
  border-left: 4px solid rgba(0, 0, 0, 0.24);
  border-radius: 3px 6px 6px 3px;
  box-shadow: -2px 0 12px rgba(0, 0, 0, 0.28);
  transition: transform 0.32s cubic-bezier(0.34, 1.56, 0.64, 1);
  animation: shelfFloat 4s ease-in-out infinite alternate;
}

.login-book:nth-child(2n) {
  animation-delay: 0.35s;
}

.login-book:hover {
  transform: translateY(-22px) scale(1.08);
}

.login-book b {
  writing-mode: vertical-rl;
  transform: rotate(180deg);
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.62rem;
  font-weight: 900;
  letter-spacing: 0.12em;
}

.login-book.tall {
  height: 142px;
}

.login-book.teal {
  background: linear-gradient(160deg, #1a7a6e, #0f9b8a);
}
.login-book.orange {
  background: linear-gradient(160deg, #e07b2a, #f09040);
}
.login-book.green {
  background: linear-gradient(160deg, #2d8c5c, #3aaa70);
}
.login-book.blue {
  background: linear-gradient(160deg, #2e6db4, #4080cc);
}
.login-book.ochre {
  background: linear-gradient(160deg, #d48806, #f5b041);
}
.login-book.pink {
  background: linear-gradient(160deg, #d4607a, #e8809a);
}

.auth-card {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  justify-content: center;
  padding: 1.35rem;
  border-radius: 28px;
}

.auth-header {
  margin-bottom: 1.25rem;
}

.auth-header h2 {
  margin-top: 0.7rem;
  color: var(--primary);
  font-family: Outfit, Inter, sans-serif;
  font-size: 1.7rem;
  font-weight: 900;
  letter-spacing: -0.03em;
}

.auth-header p,
.auth-footer,
.demo-box p {
  color: var(--text-muted);
}

.auth-submit {
  width: 100%;
  margin-top: 0.35rem;
}

.demo-box {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-top: 1.2rem;
  padding: 0.9rem;
  border: 1px solid var(--border);
  border-radius: 6px;
  background: var(--surface-solid);
}

.demo-box code {
  padding: 0.1rem 0.3rem;
  border-radius: 4px;
  background: var(--bg-soft);
}

.auth-footer {
  margin-top: 1.2rem;
  text-align: center;
}

.auth-footer a {
  color: var(--accent);
  font-weight: 900;
}

@keyframes shelfFloat {
  from {
    transform: translateY(0);
  }
  to {
    transform: translateY(-8px);
  }
}

@media (max-width: 900px) {
  .auth-shell {
    grid-template-columns: 1fr;
  }

  .auth-art {
    min-height: 420px;
  }
}
</style>
