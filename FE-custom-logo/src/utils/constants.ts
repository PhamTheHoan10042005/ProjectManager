import {
  KanbanStatus,
  ProjectRole,
  TaskPriority,
  type KanbanStatus as KanbanStatusType,
  type ProjectRole as ProjectRoleType,
  type TaskPriority as TaskPriorityType,
} from '@/types'

export const KANBAN_COLUMNS: { status: KanbanStatusType; label: string }[] = [
  { status: KanbanStatus.Backlog, label: 'Backlog' },
  { status: KanbanStatus.ToDo, label: 'To Do' },
  { status: KanbanStatus.InProgress, label: 'In Progress' },
  { status: KanbanStatus.Review, label: 'Review' },
  { status: KanbanStatus.Done, label: 'Done' },
]

export const PRIORITY_LABELS: Record<TaskPriorityType, string> = {
  [TaskPriority.Low]: 'Thấp',
  [TaskPriority.Medium]: 'Trung bình',
  [TaskPriority.High]: 'Cao',
  [TaskPriority.Critical]: 'Khẩn cấp',
}

export const PRIORITY_COLORS: Record<TaskPriorityType, string> = {
  [TaskPriority.Low]: '#64748b',
  [TaskPriority.Medium]: '#2457ff',
  [TaskPriority.High]: '#d97706',
  [TaskPriority.Critical]: '#dc2626',
}

export const ROLE_LABELS: Record<ProjectRoleType, string> = {
  [ProjectRole.Owner]: 'Owner',
  [ProjectRole.Manager]: 'Project Manager',
  [ProjectRole.Member]: 'Member',
  [ProjectRole.Viewer]: 'Viewer',
}

export const GLOBAL_ROLE_LABELS: Record<string, string> = {
  Admin: 'Quản trị viên',
  ProjectManager: 'Project Manager',
  Member: 'Member',
  Viewer: 'Viewer',
}

export const GLOBAL_ROLES = ['Admin', 'ProjectManager', 'Member', 'Viewer'] as const

export const DEFAULT_PROJECT_COLORS = [
  '#2457ff',
  '#14b8a6',
  '#8b5cf6',
  '#ec4899',
  '#f43f5e',
  '#f97316',
  '#22c55e',
  '#0ea5e9',
]

export function kanbanStatusLabel(status: KanbanStatusType): string {
  return KANBAN_COLUMNS.find((c) => c.status === status)?.label ?? String(status)
}
