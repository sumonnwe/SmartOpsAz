/**
 * Device Management - Register Device Page
 * 
 * Features:
 * - Form to register new IoT devices
 * - Validation of required fields
 * - Device type and gateway selection
 * - Success/error handling
 * - Navigation back to device list
 */

import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
    Box,
    Button,
    Card,
    CardContent,
    Grid,
    TextField,
    Typography,
    Alert,
    Divider,
    MenuItem
} from '@mui/material'
import {
    ArrowBack as ArrowBackIcon,
    Save as SaveIcon
} from '@mui/icons-material'
import { deviceService } from '../../../services/deviceService'
import { DeviceType, DeviceTypeLabel } from '../../../types/api.types'

interface RegisterDeviceFormData {
    deviceCode: string
    name: string
    deviceType: DeviceType | ''
    gatewayId: string
    firmwareVersion: string
}

export const DevicesRegisterPage: React.FC = () => {
    const navigate = useNavigate()

    // Form state
    const [formData, setFormData] = useState<RegisterDeviceFormData>({
        deviceCode: '',
        name: '',
        deviceType: '',
        gatewayId: '',
        firmwareVersion: ''
    })

    // UI state
    const [isLoading, setIsLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [success, setSuccess] = useState(false)
    const [formErrors, setFormErrors] = useState<Record<string, string>>({})

    // Handle input change
    const handleInputChange = (field: keyof RegisterDeviceFormData, value: string | number) => {
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

        if (!formData.deviceCode.trim()) {
            errors.deviceCode = 'Device Code is required'
        }
        if (!formData.name.trim()) {
            errors.name = 'Device Name is required'
        }
        if (formData.name.trim().length < 2) {
            errors.name = 'Device Name must be at least 2 characters'
        }
        if (formData.name.trim().length > 150) {
            errors.name = 'Device Name must not exceed 150 characters'
        }
        if (!formData.deviceType) {
            errors.deviceType = 'Device Type is required'
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
            const result = await deviceService.register({
                deviceCode: formData.deviceCode.trim(),
                name: formData.name.trim(),
                deviceType: formData.deviceType as DeviceType,
                gatewayId: formData.gatewayId.trim() || undefined,
                firmwareVersion: formData.firmwareVersion.trim() || undefined
            })

            setSuccess(true)
            // Navigate back to device list after successful registration
            setTimeout(() => {
                navigate('/devices', { state: { successMessage: `Device ${result.deviceCode} registered successfully` } })
            }, 1500)
        } catch (err: any) {
            if (err.status === 400) {
                setError(err.message || 'Invalid input. Please check your entries.')
            } else if (err.status === 401) {
                setError('Your session has expired. Please log in again.')
            } else if (err.status === 403) {
                setError('You do not have permission to register devices.')
            } else if (err.status === 409) {
                setError('This Device Code already exists.')
            } else {
                setError(err.message || 'Failed to register device. Please try again.')
            }
        } finally {
            setIsLoading(false)
        }
    }

    // Handle back
    const handleBack = () => {
        navigate('/devices')
    }

    // Get device type entries for select dropdown
    const deviceTypes = Object.entries(DeviceType).filter(([key]) => isNaN(Number(key))) as Array<[string, DeviceType]>

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

            <Typography variant="h4" sx={{ fontWeight: 'bold', mb: 3 }}>
                Register New Device
            </Typography>

            {success && (
                <Alert severity="success" sx={{ mb: 2 }}>
                    Device registered successfully! Redirecting...
                </Alert>
            )}

            {error && (
                <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
                    {error}
                </Alert>
            )}

            <Card>
                <CardContent>
                    <form onSubmit={handleSubmit}>
                        <Grid container spacing={3}>
                            <Grid item xs={12}>
                                <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                                    Device Information
                                </Typography>
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <TextField
                                    fullWidth
                                    label="Device Code"
                                    placeholder="Enter unique device code..."
                                    value={formData.deviceCode}
                                    onChange={(e) => handleInputChange('deviceCode', e.target.value)}
                                    error={Boolean(formErrors.deviceCode)}
                                    helperText={formErrors.deviceCode}
                                    disabled={isLoading}
                                    required
                                />
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <TextField
                                    fullWidth
                                    label="Device Name"
                                    placeholder="Enter device name..."
                                    value={formData.name}
                                    onChange={(e) => handleInputChange('name', e.target.value)}
                                    error={Boolean(formErrors.name)}
                                    helperText={formErrors.name}
                                    disabled={isLoading}
                                    required
                                />
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <TextField
                                    fullWidth
                                    select
                                    label="Device Type"
                                    value={formData.deviceType}
                                    onChange={(e) => handleInputChange('deviceType', e.target.value)}
                                    error={Boolean(formErrors.deviceType)}
                                    helperText={formErrors.deviceType}
                                    disabled={isLoading}
                                    required
                                >
                                    <MenuItem value="">Select Device Type</MenuItem>
                                    {deviceTypes.map(([, value]) => (
                                        <MenuItem key={value} value={value}>
                                            {DeviceTypeLabel[value]}
                                        </MenuItem>
                                    ))}
                                </TextField>
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <TextField
                                    fullWidth
                                    label="Gateway ID (Optional)"
                                    placeholder="Enter gateway ID..."
                                    value={formData.gatewayId}
                                    onChange={(e) => handleInputChange('gatewayId', e.target.value)}
                                    disabled={isLoading}
                                />
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <TextField
                                    fullWidth
                                    label="Firmware Version (Optional)"
                                    placeholder="Enter firmware version..."
                                    value={formData.firmwareVersion}
                                    onChange={(e) => handleInputChange('firmwareVersion', e.target.value)}
                                    disabled={isLoading}
                                />
                            </Grid>

                            <Grid item xs={12}>
                                <Divider />
                            </Grid>

                            <Grid item xs={12}>
                                <Box sx={{ display: 'flex', gap: 1 }}>
                                    <Button
                                        type="submit"
                                        variant="contained"
                                        startIcon={<SaveIcon />}
                                        disabled={isLoading}
                                    >
                                        {isLoading ? 'Registering...' : 'Register Device'}
                                    </Button>
                                    <Button
                                        variant="outlined"
                                        onClick={handleBack}
                                        disabled={isLoading}
                                    >
                                        Cancel
                                    </Button>
                                </Box>
                            </Grid>
                        </Grid>
                    </form>
                </CardContent>
            </Card>
        </Box>
    )
}
