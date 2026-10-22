import React, { useState, useEffect } from 'react'
import {
  Box,
  Grid,
  //Paper,
  Typography,
  CircularProgress,
  Alert,
  Button,
  Card,
  CardContent,
  Stack
} from '@mui/material'
import RefreshIcon from '@mui/icons-material/Refresh'
import { apiClient } from '../../../services/apiClient'
import { DashboardSummary } from '../../../types/api.types'
import { formatDateTime } from '../../../utils/formatters'

const RefreshIconComponent = (RefreshIcon as any).default ?? RefreshIcon

export const DashboardPage: React.FC = () => {
  const [summary, setSummary] = useState<DashboardSummary | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [lastRefresh, setLastRefresh] = useState<string>(new Date().toISOString())

  const fetchSummary = React.useCallback(async () => {
    setIsLoading(true)
    setError(null)

    const response = await apiClient.get<DashboardSummary>('dashboard','/api/v1/dashboard/summary')

    if (response.error) {
      setError(response.error.message || 'Failed to load dashboard')
    } else if (response.data) {
      setSummary(response.data)
      setLastRefresh(new Date().toISOString())
    }

    setIsLoading(false)
  }, [])

  useEffect(() => {
    fetchSummary()

    // Refresh every 30 seconds
    const interval = setInterval(fetchSummary, 30000)
    return () => clearInterval(interval)
  }, [fetchSummary])

  if (isLoading && !summary) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    )
  }

  const MetricCard: React.FC<{ title: string; value: number; color?: string }> = ({
    title,
    value,
    color = '#1976d2'
  }) => (
    <Card>
      <CardContent>
        <Typography color="textSecondary" gutterBottom>
          {title}
        </Typography>
        <Typography
          variant="h4"
          sx={{
            fontWeight: 'bold',
            color: color
          }}
        >
          {value}
        </Typography>
      </CardContent>
    </Card>
  )

  return (
    <Box>
      <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 3 }}>
        <Typography variant="h4" sx={{ fontWeight: 'bold' }}>
          Dashboard
        </Typography>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Typography variant="caption" color="textSecondary">
            Last updated: {formatDateTime(lastRefresh)}
          </Typography>
            <Button
                startIcon={<RefreshIconComponent />}
                onClick={fetchSummary}
                disabled={isLoading}
            >
                Refresh
            </Button>
        </Box>
      </Stack>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {summary && (
        <>
          {/* Inventory Metrics */}
          <Typography variant="h6" sx={{ mb: 2, mt: 3, fontWeight: 'bold' }}>
            Inventory Metrics
          </Typography>
          <Grid container spacing={2} sx={{ mb: 3 }}>
            <Grid item xs={12} sm={6} md={3}>
              <MetricCard title="Total Assets" value={summary.totalAssets} />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <MetricCard title="Available" value={summary.availableAssets} color="#4caf50" />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <MetricCard title="In Use" value={summary.assetsInUse} color="#2196f3" />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <MetricCard
                title="Maintenance"
                value={summary.assetsUnderMaintenance}
                color="#ff9800"
              />
            </Grid>
          </Grid>

          {/* Device Metrics */}
          <Typography variant="h6" sx={{ mb: 2, mt: 3, fontWeight: 'bold' }}>
            Device Metrics
          </Typography>
          <Grid container spacing={2}>
            <Grid item xs={12} sm={6} md={3}>
              <MetricCard title="Total Devices" value={summary.totalDevices} />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <MetricCard title="Online" value={summary.onlineDevices} color="#4caf50" />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <MetricCard title="Offline" value={summary.offlineDevices} color="#f44336" />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <MetricCard title="Warning" value={summary.warningDevices} color="#ff9800" />
            </Grid>
            <Grid item xs={12} sm={6} md={3}>
              <MetricCard title="Critical" value={summary.criticalDevices} color="#d32f2f" />
            </Grid>
          </Grid>
        </>
      )}
    </Box>
  )
}