import React from 'react'
import { createBrowserRouter } from 'react-router-dom'
import { ProtectedRoute } from '../components/common/ProtectedRoute'
import { MainLayout } from '../components/layout/MainLayout'
import { LoginPage } from '../features/auth/pages/LoginPage' 

// Lazy load page components for better performance
const DashboardPage = React.lazy(
  () => import('../features/dashboard/pages/DashboardPage').then(m => ({ default: m.DashboardPage }))
)
const InventoryPage = React.lazy(
  () => import('../features/inventory/pages/InventoryListPage').then(m => ({ default: m.InventoryListPage }))
)
const InventoryRegisterPage = React.lazy(
    () => import('../features/inventory/pages/InventoryRegisterPage').then(m => ({ default: m.InventoryRegisterPage }))
)
const InventoryDetailPage = React.lazy(
    () => import('../features/inventory/pages/InventoryDetailPage').then(m => ({ default: m.InventoryDetailPage }))
)
const InventoryEditPage = React.lazy(
    () => import('../features/inventory/pages/InventoryEditPage').then(m => ({ default: m.InventoryEditPage }))
)
const DevicesPage = React.lazy(
  () => import('../features/devices/pages/DevicesListPage').then(m => ({ default: m.DevicesListPage }))
)

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />
  },
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <MainLayout>
          <React.Suspense fallback={<div>Loading...</div>}>
            <DashboardPage />
          </React.Suspense>
        </MainLayout>
      </ProtectedRoute>
    )
  },
  {
    path: '/dashboard',
    element: (
      <ProtectedRoute>
        <MainLayout>
          <React.Suspense fallback={<div>Loading...</div>}>
            <DashboardPage />
          </React.Suspense>
        </MainLayout>
      </ProtectedRoute>
    )
  },
  {
    path: '/inventory',
    element: (
      <ProtectedRoute>
        <MainLayout>
          <React.Suspense fallback={<div>Loading...</div>}>
            <InventoryPage />
          </React.Suspense>
        </MainLayout>
      </ProtectedRoute>
    )
  },
  {
    path: 'inventory/register',
    element: <InventoryRegisterPage />
  },
  {
    path: 'inventory/:id',
    element: <InventoryDetailPage />
  },
  {
    path: 'inventory/:id/edit',
    element: <InventoryEditPage />
  },
  {
    path: '/devices',
    element: (
      <ProtectedRoute>
        <MainLayout>
          <React.Suspense fallback={<div>Loading...</div>}>
            <DevicesPage />
          </React.Suspense>
        </MainLayout>
      </ProtectedRoute>
    )
  }
])