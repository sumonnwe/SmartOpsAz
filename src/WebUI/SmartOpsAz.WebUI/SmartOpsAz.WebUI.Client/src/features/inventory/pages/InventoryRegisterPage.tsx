/**
 * Inventory Management - Register Asset Page
 * 
 * Features:
 * - Form to register new inventory assets
 * - Validation of required fields
 * - Success/error handling
 * - Navigation back to inventory list
 */

import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
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

interface RegisterAssetFormData {
    assetTag: string
    deviceName: string
    serialNumber: string
    deviceType: string
    manufacturer: string
    modelNumber: string
    purchaseDate: string
    warrantyExpiration: string
    department: string
    location: string
}

export const InventoryRegisterPage: React.FC = () => {
    const navigate = useNavigate()

    // Form state
    const [formData, setFormData] = useState<RegisterAssetFormData>({
        assetTag: '',
        deviceName: '',
        serialNumber: '',
        deviceType: '',
        manufacturer: '',
        modelNumber: '',
        purchaseDate: '',
        warrantyExpiration: '',
        department: '',
        location: ''
    })

    // UI state
    const [isLoading, setIsLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [success, setSuccess] = useState(false)
    const [formErrors, setFormErrors] = useState<Record<string, string>>({})

    // Handle input change
    const handleInputChange = (field: keyof RegisterAssetFormData, value: string) => {
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

        if (!formData.assetTag.trim()) {
            errors.assetTag = 'Asset Tag is required'
        }
        if (!formData.deviceName.trim()) {
            errors.deviceName = 'Device Name is required'
        }
        if (!formData.serialNumber.trim()) {
            errors.serialNumber = 'Serial Number is required'
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

        if (!validateForm()) {
            return
        }

        setIsLoading(true)
        setError(null)
        setSuccess(false)

        try {
            const result = await inventoryService.register({
                assetTag: formData.assetTag.trim(),
                deviceName: formData.deviceName.trim(),
                serialNumber: formData.serialNumber.trim(),
                deviceType: formData.deviceType.trim(),
                manufacturer: formData.manufacturer.trim(),
                modelNumber: formData.modelNumber.trim(),
                purchaseDate: formData.purchaseDate ? new Date(formData.purchaseDate).toISOString() : undefined,
                warrantyExpiration: formData.warrantyExpiration ? new Date(formData.warrantyExpiration).toISOString() : undefined,
                department: formData.department.trim() || undefined,
                location: formData.location.trim() || undefined
            })

            setSuccess(true)
            // Navigate back to inventory list after successful registration
            setTimeout(() => {
                navigate('/inventory', { state: { successMessage: `Asset ${result.assetTag} registered successfully` } })
            }, 1500)
        } catch (err: any) {
            if (err.status === 400) {
                setError(err.message || 'Invalid input. Please check your entries.')
            } else if (err.status === 401) {
                setError('Your session has expired. Please log in again.')
            } else if (err.status === 403) {
                setError('You do not have permission to register inventory.')
            } else if (err.status === 409) {
                setError('This Asset Tag or Serial Number already exists.')
            } else {
                setError(err.message || 'Failed to register asset. Please try again.')
            }
        } finally {
            setIsLoading(false)
        }
    }

    // Handle cancel
    const handleCancel = () => {
        navigate('/inventory')
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
                >
                    Back
                </Button>
                <Typography variant="h4" sx={{ fontWeight: 'bold' }}>
                    Register New Asset
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
                    Asset registered successfully! Redirecting...
                </Alert>
            )}

            {/* Form */}
            <Card component="form" onSubmit={handleSubmit}>
                <CardContent>
                    {/* Basic Information Section */}
                    <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                        Basic Information
                    </Typography>
                    <Grid container spacing={2} sx={{ mb: 3 }}>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Asset Tag *"
                                placeholder="e.g., BIO-2024-001"
                                value={formData.assetTag}
                                onChange={(e) => handleInputChange('assetTag', e.target.value)}
                                disabled={isLoading}
                                error={Boolean(formErrors.assetTag)}
                                helperText={formErrors.assetTag}
                                required
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Serial Number *"
                                placeholder="Device serial number"
                                value={formData.serialNumber}
                                onChange={(e) => handleInputChange('serialNumber', e.target.value)}
                                disabled={isLoading}
                                error={Boolean(formErrors.serialNumber)}
                                helperText={formErrors.serialNumber}
                                required
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <TextField
                                fullWidth
                                label="Device Name *"
                                placeholder="e.g., Siemens SOMATOM"
                                value={formData.deviceName}
                                onChange={(e) => handleInputChange('deviceName', e.target.value)}
                                disabled={isLoading}
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
                                disabled={isLoading}
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
                                disabled={isLoading}
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
                                disabled={isLoading}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Purchase Date"
                                type="date"
                                value={formData.purchaseDate}
                                onChange={(e) => handleInputChange('purchaseDate', e.target.value)}
                                disabled={isLoading}
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
                                disabled={isLoading}
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
                                disabled={isLoading}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Location"
                                placeholder="e.g., Building A, Floor 2"
                                value={formData.location}
                                onChange={(e) => handleInputChange('location', e.target.value)}
                                disabled={isLoading}
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
                            disabled={isLoading}
                        >
                            Cancel
                        </Button>
                        <Button
                            variant="contained"
                            startIcon={<SaveIcon />}
                            type="submit"
                            disabled={isLoading}
                            sx={{ minWidth: 120 }}
                        >
                            {isLoading ? <CircularProgress size={24} /> : 'Register Asset'}
                        </Button>
                    </Box>
                </Paper>
            </Card>
        </Box>
    )
}