/**
 * Inventory Management - Detail Page
 * 
 * Features:
 * - Display detailed information about an inventory item
 * - View-only mode with formatted data
 * - Navigation to edit page
 * - Back navigation to inventory list
 */

import React, { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import {
    Box,
    Button,
    Card,
    CardContent,
    CircularProgress,
    Grid,
    //Paper,
    Typography,
    Alert,
    Chip,
    //Divider,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle
} from '@mui/material'
import {
    ArrowBack as ArrowBackIcon,
    Edit as EditIcon,
    Delete as DeleteIcon
} from '@mui/icons-material'
import { inventoryService } from '../../../services/inventoryService'
import { useAuth } from '../../auth/context/AuthContext'
import {
    InventoryItem,
    InventoryStatusLabel,
    InventoryStatusColor
} from '../../../types/api.types'

export const InventoryDetailPage: React.FC = () => {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()
    const { user } = useAuth()

    // State
    const [item, setItem] = useState<InventoryItem | null>(null)
    const [isLoading, setIsLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)
    const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false)

    // Check if user can retire items (BiomedicalEngineer)
    const canRetire = user?.role === 'BiomedicalEngineer' || user?.role === 'HospitalAdministrator'

    // Load inventory item details
    useEffect(() => {
        const loadItem = async () => {
            if (!id) {
                setError('Invalid inventory ID')
                setIsLoading(false)
                return
            }

            setIsLoading(true)
            setError(null)

            try {
                const result = await inventoryService.getById(id)
                setItem(result)
            } catch (err: any) {
                if (err.status === 401) {
                    setError('Your session has expired. Please log in again.')
                } else if (err.status === 403) {
                    setError('You do not have permission to view this inventory item.')
                } else if (err.status === 404) {
                    setError('Inventory item not found.')
                } else {
                    setError(err.message || 'Failed to load inventory details.')
                }
            } finally {
                setIsLoading(false)
            }
        }

        loadItem()
    }, [id])

    // Handle delete (retire)
    const handleDeleteClick = () => {
        setDeleteConfirmOpen(true)
    }

    const handleDeleteConfirm = async () => {
        if (!item) return

        setIsLoading(true)
        try {
            await inventoryService.retire(item.inventoryId)
            setDeleteConfirmOpen(false)
            navigate('/inventory', { state: { successMessage: 'Asset retired successfully' } })
        } catch (err: any) {
            if (err.status === 403) {
                setError('Only BiomedicalEngineers can retire inventory.')
            } else {
                setError(err.message || 'Failed to retire inventory.')
            }
        } finally {
            setIsLoading(false)
        }
    }

    // Handle navigation
    const handleEdit = () => {
        navigate(`/inventory/${id}/edit`)
    }

    const handleBack = () => {
        navigate('/inventory')
    }

    if (isLoading && !item) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '400px' }}>
                <CircularProgress />
            </Box>
        )
    }

    if (error && !item) {
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

    if (!item) {
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
                <Alert severity="warning">Inventory item not found.</Alert>
            </Box>
        )
    }

    return (
        <Box sx={{ width: '100%' }}>
            {/* Header */}
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <Button
                        variant="text"
                        startIcon={<ArrowBackIcon />}
                        onClick={handleBack}
                        sx={{ color: 'primary.main' }}
                    >
                        Back
                    </Button>
                    <Typography variant="h4" sx={{ fontWeight: 'bold' }}>
                        Asset Details
                    </Typography>
                </Box>
                <Box sx={{ display: 'flex', gap: 1 }}>
                    <Button
                        variant="contained"
                        startIcon={<EditIcon />}
                        onClick={handleEdit}
                        disabled={isLoading}
                    >
                        Edit
                    </Button>
                    {canRetire && (
                        <Button
                            variant="outlined"
                            color="error"
                            startIcon={<DeleteIcon />}
                            onClick={handleDeleteClick}
                            disabled={isLoading}
                        >
                            Retire
                        </Button>
                    )}
                </Box>
            </Box>

            {/* Error Alert */}
            {error && (
                <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
                    {error}
                </Alert>
            )}

            {/* Basic Information */}
            <Card sx={{ mb: 2 }}>
                <CardContent>
                    <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                        Basic Information
                    </Typography>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Asset Tag</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>{item.assetTag}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Device Name</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>{item.deviceName}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Serial Number</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>{item.serialNumber}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Status</Typography>
                            <Chip
                                label={InventoryStatusLabel[item.status]}
                                size="small"
                                sx={{
                                    backgroundColor: InventoryStatusColor[item.status],
                                    color: 'white',
                                    mt: 0.5
                                }}
                            />
                        </Grid>
                    </Grid>
                </CardContent>
            </Card>

            {/* Device Details */}
            <Card sx={{ mb: 2 }}>
                <CardContent>
                    <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                        Device Details
                    </Typography>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Device Type</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>{item.deviceType || '-'}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Manufacturer</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>{item.manufacturer || '-'}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Model Number</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>{item.modelNumber || '-'}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Purchase Date</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                {item.purchaseDate ? new Date(item.purchaseDate).toLocaleDateString() : '-'}
                            </Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Warranty Expiration</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                {item.warrantyExpiration ? new Date(item.warrantyExpiration).toLocaleDateString() : '-'}
                            </Typography>
                        </Grid>
                    </Grid>
                </CardContent>
            </Card>

            {/* Location Information */}
            <Card sx={{ mb: 2 }}>
                <CardContent>
                    <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                        Location Information
                    </Typography>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Department</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>{item.department || '-'}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Location</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>{item.location || '-'}</Typography>
                        </Grid>
                    </Grid>
                </CardContent>
            </Card>

            {/* Metadata */}
            <Card>
                <CardContent>
                    <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                        Metadata
                    </Typography>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Created At</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                {new Date(item.createdAt).toLocaleString()}
                            </Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography color="textSecondary" variant="body2">Updated At</Typography>
                            <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                {new Date(item.updatedAt).toLocaleString()}
                            </Typography>
                        </Grid>
                    </Grid>
                </CardContent>
            </Card>

            {/* Delete Confirmation Dialog */}
            <Dialog open={deleteConfirmOpen} onClose={() => setDeleteConfirmOpen(false)}>
                <DialogTitle>Confirm Retirement</DialogTitle>
                <DialogContent>
                    <Typography>
                        Are you sure you want to retire this inventory item?
                    </Typography>
                    <Typography variant="body2" color="textSecondary" sx={{ mt: 2 }}>
                        <strong>Asset Tag:</strong> {item.assetTag}
                    </Typography>
                    <Typography variant="body2" color="textSecondary">
                        <strong>Device Name:</strong> {item.deviceName}
                    </Typography>
                    <Typography variant="body2" color="textSecondary">
                        This action cannot be undone.
                    </Typography>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDeleteConfirmOpen(false)}>Cancel</Button>
                    <Button
                        onClick={handleDeleteConfirm}
                        variant="contained"
                        color="error"
                        disabled={isLoading}
                    >
                        {isLoading ? <CircularProgress size={20} /> : 'Retire'}
                    </Button>
                </DialogActions>
            </Dialog>
        </Box>
    )
}