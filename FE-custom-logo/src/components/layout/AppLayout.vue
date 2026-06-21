<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import { useNotificationStore } from '@/stores/notification.store'

const authStore = useAuthStore()
const notificationStore = useNotificationStore()
const router = useRouter()

const theme = ref<'light' | 'dark'>('light')

function applyTheme(nextTheme: 'light' | 'dark') {
  theme.value = nextTheme
  document.documentElement.dataset.theme = nextTheme
  localStorage.setItem('theme', nextTheme)
}

function toggleTheme() {
  applyTheme(theme.value === 'dark' ? 'light' : 'dark')
}

function logout() {
  authStore.logout()
  router.push('/login')
}

onMounted(() => {
  const savedTheme = localStorage.getItem('theme') as 'light' | 'dark' | null
  const prefersDark = window.matchMedia?.('(prefers-color-scheme: dark)').matches
  applyTheme(savedTheme || (prefersDark ? 'dark' : 'light'))
  notificationStore.fetchNotifications()
})
</script>

<template>
  <div class="app-shell">
    <header class="navbar">
      <RouterLink to="/" class="brand" aria-label="Project Manager">
        <img src="/brand-logo.png" alt="" class="brand-logo" />
        <span class="brand-text">MixiPM</span>
      </RouterLink>

      <nav class="nav-links" aria-label="Điều hướng chính">
        <RouterLink to="/">Tổng quan</RouterLink>
        <RouterLink to="/projects">Dự án</RouterLink>
        <RouterLink to="/notifications" class="notif-link">
          Thông báo
          <span v-if="notificationStore.unreadCount" class="notif-badge">
            {{ notificationStore.unreadCount }}
          </span>
        </RouterLink>
        <RouterLink v-if="authStore.isAdmin" to="/admin">Quản trị</RouterLink>
      </nav>

      <div class="nav-auth">
        <button class="theme-toggle" type="button" @click="toggleTheme">
          {{ theme === 'dark' ? 'Dark' : 'Light' }}
        </button>
        <span class="user-chip">{{ authStore.user?.fullName || authStore.user?.email }}</span>
        <button class="btn btn-secondary btn-sm" @click="logout">Đăng xuất</button>
      </div>
    </header>

    <main class="main-content">
      <RouterView v-slot="{ Component }">
        <Transition name="page" mode="out-in">
          <component :is="Component" />
        </Transition>
      </RouterView>
    </main>

    <footer class="site-footer">
      <div class="footer-inner">
        <div class="footer-grid">
          <section class="footer-brand">
            <RouterLink to="/" class="footer-logo">
              <img src="/brand-logo.png" alt="" />
              <span>ProjectManager</span>
            </RouterLink>
            <p>
              Không gian quản lý dự án hiện đại cho đội nhóm: Kanban, sprint, task,
              thông báo và phân quyền trong một workflow thống nhất.
            </p>
            <div class="social-row" aria-label="Liên kết nhanh">
              <RouterLink to="/projects" title="Dự án">PM</RouterLink>
              <RouterLink to="/notifications" title="Thông báo">NT</RouterLink>
              <RouterLink v-if="authStore.isAdmin" to="/admin" title="Quản trị">AD</RouterLink>
              <RouterLink to="/" title="Tổng quan">OV</RouterLink>
            </div>
          </section>

          <section class="footer-col">
            <h3>Khám phá</h3>
            <RouterLink to="/">Trang chủ</RouterLink>
            <RouterLink to="/projects">Dự án</RouterLink>
            <RouterLink to="/notifications">Thông báo</RouterLink>
            <RouterLink v-if="authStore.isAdmin" to="/admin">Quản trị</RouterLink>
          </section>

          <section class="footer-col">
            <h3>Dịch vụ</h3>
            <span>Project Service</span>
            <span>Task & Kanban</span>
            <span>Notify Service</span>
            <span>Member Roles</span>
            <span>Activity Log</span>
          </section>

          <section class="footer-contact">
            <h3>Bản tin</h3>
            <p>Nhận thông báo cập nhật sprint, task mới và hoạt động dự án mỗi tuần.</p>
            <form class="newsletter" @submit.prevent>
              <input type="email" placeholder="Email của bạn..." />
              <button type="submit">Đăng ký</button>
            </form>

            <h3 class="contact-title">Liên hệ</h3>
            <p class="contact-line">Đường 3/2, Xuân Khánh, Ninh Kiều, Cần Thơ</p>
            <p class="contact-line">Hotline: 1900 6789</p>
            <p class="contact-line">info@projectmanager.net</p>
          </section>
        </div>

        <div class="footer-bottom">
          <span>© 2026 ProjectManager — Nhóm 3 · All Rights Reserved</span>
          <div>
            <a href="#">Chính sách bảo mật</a>
            <a href="#">Điều khoản dịch vụ</a>
            <a href="#">Cookie</a>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.app-shell {
  min-height: 100vh;
}

