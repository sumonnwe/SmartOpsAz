import React from 'react'
import {
    AppBar,
    Toolbar,
    Typography,
    IconButton,
    Menu,
    MenuItem,
    Box,
    Chip
} from '@mui/material'
import { useNavigate } from 'react-router-dom'
import MenuIcon from '@mui/icons-material/Menu'
import AccountCircleIcon from '@mui/icons-material/AccountCircle'
import LogoutIcon from '@mui/icons-material/Logout'
import { useAuth } from '../../features/auth/context/AuthContext'

// Temporary workaround for Vite/MUI icon module wrappers.
// Remove after fixing the underlying dependency resolution.
const resolveIcon = (icon: any): React.ElementType =>
    icon?.default ?? icon

const MenuIconComponent = resolveIcon(MenuIcon)
const AccountCircleIconComponent = resolveIcon(AccountCircleIcon)
const LogoutIconComponent = resolveIcon(LogoutIcon)

interface TopBarProps {
    onMenuClick?: () => void
    showMenuButton?: boolean
}

export const TopBar: React.FC<TopBarProps> = ({
    onMenuClick,
    showMenuButton = false
}) => {
    const navigate = useNavigate()
    const { user, logout } = useAuth()

    const [anchorEl, setAnchorEl] =
        React.useState<null | HTMLElement>(null)

    const handleProfileClick = (
        event: React.MouseEvent<HTMLElement>
    ) => {
        setAnchorEl(event.currentTarget)
    }

    const handleProfileClose = () => {
        setAnchorEl(null)
    }

    const handleLogout = () => {
        handleProfileClose()
        logout()
        navigate('/login')
    }

    return (
        <AppBar position="static">
            <Toolbar>
                {showMenuButton && (
                    <IconButton
                        color="inherit"
                        aria-label="open drawer"
                        edge="start"
                        onClick={onMenuClick}
                        sx={{ mr: 2 }}
                    >
                        <MenuIconComponent />
                    </IconButton>
                )}

                <Typography
                    variant="h6"
                    component="div"
                    sx={{ flexGrow: 1 }}
                >
                    SmartOps — Medical Asset Monitoring
                </Typography>

                {user && (
                    <Box
                        sx={{
                            display: 'flex',
                            alignItems: 'center',
                            gap: 2
                        }}
                    >
                        <Chip
                            avatar={<AccountCircleIconComponent />}
                            label={user.username}
                            onClick={handleProfileClick}
                            clickable
                        />

                        <Menu
                            anchorEl={anchorEl}
                            open={Boolean(anchorEl)}
                            onClose={handleProfileClose}
                        >
                            <MenuItem disabled>
                                <Typography variant="caption">
                                    Role: {user.role}
                                </Typography>
                            </MenuItem>

                            <MenuItem onClick={handleLogout}>
                                <LogoutIconComponent sx={{ mr: 1 }} />
                                Logout
                            </MenuItem>
                        </Menu>
                    </Box>
                )}
            </Toolbar>
        </AppBar>
    )
}
