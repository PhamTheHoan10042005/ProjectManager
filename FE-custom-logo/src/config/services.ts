const useGateway = import.meta.env.VITE_USE_GATEWAY === 'true'

function resolveBaseUrl(serviceUrl: string | undefined, prefix: string | undefined): string {
  if (useGateway) {
    return import.meta.env.VITE_GATEWAY_API || ''
  }

  if (import.meta.env.DEV && prefix) {
    return prefix
  }

  return serviceUrl || ''
}

export const API_BASE = {
  project: resolveBaseUrl(import.meta.env.VITE_PROJECT_API, import.meta.env.VITE_PROJECT_PREFIX),
  task: resolveBaseUrl(import.meta.env.VITE_TASK_API, import.meta.env.VITE_TASK_PREFIX),
  notify: resolveBaseUrl(import.meta.env.VITE_NOTIFY_API, import.meta.env.VITE_NOTIFY_PREFIX),
}

export const SERVICE_PORTS = {
  project: 5116,
  task: 5027,
  notify: 5286,
  gateway: 5114,
} as const
