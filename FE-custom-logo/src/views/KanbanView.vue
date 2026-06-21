<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { taskApi } from '@/api/task.api'
import { projectApi } from '@/api/project.api'
import { extractErrorMessage } from '@/api/http'
import KanbanColumn from '@/components/kanban/KanbanColumn.vue'
import { KanbanStatus, TaskPriority, type KanbanBoard, type KanbanStatus as KanbanStatusType, type Project, type Sprint } from '@/types'

const props = defineProps<{ projectId: string }>()
const route = useRoute()
const id = computed(() => props.projectId || String(route.params.projectId))

const board = ref<KanbanBoard | null>(null)
const project = ref<Project | null>(null)
const sprints = ref<Sprint[]>([])
const selectedSprintId = ref<string>('')
const loading = ref(true)
const error = ref<string | null>(null)
const showTaskModal = ref(false)
const createStatus = ref<KanbanStatusType>(KanbanStatus.Backlog)

const taskForm = ref({
  title: '',
  description: '',
  priority: TaskPriority.Medium,
  labelColor: '#2457ff',
  deadline: '',
})

async function loadBoard() {
  loading.value = true
  error.value = null
  try {
    const sprintId = selectedSprintId.value || undefined
    const [boardRes, projectRes, sprintsRes] = await Promise.all([
      taskApi.getKanban(id.value, sprintId),
      projectApi.getById(id.value),
      projectApi.getSprints(id.value),
    ])
    board.value = boardRes.data
    project.value = projectRes.data
    sprints.value = sprintsRes.data
  } catch (e) {
    error.value = extractErrorMessage(e)
  } finally {
    loading.value = false
  }
}

async function onDrop(taskId: string, status: KanbanStatusType, orderIndex: number) {
  try {
    await taskApi.moveTask(id.value, taskId, { status, orderIndex })
    await loadBoard()
  } catch (e) {
    error.value = extractErrorMessage(e)
  }
}

function openCreateTask(status: KanbanStatusType) {
  createStatus.value = status
  showTaskModal.value = true
}

async function createTask() {
  try {
    await taskApi.createTask(id.value, {
      title: taskForm.value.title,
      description: taskForm.value.description || undefined,
      priority: taskForm.value.priority,
      labelColor: taskForm.value.labelColor,
      deadline: taskForm.value.deadline || undefined,
      status: createStatus.value,
      sprintId: selectedSprintId.value || undefined,
    })
    showTaskModal.value = false
    taskForm.value = { title: '', description: '', priority: TaskPriority.Medium, labelColor: '#2457ff', deadline: '' }
    await loadBoard()
  } catch (e) {
    error.value = extractErrorMessage(e)
  }
}

watch(selectedSprintId, loadBoard)
onMounted(loadBoard)
</script>

<template>
  <div>
    <div class="page-header">
      <div>
        <RouterLink :to="`/projects/${id}`" class="back-link">Quay lại {{ project?.name || 'Dự án' }}</RouterLink>
        <h1 class="page-title">Kanban Board</h1>
        <p class="page-subtitle">Task & Kanban Service (N2) - Backlog, To Do, In Progress, Review, Done.</p>
      </div>
      <div class="filters">
        <select v-model="selectedSprintId" class="sprint-select">
          <option value="">Tất cả sprint</option>
          <option v-for="s in sprints" :key="s.id" :value="s.id">
            {{ s.name }}{{ s.isActive ? ' (active)' : '' }}
          </option>
        </select>
        <button class="btn btn-primary" @click="openCreateTask(KanbanStatus.Backlog)">Task mới</button>
      </div>
    </div>

    <div v-if="error" class="error-banner">{{ error }}</div>
    <div v-if="loading" class="loading-center"><div class="spinner" /></div>

    <div v-else-if="board" class="kanban-board">
      <KanbanColumn
        v-for="column in board.columns"
        :key="column.status"
        :column="column"
        :project-id="id"
        @drop="onDrop"
        @create-task="openCreateTask"
      />
    </div>

    <div v-if="showTaskModal" class="modal-overlay" @click.self="showTaskModal = false">
      <div class="modal card">
        <div class="modal-header">
          <h2>Tạo task mới</h2>
          <button type="button" class="btn btn-ghost btn-sm" @click="showTaskModal = false">Đóng</button>
        </div>
        <form @submit.prevent="createTask">
          <div class="form-group">
            <label>Tiêu đề</label>
            <input v-model="taskForm.title" required />
          </div>
          <div class="form-group">
            <label>Mô tả</label>
            <textarea v-model="taskForm.description" />
          </div>
          <div class="grid-2">
            <div class="form-group">
              <label>Độ ưu tiên</label>
              <select v-model.number="taskForm.priority">
                <option :value="TaskPriority.Low">Thấp</option>
                <option :value="TaskPriority.Medium">Trung bình</option>
                <option :value="TaskPriority.High">Cao</option>
                <option :value="TaskPriority.Critical">Khẩn cấp</option>
              </select>
            </div>
            <div class="form-group">
              <label>Deadline</label>
              <input v-model="taskForm.deadline" type="date" />
            </div>
          </div>
          <div class="form-group">
            <label>Nhãn màu</label>
            <input v-model="taskForm.labelColor" type="color" />
          </div>
          <div class="modal-actions">
            <button type="button" class="btn btn-secondary" @click="showTaskModal = false">Hủy</button>
            <button type="submit" class="btn btn-primary">Tạo task</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.back-link {
  color: var(--text-muted);
  font-size: 0.875rem;
  font-weight: 750;
  display: inline-block;
  margin-bottom: 0.5rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
  margin-bottom: 1rem;
}

.filters {
  display: flex;
  gap: 0.75rem;
  align-items: center;
}

.sprint-select {
  min-width: 210px;
  padding: 0.62rem 0.8rem;
  border-radius: 8px;
  border: 1px solid var(--border);
  background: var(--surface-solid);
  color: var(--text);
}

.kanban-board {
  display: flex;
  gap: 1rem;
  overflow-x: auto;
  min-height: calc(100vh - 230px);
  padding: 0.1rem 0.15rem 1rem;
  scroll-snap-type: x proximity;
}

.kanban-board::-webkit-scrollbar {
  height: 10px;
}

.kanban-board::-webkit-scrollbar-thumb {
  border-radius: 999px;
  background: var(--border-strong);
}

@media (max-width: 760px) {
  .page-header,
  .filters {
    flex-direction: column;
    align-items: stretch;
  }
}
</style>
