import { taskHttp } from './http'
import { toServiceProjectId, toServiceSprintId } from '@/utils/projectId'
import type {
  CreateTaskRequest,
  KanbanBoard,
  MoveTaskRequest,
  SubTask,
  TaskItem,
  TaskTimeSummary,
  UpdateTaskRequest,
} from '@/types'

function pid(projectId: string) {
  return toServiceProjectId(projectId)
}

function sid(sprintId: string) {
  return toServiceSprintId(sprintId)
}

export const taskApi = {
  getTasks(projectId: string, sprintId?: string) {
    return taskHttp.get<TaskItem[]>(`/api/projects/${pid(projectId)}/tasks`, {
      params: sprintId ? { sprintId: sid(sprintId) } : undefined,
    })
  },

  getTask(projectId: string, taskId: string) {
    return taskHttp.get<TaskItem>(`/api/projects/${pid(projectId)}/tasks/${taskId}`)
  },

  createTask(projectId: string, data: CreateTaskRequest) {
    const payload = { ...data }
    if (payload.sprintId) payload.sprintId = sid(payload.sprintId)
    return taskHttp.post<TaskItem>(`/api/projects/${pid(projectId)}/tasks`, payload)
  },

  updateTask(projectId: string, taskId: string, data: UpdateTaskRequest) {
    const payload = { ...data }
    if (payload.sprintId) payload.sprintId = sid(payload.sprintId)
    return taskHttp.put<TaskItem>(`/api/projects/${pid(projectId)}/tasks/${taskId}`, payload)
  },

  assignTask(projectId: string, taskId: string, assigneeId: string) {
    return taskHttp.patch<TaskItem>(`/api/projects/${pid(projectId)}/tasks/${taskId}/assign`, { assigneeId })
  },

  updateStatus(projectId: string, taskId: string, status: number, orderIndex?: number) {
    return taskHttp.patch<TaskItem>(`/api/projects/${pid(projectId)}/tasks/${taskId}/status`, {
      status,
      orderIndex,
    })
  },

  moveTask(projectId: string, taskId: string, data: MoveTaskRequest) {
    return taskHttp.patch<TaskItem>(`/api/projects/${pid(projectId)}/tasks/${taskId}/move`, data)
  },

  deleteTask(projectId: string, taskId: string) {
    return taskHttp.delete(`/api/projects/${pid(projectId)}/tasks/${taskId}`)
  },

  getKanban(projectId: string, sprintId?: string) {
    return taskHttp.get<KanbanBoard>(`/api/projects/${pid(projectId)}/kanban`, {
      params: sprintId ? { sprintId: sid(sprintId) } : undefined,
    })
  },

  getSubTasks(projectId: string, taskId: string) {
    return taskHttp.get<SubTask[]>(`/api/projects/${pid(projectId)}/tasks/${taskId}/subtasks`)
  },

  createSubTask(projectId: string, taskId: string, title: string) {
    return taskHttp.post<SubTask>(`/api/projects/${pid(projectId)}/tasks/${taskId}/subtasks`, { title })
  },

  toggleSubTask(projectId: string, taskId: string, subTaskId: string, isCompleted: boolean, title: string) {
    return taskHttp.put<SubTask>(`/api/projects/${pid(projectId)}/tasks/${taskId}/subtasks/${subTaskId}`, {
      title,
      isCompleted,
    })
  },

  deleteSubTask(projectId: string, taskId: string, subTaskId: string) {
    return taskHttp.delete(`/api/projects/${pid(projectId)}/tasks/${taskId}/subtasks/${subTaskId}`)
  },

  getTimeLogs(projectId: string, taskId: string) {
    return taskHttp.get<TaskTimeSummary>(`/api/projects/${pid(projectId)}/tasks/${taskId}/timelogs`)
  },

  createTimeLog(
    projectId: string,
    taskId: string,
    payload: { userId: string; hours: number; description?: string; loggedDate: string },
  ) {
    return taskHttp.post(`/api/projects/${pid(projectId)}/tasks/${taskId}/timelogs`, payload)
  },

  deleteTimeLog(projectId: string, taskId: string, timeLogId: string) {
    return taskHttp.delete(`/api/projects/${pid(projectId)}/tasks/${taskId}/timelogs/${timeLogId}`)
  },
}
