/**
 * Device Management - Detail Page
 * 
 * Features:
 * - Display detailed information about a device
 * - View-only mode with formatted data
 * - Navigation to edit page
 * - Link/Unlink asset functionality
 * - Back navigation to device list
 */

import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import {
    Box,
    Button,
    Card,
    CardContent,
    CircularProgress,
    Grid,
    Typography,
    Alert,
    Chip,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle
} from '@mui/material'
import {
    ArrowBack as ArrowBackIcon,
    Edit as EditIcon,
    Link as LinkIcon,
    LinkOff as LinkOffIcon,
    Delete as DeleteIcon
} from '@mui/icons-material'
import { deviceService } from '../../../services/deviceService'
import { useAuth } from '../../auth/context/AuthContext'
import {
    IoTDevice,
    DeviceStatusLabel,
    DeviceStatusColor,
    DeviceTypeLabel
} from '../../../types/api.types'
 

export const DevicesDetailPage: React.FC = () => {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()
    const { user } = useAuth()

    // State
    const [device, setDevice] = useState<IoTDevice | null>(null)
    const [isLoading, setIsLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)
    const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false)
    const [unlinkAssetConfirmOpen, setUnlinkAssetConfirmOpen] = useState(false)

    // Check if user can modify devices (BiomedicalEngineer or Admin)
    const canModify = user?.role === 'BiomedicalEngineer' || user?.role === 'HospitalAdministrator'

    // Load device details
    useEffect(() => {
        const loadDevice = async () => {
            if (!id) {
                setError('Invalid device ID')
                setIsLoading(false)
                return
            }

            setIsLoading(true)
            setError(null)

            try {
                const result = await deviceService.getById(id)
                setDevice(result)
            } catch (err: any) {
                if (err.status === 401) {
                    setError('Your session has expired. Please log in again.')
                } else if (err.status === 403) {
                    setError('You do not have permission to view this device.')
                } else if (err.status === 404) {
                    setError('Device not found.')
                } else {
                    setError(err.message || 'Failed to load device details.')
                }
            } finally {
                setIsLoading(false)
            }
        }

        loadDevice()
    }, [id])

    // Handle delete
    const handleDeleteClick = () => {
        setDeleteConfirmOpen(true)
    }

    const handleDeleteConfirm = async () => {
        if (!device) return

        setIsLoading(true)
        try {
            await deviceService.delete(device.id)
            setDeleteConfirmOpen(false)
            navigate('/devices', { state: { successMessage: 'Device deleted successfully' } })
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

    // Handle unlink asset
    const handleUnlinkAssetClick = () => {
        setUnlinkAssetConfirmOpen(true)
    }

    const handleUnlinkAssetConfirm = async () => {
        if (!device) return

        setIsLoading(true)
        try {
            await deviceService.unlinkAsset(device.id)
            setUnlinkAssetConfirmOpen(false)
            // Reload device to show updated state
            const updated = await deviceService.getById(device.id)
            setDevice(updated)
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

    // Handle navigation
    const handleEdit = () => {
        if (device) {
            navigate(`/devices/${device.id}/edit`)
        }
    }

    const handleLinkAsset = () => {
        if (device) {
            navigate(`/devices/${device.id}/link-asset`)
        }
    }

    const handleBack = () => {
        navigate('/devices')
    }

    if (isLoading && !device) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '400px' }}>
                <CircularProgress />
            </Box>
        )
    }

    if (error && !device) {
        return (
            <Box sx={{ width: '100%' }}>
                <Button
                    variant="text"
                    startIcon={<ArrowBackIcon />}
                    onClick={handleBack}
                    sx={{ mb: 2, color: 'primary.main' }}
                >
                    Back
                </Button>
                <Alert severity="error">{error}</Alert>
            </Box>
        )
    }

    if (!device) {
        return (
            <Box sx={{ width: '100%' }}>
                <Button
                    variant="text"
                    startIcon={<ArrowBackIcon />}
                    onClick={handleBack}
                    sx={{ mb: 2, color: 'primary.main' }}
                >
                    Back
                </Button>
                <Alert severity="warning">Device not found</Alert>
            </Box>
        )
    }

    return (
        <Box sx={{ width: '100%' }}>
            <Button
                variant="text"
                startIcon={<ArrowBackIcon />}
                onClick={handleBack}
                sx={{ mb: 2, color: 'primary.main' }}
            >
                Back
            </Button>

            {error && (
                <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
                    {error}
                </Alert>
            )}

            <Box sx={{ display: 'flex', gap: 1, mb: 3, flexWrap: 'wrap' }}>
                {canModify && (
                    <Button variant="contained" startIcon={<EditIcon />} onClick={handleEdit}>
                        Edit
                    </Button>
                )}
                {canModify && !device.linkedAssetId && (
                    <Button variant="outlined" startIcon={<LinkIcon />} onClick={handleLinkAsset}>
                        Link Asset
                    </Button>
                )}
                {canModify && device.linkedAssetId && (
                    <Button variant="outlined" color="warning" startIcon={<LinkOffIcon />} onClick={handleUnlinkAssetClick}>
                        Unlink Asset
                    </Button>
                )}
                {canModify && (
                    <Button variant="outlined" color="error" startIcon={<DeleteIcon />} onClick={handleDeleteClick}>
                        Delete
                    </Button>
                )}
            </Box>

            <Card sx={{ mb: 3 }}>
                <CardContent>
                    <Typography variant="h5" sx={{ fontWeight: 'bold', mb: 2 }}>
                        Device Information
                    </Typography>

                    <Grid container spacing={3}>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary">Device Code</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                {device.deviceCode}
                            </Typography>
                        </Grid>

                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary">Name</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                {device.name}
                            </Typography>
                        </Grid>

                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary">Device Type</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                {DeviceTypeLabel[device.deviceType]}
                            </Typography>
                        </Grid>

                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary">Status</Typography>
                            <Chip
                                label={DeviceStatusLabel[device.status]}
                                size="small"
                                sx={{
                                    backgroundColor: DeviceStatusColor[device.status],
                                    color: 'white'
                                }}
                            />
                        </Grid>

                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary">Firmware Version</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                {device.firmwareVersion || 'Not specified'}
                            </Typography>
                        </Grid>

                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary">Gateway ID</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                {device.gatewayId || 'Not assigned'}
                            </Typography>
                        </Grid>

                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary">Linked Asset ID</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                {device.linkedAssetId || 'Not linked'}
                            </Typography>
                        </Grid>

                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary">Registered</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                {new Date(device.registeredAtUtc).toLocaleString()}
                            </Typography>
                        </Grid>
                    </Grid>
                </CardContent>
            </Card>

            <Card>
                <CardContent>
                    <Typography variant="h5" sx={{ fontWeight: 'bold', mb: 2 }}>
                        Device Metrics
                    </Typography>

                    <Grid container spacing={3}>
                        <Grid item xs={12} sm={6} md={3}>
                            <Typography color="textSecondary" variant="body2">
                                Battery
                            </Typography>
                            <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
                                {device.batteryPercentage !== null && device.batteryPercentage !== undefined
                                    ? `${Math.round(device.batteryPercentage)}%`
                                    : 'N/A'}
                            </Typography>
                        </Grid>

                        <Grid item xs={12} sm={6} md={3}>
                            <Typography color="textSecondary" variant="body2">
                                Signal Strength (RSSI)
                            </Typography>
                            <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
                                {device.signalStrengthRssi !== null && device.signalStrengthRssi !== undefined
                                    ? `${device.signalStrengthRssi} dBm`
                                    : 'N/A'}
                            </Typography>
                        </Grid>

                        <Grid item xs={12} sm={6} md={3}>
                            <Typography color="textSecondary" variant="body2">
                                CPU Usage
                            </Typography>
                            <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
                                {device.cpuUsagePercentage !== null && device.cpuUsagePercentage !== undefined
                                    ? `${Math.round(device.cpuUsagePercentage)}%`
                                    : 'N/A'}
                            </Typography>
                        </Grid>

                        <Grid item xs={12} sm={6} md={3}>
                            <Typography color="textSecondary" variant="body2">
                                Memory Usage
                            </Typography>
                            <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
                                {device.memoryUsagePercentage !== null && device.memoryUsagePercentage !== undefined
                                    ? `${Math.round(device.memoryUsagePercentage)}%`
                                    : 'N/A'}
                            </Typography>
                        </Grid>

                        <Grid item xs={12}>
                            <Typography color="textSecondary" variant="body2">
                                Last Heartbeat
                            </Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                {device.lastHeartbeatAtUtc
                                    ? new Date(device.lastHeartbeatAtUtc).toLocaleString()
                                    : 'Never'}
                            </Typography>
                        </Grid>
                    </Grid>
                </CardContent>
            </Card>

            {/* Delete Confirmation Dialog */}
            <Dialog open={deleteConfirmOpen} onClose={() => setDeleteConfirmOpen(false)}>
                <DialogTitle>Confirm Delete</DialogTitle>
                <DialogContent>
                    <Typography>
                        Are you sure you want to delete device <strong>{device.name}</strong>? This action cannot be undone.
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
                        Are you sure you want to unlink the asset from device <strong>{device.name}</strong>?
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
