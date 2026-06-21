<script setup lang="ts">
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.store'

const authStore = useAuthStore()

onMounted(() => {
  const savedTheme = localStorage.getItem('theme')
  const prefersDark = window.matchMedia?.('(prefers-color-scheme: dark)').matches
  document.documentElement.dataset.theme = savedTheme === 'dark' || (!savedTheme && prefersDark) ? 'dark' : 'light'
  authStore.fetchMe()
})
</script>

<template>
  <RouterView />
</template>
