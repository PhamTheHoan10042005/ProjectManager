import { defineConfig, loadEnv, type ProxyOptions } from 'vite'
import vue from '@vitejs/plugin-vue'
import { fileURLToPath, URL } from 'node:url'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const useGateway = env.VITE_USE_GATEWAY === 'false'
  const projectTarget = env.VITE_PROJECT_API || 'http://localhost:5116'
  const taskTarget = env.VITE_TASK_API || 'http://localhost:5027'
  
  // ĐÃ SỬA: Đổi từ 7159 thành 5286 cho đúng cổng Swagger Visual Studio của Hải
// Sửa 'http://localhost:5286' thành 'http://127.0.0.1:5286'
const notifyTarget = env.VITE_NOTIFY_API || 'http://127.0.0.1:5286'
  const gatewayTarget = env.VITE_GATEWAY_API || 'http://localhost:5114'

  const proxy: Record<string, string | ProxyOptions> = useGateway
    ? {
        '/api': {
          target: gatewayTarget,
          changeOrigin: true,
        },
      }
    : {
        '/project-api': {
          target: projectTarget,
          changeOrigin: true,
          rewrite: (path: string) => path.replace(/^\/project-api/, ''),
        },
        '/task-api': {
          target: taskTarget,
          changeOrigin: true,
          rewrite: (path: string) => path.replace(/^\/task-api/, ''),
        },
        '/notify-api': {
          target: notifyTarget,
          changeOrigin: true,
          rewrite: (path: string) => path.replace(/^\/notify-api/, ''), 
        },
      }

  return {
    plugins: [vue()],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
      },
    },
    server: {
      port: 5173,
      host: true,
      proxy,
    },
    preview: {
      port: 5173,
      host: true,
    },
  }
})