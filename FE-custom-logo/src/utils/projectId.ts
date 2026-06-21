/** Chuyển ID số (Project Service) sang Guid (Task/Notify Service). */
export function projectIdToGuid(projectId: string | number): string {
  const n = Number(projectId)
  const hex = n.toString(16).padStart(12, '0')
  return `00000000-0000-0000-0001-${hex}`
}

export function sprintIdToGuid(sprintId: string | number): string {
  const n = Number(sprintId)
  const hex = n.toString(16).padStart(12, '0')
  return `00000000-0000-0000-0002-${hex}`
}

export function toServiceProjectId(projectId: string | number): string {
  if (String(projectId).includes('-')) return String(projectId)
  return projectIdToGuid(projectId)
}

export function toServiceSprintId(sprintId: string | number): string {
  if (String(sprintId).includes('-')) return String(sprintId)
  return sprintIdToGuid(sprintId)
}
