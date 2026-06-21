<script setup lang="ts">
import { onMounted } from 'vue'
import { useNotificationStore } from '@/stores/notification.store'
import { formatDateTime } from '@/utils/date'

const store = useNotificationStore()

onMounted(() => store.fetchNotifications())
</script>

<template>
  <div>
    <div class="page-header">
      <div>
        <span class="eyebrow">Notify Service</span>
        <h1 class="page-title">Thông báo</h1>
        <p class="page-subtitle">Theo dõi comment, activity và cập nhật mới từ dự án.</p>
      </div>
      <button v-if="store.unreadCount > 0" class="btn btn-secondary" @click="store.markAllRead()">
        Đánh dấu tất cả đã đọc
      </button>
    </div>

    <div v-if="store.loading" class="loading-center"><div class="spinner" /></div>

    <div v-else-if="store.items.length === 0" class="card empty-state">
      Không có thông báo nào.
    </div>

    <div v-else class="notif-list">
      <div
        v-for="item in store.items"
        :key="item.id"
        class="notif-item card"
        :class="{ unread: !item.isRead }"
        @click="!item.isRead && store.markRead(item.id)"
      >
        <div class="notif-content">
          <strong>{{ item.title }}</strong>
          <p>{{ item.message }}</p>
          <small>{{ formatDateTime(item.createdAt) }}</small>
        </div>
        <span v-if="!item.isRead" class="unread-dot" />
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  gap: 1rem;
  margin-bottom: 1rem;
}

.eyebrow {
  display: inline-block;
  margin-bottom: 0.6rem;
  color: var(--primary-hover);
  font-size: 0.78rem;
  font-weight: 850;
  text-transform: uppercase;
}

.notif-list {
  display: grid;
  gap: 0.75rem;
}

.notif-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  cursor: pointer;
  transition: transform 0.22s var(--ease), border-color 0.22s var(--ease), box-shadow 0.22s var(--ease);
}

.notif-item:hover {
  transform: translateY(-3px);
  box-shadow: var(--shadow);
}

.notif-item.unread {
  border-color: var(--primary);
  background: linear-gradient(135deg, var(--primary-soft), var(--bg-card));
}

.notif-content p {
  color: var(--text-muted);
  margin: 0.25rem 0;
}

.notif-content small {
  color: var(--text-muted);
  font-size: 0.78rem;
  font-weight: 750;
}

.unread-dot {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background: var(--primary);
  box-shadow: 0 0 0 6px var(--primary-soft);
  flex-shrink: 0;
}

@media (max-width: 720px) {
  .page-header {
    align-items: stretch;
    flex-direction: column;
  }
}
</style>