.navbar {
  position: sticky;
  top: 0;
  z-index: 100;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1.25rem;
  min-height: 74px;
  padding: 0.9rem clamp(1rem, 5vw, 3.75rem);
  background: var(--bg-elevated);
  border-bottom: 1px solid var(--border);
  box-shadow: 0 4px 20px rgba(17, 75, 68, 0.05);
  backdrop-filter: blur(14px);
}

.brand {
  display: inline-flex;
  align-items: center;
  gap: 0.65rem;
  flex-shrink: 0;
}

.brand-logo {
  width: 36px;
  height: 36px;
  border-radius: 9px;
  object-fit: contain;
  box-shadow: 0 8px 20px rgba(17, 75, 68, 0.22);
}

.brand-text {
  color: var(--primary-hover);
  font-family: Outfit, Inter, sans-serif;
  font-size: 1.35rem;
  font-weight: 900;
  letter-spacing: 0.02em;
}

.nav-links {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: clamp(0.8rem, 2vw, 2rem);
  flex: 1;
}

.nav-links a {
  position: relative;
  color: var(--text-muted);
  padding-bottom: 0.18rem;
  font-size: 0.98rem;
  font-weight: 800;
  transition: color 0.25s var(--ease);
}

.nav-links a::after {
  content: '';
  position: absolute;
  left: 0;
  bottom: -0.25rem;
  width: 0;
  height: 2px;
  border-radius: 999px;
  background: var(--accent);
  transition: width 0.25s var(--ease);
}

.nav-links a:hover,
.nav-links a.router-link-active {
  color: var(--primary-hover);
}

.nav-links a:hover::after,
.nav-links a.router-link-active::after {
  width: 100%;
}

.notif-link {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
}

.notif-badge {
  display: inline-grid;
  place-items: center;
  min-width: 20px;
  height: 20px;
  padding: 0 0.35rem;
  border-radius: 999px;
  background: var(--accent);
  color: #fff;
  font-size: 0.68rem;
  font-weight: 900;
}

.nav-auth {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 0.7rem;
  flex-shrink: 0;
}

.theme-toggle {
  min-height: 34px;
  padding: 0.35rem 0.75rem;
  border: 1px solid var(--border);
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.64);
  color: var(--primary-hover);
  cursor: pointer;
  font-weight: 900;
  transition: transform 0.25s var(--ease), border-color 0.25s var(--ease);
}

.theme-toggle:hover {
  transform: translateY(-1px);
  border-color: var(--border-strong);
}

