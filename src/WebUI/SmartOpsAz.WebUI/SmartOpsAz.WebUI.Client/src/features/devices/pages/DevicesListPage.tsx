import React from 'react'
/**
 * Device Management - List Page
 * 
 * Features:
 * - Display paginated device table with real data from DeviceService
 * - Pagination with configurable page size
 * - Loading, error, and empty states
 * - Actions: View Details, Edit, Link Asset, Unlink Asset, Delete
 * - Role-based access control (BiomedicalEngineer-only for modifications)
 * - Status badges with color coding
 */

import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import {
    Box,
    Button,
    CircularProgress,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TablePagination,
    TableRow,
    Typography,
    Alert,
    Chip,
    IconButton,
    Menu,
    MenuItem
} from '@mui/material'
import {
    MoreVert as MoreVertIcon,
    Add as AddIcon,
    Refresh as RefreshIcon
} from '@mui/icons-material'
import { deviceService } from '../../../services/deviceService'
import { useAuth } from '../../auth/context/AuthContext'
import {
    IoTDevice,
    DeviceStatusLabel,
    DeviceStatusColor,
    DeviceTypeLabel
} from '../../../types/api.types'

export const DevicesListPage: React.FC = () => {
    // Navigation
    const navigate = useNavigate()

    // State management
    const [devices, setDevices] = useState<IoTDevice[]>([])
    const [isLoading, setIsLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)
    const [page, setPage] = useState(0)
    const [rowsPerPage, setRowsPerPage] = useState(10)
    const [totalCount, setTotalCount] = useState(0)

    // UI state
    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null)
    const [selectedDevice, setSelectedDevice] = useState<IoTDevice | null>(null)
    const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false)
    const [unlinkAssetConfirmOpen, setUnlinkAssetConfirmOpen] = useState(false)

    const { user } = useAuth()

    // Check if user can modify devices (BiomedicalEngineer or Admin)
    const canModify = user?.role === 'BiomedicalEngineer' || user?.role === 'HospitalAdministrator'

    // Load devices data
    const loadDevices = useCallback(async () => {
        setIsLoading(true)
        setError(null)

        try {
            const result = await deviceService.search(page + 1, rowsPerPage)
            setDevices(result.items)
            setTotalCount(result.totalCount || 0)
        } catch (err: any) {
            if (err.status === 401) {
                setError('Your session has expired. Please log in again.')
            } else if (err.status === 403) {
                setError('You do not have permission to view devices.')
            } else if (err.status === 404) {
                setError('Devices not found.')
            } else {
                setError(err.message || 'Failed to load devices. Please try again.')
            }
            setDevices([])
            setTotalCount(0)
        } finally {
            setIsLoading(false)
        }
    }, [page, rowsPerPage])

    // Load devices on component mount and when pagination changes
    useEffect(() => {
        loadDevices()
    }, [loadDevices])

    // Handle pagination
    const handleChangePage = (_event: unknown, newPage: number) => {
        setPage(newPage)
    }

    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        setRowsPerPage(parseInt(event.target.value, 10))
        setPage(0)
    }

    // Handle menu actions
    const handleMenuOpen = (event: React.MouseEvent<HTMLButtonElement>, device: IoTDevice) => {
        setAnchorEl(event.currentTarget)
        setSelectedDevice(device)
    }

    const handleMenuClose = () => {
        setAnchorEl(null)
    }

    // Handle register device
    const handleRegisterDevice = () => {
        navigate('/devices/register')
    }

    // Handle view details
    const handleViewDetails = () => {
        if (!selectedDevice) return
        navigate(`/devices/${selectedDevice.id}`)
        handleMenuClose()
    }

    // Handle edit
    const handleEdit = () => {
        if (!selectedDevice) return
        navigate(`/devices/${selectedDevice.id}/edit`)
        handleMenuClose()
    }

    // Handle link asset
    const handleLinkAsset = () => {
        if (!selectedDevice) return
        navigate(`/devices/${selectedDevice.id}/link-asset`)
        handleMenuClose()
    }

    // Handle unlink asset
    const handleUnlinkAssetClick = () => {
        setUnlinkAssetConfirmOpen(true)
        handleMenuClose()
    }

    const handleUnlinkAssetConfirm = async () => {
        if (!selectedDevice) return

        setIsLoading(true)
        try {
            await deviceService.unlinkAsset(selectedDevice.id)
            setUnlinkAssetConfirmOpen(false)
            setSelectedDevice(null)
            await loadDevices()
        } catch (err: any) {
            if (err.status === 403) {
                setError('Only BiomedicalEngineers can unlink assets.')
            } else {
                setError(err.message || 'Failed to unlink asset.')
            }
        } finally {
            setIsLoading(false)
        }
    }

    // Handle delete
    const handleDeleteClick = () => {
        setDeleteConfirmOpen(true)
        handleMenuClose()
    }

    const handleDeleteConfirm = async () => {
        if (!selectedDevice) return

        setIsLoading(true)
        try {
            await deviceService.delete(selectedDevice.id)
            setDeleteConfirmOpen(false)
            setSelectedDevice(null)
            await loadDevices()
        } catch (err: any) {
            if (err.status === 403) {
                setError('Only BiomedicalEngineers can delete devices.')
            } else {
                setError(err.message || 'Failed to delete device.')
            }
        } finally {
            setIsLoading(false)
        }
    }

    return (
        <Box sx={{ width: '100%' }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography variant="h4" sx={{ fontWeight: 'bold' }}>
                    Device Management
                </Typography>
                <Box sx={{ display: 'flex', gap: 1 }}>
                    <Button
                        variant="outlined"
                        startIcon={<RefreshIcon />}
                        onClick={() => loadDevices()}
                        disabled={isLoading}
                    >
                        Refresh
                    </Button>
                    {canModify && (
                        <Button
                            variant="contained"
                            startIcon={<AddIcon />}
                            onClick={handleRegisterDevice}
                        >
                            Register Device
                        </Button>
                    )}
                </Box>
            </Box>

            {error && (
                <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
                    {error}
                </Alert>
            )}

            {isLoading && devices.length === 0 ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '400px' }}>
                    <CircularProgress />
                </Box>
            ) : (
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow sx={{ backgroundColor: '#f5f5f5' }}>
                                <TableCell sx={{ fontWeight: 'bold' }}>Device Code</TableCell>
                                <TableCell sx={{ fontWeight: 'bold' }}>Name</TableCell>
                                <TableCell sx={{ fontWeight: 'bold' }}>Type</TableCell>
                                <TableCell sx={{ fontWeight: 'bold' }}>Status</TableCell>
                                <TableCell sx={{ fontWeight: 'bold' }}>Battery (%)</TableCell>
                                <TableCell sx={{ fontWeight: 'bold' }}>Last Heartbeat</TableCell>
                                <TableCell sx={{ fontWeight: 'bold' }} align="right">
                                    Actions
                                </TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {devices.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={7} align="center">
                                        <Typography variant="body2" color="textSecondary">
                                            No devices found
                                        </Typography>
                                    </TableCell>
                                </TableRow>
                            ) : (
                                devices.map((device) => (
                                    <TableRow key={device.id} hover>
                                        <TableCell>{device.deviceCode}</TableCell>
                                        <TableCell>{device.name}</TableCell>
                                        <TableCell>{DeviceTypeLabel[device.deviceType]}</TableCell>
                                        <TableCell>
                                            <Chip
                                                label={DeviceStatusLabel[device.status]}
                                                size="small"
                                                sx={{
                                                    backgroundColor: DeviceStatusColor[device.status],
                                                    color: 'white'
                                                }}
                                            />
                                        </TableCell>
                                        <TableCell>
                                            {device.batteryPercentage !== null && device.batteryPercentage !== undefined
                                                ? `${Math.round(device.batteryPercentage)}%`
                                                : 'N/A'}
                                        </TableCell>
                                        <TableCell>
                                            {device.lastHeartbeatAtUtc
                                                ? new Date(device.lastHeartbeatAtUtc).toLocaleString()
                                                : 'Never'}
                                        </TableCell>
                                        <TableCell align="right">
                                            <IconButton
                                                onClick={(e) => handleMenuOpen(e, device)}
                                                size="small"
                                            >
                                                <MoreVertIcon />
                                            </IconButton>
                                        </TableCell>
                                    </TableRow>
                                ))
                            )}
                        </TableBody>
                    </Table>
                    <TablePagination
                        rowsPerPageOptions={[10, 25, 50, 100]}
                        component="div"
                        count={totalCount}
                        rowsPerPage={rowsPerPage}
                        page={page}
                        onPageChange={handleChangePage}
                        onRowsPerPageChange={handleChangeRowsPerPage}
                    />
                </TableContainer>
            )}

            {/* Action Menu */}
            <Menu anchorEl={anchorEl} open={Boolean(anchorEl)} onClose={handleMenuClose}>
                <MenuItem onClick={handleViewDetails}>View Details</MenuItem>
                {canModify && <MenuItem onClick={handleEdit}>Edit</MenuItem>}
                {canModify && !selectedDevice?.linkedAssetId && (
                    <MenuItem onClick={handleLinkAsset}>Link Asset</MenuItem>
                )}
                {canModify && selectedDevice?.linkedAssetId && (
                    <MenuItem onClick={handleUnlinkAssetClick}>Unlink Asset</MenuItem>
                )}
                {canModify && <MenuItem onClick={handleDeleteClick} sx={{ color: 'error.main' }}>
                    Delete
                </MenuItem>}
            </Menu>

            {/* Delete Confirmation Dialog */}
            <Dialog open={deleteConfirmOpen} onClose={() => setDeleteConfirmOpen(false)}>
                <DialogTitle>Confirm Delete</DialogTitle>
                <DialogContent>
                    <Typography>
                        Are you sure you want to delete device <strong>{selectedDevice?.name}</strong>? This action cannot be undone.
                    </Typography>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDeleteConfirmOpen(false)}>Cancel</Button>
                    <Button onClick={handleDeleteConfirm} color="error" variant="contained">
                        Delete
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Unlink Asset Confirmation Dialog */}
            <Dialog open={unlinkAssetConfirmOpen} onClose={() => setUnlinkAssetConfirmOpen(false)}>
                <DialogTitle>Confirm Unlink Asset</DialogTitle>
                <DialogContent>
                    <Typography>
                        Are you sure you want to unlink the asset from device <strong>{selectedDevice?.name}</strong>?
                    </Typography>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setUnlinkAssetConfirmOpen(false)}>Cancel</Button>
                    <Button onClick={handleUnlinkAssetConfirm} color="primary" variant="contained">
                        Unlink
                    </Button>
                </DialogActions>
            </Dialog>
        </Box>
    )
}