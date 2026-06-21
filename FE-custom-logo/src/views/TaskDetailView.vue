<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { taskApi } from '@/api/task.api'
import { notifyApi } from '@/api/notify.api'
import { extractErrorMessage } from '@/api/http'
import { useAuthStore } from '@/stores/auth.store'
import {
  KanbanStatus,
  type Comment,
  type KanbanStatus as KanbanStatusType,
  type SubTask,
  type TaskItem,
  type TaskTimeSummary,
} from '@/types'
import { kanbanStatusLabel, PRIORITY_COLORS, PRIORITY_LABELS } from '@/utils/constants'
import { formatDate, formatDateTime } from '@/utils/date'

const props = defineProps<{ projectId: string; taskId: string }>()
const route = useRoute()
const authStore = useAuthStore()

const projectId = computed(() => props.projectId || String(route.params.projectId))
const taskId = computed(() => props.taskId || String(route.params.taskId))

const task = ref<TaskItem | null>(null)
const subTasks = ref<SubTask[]>([])
const timeSummary = ref<TaskTimeSummary | null>(null)
const comments = ref<Comment[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

const newSubTask = ref('')
const newComment = ref('')
const timeForm = ref({ hours: 1, description: '', loggedDate: new Date().toISOString().slice(0, 10) })

async function loadAll() {
  loading.value = true
  error.value = null
  try {
    const [taskRes, subTasksRes, timeRes, commentsRes] = await Promise.allSettled([
      taskApi.getTask(projectId.value, taskId.value),
      taskApi.getSubTasks(projectId.value, taskId.value),
      taskApi.getTimeLogs(projectId.value, taskId.value),
      notifyApi.getComments(projectId.value, taskId.value),
    ])

    if (taskRes.status === 'fulfilled') task.value = taskRes.value.data
    else throw taskRes.reason

    subTasks.value = subTasksRes.status === 'fulfilled' ? subTasksRes.value.data : []
    timeSummary.value = timeRes.status === 'fulfilled' ? timeRes.value.data : null
    comments.value = commentsRes.status === 'fulfilled' ? commentsRes.value.data : []
  } catch (e) {
    error.value = extractErrorMessage(e)
  } finally {
    loading.value = false
  }
}

async function updateStatus(status: KanbanStatusType) {
  try {
    const { data } = await taskApi.updateStatus(projectId.value, taskId.value, status)
    task.value = data
  } catch (e) {
    error.value = extractErrorMessage(e)
  }
}

async function addSubTask() {
  if (!newSubTask.value.trim()) return
  try {
    await taskApi.createSubTask(projectId.value, taskId.value, newSubTask.value.trim())
    newSubTask.value = ''
    const { data } = await taskApi.getSubTasks(projectId.value, taskId.value)
    subTasks.value = data
  } catch (e) {
    error.value = extractErrorMessage(e)
  }
}

async function toggleSubTask(sub: SubTask) {
  try {
    await taskApi.toggleSubTask(projectId.value, taskId.value, sub.id, !sub.isCompleted, sub.title)
    const { data } = await taskApi.getSubTasks(projectId.value, taskId.value)
    subTasks.value = data
  } catch (e) {
    error.value = extractErrorMessage(e)
  }
}

async function addTimeLog() {
  if (!authStore.user?.id) return
  try {
    await taskApi.createTimeLog(projectId.value, taskId.value, {
      userId: authStore.user.id,
      hours: timeForm.value.hours,
      description: timeForm.value.description || undefined,
      loggedDate: new Date(timeForm.value.loggedDate).toISOString(),
    })
    const { data } = await taskApi.getTimeLogs(projectId.value, taskId.value)
    timeSummary.value = data
    timeForm.value.description = ''
  } catch (e) {
    error.value = extractErrorMessage(e)
  }
}

async function addComment() {
  if (!newComment.value.trim()) return
  try {
    await notifyApi.addComment(projectId.value, taskId.value, newComment.value.trim())
    newComment.value = ''
    const { data } = await notifyApi.getComments(projectId.value, taskId.value)
    comments.value = data
  } catch (e) {
    error.value = extractErrorMessage(e)
  }
}

onMounted(loadAll)
</script>

<template>
  <div>
    <div v-if="loading" class="loading-center"><div class="spinner" /></div>

    <template v-else-if="task">
      <RouterLink :to="`/projects/${projectId}/kanban`" class="back-link">Quay lại Kanban</RouterLink>

      <div class="task-header card">
        <div>
          <h1 class="page-title">{{ task.title }}</h1>
          <p class="page-subtitle">{{ task.description || 'Không có mô tả' }}</p>
        </div>
        <div class="badges">
          <span class="badge" :style="{ background: `${PRIORITY_COLORS[task.priority]}22`, color: PRIORITY_COLORS[task.priority] }">
            {{ PRIORITY_LABELS[task.priority] }}
          </span>
          <span class="badge badge-primary">{{ kanbanStatusLabel(task.status) }}</span>
        </div>
      </div>

      <div v-if="error" class="error-banner">{{ error }}</div>

      <div class="task-grid">
        <div class="card panel">
          <h3>Trạng thái</h3>
          <select :value="task.status" class="status-select" @change="updateStatus(Number(($event.target as HTMLSelectElement).value) as KanbanStatusType)">
            <option :value="KanbanStatus.Backlog">Backlog</option>
            <option :value="KanbanStatus.ToDo">To Do</option>
            <option :value="KanbanStatus.InProgress">In Progress</option>
            <option :value="KanbanStatus.Review">Review</option>
            <option :value="KanbanStatus.Done">Done</option>
          </select>
          <dl class="meta-list">
            <dt>Deadline</dt>
            <dd>{{ formatDate(task.deadline) }}</dd>
            <dt>Giờ đã log</dt>
            <dd>{{ timeSummary?.totalHours ?? task.totalLoggedHours }}h</dd>
          </dl>
        </div>

        <div class="card panel">
          <h3>Sub-task</h3>
          <form class="inline-form" @submit.prevent="addSubTask">
            <input v-model="newSubTask" placeholder="Thêm sub-task..." />
            <button class="btn btn-secondary btn-sm" type="submit">Thêm</button>
          </form>
          <ul class="subtask-list">
            <li v-for="sub in subTasks" :key="sub.id">
              <label>
                <input type="checkbox" :checked="sub.isCompleted" @change="toggleSubTask(sub)" />
                <span :class="{ done: sub.isCompleted }">{{ sub.title }}</span>
              </label>
            </li>
          </ul>
        </div>

        <div class="card panel">
          <h3>Log thời gian</h3>
          <form class="time-form" @submit.prevent="addTimeLog">
            <input v-model.number="timeForm.hours" type="number" min="0.25" max="24" step="0.25" />
            <input v-model="timeForm.loggedDate" type="date" />
            <input v-model="timeForm.description" placeholder="Mô tả..." />
            <button class="btn btn-secondary btn-sm" type="submit">Log</button>
          </form>
          <ul v-if="timeSummary?.logs.length" class="timelog-list">
            <li v-for="log in timeSummary.logs" :key="log.id">
              <strong>{{ log.hours }}h</strong>
              <span>{{ log.description || 'Không mô tả' }}</span>
              <small>{{ formatDate(log.loggedDate) }}</small>
            </li>
          </ul>
        </div>

        <div class="card panel">
          <h3>Bình luận</h3>
          <form class="comment-form" @submit.prevent="addComment">
            <textarea v-model="newComment" placeholder="Viết bình luận..." rows="3" />
            <button class="btn btn-primary btn-sm" type="submit">Gửi</button>
          </form>
          <div v-if="comments.length === 0" class="empty-comments">Chưa có bình luận</div>
          <div v-for="comment in comments" :key="comment.id" class="comment-item">
            <strong>{{ comment.userName || comment.userId }}</strong>
            <p>{{ comment.content }}</p>
            <small>{{ formatDateTime(comment.createdAt) }}</small>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.back-link {
  display: inline-block;
  margin-bottom: 0.75rem;
  color: var(--text-muted);
  font-size: 0.875rem;
  font-weight: 800;
}

.task-header {
  display: flex;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1rem;
  background: linear-gradient(135deg, var(--bg-card), var(--primary-soft));
}

.badges {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
  align-items: flex-start;
}

.task-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 1rem;
  align-items: start;
}

