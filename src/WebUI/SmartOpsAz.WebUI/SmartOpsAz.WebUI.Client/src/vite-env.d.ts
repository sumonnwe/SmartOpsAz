/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_IDENTITY_API_URL?: string
  readonly VITE_INVENTORY_API_URL?: string
  readonly VITE_DEVICE_API_URL?: string
  readonly VITE_DASHBOARD_API_URL?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}