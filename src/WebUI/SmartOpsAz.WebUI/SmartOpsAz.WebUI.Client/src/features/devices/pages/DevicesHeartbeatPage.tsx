import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import {
  Box,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  CircularProgress,
  Alert,
  Button,
  Card,
  CardContent,
  Grid,
  Chip,
  Pagination,
  Stack
} from '@mui/material'
import { ArrowBack as ArrowBackIcon } from '@mui/icons-material'
import { deviceService } from '../../../services/deviceService'
import { IoTDevice, DeviceStatusColor, DeviceStatusLabel } from '../../../types/api.types'

interface Heartbeat {
  id: string
  deviceId: string
  recordedAtUtc: string
  batteryPercentage?: number
  signalStrengthRssi?: number
  cpuUsagePercentage?: number
  memoryUsagePercentage?: number
  firmwareVersion?: string
  receivedAtUtc: string
}

export default function DevicesHeartbeatPage() {
  const { deviceId } = useParams<{ deviceId: string }>()
  const navigate = useNavigate()

  const [device, setDevice] = useState<IoTDevice | null>(null)
  const [heartbeats, setHeartbeats] = useState<Heartbeat[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [page, setPage] = useState(1)
  const pageSize = 50

  useEffect(() => {
    loadData()
  }, [deviceId, page])

  const loadData = async () => {
    if (!deviceId) return

    setLoading(true)
    setError(null)

    try {
      // Load device details
      const deviceData = await deviceService.getById(deviceId)
      setDevice(deviceData)

      // Load heartbeats
      const heartbeatData = await deviceService.getHeartbeats(deviceId, page, pageSize)
      setHeartbeats(heartbeatData.items || [])
    } catch (err: any) {
      if (err.status === 401) {
        setError('Your session has expired. Please log in again.')
      } else if (err.status === 404) {
        setError('Device not found.')
      } else {
        setError(err.message || 'Failed to load heartbeat data.')
      }
    } finally {
      setLoading(false)
    }
  }

  const handlePageChange = (_: any, newPage: number) => {
    setPage(newPage)
  }

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Box>
    )
  }

  if (error) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="error">{error}</Alert>
        <Button 
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate(-1)}
          sx={{ mt: 2 }}
        >
          Go Back
        </Button>
      </Box>
    )
  }

  return (
    <Box sx={{ p: 3 }}>
      {/* Header */}
      <Box sx={{ mb: 4, display: 'flex', alignItems: 'center', gap: 2 }}>
        <Button 
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate(-1)}
          variant="outlined"
        >
          Back
        </Button>
        <Typography variant="h4">
          Device Heartbeat History
        </Typography>
      </Box>

      {/* Device Summary Card */}
      {device && (
        <Card sx={{ mb: 3 }}>
          <CardContent>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6} md={3}>
                <Typography color="textSecondary" gutterBottom>
                  Device Code
                </Typography>
                <Typography variant="h6">{device.deviceCode}</Typography>
              </Grid>
              <Grid item xs={12} sm={6} md={3}>
                <Typography color="textSecondary" gutterBottom>
                  Name
                </Typography>
                <Typography variant="h6">{device.name}</Typography>
              </Grid>
              <Grid item xs={12} sm={6} md={3}>
                <Typography color="textSecondary" gutterBottom>
                  Status
                </Typography>
                <Chip
                  label={DeviceStatusLabel[device.status]}
                  sx={{
                    backgroundColor: DeviceStatusColor[device.status],
                    color: 'white',
                    fontWeight: 'bold'
                  }}
                />
              </Grid>
              <Grid item xs={12} sm={6} md={3}>
                <Typography color="textSecondary" gutterBottom>
                  Last Heartbeat
                </Typography>
                <Typography variant="body2">
                  {device.lastHeartbeatAtUtc
                    ? new Date(device.lastHeartbeatAtUtc).toLocaleString()
                    : 'Never'}
                </Typography>
              </Grid>
            </Grid>
          </CardContent>
        </Card>
      )}

      {/* Heartbeats Table */}
      <TableContainer component={Paper}>
        <Table>
          <TableHead sx={{ backgroundColor: '#f5f5f5' }}>
            <TableRow>
              <TableCell><strong>Recorded At (UTC)</strong></TableCell>
              <TableCell align="right"><strong>Battery %</strong></TableCell>
              <TableCell align="right"><strong>Signal (dBm)</strong></TableCell>
              <TableCell align="right"><strong>CPU %</strong></TableCell>
              <TableCell align="right"><strong>Memory %</strong></TableCell>
              <TableCell><strong>Firmware</strong></TableCell>
              <TableCell><strong>Received At (UTC)</strong></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {heartbeats.length > 0 ? (
              heartbeats.map((hb) => (
                <TableRow key={hb.id} hover>
                  <TableCell>
                    {new Date(hb.recordedAtUtc).toLocaleString()}
                  </TableCell>
                  <TableCell align="right">
                    {hb.batteryPercentage !== null && hb.batteryPercentage !== undefined
                      ? `${hb.batteryPercentage.toFixed(1)}%`
                      : '-'}
                  </TableCell>
                  <TableCell align="right">
                    {hb.signalStrengthRssi !== null && hb.signalStrengthRssi !== undefined
                      ? hb.signalStrengthRssi
                      : '-'}
                  </TableCell>
                  <TableCell align="right">
                    {hb.cpuUsagePercentage !== null && hb.cpuUsagePercentage !== undefined
                      ? `${hb.cpuUsagePercentage.toFixed(1)}%`
                      : '-'}
                  </TableCell>
                  <TableCell align="right">
                    {hb.memoryUsagePercentage !== null && hb.memoryUsagePercentage !== undefined
                      ? `${hb.memoryUsagePercentage.toFixed(1)}%`
                      : '-'}
                  </TableCell>
                  <TableCell>{hb.firmwareVersion || '-'}</TableCell>
                  <TableCell>
                    {new Date(hb.receivedAtUtc).toLocaleString()}
                  </TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={7} align="center" sx={{ py: 3 }}>
                  <Typography color="textSecondary">No heartbeat records found</Typography>
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Pagination */}
      {heartbeats.length > 0 && (
        <Stack sx={{ mt: 2, display: 'flex', alignItems: 'center' }}>
          <Pagination
            count={Math.ceil(heartbeats.length / pageSize)}
            page={page}
            onChange={handlePageChange}
            color="primary"
          />
        </Stack>
      )}
    </Box>
  )
}
