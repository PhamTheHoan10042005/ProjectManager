import { notifyHttp } from './http'
import { toServiceProjectId } from '@/utils/projectId'
import type { ActivityLog, Comment, Notification } from '@/types'

function pid(projectId: string) {
  return toServiceProjectId(projectId)
}

export const notifyApi = {
  getComments(projectId: string, taskId: string) {
    return notifyHttp.get<Comment[]>(`/api/projects/${pid(projectId)}/tasks/${taskId}/comments`)
  },

  addComment(projectId: string, taskId: string, content: string) {
    return notifyHttp.post<Comment>(`/api/projects/${pid(projectId)}/tasks/${taskId}/comments`, { content })
  },

  getNotifications(unreadOnly = false) {
    return notifyHttp.get<Notification[]>('/api/notifications', {
      params: { unreadOnly },
    })
  },

  markAsRead(id: string) {
    return notifyHttp.patch<Notification>(`/api/notifications/${id}/read`)
  },

  markAllAsRead() {
    return notifyHttp.post('/api/notifications/read-all')
  },

  getActivities(projectId: string) {
    return notifyHttp.get<ActivityLog[]>(`/api/projects/${pid(projectId)}/activities`)
  },
}
