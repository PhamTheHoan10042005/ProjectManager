<script setup lang="ts">
import { PRIORITY_COLORS, PRIORITY_LABELS } from '@/utils/constants'
import { formatDate, isOverdue } from '@/utils/date'
import type { TaskItem } from '@/types'

defineProps<{
  task: TaskItem
  projectId: string
}>()

const emit = defineEmits<{
  dragStart: [taskId: string]
}>()

function onDragStart(event: DragEvent, taskId: string) {
  event.dataTransfer?.setData('text/plain', taskId)
  event.dataTransfer!.effectAllowed = 'move'
  emit('dragStart', taskId)
}
</script>

<template>
  <div
    class="task-card"
    draggable="true"
    :style="{ '--accent': task.labelColor || PRIORITY_COLORS[task.priority] }"
    @dragstart="onDragStart($event, task.id)"
  >
    <RouterLink :to="`/projects/${projectId}/tasks/${task.id}`" class="task-link">
      <span class="accent-bar" />
      <h4>{{ task.title }}</h4>
      <div class="task-meta">
        <span class="badge" :style="{ background: `${PRIORITY_COLORS[task.priority]}22`, color: PRIORITY_COLORS[task.priority] }">
          {{ PRIORITY_LABELS[task.priority] }}
        </span>
        <span v-if="task.deadline" class="deadline" :class="{ overdue: isOverdue(task.deadline) }">
          {{ formatDate(task.deadline) }}
        </span>
      </div>
      <div v-if="task.subTaskCount || task.totalLoggedHours" class="task-stats">
        <span v-if="task.subTaskCount">{{ task.subTaskCount }} sub-task</span>
        <span v-if="task.totalLoggedHours">{{ task.totalLoggedHours }}h</span>
      </div>
    </RouterLink>
  </div>
</template>

<style scoped>
.task-card {
  --accent: var(--primary);
  position: relative;
  overflow: hidden;
  border: 1px solid var(--border);
  border-radius: 8px;
  background: var(--surface-solid);
  cursor: grab;
  transition: transform 0.22s var(--ease), box-shadow 0.22s var(--ease), border-color 0.22s var(--ease);
}

.task-card:hover {
  transform: translateY(-3px);
  border-color: var(--border-strong);
  box-shadow: var(--shadow-soft);
}

.task-card:active {
  cursor: grabbing;
  transform: rotate(1deg) scale(0.99);
}

.task-link {
  display: block;
  padding: 0.9rem;
}

.accent-bar {
  display: block;
  width: 54px;
  height: 5px;
  margin-bottom: 0.75rem;
  border-radius: 999px;
  background: var(--accent);
}

.task-link h4 {
  margin-bottom: 0.62rem;
  font-size: 0.96rem;
  line-height: 1.3;
}

.task-meta {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.deadline {
  color: var(--text-muted);
  font-size: 0.76rem;
  font-weight: 750;
}

.deadline.overdue {
  color: var(--danger);
}

.task-stats {
  display: flex;
  gap: 0.75rem;
  margin-top: 0.65rem;
  color: var(--text-muted);
  font-size: 0.76rem;
  font-weight: 750;
}
</style>
