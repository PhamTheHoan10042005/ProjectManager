<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { projectApi } from '@/api/project.api'
import { extractErrorMessage } from '@/api/http'
import { DEFAULT_PROJECT_COLORS } from '@/utils/constants'
import { formatDate } from '@/utils/date'
import type { Project } from '@/types'

const projects = ref<Project[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
const showModal = ref(false)

const form = ref({
  name: '',
  description: '',
  startDate: '',
  endDate: '',
  color: DEFAULT_PROJECT_COLORS[0],
})

async function loadProjects() {
  loading.value = true
  error.value = null
  try {
    const { data } = await projectApi.getAll()
    projects.value = data
  } catch (e) {
    error.value = extractErrorMessage(e)
  } finally {
    loading.value = false
  }
}

async function createProject() {
  error.value = null
  try {
    await projectApi.create({
      name: form.value.name,
      description: form.value.description || undefined,
      startDate: form.value.startDate || undefined,
      endDate: form.value.endDate || undefined,
      color: form.value.color,
    })
    showModal.value = false
    form.value = { name: '', description: '', startDate: '', endDate: '', color: DEFAULT_PROJECT_COLORS[0] }
    await loadProjects()
  } catch (e) {
    error.value = extractErrorMessage(e)
  }
}

onMounted(loadProjects)
</script>

<template>
  <div class="projects-page">
    <div class="page-header projects-header">
      <div>
        <span class="scope">Project Service</span>
        <h1 class="page-title">Dự án</h1>
        <p class="page-subtitle">Mỗi dự án là một tập workspace gồm thành viên, sprint, task và nhật ký hoạt động.</p>
      </div>
      <button class="btn btn-primary" @click="showModal = true">Tạo dự án</button>
    </div>

    <div v-if="error" class="error-banner">{{ error }}</div>
    <div v-if="loading" class="loading-center"><div class="spinner" /></div>

    <div v-else-if="projects.length === 0" class="card empty-state">
      <p>Chưa có dự án nào. Hãy tạo dự án đầu tiên.</p>
      <button class="btn btn-primary" @click="showModal = true">New Project</button>
    </div>

    <div v-else class="project-grid">
      <RouterLink
        v-for="project in projects"
        :key="project.id"
        :to="`/projects/${project.id}`"
        class="project-card card"
      >
        <div class="project-head">
          <span class="project-logo" :style="{ '--project-color': project.color || '#1e5652' }">
            <b>{{ project.name.slice(0, 2).toUpperCase() }}</b>
          </span>
          <span class="badge badge-success">Ready</span>
        </div>
        <h3>{{ project.name }}</h3>
        <p>{{ project.description || 'No description provided' }}</p>
        <div class="project-meta">
          <span>{{ formatDate(project.startDate) }}</span>
          <span v-if="project.memberCount">{{ project.memberCount }} thành viên</span>
          <span v-else>Private</span>
        </div>
      </RouterLink>
    </div>

    <div v-if="showModal" class="modal-overlay" @click.self="showModal = false">
      <div class="modal card">
        <div class="modal-header">
          <h2>Tạo dự án</h2>
          <button class="btn btn-ghost btn-sm" @click="showModal = false">Đóng</button>
        </div>
        <form @submit.prevent="createProject">
          <div class="form-group">
            <label>Tên dự án</label>
            <input v-model="form.name" required placeholder="Website quản lý sprint" />
          </div>
          <div class="form-group">
            <label>Mô tả</label>
            <textarea v-model="form.description" placeholder="Mục tiêu, phạm vi hoặc ghi chú ngắn" />
          </div>
          <div class="grid-2">
            <div class="form-group">
              <label>Ngày bắt đầu</label>
              <input v-model="form.startDate" type="date" />
            </div>
            <div class="form-group">
              <label>Ngày kết thúc</label>
              <input v-model="form.endDate" type="date" />
            </div>
          </div>
          <div class="form-group">
            <label>Màu nhận diện</label>
            <div class="color-picker">
              <button
                v-for="color in DEFAULT_PROJECT_COLORS"
                :key="color"
                type="button"
                class="color-dot"
                :class="{ active: form.color === color }"
                :style="{ background: color }"
                @click="form.color = color"
              />
            </div>
          </div>
          <div class="modal-actions">
            <button type="button" class="btn btn-secondary" @click="showModal = false">Hủy</button>
            <button type="submit" class="btn btn-primary">Tạo</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.projects-page {
  display: grid;
  gap: 1.2rem;
}

.projects-header {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 1rem;
  padding: 1.6rem;
  border: 1px solid var(--border);
  border-radius: 24px;
  background:
    radial-gradient(circle at 82% 20%, rgba(207, 124, 7, 0.14), transparent 18rem),
    linear-gradient(135deg, rgba(232, 245, 233, 0.92), rgba(255, 255, 255, 0.72));
  box-shadow: var(--shadow-soft);
}

.scope {
  display: block;
  margin-bottom: 0.5rem;
  color: var(--text-muted);
  font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, monospace;
  font-size: 0.78rem;
}

.project-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1rem;
}

