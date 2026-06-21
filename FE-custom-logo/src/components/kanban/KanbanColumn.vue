<script setup lang="ts">
import { ref } from 'vue'
import TaskCard from './TaskCard.vue'
import type { KanbanColumn } from '@/types'

const props = defineProps<{
  column: KanbanColumn
  projectId: string
}>()

const emit = defineEmits<{
  drop: [taskId: string, status: import('@/types').KanbanStatus, orderIndex: number]
  createTask: [status: import('@/types').KanbanStatus]
}>()

const isDragOver = ref(false)

function onDragOver(event: DragEvent) {
  event.preventDefault()
  isDragOver.value = true
}

function onDragLeave() {
  isDragOver.value = false
}

function onDrop(event: DragEvent) {
  event.preventDefault()
  isDragOver.value = false
  const taskId = event.dataTransfer?.getData('text/plain')
  if (!taskId) return
  emit('drop', taskId, props.column.status, props.column.tasks.length)
}
</script>

<template>
  <div
    class="kanban-column card"
    :class="{ 'drag-over': isDragOver }"
    @dragover="onDragOver"
    @dragleave="onDragLeave"
    @drop="onDrop"
  >
    <div class="column-header">
      <div>
        <h3>{{ column.name }}</h3>
        <span>{{ column.tasks.length }} task</span>
      </div>
      <button class="add-btn" title="Thêm task" @click="emit('createTask', column.status)">+</button>
    </div>

    <TransitionGroup name="task-list" tag="div" class="column-tasks">
      <TaskCard
        v-for="task in column.tasks"
        :key="task.id"
        :task="task"
        :project-id="projectId"
      />
      <p v-if="column.tasks.length === 0" key="empty" class="empty-col">Kéo thả task vào đây</p>
    </TransitionGroup>
  </div>
</template>

<style scoped>
.kanban-column {
  min-width: 292px;
  max-width: 340px;
  flex: 1 0 292px;
  display: flex;
  flex-direction: column;
  max-height: calc(100vh - 230px);
  scroll-snap-align: start;
  transition: transform 0.24s var(--ease), border-color 0.24s var(--ease), background 0.24s var(--ease);
}

.kanban-column.drag-over {
  transform: translateY(-4px) scale(1.01);
  border-color: var(--primary);
  background: var(--primary-soft);
}

.column-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  margin-bottom: 0.75rem;
  padding-bottom: 0.75rem;
  border-bottom: 1px solid var(--border);
}

.column-header h3 {
  font-size: 0.98rem;
}

.column-header span {
  color: var(--text-muted);
  font-size: 0.78rem;
  font-weight: 750;
}

.add-btn {
  display: grid;
  place-items: center;
  width: 34px;
  height: 34px;
  border-radius: 8px;
  background: var(--surface-solid);
  color: var(--primary-hover);
  border: 1px solid var(--border);
  cursor: pointer;
  font-size: 1.2rem;
  font-weight: 850;
  transition: transform 0.22s var(--ease), box-shadow 0.22s var(--ease);
}

.add-btn:hover {
  transform: translateY(-2px) rotate(8deg);
  box-shadow: var(--shadow-soft);
}

.column-tasks {
  display: flex;
  flex-direction: column;
  gap: 0.65rem;
  overflow-y: auto;
  flex: 1;
  padding-right: 0.15rem;
}

.empty-col {
  display: grid;
  place-items: center;
  min-height: 120px;
  color: var(--text-muted);
  font-size: 0.86rem;
  text-align: center;
  border: 1px dashed var(--border-strong);
  border-radius: 8px;
  background: var(--surface-solid);
}

.task-list-move,
.task-list-enter-active,
.task-list-leave-active {
  transition: all 0.22s var(--ease);
}

.task-list-enter-from,
.task-list-leave-to {
  opacity: 0;
  transform: translateY(10px) scale(0.98);
}
</style>
