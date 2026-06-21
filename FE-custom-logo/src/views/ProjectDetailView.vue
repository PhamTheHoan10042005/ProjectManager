<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { projectApi } from '@/api/project.api'
import { notifyApi } from '@/api/notify.api'
import { authApi } from '@/api/auth.api'
import { extractErrorMessage } from '@/api/http'
import { ProjectRole, type ActivityLog, type Project, type ProjectMember, type Sprint, type User } from '@/types'
import { ROLE_LABELS } from '@/utils/constants'
import { formatDate, formatDateTime } from '@/utils/date'

const props = defineProps<{ projectId: string }>()
const route = useRoute()
const id = computed(() => props.projectId || String(route.params.projectId))

const project = ref<Project | null>(null)
const members = ref<ProjectMember[]>([])
const sprints = ref<Sprint[]>([])
const activities = ref<ActivityLog[]>([])
const users = ref<User[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

const showMemberModal = ref(false)
const showSprintModal = ref(false)

const memberForm = ref({ userId: '', role: ProjectRole.Member })
const sprintForm = ref({ name: '', goal: '', startDate: '', endDate: '' })

async function loadAll() {
  loading.value = true
  error.value = null
  try {
    const [projectRes, membersRes, sprintsRes, activitiesRes, usersRes] = await Promise.allSettled([
      projectApi.getById(id.value),
      projectApi.getMembers(id.value),
      projectApi.getSprints(id.value),
      notifyApi.getActivities(id.value),
      authApi.listUsers(),
    ])

    if (projectRes.status === 'fulfilled') project.value = projectRes.value.data
    else throw projectRes.reason

    members.value = membersRes.status === 'fulfilled' ? membersRes.value.data : []
    sprints.value = sprintsRes.status === 'fulfilled' ? sprintsRes.value.data : []
    activities.value = activitiesRes.status === 'fulfilled' ? activitiesRes.value.data : []
    users.value = usersRes.status === 'fulfilled' ? usersRes.value.data : []
  } catch (e) {
    error.value = extractErrorMessage(e)
  } finally {
    loading.value = false
  }
}

function memberLabel(userId: string) {
  const u = users.value.find((x) => x.id === userId)
  return u ? `${u.fullName} (${u.email})` : userId
}

async function addMember() {
  try {
    await projectApi.addMember(id.value, memberForm.value)
    showMemberModal.value = false
    memberForm.value = { userId: '', role: ProjectRole.Member }
    const { data } = await projectApi.getMembers(id.value)
    members.value = data
  } catch (e) {
    error.value = extractErrorMessage(e)
  }
}

async function createSprint() {
  try {
    await projectApi.createSprint(id.value, sprintForm.value)
    showSprintModal.value = false
    sprintForm.value = { name: '', goal: '', startDate: '', endDate: '' }
    const { data } = await projectApi.getSprints(id.value)
    sprints.value = data
  } catch (e) {
    error.value = extractErrorMessage(e)
  }
}

async function startSprint(sprintId: string) {
  try {
    await projectApi.startSprint(id.value, sprintId)
    const { data } = await projectApi.getSprints(id.value)
    sprints.value = data
  } catch (e) {
    error.value = extractErrorMessage(e)
  }
}

function extractTaskId(description: string) {
  return description.match(/[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}/i)?.[0]
}

onMounted(loadAll)
</script>

<template>
  <div>
    <div v-if="loading" class="loading-center"><div class="spinner" /></div>

    <template v-else-if="project">
      <div class="project-hero">
        <div>
          <RouterLink to="/projects" class="back-link">Danh sách dự án</RouterLink>
          <h1 class="page-title">
            <span class="dot" :style="{ background: project.color || '#2457ff' }" />
            {{ project.name }}
          </h1>
          <p class="page-subtitle">{{ project.description || 'Không có mô tả' }}</p>
        </div>
        <RouterLink :to="`/projects/${id}/kanban`" class="btn btn-primary">Mở Kanban</RouterLink>
      </div>

      <div v-if="error" class="error-banner">{{ error }}</div>

      <div class="overview-grid">
        <div class="card metric">
          <span>Bắt đầu</span>
          <strong>{{ formatDate(project.startDate) }}</strong>
        </div>
        <div class="card metric">
          <span>Kết thúc</span>
          <strong>{{ formatDate(project.endDate) }}</strong>
        </div>
        <div class="card metric">
          <span>Thành viên</span>
          <strong>{{ members.length }}</strong>
        </div>
        <div class="card metric">
          <span>Sprint</span>
          <strong>{{ sprints.length }}</strong>
        </div>
      </div>

      <div class="detail-grid">
        <section class="section card">
          <div class="section-header">
            <h2>Thành viên</h2>
            <button class="btn btn-secondary btn-sm" @click="showMemberModal = true">Thêm</button>
          </div>
          <div v-if="members.length === 0" class="empty-line">Chưa có thành viên</div>
          <div v-else class="member-list">
            <div v-for="member in members" :key="member.id" class="member-item">
              <strong>{{ memberLabel(member.userId) }}</strong>
              <span class="badge badge-primary">{{ ROLE_LABELS[member.role] }}</span>
            </div>
          </div>
        </section>

        <section class="section card">
          <div class="section-header">
            <h2>Sprint</h2>
            <button class="btn btn-secondary btn-sm" @click="showSprintModal = true">Tạo sprint</button>
          </div>
          <div v-if="sprints.length === 0" class="empty-line">Chưa có sprint</div>
          <div v-else class="sprint-list">
            <div v-for="sprint in sprints" :key="sprint.id" class="sprint-item">
              <div>
                <strong>{{ sprint.name }}</strong>
                <span v-if="sprint.isActive" class="badge badge-success">Đang chạy</span>
                <p>{{ sprint.goal || 'Không có mục tiêu' }}</p>
                <small>{{ formatDate(sprint.startDate) }} - {{ formatDate(sprint.endDate) }}</small>
              </div>
              <button v-if="!sprint.isActive" class="btn btn-secondary btn-sm" @click="startSprint(sprint.id)">
                Bắt đầu
              </button>
            </div>
          </div>
        </section>
      </div>

      <section v-if="activities.length" class="section card activity-card">
        <div class="section-header">
          <h2>Nhật ký hoạt động</h2>
        </div>
        <div class="activity-list">
          <div v-for="act in activities" :key="act.id" class="activity-item">
            <strong>{{ act.userName || act.userId }}</strong>
            <span>{{ act.description }}</span>
            <RouterLink
              v-if="extractTaskId(act.description)"
              :to="`/projects/${id}/tasks/${extractTaskId(act.description)}`"
              class="activity-link"
            >
              Mở task để xem bình luận
            </RouterLink>
            <small>{{ formatDateTime(act.createdAt) }}</small>
          </div>
        </div>
      </section>
    </template>

    <div v-if="showMemberModal" class="modal-overlay" @click.self="showMemberModal = false">
      <div class="modal card">
        <div class="modal-header">
          <h2>Thêm thành viên</h2>
          <button type="button" class="btn btn-ghost btn-sm" @click="showMemberModal = false">Đóng</button>
        </div>
        <form @submit.prevent="addMember">
          <div class="form-group">
            <label>Chọn người dùng</label>
            <select v-model="memberForm.userId" required>
              <option value="" disabled>Chọn người dùng</option>
              <option v-for="u in users" :key="u.id" :value="u.id">
                {{ u.fullName }} ({{ u.email }})
              </option>
            </select>
          </div>
          <div class="form-group">
            <label>Vai trò</label>
            <select v-model.number="memberForm.role">
              <option :value="ProjectRole.Manager">Manager</option>
              <option :value="ProjectRole.Member">Member</option>
              <option :value="ProjectRole.Viewer">Viewer</option>
            </select>
          </div>
          <div class="modal-actions">
            <button type="button" class="btn btn-secondary" @click="showMemberModal = false">Hủy</button>
            <button type="submit" class="btn btn-primary">Thêm</button>
          </div>
        </form>
      </div>
    </div>

    <div v-if="showSprintModal" class="modal-overlay" @click.self="showSprintModal = false">
      <div class="modal card">
        <div class="modal-header">
          <h2>Tạo sprint</h2>
          <button type="button" class="btn btn-ghost btn-sm" @click="showSprintModal = false">Đóng</button>
        </div>
        <form @submit.prevent="createSprint">
          <div class="form-group">
            <label>Tên sprint</label>
            <input v-model="sprintForm.name" required />
          </div>
          <div class="form-group">
            <label>Mục tiêu</label>
            <textarea v-model="sprintForm.goal" />
          </div>
          <div class="grid-2">
            <div class="form-group">
              <label>Bắt đầu</label>
              <input v-model="sprintForm.startDate" type="date" required />
            </div>
            <div class="form-group">
              <label>Kết thúc</label>
              <input v-model="sprintForm.endDate" type="date" required />
            </div>
          </div>
          <div class="modal-actions">
            <button type="button" class="btn btn-secondary" @click="showSprintModal = false">Hủy</button>
            <button type="submit" class="btn btn-primary">Tạo</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.project-hero {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1rem;
  padding: clamp(1.2rem, 3vw, 2rem);
  border: 1px solid var(--border);
  border-radius: 8px;
  background: linear-gradient(135deg, var(--bg-card), var(--primary-soft));
  box-shadow: var(--shadow-soft);
}

.back-link {
  display: inline-block;
  margin-bottom: 0.6rem;
  color: var(--text-muted);
  font-size: 0.88rem;
  font-weight: 800;
}

.page-title {
  display: flex;
  align-items: center;
  gap: 0.7rem;
}

.dot {
  width: 18px;
  height: 18px;
  border-radius: 50%;
  box-shadow: 0 0 0 8px var(--primary-soft);
}

.overview-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 1rem;
  margin-bottom: 1rem;
}

