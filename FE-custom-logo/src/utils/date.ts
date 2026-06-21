export function formatDate(value?: string | null): string {
  if (!value) return '—'
  return new Date(value).toLocaleDateString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
}

export function formatDateTime(value?: string | null): string {
  if (!value) return '—'
  return new Date(value).toLocaleString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

export function toInputDate(value?: string | null): string {
  if (!value) return ''
  return value.slice(0, 10)
}

export function isOverdue(deadline?: string | null): boolean {
  if (!deadline) return false
  return new Date(deadline) < new Date()
}
