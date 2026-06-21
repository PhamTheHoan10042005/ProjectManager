export const ProjectRole = {
  Owner: 0,
  Manager: 1,
  Member: 2,
  Viewer: 3,
} as const
export type ProjectRole = (typeof ProjectRole)[keyof typeof ProjectRole]

export const KanbanStatus = {
  Backlog: 0,
  ToDo: 1,
  InProgress: 2,
  Review: 3,
  Done: 4,
} as const
export type KanbanStatus = (typeof KanbanStatus)[keyof typeof KanbanStatus]

export const TaskPriority = {
  Low: 0,
  Medium: 1,
  High: 2,
  Critical: 3,
} as const
export type TaskPriority = (typeof TaskPriority)[keyof typeof TaskPriority]

export interface User {
  id: string
  email: string
  fullName: string
  role?: string
}

export interface AuthResponse {
  token: string
  user: User
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
  fullName: string
}

export interface Project {
  id: string
  name: string
  description?: string
  startDate?: string
  endDate?: string
  color?: string
  createdAt?: string
  memberCount?: number
  taskCount?: number
}

export interface CreateProjectRequest {
  name: string
  description?: string
  startDate?: string
  endDate?: string
  color?: string
}

export interface ProjectMember {
  id: string
  userId: string
  projectId: string
  role: ProjectRole
  email?: string
  fullName?: string
}

export interface AddMemberRequest {
  userId: string
  role: ProjectRole
}

export interface Sprint {
  id: string
  projectId: string
  name: string
  goal?: string
  startDate: string
  endDate: string
  isActive: boolean
}

export interface CreateSprintRequest {
  name: string
  goal?: string
  startDate: string
  endDate: string
}

export interface TaskItem {
  id: string
  projectId: string
  sprintId?: string
  title: string
  description?: string
  priority: TaskPriority
  labelColor?: string
  deadline?: string
  status: KanbanStatus
  assigneeId?: string
  orderIndex: number
  createdAt: string
  updatedAt: string
  subTaskCount: number
  totalLoggedHours: number
}

export interface CreateTaskRequest {
  title: string
  description?: string
  priority?: TaskPriority
  labelColor?: string
  deadline?: string
  status?: KanbanStatus
  sprintId?: string
  assigneeId?: string
}

export interface UpdateTaskRequest {
  title: string
  description?: string
  priority: TaskPriority
  labelColor?: string
  deadline?: string
  sprintId?: string
}

export interface MoveTaskRequest {
  status: KanbanStatus
  orderIndex: number
}

export interface KanbanColumn {
  status: KanbanStatus
  name: string
  tasks: TaskItem[]
}

export interface KanbanBoard {
  projectId: string
  sprintId?: string
  columns: KanbanColumn[]
}

export interface SubTask {
  id: string
  taskId: string
  title: string
  isCompleted: boolean
  orderIndex: number
}

export interface TimeLog {
  id: string
  taskId: string
  userId: string
  hours: number
  description?: string
  loggedDate: string
  createdAt: string
}

export interface TaskTimeSummary {
  taskId: string
  totalHours: number
  logs: TimeLog[]
}

export interface Comment {
  id: string
  taskId: string
  userId: string
  userName?: string
  content: string
  createdAt: string
}

export interface Notification {
  id: string
  userId: string
  title: string
  message: string
  type?: string
  isRead: boolean
  createdAt: string
  relatedTaskId?: string
  relatedProjectId?: string
}

export interface ActivityLog {
  id: string
  projectId: string
  userId: string
  userName?: string
  action: string
  description: string
  createdAt: string
}