.panel h3 {
  margin-bottom: 0.85rem;
}

.status-select {
  width: 100%;
  padding: 0.65rem 0.75rem;
  margin-bottom: 0.85rem;
}

.meta-list {
  display: grid;
  grid-template-columns: auto 1fr;
  gap: 0.45rem 1rem;
}

.meta-list dt,
.timelog-list small,
.empty-comments,
.comment-item small {
  color: var(--text-muted);
}

.inline-form,
.time-form,
.comment-form {
  display: flex;
  gap: 0.55rem;
  margin-bottom: 0.85rem;
  flex-wrap: wrap;
}

.inline-form input,
.time-form input {
  flex: 1;
  min-width: 130px;
  padding: 0.62rem 0.75rem;
}

.comment-form textarea {
  flex: 1 1 100%;
  padding: 0.75rem;
  resize: vertical;
}

.subtask-list,
.timelog-list {
  list-style: none;
  display: grid;
  gap: 0.5rem;
}

.subtask-list li,
.timelog-list li,
.comment-item {
  padding: 0.75rem;
  border: 1px solid var(--border);
  border-radius: 8px;
  background: var(--surface-solid);
}

.subtask-list label {
  display: flex;
  align-items: center;
  gap: 0.55rem;
  cursor: pointer;
}

.subtask-list .done {
  color: var(--text-muted);
  text-decoration: line-through;
}

.timelog-list li {
  display: grid;
  gap: 0.15rem;
}

.comment-item {
  margin-top: 0.65rem;
}

.comment-item p {
  margin: 0.25rem 0;
}

@media (max-width: 900px) {
  .task-header,
  .task-grid {
    grid-template-columns: 1fr;
    flex-direction: column;
  }
}
</style>
