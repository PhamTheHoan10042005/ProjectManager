import { projectHttp } from './http'
import type {
  AddMemberRequest,
  CreateProjectRequest,
  CreateSprintRequest,
  Project,
  ProjectMember,
  Sprint,
} from '@/types'

export const projectApi = {
  getAll() {
    return projectHttp.get<Project[]>('/api/projects')
  },

  getById(id: string) {
    return projectHttp.get<Project>(`/api/projects/${id}`)
  },

  create(data: CreateProjectRequest) {
    return projectHttp.post<Project>('/api/projects', data)
  },

  update(id: string, data: CreateProjectRequest) {
    return projectHttp.put<Project>(`/api/projects/${id}`, data)
  },

  remove(id: string) {
    return projectHttp.delete(`/api/projects/${id}`)
  },

  getMembers(projectId: string) {
    return projectHttp.get<ProjectMember[]>(`/api/projects/${projectId}/members`)
  },

  addMember(projectId: string, data: AddMemberRequest) {
    return projectHttp.post<ProjectMember>(`/api/projects/${projectId}/members`, data)
  },

  updateMember(projectId: string, memberId: string, role: number) {
    return projectHttp.put<ProjectMember>(`/api/projects/${projectId}/members/${memberId}`, { role })
  },

  removeMember(projectId: string, memberId: string) {
    return projectHttp.delete(`/api/projects/${projectId}/members/${memberId}`)
  },

  getSprints(projectId: string) {
    return projectHttp.get<Sprint[]>(`/api/projects/${projectId}/sprints`)
  },

  createSprint(projectId: string, data: CreateSprintRequest) {
    return projectHttp.post<Sprint>(`/api/projects/${projectId}/sprints`, data)
  },

  startSprint(projectId: string, sprintId: string) {
    return projectHttp.post<Sprint>(`/api/projects/${projectId}/sprints/${sprintId}/start`)
  },
}
