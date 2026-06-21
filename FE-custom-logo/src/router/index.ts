import { createRouter, createWebHistory } from 'vue-router'
import { getToken } from '@/api/http'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue'),
      meta: { guest: true },
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/views/RegisterView.vue'),
      meta: { guest: true },
    },
    {
      path: '/',
      component: () => import('@/components/layout/AppLayout.vue'),
      meta: { requiresAuth: true },
      children: [
        {
          path: '',
          name: 'home',
          component: () => import('@/views/HomeView.vue'),
        },
        {
          path: 'projects',
          name: 'projects',
          component: () => import('@/views/ProjectsView.vue'),
        },
        {
          path: 'projects/:projectId',
          name: 'project-detail',
          component: () => import('@/views/ProjectDetailView.vue'),
          props: true,
        },
        {
          path: 'projects/:projectId/kanban',
          name: 'kanban',
          component: () => import('@/views/KanbanView.vue'),
          props: true,
        },
        {
          path: 'projects/:projectId/tasks/:taskId',
          name: 'task-detail',
          component: () => import('@/views/TaskDetailView.vue'),
          props: true,
        },
        {
          path: 'notifications',
          name: 'notifications',
          component: () => import('@/views/NotificationsView.vue'),
        },
        {
          path: 'admin',
          name: 'admin',
          component: () => import('@/views/AdminView.vue'),
          meta: { requiresAdmin: true },
        },
      ],
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/',
    },
  ],
})

router.beforeEach(async (to) => {
  const hasToken = !!getToken()

  if (to.meta.requiresAuth && !hasToken) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  if (to.meta.guest && hasToken) {
    return { name: 'home' }
  }

  if (to.meta.requiresAdmin && hasToken) {
    const { useAuthStore } = await import('@/stores/auth.store')
    const authStore = useAuthStore()
    if (!authStore.user) await authStore.fetchMe()
    if (!authStore.isAdmin) return { name: 'home' }
  }

  return true
})

export default router
