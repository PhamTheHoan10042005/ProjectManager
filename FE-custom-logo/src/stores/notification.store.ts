import { defineStore } from 'pinia'
import { ref } from 'vue'
import { notifyApi } from '@/api/notify.api'
import type { Notification } from '@/types'

export const useNotificationStore = defineStore('notification', () => {
  const items = ref<Notification[]>([])
  const loading = ref(false)

  const unreadCount = ref(0)

  async function fetchNotifications(unreadOnly = false) {
    loading.value = true
    try {
      const { data } = await notifyApi.getNotifications(unreadOnly)
      items.value = data
      unreadCount.value = data.filter((n) => !n.isRead).length
    } catch {
      items.value = []
      unreadCount.value = 0
    } finally {
      loading.value = false
    }
  }

  async function markRead(id: string) {
    await notifyApi.markAsRead(id)
    const item = items.value.find((n) => n.id === id)
    if (item && !item.isRead) {
      item.isRead = true
      unreadCount.value = Math.max(0, unreadCount.value - 1)
    }
  }

  async function markAllRead() {
    await notifyApi.markAllAsRead()
    items.value.forEach((n) => {
      n.isRead = true
    })
    unreadCount.value = 0
  }

  return {
    items,
    loading,
    unreadCount,
    fetchNotifications,
    markRead,
    markAllRead,
  }
})
