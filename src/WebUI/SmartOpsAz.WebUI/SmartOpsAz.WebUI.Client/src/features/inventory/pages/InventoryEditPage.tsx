/**
 * Inventory Management - Edit Page
 * 
 * Features:
 * - Edit existing inventory asset information
 * - Form validation
 * - Load current data on mount
 * - Success/error handling
 * - Navigation back to detail or list page
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
    TextField,
    Typography,
    Alert,
    Paper,
    Divider
} from '@mui/material'
import {
    ArrowBack as ArrowBackIcon,
    Save as SaveIcon
} from '@mui/icons-material'
import { inventoryService } from '../../../services/inventoryService'
import {
    InventoryItem
} from '../../../types/api.types'

interface EditAssetFormData {
    deviceName: string
    deviceType: string
    manufacturer: string
    modelNumber: string
    purchaseDate: string
    warrantyExpiration: string
    department: string
    location: string
}

export const InventoryEditPage: React.FC = () => {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()

    // Form state
    const [formData, setFormData] = useState<EditAssetFormData>({
        deviceName: '',
        deviceType: '',
        manufacturer: '',
        modelNumber: '',
        purchaseDate: '',
        warrantyExpiration: '',
        department: '',
        location: ''
    })

    // UI state
    const [item, setItem] = useState<InventoryItem | null>(null)
    const [isLoading, setIsLoading] = useState(true)
    const [isSaving, setIsSaving] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [success, setSuccess] = useState(false)
    const [formErrors, setFormErrors] = useState<Record<string, string>>({})

    // Load inventory item on mount
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
                setFormData({
                    deviceName: result.deviceName || '',
                    deviceType: result.deviceType || '',
                    manufacturer: result.manufacturer || '',
                    modelNumber: result.modelNumber || '',
                    purchaseDate: result.purchaseDate ? result.purchaseDate.split('T')[0] : '',
                    warrantyExpiration: result.warrantyExpiration ? result.warrantyExpiration.split('T')[0] : '',
                    department: result.department || '',
                    location: result.location || ''
                })
            } catch (err: any) {
                if (err.status === 401) {
                    setError('Your session has expired. Please log in again.')
                } else if (err.status === 403) {
                    setError('You do not have permission to edit this inventory item.')
                } else if (err.status === 404) {
                    setError('Inventory item not found.')
                } else {
                    setError(err.message || 'Failed to load inventory item.')
                }
            } finally {
                setIsLoading(false)
            }
        }

        loadItem()
    }, [id])

    // Handle input change
    const handleInputChange = (field: keyof EditAssetFormData, value: string) => {
        setFormData((prev) => ({
            ...prev,
            [field]: value
        }))
        // Clear error for this field when user starts typing
        if (formErrors[field]) {
            setFormErrors((prev) => ({
                ...prev,
                [field]: ''
            }))
        }
    }

    // Validate form
    const validateForm = (): boolean => {
        const errors: Record<string, string> = {}

        if (!formData.deviceName.trim()) {
            errors.deviceName = 'Device Name is required'
        }
        if (!formData.deviceType.trim()) {
            errors.deviceType = 'Device Type is required'
        }
        if (!formData.manufacturer.trim()) {
            errors.manufacturer = 'Manufacturer is required'
        }

        setFormErrors(errors)
        return Object.keys(errors).length === 0
    }

    // Handle form submission
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()

        if (!validateForm() || !item || !id) {
            return
        }

        setIsSaving(true)
        setError(null)
        setSuccess(false)

        try {
            await inventoryService.update(id, {
                deviceName: formData.deviceName.trim(),
                deviceType: formData.deviceType.trim(),
                manufacturer: formData.manufacturer.trim(),
                modelNumber: formData.modelNumber.trim(),
                purchaseDate: formData.purchaseDate ? new Date(formData.purchaseDate).toISOString() : undefined,
                warrantyExpiration: formData.warrantyExpiration ? new Date(formData.warrantyExpiration).toISOString() : undefined,
                department: formData.department.trim() || undefined,
                location: formData.location.trim() || undefined
            })

            setSuccess(true)
            // Navigate back to detail view after successful update
            setTimeout(() => {
                navigate(`/inventory/${id}`, { state: { successMessage: 'Asset updated successfully' } })
            }, 1500)
        } catch (err: any) {
            if (err.status === 400) {
                setError(err.message || 'Invalid input. Please check your entries.')
            } else if (err.status === 401) {
                setError('Your session has expired. Please log in again.')
            } else if (err.status === 403) {
                setError('You do not have permission to edit this inventory item.')
            } else if (err.status === 404) {
                setError('Inventory item not found.')
            } else {
                setError(err.message || 'Failed to update asset. Please try again.')
            }
        } finally {
            setIsSaving(false)
        }
    }

    // Handle cancel
    const handleCancel = () => {
        if (id) {
            navigate(`/inventory/${id}`)
        } else {
            navigate('/inventory')
        }
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
                    onClick={handleCancel}
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
                    onClick={handleCancel}
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
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
                <Button
                    variant="text"
                    startIcon={<ArrowBackIcon />}
                    onClick={handleCancel}
                    sx={{ color: 'primary.main' }}
                    disabled={isSaving}
                >
                    Back
                </Button>
                <Typography variant="h4" sx={{ fontWeight: 'bold' }}>
                    Edit Asset
                </Typography>
            </Box>

            {/* Alerts */}
            {error && (
                <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
                    {error}
                </Alert>
            )}
            {success && (
                <Alert severity="success" sx={{ mb: 2 }}>
                    Asset updated successfully! Redirecting...
                </Alert>
            )}

            {/* Asset Identification (Read-only) */}
            <Card sx={{ mb: 3 }}>
                <CardContent>
                    <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                        Asset Identification (Read-only)
                    </Typography>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Asset Tag"
                                value={item.assetTag}
                                disabled
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Serial Number"
                                value={item.serialNumber}
                                disabled
                            />
                        </Grid>
                    </Grid>
                </CardContent>
            </Card>

            {/* Form */}
            <Card component="form" onSubmit={handleSubmit}>
                <CardContent>
                    {/* Basic Information Section */}
                    <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                        Basic Information
                    </Typography>
                    <Grid container spacing={2} sx={{ mb: 3 }}>
                        <Grid item xs={12}>
                            <TextField
                                fullWidth
                                label="Device Name *"
                                placeholder="e.g., Siemens SOMATOM"
                                value={formData.deviceName}
                                onChange={(e) => handleInputChange('deviceName', e.target.value)}
                                disabled={isSaving}
                                error={Boolean(formErrors.deviceName)}
                                helperText={formErrors.deviceName}
                                required
                            />
                        </Grid>
                    </Grid>

                    <Divider sx={{ my: 3 }} />

                    {/* Device Details Section */}
                    <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                        Device Details
                    </Typography>
                    <Grid container spacing={2} sx={{ mb: 3 }}>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Device Type *"
                                placeholder="e.g., CT Scanner, Ultrasound, X-Ray"
                                value={formData.deviceType}
                                onChange={(e) => handleInputChange('deviceType', e.target.value)}
                                disabled={isSaving}
                                error={Boolean(formErrors.deviceType)}
                                helperText={formErrors.deviceType}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Manufacturer *"
                                placeholder="e.g., Siemens, GE Healthcare"
                                value={formData.manufacturer}
                                onChange={(e) => handleInputChange('manufacturer', e.target.value)}
                                disabled={isSaving}
                                error={Boolean(formErrors.manufacturer)}
                                helperText={formErrors.manufacturer}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Model Number"
                                placeholder="e.g., SOMATOM Definition AS"
                                value={formData.modelNumber}
                                onChange={(e) => handleInputChange('modelNumber', e.target.value)}
                                disabled={isSaving}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Purchase Date"
                                type="date"
                                value={formData.purchaseDate}
                                onChange={(e) => handleInputChange('purchaseDate', e.target.value)}
                                disabled={isSaving}
                                InputLabelProps={{ shrink: true }}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Warranty Expiration"
                                type="date"
                                value={formData.warrantyExpiration}
                                onChange={(e) => handleInputChange('warrantyExpiration', e.target.value)}
                                disabled={isSaving}
                                InputLabelProps={{ shrink: true }}
                            />
                        </Grid>
                    </Grid>

                    <Divider sx={{ my: 3 }} />

                    {/* Location Information Section */}
                    <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                        Location Information
                    </Typography>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Department"
                                placeholder="e.g., Radiology, Cardiology"
                                value={formData.department}
                                onChange={(e) => handleInputChange('department', e.target.value)}
                                disabled={isSaving}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Location"
                                placeholder="e.g., Building A, Floor 2"
                                value={formData.location}
                                onChange={(e) => handleInputChange('location', e.target.value)}
                                disabled={isSaving}
                            />
                        </Grid>
                    </Grid>
                </CardContent>

                {/* Form Actions */}
                <Paper sx={{ p: 2, backgroundColor: '#f5f5f5', borderTop: '1px solid #e0e0e0' }}>
                    <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
                        <Button
                            variant="outlined"
                            onClick={handleCancel}
                            disabled={isSaving}
                        >
                            Cancel
                        </Button>
                        <Button
                            variant="contained"
                            startIcon={<SaveIcon />}
                            type="submit"
                            disabled={isSaving}
                            sx={{ minWidth: 120 }}
                        >
                            {isSaving ? <CircularProgress size={24} /> : 'Update Asset'}
                        </Button>
                    </Box>
                </Paper>
            </Card>
        </Box>
    )
}