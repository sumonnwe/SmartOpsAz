/**
 * Device Management - Edit Page
 * 
 * Features:
 * - Edit existing device information
 * - Form validation
 * - Load current data on mount
 * - Success/error handling
 * - Navigation back to detail page
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
    TextField,
    Typography,
    Alert,
    Divider
} from '@mui/material'
import {
    ArrowBack as ArrowBackIcon,
    Save as SaveIcon
} from '@mui/icons-material'
import { deviceService } from '../../../services/deviceService'
import { IoTDevice } from '../../../types/api.types'

interface EditDeviceFormData {
    name: string
    firmwareVersion: string
}

export const DevicesEditPage: React.FC = () => {
    const { id } = useParams<{ id: string }>()
    const navigate = useNavigate()

    // Form state
    const [formData, setFormData] = useState<EditDeviceFormData>({
        name: '',
        firmwareVersion: ''
    })

    // UI state
    const [device, setDevice] = useState<IoTDevice | null>(null)
    const [isLoading, setIsLoading] = useState(true)
    const [isSaving, setIsSaving] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [success, setSuccess] = useState(false)
    const [formErrors, setFormErrors] = useState<Record<string, string>>({})

    // Load device on mount
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
                setFormData({
                    name: result.name || '',
                    firmwareVersion: result.firmwareVersion || ''
                })
            } catch (err: any) {
                if (err.status === 401) {
                    setError('Your session has expired. Please log in again.')
                } else if (err.status === 403) {
                    setError('You do not have permission to edit this device.')
                } else if (err.status === 404) {
                    setError('Device not found.')
                } else {
                    setError(err.message || 'Failed to load device.')
                }
            } finally {
                setIsLoading(false)
            }
        }

        loadDevice()
    }, [id])

    // Handle input change
    const handleInputChange = (field: keyof EditDeviceFormData, value: string) => {
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

        if (!formData.name.trim()) {
            errors.name = 'Device Name is required'
        }
        if (formData.name.trim().length < 2) {
            errors.name = 'Device Name must be at least 2 characters'
        }
        if (formData.name.trim().length > 150) {
            errors.name = 'Device Name must not exceed 150 characters'
        }

        setFormErrors(errors)
        return Object.keys(errors).length === 0
    }

    // Handle form submission
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()

        if (!validateForm() || !device || !id) {
            return
        }

        setIsSaving(true)
        setError(null)
        setSuccess(false)

        try {
            await deviceService.update(id, {
                name: formData.name.trim(),
                firmwareVersion: formData.firmwareVersion.trim() || undefined
            })

            setSuccess(true)
            // Navigate back to detail page after successful update
            setTimeout(() => {
                navigate(`/devices/${id}`, { state: { successMessage: 'Device updated successfully' } })
            }, 1500)
        } catch (err: any) {
            if (err.status === 400) {
                setError(err.message || 'Invalid input. Please check your entries.')
            } else if (err.status === 401) {
                setError('Your session has expired. Please log in again.')
            } else if (err.status === 403) {
                setError('You do not have permission to edit this device.')
            } else if (err.status === 404) {
                setError('Device not found.')
            } else {
                setError(err.message || 'Failed to update device.')
            }
        } finally {
            setIsSaving(false)
        }
    }

    // Handle back
    const handleBack = () => {
        navigate(`/devices/${id}`)
    }

    if (isLoading) {
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

            <Typography variant="h4" sx={{ fontWeight: 'bold', mb: 3 }}>
                Edit Device: {device.name}
            </Typography>

            {success && (
                <Alert severity="success" sx={{ mb: 2 }}>
                    Device updated successfully! Redirecting...
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
                                    value={device.deviceCode}
                                    disabled
                                    helperText="Device code cannot be changed"
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
                                    disabled={isLoading || isSaving}
                                    required
                                />
                            </Grid>

                            <Grid item xs={12} sm={6}>
                                <TextField
                                    fullWidth
                                    label="Firmware Version"
                                    placeholder="Enter firmware version..."
                                    value={formData.firmwareVersion}
                                    onChange={(e) => handleInputChange('firmwareVersion', e.target.value)}
                                    disabled={isLoading || isSaving}
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
                                        disabled={isLoading || isSaving}
                                    >
                                        {isSaving ? 'Saving...' : 'Save Changes'}
                                    </Button>
                                    <Button
                                        variant="outlined"
                                        onClick={handleBack}
                                        disabled={isLoading || isSaving}
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
