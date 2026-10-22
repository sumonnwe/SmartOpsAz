import React from 'react'
import {
  Drawer,
  List, 
  ListItemIcon,
    ListItemText,
ListItemButton,
Divider,
  useMediaQuery,
  useTheme,
  Box,
  Typography
} from '@mui/material'
import { useNavigate, useLocation } from 'react-router-dom'
import DashboardIcon from '@mui/icons-material/Dashboard'
import InventoryIcon from '@mui/icons-material/Inventory2'
import DeviceIcon from '@mui/icons-material/Devices'
import { useAuth } from '../../features/auth/context/AuthContext'

// Temporary compatibility fix for wrapped icon imports
const DashboardIconComponent =
    (DashboardIcon as any).default ?? DashboardIcon

const InventoryIconComponent =
    (InventoryIcon as any).default ?? InventoryIcon

const DeviceIconComponent =
    (DeviceIcon as any).default ?? DeviceIcon


interface SidebarProps {
  open: boolean
  onClose?: () => void
}

export const Sidebar: React.FC<SidebarProps> = ({ open, onClose }) => {
  const navigate = useNavigate()
  const location = useLocation()
  const theme = useTheme()
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'))
  const { hasRole } = useAuth()
  
    const menuItems = [
        {
            label: 'Dashboard',
            icon: <DashboardIconComponent />,
            path: '/dashboard',
            roles: undefined
        },
        {
            label: 'Inventory',
            icon: <InventoryIconComponent />,
            path: '/inventory',
            roles: undefined
        },
        {
            label: 'Devices',
            icon: <DeviceIconComponent />,
            path: '/devices',
            roles: undefined
        }
    ]

  const handleNavigate = (path: string) => {
    navigate(path)
    if (isMobile && onClose) {
      onClose()
    }
  }

  const isActive = (path: string) => location.pathname.startsWith(path)

  const drawer = (
    <Box sx={{ width: 260 }}>
      <Box sx={{ p: 2 }}>
        <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
          SmartOps
        </Typography>
      </Box>
      <Divider />
      <List>
        {menuItems
          .filter((item) => !item.roles || hasRole(item.roles))
          .map((item) => (
              <ListItemButton
                  key={item.path}
                  onClick={() => handleNavigate(item.path)}
                  selected={isActive(item.path)}
              sx={{
                backgroundColor: isActive(item.path)
                  ? 'action.selected'
                  : 'transparent',
                '&:hover': {
                  backgroundColor: 'action.hover'
                }
              }}
            >
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.label} />
            </ListItemButton>
          ))}
      </List>
    </Box>
    )
     

  if (isMobile) {
    return (
      <Drawer anchor="left" open={open} onClose={onClose}>
        {drawer}
      </Drawer>
    )
  }

  return (
    <Drawer
      variant="permanent"
      sx={{
        width: 260,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: 260,
          boxSizing: 'border-box'
        }
      }}
    >
      {drawer}
    </Drawer>
  )
}