.metric span,
.empty-line,
.sprint-item p,
.sprint-item small,
.activity-item small {
  color: var(--text-muted);
}

.metric strong {
  display: block;
  margin-top: 0.35rem;
  font-size: 1.35rem;
}

.detail-grid {
  display: grid;
  grid-template-columns: minmax(0, 0.85fr) minmax(0, 1.15fr);
  gap: 1rem;
}

.section {
  margin-bottom: 1rem;
}

.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 0.85rem;
}

.member-list,
.sprint-list,
.activity-list {
  display: grid;
  gap: 0.65rem;
}

.member-item,
.sprint-item,
.activity-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  padding: 0.85rem;
  border: 1px solid var(--border);
  border-radius: 8px;
  background: var(--surface-solid);
}

.member-item strong,
.sprint-item strong {
  margin-right: 0.5rem;
}

.sprint-item p {
  margin: 0.2rem 0;
}

.activity-card {
  margin-top: 1rem;
}

.activity-item {
  align-items: flex-start;
  flex-direction: column;
}

.activity-link {
  width: fit-content;
  color: var(--accent);
  font-size: 0.86rem;
  font-weight: 900;
}

.activity-link:hover {
  text-decoration: underline;
}

@media (max-width: 900px) {
  .project-hero,
  .detail-grid {
    grid-template-columns: 1fr;
    flex-direction: column;
    align-items: stretch;
  }

  .overview-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
