<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import { projectApi } from '@/api/project.api'
import { useAuthStore } from '@/stores/auth.store'
import { useNotificationStore } from '@/stores/notification.store'
import { extractErrorMessage } from '@/api/http'
import { GLOBAL_ROLE_LABELS } from '@/utils/constants'
import type { Project } from '@/types'

const authStore = useAuthStore()
const notificationStore = useNotificationStore()

const projects = ref<Project[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

const shelfRows = [
  ['API', 'KANBAN', 'SPRINT', 'TEAM', 'TASK', 'NOTIFY', 'ADMIN'],
  ['BACKLOG', 'REVIEW', 'DONE', 'MEMBER', 'COMMENT', 'TIME'],
  ['PROJECT', 'N1', 'N2', 'N3', 'AUTH', 'DEPLOY', 'LOG'],
]

async function load() {
  loading.value = true
  error.value = null
  try {
    const { data } = await projectApi.getAll()
    projects.value = data
    await notificationStore.fetchNotifications(true)
  } catch (e) {
    error.value = extractErrorMessage(e)
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="home">
    <section class="hero-section">
      <div class="hero-left">
        <span class="hero-kicker">Microservice Project Workspace</span>
        <h1 class="hero-title">Điều phối dự án như một thư viện công việc sống động.</h1>
        <p class="hero-description">
          Quản lý dự án, sprint, Kanban, task, thông báo và phân quyền trong một giao diện
          có nhận diện riêng, sáng sủa và nhiều chuyển động mượt.
        </p>
        <div class="hero-actions">
          <RouterLink to="/projects" class="btn btn-primary">Mở dự án</RouterLink>
          <RouterLink to="/notifications" class="btn btn-secondary">Thông báo</RouterLink>
        </div>
        <div class="role-chip" v-if="authStore.user?.role">
          {{ GLOBAL_ROLE_LABELS[authStore.user.role] || authStore.user.role }}
        </div>
      </div>

      <div class="hero-right">
        <svg class="wave-border" viewBox="0 0 120 800" preserveAspectRatio="none" aria-hidden="true">
          <path d="M0 0 C92 110 20 220 82 330 C132 418 48 520 96 650 C112 706 82 760 120 800 L0 800 Z" />
        </svg>
        <div class="workspace-shelf">
          <div v-for="(row, rowIndex) in shelfRows" :key="rowIndex" class="shelf-row">
            <div class="shelf-blocks">
              <span
                v-for="(item, itemIndex) in row"
                :key="item"
                class="block"
                :class="[
                  ['teal', 'orange', 'green', 'pink', 'blue', 'ochre', 'beige'][itemIndex % 7],
                  itemIndex % 5 === 0 ? 'tall' : itemIndex % 3 === 0 ? 'short' : 'medium',
                  itemIndex % 4 === 0 ? 'lean-left' : itemIndex % 4 === 2 ? 'lean-right' : ''
                ]"
              >
                <b>{{ item }}</b>
              </span>
            </div>
            <div class="shelf-plank" />
          </div>
        </div>
      </div>
    </section>

    <div v-if="error" class="error-banner">{{ error }}</div>

    <section class="marquee-wrap">
      <div class="marquee-track">
        <span v-for="item in 2" :key="item">
          Project Service • Task Service • Notify Service • Sprint Planning • Kanban Board • Time Log • Activity Feed •
        </span>
      </div>
    </section>

    <section class="stats-grid">
      <div class="stat-card">
        <strong>{{ projects.length }}</strong>
        <span>Dự án</span>
      </div>
      <div class="stat-card">
        <strong>{{ notificationStore.unreadCount }}</strong>
        <span>Thông báo chưa đọc</span>
      </div>
      <div class="stat-card">
        <strong>3</strong>
        <span>Microservices</span>
      </div>
      <div class="stat-card">
        <strong>24/7</strong>
        <span>Workspace</span>
      </div>
    </section>

    <section class="section">
      <div class="section-header">
        <div>
          <span class="section-num">01</span>
          <h2>Dự án gần đây</h2>
          <p>Mỗi dự án là một workspace sẵn sàng mở Kanban, sprint và thành viên.</p>
        </div>
        <RouterLink to="/projects" class="btn btn-secondary btn-sm">Xem tất cả</RouterLink>
      </div>

      <div v-if="loading" class="loading-center"><div class="spinner" /></div>

      <div v-else-if="projects.length === 0" class="card empty-state">
        <p>Chưa có dự án. Hãy tạo dự án đầu tiên để bắt đầu.</p>
        <RouterLink to="/projects" class="btn btn-primary">Tạo dự án</RouterLink>
      </div>

      <div v-else class="project-grid">
        <RouterLink
          v-for="project in projects.slice(0, 4)"
          :key="project.id"
          :to="`/projects/${project.id}`"
          class="feature-card card"
        >
          <div class="feature-top">
            <span class="project-token" :style="{ background: project.color || '#1e5652' }" />
            <span class="badge badge-success">Active</span>
          </div>
          <h3>{{ project.name }}</h3>
          <p>{{ project.description || 'Không có mô tả' }}</p>
        </RouterLink>
      </div>
    </section>
  </div>
</template>

<style scoped>
.home {
  display: grid;
  gap: 1.4rem;
}

.hero-section {
  min-height: 620px;
  display: grid;
  grid-template-columns: minmax(0, 0.95fr) minmax(420px, 1.05fr);
  overflow: hidden;
  border: 1px solid var(--border);
  border-radius: 28px;
  background: linear-gradient(135deg, #e8f5e9 0%, #f8fbf1 44%, #f3ead6 100%);
  box-shadow: var(--shadow);
}

:root[data-theme='dark'] .hero-section {
  background: linear-gradient(135deg, #16231f 0%, #172b26 44%, #2a1f12 100%);
}

.hero-left {
  z-index: 2;
  display: flex;
  flex-direction: column;
  justify-content: center;
  padding: clamp(2rem, 5vw, 4.8rem);
}

.hero-kicker {
  width: fit-content;
  margin-bottom: 1rem;
  padding: 0.35rem 0.78rem;
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.58);
  color: var(--primary-hover);
  font-size: 0.82rem;
  font-weight: 900;
}

.hero-title {
  max-width: 650px;
  color: var(--primary);
  font-family: Outfit, Inter, sans-serif;
  font-size: clamp(3rem, 6vw, 5.9rem);
  font-weight: 900;
  letter-spacing: -0.055em;
  line-height: 0.94;
}

.hero-description {
  max-width: 560px;
  margin: 1.25rem 0 1.5rem;
  color: var(--text-muted);
  font-size: 1.02rem;
  line-height: 1.8;
}

.hero-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 0.8rem;
}

.role-chip {
  width: fit-content;
  margin-top: 1.2rem;
  padding: 0.5rem 0.75rem;
  border: 1px dashed var(--border-strong);
  border-radius: 999px;
  color: var(--primary-hover);
  font-weight: 900;
}

.hero-right {
  position: relative;
  display: flex;
  align-items: stretch;
  min-height: 620px;
  overflow: hidden;
  background:
    radial-gradient(circle at 70% 15%, rgba(245, 176, 65, 0.16), transparent 14rem),
    #2c1f14;
}

.wave-border {
  position: absolute;
  inset: 0 auto 0 0;
  z-index: 3;
  width: 112px;
  height: 100%;
  filter: drop-shadow(6px 0 14px rgba(0, 0, 0, 0.2));
  fill: #e8f5e9;
}

:root[data-theme='dark'] .wave-border {
  fill: #16231f;
}

.workspace-shelf {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: space-around;
  gap: 1rem;
  padding: 2rem 2rem 2rem 8rem;
}

.shelf-row {
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
}

.shelf-blocks {
  display: flex;
  align-items: flex-end;
  gap: 0.42rem;
  padding-bottom: 0.1rem;
}

.shelf-plank {
  height: 13px;
  border-radius: 4px;
  background: linear-gradient(#9b6a42 0%, #7b4f2e 62%, #5c3820 100%);
  box-shadow: 0 8px 18px rgba(0, 0, 0, 0.55);
}

.block {
  position: relative;
  flex-shrink: 0;
  display: grid;
  place-items: center;
  width: 36px;
  height: 112px;
  overflow: hidden;
  border-left: 4px solid rgba(0, 0, 0, 0.25);
  border-radius: 3px 5px 5px 3px;
  cursor: pointer;
  box-shadow: -2px 0 10px rgba(0, 0, 0, 0.45);
  transition: transform 0.32s cubic-bezier(0.34, 1.56, 0.64, 1), box-shadow 0.32s;
}

.block::after {
  content: '';
  position: absolute;
  top: 0;
  left: -70%;
  width: 38%;
  height: 100%;
  background: linear-gradient(120deg, transparent 0%, rgba(255, 255, 255, 0.32) 50%, transparent 100%);
  transform: skew(-15deg);
  animation: blockShimmer 4.1s ease-in-out infinite;
}

.block:nth-child(2)::after {
  animation-delay: 0.35s;
}
.block:nth-child(3)::after {
  animation-delay: 0.7s;
}
.block:nth-child(4)::after {
  animation-delay: 1.05s;
}
.block:nth-child(5)::after {
  animation-delay: 1.4s;
}

.block:hover {
  z-index: 20;
  box-shadow: 0 20px 38px rgba(0, 0, 0, 0.62);
  transform: translateY(-24px) scale(1.08) !important;
}

.block b {
  writing-mode: vertical-rl;
  text-orientation: mixed;
  transform: rotate(180deg);
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.64rem;
  font-weight: 900;
  letter-spacing: 0.12em;
}

.block.tall {
  height: 146px;
  width: 40px;
}

.block.medium {
  height: 118px;
}

.block.short {
  height: 86px;
  width: 32px;
}

.block.lean-left {
  transform-origin: 0 100%;
  transform: rotate(-12deg);
}

.block.lean-right {
  transform-origin: 100% 100%;
  transform: rotate(10deg);
}

.block.teal {
  background: linear-gradient(160deg, #1a7a6e, #0f9b8a);
}
.block.orange {
  background: linear-gradient(160deg, #e07b2a, #f09040);
}
.block.green {
  background: linear-gradient(160deg, #2d8c5c, #3aaa70);
}
.block.pink {
  background: linear-gradient(160deg, #d4607a, #e8809a);
}
.block.blue {
  background: linear-gradient(160deg, #2e6db4, #4080cc);
}
.block.ochre {
  background: linear-gradient(160deg, #d48806, #f5b041);
}
.block.beige {
  background: linear-gradient(160deg, #d8ceb8, #ede5d0);
}
.block.beige b {
  color: rgba(60, 40, 20, 0.78);
}

.marquee-wrap {
  overflow: hidden;
  border: 1px solid rgba(30, 86, 82, 0.12);
  border-radius: 999px;
  background: rgba(232, 245, 233, 0.8);
}

.marquee-track {
  display: flex;
  width: max-content;
  animation: marquee 24s linear infinite;
}

.marquee-track:hover {
  animation-play-state: paused;
}

.marquee-track span {
  white-space: nowrap;
  padding: 0.95rem 1.5rem;
  color: var(--primary-hover);
  font-weight: 800;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  overflow: hidden;
  border-radius: 24px;
  background: linear-gradient(135deg, #1e3a24, #2d5c3a, #3a8a52);
}

.stat-card {
  padding: 1.8rem 1rem;
  text-align: center;
  border-right: 1px solid rgba(255, 255, 255, 0.16);
  transition: transform 0.28s var(--ease);
}

.stat-card:last-child {
  border-right: 0;
}

.stat-card:hover {
  transform: translateY(-6px);
}

.stat-card strong {
  display: block;
  color: #fff;
  font-family: Outfit, Inter, sans-serif;
  font-size: 2.8rem;
  font-weight: 900;
  line-height: 1;
}

.stat-card span {
  display: block;
  margin-top: 0.35rem;
  color: rgba(255, 255, 255, 0.72);
  font-weight: 700;
}

.section {
  display: grid;
  gap: 1rem;
  padding: 1rem 0;
}

.section-header {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 1rem;
}

.section-num {
  display: block;
  margin-bottom: 0.15rem;
  color: var(--accent);
  font-family: Outfit, Inter, sans-serif;
  font-size: 3.8rem;
  font-weight: 900;
  line-height: 1;
}

.section h2 {
  color: var(--primary);
  font-family: Outfit, Inter, sans-serif;
  font-size: 2rem;
  font-weight: 900;
}

.section-header p,
.feature-card p {
  color: var(--text-muted);
}

.project-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
}

.feature-card {
  min-height: 210px;
  display: grid;
  gap: 0.8rem;
  transition: transform 0.32s var(--ease), box-shadow 0.32s var(--ease);
}

.feature-card:hover {
  transform: translateY(-10px);
  box-shadow: var(--shadow);
}

.feature-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.project-token {
  width: 54px;
  height: 74px;
  border-radius: 8px;
  border-left: 5px solid rgba(0, 0, 0, 0.22);
  box-shadow: -2px 0 12px rgba(0, 0, 0, 0.16);
}

.feature-card h3 {
  color: var(--primary);
  font-size: 1.15rem;
}

@keyframes blockShimmer {
  0% {
    opacity: 0;
    left: -70%;
  }
  12% {
    opacity: 1;
  }
  42%,
  100% {
    opacity: 0;
    left: 135%;
  }
}

@keyframes marquee {
  from {
    transform: translateX(0);
  }
  to {
    transform: translateX(-50%);
  }
}

@media (max-width: 960px) {
  .hero-section {
    grid-template-columns: 1fr;
  }

  .hero-right {
    min-height: 460px;
  }

  .wave-border {
    display: none;
  }

  .workspace-shelf {
    padding: 2rem;
  }

  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 620px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }

  .section-header {
    align-items: stretch;
    flex-direction: column;
  }
}
</style>