.user-chip {
  max-width: 170px;
  overflow: hidden;
  color: var(--text-muted);
  font-size: 0.84rem;
  font-weight: 700;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.main-content {
  width: min(1320px, calc(100% - 2rem));
  margin: 0 auto;
  padding: 1.5rem 0 2.5rem;
}

.site-footer {
  margin-top: 2rem;
  padding: 4rem clamp(1rem, 5vw, 3.75rem) 2rem;
  color: rgba(250, 249, 246, 0.78);
  background:
    radial-gradient(circle at 12% 0%, rgba(80, 170, 103, 0.16), transparent 24rem),
    linear-gradient(135deg, #102817, #173821 55%, #12311c);
}

.footer-inner {
  width: min(1320px, 100%);
  margin: 0 auto;
}

.footer-grid {
  display: grid;
  grid-template-columns: 1.35fr 0.85fr 0.85fr 1.35fr;
  gap: clamp(2rem, 5vw, 4rem);
}

.footer-logo {
  display: inline-flex;
  align-items: center;
  gap: 0.8rem;
  margin-bottom: 1.2rem;
}

.footer-logo img {
  width: 48px;
  height: 48px;
  border-radius: 14px;
  box-shadow: 0 12px 28px rgba(0, 0, 0, 0.22);
}

.footer-logo span {
  color: #fff;
  font-family: Outfit, Inter, sans-serif;
  font-size: 1.7rem;
  font-weight: 900;
}

.footer-brand p,
.footer-contact p {
  max-width: 420px;
  color: rgba(250, 249, 246, 0.72);
  font-size: 1rem;
  line-height: 1.75;
}

.social-row {
  display: flex;
  gap: 0.75rem;
  margin-top: 1.6rem;
}

.social-row a {
  display: grid;
  place-items: center;
  width: 46px;
  height: 46px;
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.06);
  color: #d8e7dd;
  font-size: 0.78rem;
  font-weight: 900;
  transition: transform 0.25s var(--ease), background 0.25s var(--ease), color 0.25s var(--ease);
}

.social-row a:hover {
  transform: translateY(-4px);
  background: #4fb36b;
  color: #fff;
}

.footer-col,
.footer-contact {
  display: flex;
  flex-direction: column;
  gap: 0.8rem;
}

.footer-col h3,
.footer-contact h3 {
  margin-bottom: 0.3rem;
  color: #fff;
  font-size: 1rem;
  font-weight: 900;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.footer-col a,
.footer-col span,
.footer-bottom a {
  color: rgba(250, 249, 246, 0.68);
  font-size: 1rem;
  font-weight: 650;
  transition: color 0.25s var(--ease), transform 0.25s var(--ease);
}

.footer-col a:hover,
.footer-bottom a:hover {
  color: #f5b041;
  transform: translateX(3px);
}

.newsletter {
  display: flex;
  gap: 0.65rem;
  margin: 0.6rem 0 1.2rem;
  padding: 0.9rem;
  border: 1px solid rgba(255, 255, 255, 0.13);
  border-radius: 18px;
  background: rgba(255, 255, 255, 0.06);
}

.newsletter input {
  min-width: 0;
  flex: 1;
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 12px;
  padding: 0.75rem 0.9rem;
  background: rgba(255, 255, 255, 0.08);
  color: #fff;
}

.newsletter input::placeholder {
  color: rgba(250, 249, 246, 0.48);
}

.newsletter button {
  border-radius: 12px;
  padding: 0.75rem 1.1rem;
  background: #4fb36b;
  color: #fff;
  cursor: pointer;
  font-weight: 900;
  transition: transform 0.25s var(--ease), background 0.25s var(--ease);
}

.newsletter button:hover {
  transform: translateY(-2px);
  background: #f5b041;
}

.contact-title {
  margin-top: 0.2rem;
}

.contact-line {
  margin: 0;
}

.footer-bottom {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-top: 3.2rem;
  padding-top: 1.5rem;
  border-top: 1px solid rgba(255, 255, 255, 0.11);
  color: rgba(250, 249, 246, 0.58);
  font-weight: 650;
}

.footer-bottom div {
  display: flex;
  gap: 1.8rem;
  flex-wrap: wrap;
}

.page-enter-active,
.page-leave-active {
  transition: opacity 0.26s ease, transform 0.26s var(--ease);
}

.page-enter-from,
.page-leave-to {
  opacity: 0;
  transform: translateY(14px);
}

@media (max-width: 1080px) {
  .footer-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 980px) {
  .navbar {
    flex-wrap: wrap;
  }

  .nav-links {
    order: 3;
    flex-basis: 100%;
    justify-content: flex-start;
    overflow-x: auto;
    padding-bottom: 0.2rem;
  }
}

@media (max-width: 720px) {
  .footer-grid {
    grid-template-columns: 1fr;
  }

  .footer-bottom {
    align-items: flex-start;
    flex-direction: column;
  }

  .newsletter {
    flex-direction: column;
  }
}

@media (max-width: 620px) {
  .navbar {
    padding: 0.8rem;
  }

  .brand-text,
  .user-chip {
    display: none;
  }

  .main-content {
    width: min(100% - 1rem, 1320px);
  }
}
</style>
