import { useState, useEffect } from 'react' 
import { useNavigate } from 'react-router-dom'
import {
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
  Alert,
  Chip,
  Pagination,
  SelectChangeEvent,
  Tooltip,
  IconButton
} from '@mui/material'
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Visibility as VisibilityIcon,
  Search as SearchIcon,
  RestartAlt as RestartIcon
} from '@mui/icons-material'
import { deviceService } from '../../../services/deviceService'
import { 
  IoTDevice,
  DeviceStatusColor,
  DeviceStatusLabel,
  DeviceTypeLabel
} from '../../../types/api.types'

export default function DevicesSearchPage() {
  const navigate = useNavigate()

  const [devices, setDevices] = useState<IoTDevice[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [page, setPage] = useState(1)
  const pageSize = 20
  const [totalCount, setTotalCount] = useState(0)

  // Filter states
  const [filters, setFilters] = useState({
    deviceCode: '',
    status: '',
    gatewayId: ''
  })

  // Dialog states
  const [deleteDialog, setDeleteDialog] = useState(false)
  const [selectedDevice, setSelectedDevice] = useState<IoTDevice | null>(null)
  const [deleting, setDeleting] = useState(false)

  useEffect(() => {
    loadDevices()
  }, [page])

  const loadDevices = async () => {
    setLoading(true)
    setError(null)

    try {
      const response = await deviceService.advancedSearch({
        deviceCode: filters.deviceCode || undefined,
        status: filters.status ? parseInt(filters.status) : undefined,
        gatewayId: filters.gatewayId || undefined,
        page,
        pageSize
      })
      setDevices(response.items)
      setTotalCount(response.totalCount || 0)
    } catch (err: any) {
      if (err.status === 401) {
        setError('Your session has expired. Please log in again.')
      } else {
        setError(err.message || 'Failed to load devices.')
      }
    } finally {
      setLoading(false)
    }
  }

  const handleFilterChange = (field: string, value: string) => {
    setFilters(prev => ({
      ...prev,
      [field]: value
    }))
    setPage(1)
  }

  const handleSearch = () => {
    setPage(1)
    loadDevices()
  }

  const handleReset = () => {
    setFilters({
      deviceCode: '',
      status: '',
      gatewayId: ''
    })
    setPage(1)
  }

  const handleDeleteClick = (device: IoTDevice) => {
    setSelectedDevice(device)
    setDeleteDialog(true)
  }

  const handleDeleteConfirm = async () => {
    if (!selectedDevice) return

    setDeleting(true)
    try {
      await deviceService.delete(selectedDevice.id)
      setDeleteDialog(false)
      setSelectedDevice(null)
      loadDevices()
    } catch (err: any) {
      setError(err.message || 'Failed to delete device.')
    } finally {
      setDeleting(false)
    }
  }

  const handlePageChange = (_: any, newPage: number) => {
    setPage(newPage)
  }

  const totalPages = Math.ceil(totalCount / pageSize)

  return (
    <Box sx={{ p: 3 }}>
      {/* Header */}
      <Box sx={{ mb: 4, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4">Devices</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => navigate('/devices/register')}
        >
          Register Device
        </Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {/* Filters Card */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Grid container spacing={2} alignItems="flex-end">
            <Grid item xs={12} sm={6} md={3}>
              <TextField
                label="Device Code"
                fullWidth
                value={filters.deviceCode}
                onChange={(e) => handleFilterChange('deviceCode', e.target.value)}
                placeholder="Filter by device code"
              />
            </Grid>

            <Grid item xs={12} sm={6} md={3}>
              <FormControl fullWidth>
                <InputLabel>Status</InputLabel>
                <Select
                  value={filters.status}
                  label="Status"
                  onChange={(e: SelectChangeEvent) => handleFilterChange('status', e.target.value)}
                >
                  <MenuItem value="">All</MenuItem>
                  {Object.entries(DeviceStatusLabel).map(([key, label]) => (
                    <MenuItem key={key} value={key}>
                      {label}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6} md={3}>
              <TextField
                label="Gateway ID"
                fullWidth
                value={filters.gatewayId}
                onChange={(e) => handleFilterChange('gatewayId', e.target.value)}
                placeholder="Filter by gateway"
              />
            </Grid>

            <Grid item xs={12} sm={6} md={3} sx={{ display: 'flex', gap: 1 }}>
              <Button
                variant="contained"
                startIcon={<SearchIcon />}
                onClick={handleSearch}
                fullWidth
              >
                Search
              </Button>
              <Tooltip title="Reset filters">
                <Button
                  variant="outlined"
                  onClick={handleReset}
                  sx={{ minWidth: 40, p: 0 }}
                >
                  <RestartIcon />
                </Button>
              </Tooltip>
            </Grid>
          </Grid>
        </CardContent>
      </Card>

      {/* Results */}
      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 5 }}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          <TableContainer component={Paper}>
            <Table>
              <TableHead sx={{ backgroundColor: '#f5f5f5' }}>
                <TableRow>
                  <TableCell><strong>Device Code</strong></TableCell>
                  <TableCell><strong>Name</strong></TableCell>
                  <TableCell><strong>Type</strong></TableCell>
                  <TableCell><strong>Status</strong></TableCell>
                  <TableCell><strong>Battery %</strong></TableCell>
                  <TableCell><strong>Last Heartbeat</strong></TableCell>
                  <TableCell align="right"><strong>Actions</strong></TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {devices.length > 0 ? (
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
                            color: 'white',
                            fontWeight: 'bold'
                          }}
                        />
                      </TableCell>
                      <TableCell>
                        {device.batteryPercentage !== null && device.batteryPercentage !== undefined
                          ? `${device.batteryPercentage.toFixed(1)}%`
                          : '-'}
                      </TableCell>
                      <TableCell>
                        {device.lastHeartbeatAtUtc
                          ? new Date(device.lastHeartbeatAtUtc).toLocaleString()
                          : 'Never'}
                      </TableCell>
                      <TableCell align="right">
                        <Tooltip title="View Details">
                          <IconButton
                            size="small"
                            onClick={() => navigate(`/devices/${device.id}`)}
                          >
                            <VisibilityIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Edit">
                          <IconButton
                            size="small"
                            onClick={() => navigate(`/devices/${device.id}/edit`)}
                          >
                            <EditIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Delete">
                          <IconButton
                            size="small"
                            onClick={() => handleDeleteClick(device)}
                            color="error"
                          >
                            <DeleteIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                      </TableCell>
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell colSpan={7} align="center" sx={{ py: 3 }}>
                      <Typography color="textSecondary">No devices found</Typography>
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>

          {/* Pagination */}
          {totalPages > 1 && (
            <Stack sx={{ mt: 2, display: 'flex', alignItems: 'center' }}>
              <Pagination
                count={totalPages}
                page={page}
                onChange={handlePageChange}
                color="primary"
              />
              <Typography variant="caption" sx={{ mt: 1 }} color="textSecondary">
                Showing {devices.length} of {totalCount} devices
              </Typography>
            </Stack>
          )}
        </>
      )}

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialog} onClose={() => setDeleteDialog(false)}>
        <DialogTitle>Delete Device</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete device <strong>{selectedDevice?.deviceCode}</strong>?
          </Typography>
          <Typography variant="caption" color="textSecondary" sx={{ mt: 2, display: 'block' }}>
            This action cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialog(false)}>Cancel</Button>
          <Button
            onClick={handleDeleteConfirm}
            color="error"
            variant="contained"
            disabled={deleting}
          >
            {deleting ? <CircularProgress size={24} /> : 'Delete'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  )
}