.project-card {
  display: grid;
  gap: 0.9rem;
  min-height: 260px;
  overflow: hidden;
  transition: transform 0.32s var(--ease), border-color 0.32s var(--ease), box-shadow 0.32s var(--ease);
}

.project-card:hover {
  border-color: var(--border-strong);
  box-shadow: var(--shadow);
  transform: translateY(-10px);
}

.project-head,
.project-meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
}

.project-logo {
  position: relative;
  display: grid;
  place-items: center;
  width: 64px;
  height: 94px;
  overflow: hidden;
  border-left: 6px solid rgba(0, 0, 0, 0.24);
  border-radius: 6px 10px 10px 6px;
  background: linear-gradient(160deg, color-mix(in srgb, var(--project-color) 82%, white), var(--project-color));
  box-shadow: -3px 0 14px rgba(0, 0, 0, 0.22);
}

.project-logo::after {
  content: '';
  position: absolute;
  top: 0;
  left: -70%;
  width: 36%;
  height: 100%;
  background: linear-gradient(120deg, transparent, rgba(255, 255, 255, 0.34), transparent);
  transform: skew(-15deg);
  animation: projectShimmer 4.4s ease-in-out infinite;
}

.project-logo b {
  color: rgba(255, 255, 255, 0.9);
  font-family: Outfit, Inter, sans-serif;
  font-weight: 900;
  letter-spacing: 0.06em;
}

.project-card h3 {
  color: var(--primary);
  font-family: Outfit, Inter, sans-serif;
  font-size: 1.28rem;
  font-weight: 900;
}

.project-card p {
  color: var(--text-muted);
  min-height: 48px;
  font-size: 0.9rem;
}

.project-meta {
  align-self: end;
  padding-top: 0.75rem;
  border-top: 1px dashed var(--border-strong);
  color: var(--text-muted);
  font-size: 0.8rem;
  font-weight: 800;
}

.color-picker {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.color-dot {
  width: 28px;
  height: 28px;
  border: 1px solid var(--border);
  border-radius: 999px;
  cursor: pointer;
  transition: box-shadow 0.18s var(--ease), transform 0.18s var(--ease);
}

.color-dot:hover,
.color-dot.active {
  box-shadow: 0 0 0 3px var(--surface-solid), 0 0 0 5px var(--text);
  transform: translateY(-1px);
}

@keyframes projectShimmer {
  0% {
    opacity: 0;
    left: -70%;
  }
  14% {
    opacity: 1;
  }
  44%,
  100% {
    opacity: 0;
    left: 135%;
  }
}

@media (max-width: 720px) {
  .projects-header {
    align-items: stretch;
    flex-direction: column;
  }
}
</style>
