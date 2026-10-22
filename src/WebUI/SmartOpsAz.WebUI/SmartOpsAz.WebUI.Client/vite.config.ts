import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  },
  server: {
    proxy: {
      // Identity Service proxy
      '/identity-api': {
        target: 'http://localhost:5101',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/identity-api/, ''),
        secure: false
      },
      // Inventory Service proxy
      '/inventory-api': {
        target: 'http://localhost:5102',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/inventory-api/, ''),
        secure: false
      },
      // Device Service proxy
      '/device-api': {
        target: 'http://localhost:5103',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/device-api/, ''),
        secure: false
      },
      // Dashboard Service proxy
      '/dashboard-api': {
        target: 'http://localhost:5104',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/dashboard-api/, ''),
        secure: false
      }
    }
  }
})