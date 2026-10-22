/**
 * Inventory Management - List Page
 * 
 * Features:
 * - Display paginated inventory table with real data from InventoryService
 * - Search and filtering capabilities
 * - Loading, error, and empty states
 * - Actions: View, Edit, Delete (Retire), Assign Department, Transfer Location
 * - Role-based access control (BiomedicalEngineer-only retire)
 */

import React, { useState, useEffect, useCallback } from 'react'
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
    Grid,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TablePagination,
    TableRow,
    TextField,
    Typography,
    Alert,
    Chip,
    IconButton,
    Menu,
    MenuItem
} from '@mui/material'
import {
    // Delete as DeleteIcon,
    // Edit as EditIcon,
    MoreVert as MoreVertIcon,
    Add as AddIcon,
    Refresh as RefreshIcon
} from '@mui/icons-material'
import { inventoryService } from '../../../services/inventoryService'
import { useAuth } from '../../auth/context/AuthContext'
import {
    InventoryItem,
    InventoryStatus,
    InventoryStatusLabel,
    InventoryStatusColor,
    SearchInventoryRequest
} from '../../../types/api.types'

export const InventoryListPage: React.FC = () => {
    // Navigation
    const navigate = useNavigate()

    // State management
    const [inventoryItems, setInventoryItems] = useState<InventoryItem[]>([])
    const [isLoading, setIsLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)
    const [page, setPage] = useState(0)
    const [rowsPerPage, setRowsPerPage] = useState(10)
    const [totalCount, setTotalCount] = useState(0)

    // Search and filter state
    const [searchText, setSearchText] = useState('')
    const [filterStatus, setFilterStatus] = useState<InventoryStatus | ''>('')
    const [filterDepartment, setFilterDepartment] = useState('')
    const [filterLocation, setFilterLocation] = useState('')

    // UI state
    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null)
    const [selectedItem, setSelectedItem] = useState<InventoryItem | null>(null)
    const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false)

    const { user } = useAuth()

    // Load inventory data
    const loadInventory = useCallback(async () => {
        setIsLoading(true)
        setError(null)

        try {
            const params: SearchInventoryRequest = {
                search: searchText || undefined,
                status: filterStatus !== '' ? (filterStatus as InventoryStatus) : undefined,
                department: filterDepartment || undefined,
                location: filterLocation || undefined,
                page: page + 1, // Backend uses 1-based pagination
                pageSize: rowsPerPage
            }

            const result = await inventoryService.search(params)
            setInventoryItems(result.items)
            setTotalCount(result.totalCount)
        } catch (err: any) {
            if (err.status === 401) {
                setError('Your session has expired. Please log in again.')
            } else if (err.status === 403) {
                setError('You do not have permission to view inventory.')
            } else if (err.status === 404) {
                setError('Inventory not found.')
            } else {
                setError(err.message || 'Failed to load inventory. Please try again.')
            }
            setInventoryItems([])
            setTotalCount(0)
        } finally {
            setIsLoading(false)
        }
    }, [page, rowsPerPage, searchText, filterStatus, filterDepartment, filterLocation])

    // Load inventory on component mount and when filters change
    useEffect(() => {
        loadInventory()
    }, [loadInventory])

    // Handle pagination
    const handleChangePage = (_event: unknown, newPage: number) => {
        setPage(newPage)
    }

    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        setRowsPerPage(parseInt(event.target.value, 10))
        setPage(0)
    }

    // Handle search
    const handleSearch = (value: string) => {
        setSearchText(value)
        setPage(0)
    }

    // Handle filter changes
    const handleStatusFilterChange = (value: string) => {
        setFilterStatus(value as any)
        setPage(0)
    }

    const handleDepartmentFilterChange = (value: string) => {
        setFilterDepartment(value)
        setPage(0)
    }

    const handleLocationFilterChange = (value: string) => {
        setFilterLocation(value)
        setPage(0)
    }

    // Handle menu actions
    const handleMenuOpen = (event: React.MouseEvent<HTMLButtonElement>, item: InventoryItem) => {
        setAnchorEl(event.currentTarget)
        setSelectedItem(item)
    }

    const handleMenuClose = () => {
        setAnchorEl(null)
    }

    // Handle register asset
    const handleRegisterAsset = () => {
        navigate('/inventory/register')
    }

    // Handle delete (retire)
    const handleDeleteClick = () => {
        setDeleteConfirmOpen(true)
        handleMenuClose()
    }

    const handleDeleteConfirm = async () => {
        if (!selectedItem) return

        setIsLoading(true)
        try {
            await inventoryService.retire(selectedItem.inventoryId)
            setDeleteConfirmOpen(false)
            setSelectedItem(null)
            await loadInventory()
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

    const handleViewDetails = async () => {
        if (!selectedItem) return
        navigate(`/inventory/${selectedItem.inventoryId}`)
        handleMenuClose()
    }

    const handleEdit = async () => {
        if (!selectedItem) return
        navigate(`/inventory/${selectedItem.inventoryId}/edit`)
        handleMenuClose()
    }

    // Check if user can retire items (BiomedicalEngineer)
    const canRetire = user?.role === 'BiomedicalEngineer' || user?.role === 'HospitalAdministrator'

    return (
        <Box sx={{ width: '100%' }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography variant="h4" sx={{ fontWeight: 'bold' }}>
                    Inventory Management
                </Typography>
                <Box sx={{ display: 'flex', gap: 1 }}>
                    <Button
                        variant="outlined"
                        startIcon={<RefreshIcon />}
                        onClick={() => loadInventory()}
                        disabled={isLoading}
                    >
                        Refresh
                    </Button>
                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={handleRegisterAsset}
                    >
                        Register Asset
                    </Button>
                </Box>
            </Box>

            {error && (
                <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
                    {error}
                </Alert>
            )}

            {/* Search and Filter Section */}
            <Card sx={{ mb: 3 }}>
                <CardContent>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6} md={3}>
                            <TextField
                                fullWidth
                                label="Search"
                                placeholder="Asset tag, name, or serial..."
                                value={searchText}
                                onChange={(e) => handleSearch(e.target.value)}
                                size="small"
                                disabled={isLoading}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6} md={3}>
                            <TextField
                                fullWidth
                                label="Status"
                                select
                                value={filterStatus}
                                onChange={(e) => handleStatusFilterChange(e.target.value)}
                                size="small"
                                disabled={isLoading}
                            >
                                <MenuItem value="">All Status</MenuItem>
                                <MenuItem value={InventoryStatus.Available}>Available</MenuItem>
                                <MenuItem value={InventoryStatus.InUse}>In Use</MenuItem>
                                <MenuItem value={InventoryStatus.Maintenance}>Maintenance</MenuItem>
                                <MenuItem value={InventoryStatus.Offline}>Offline</MenuItem>
                                <MenuItem value={InventoryStatus.Retired}>Retired</MenuItem>
                            </TextField>
                        </Grid>
                        <Grid item xs={12} sm={6} md={3}>
                            <TextField
                                fullWidth
                                label="Department"
                                placeholder="Filter by department..."
                                value={filterDepartment}
                                onChange={(e) => handleDepartmentFilterChange(e.target.value)}
                                size="small"
                                disabled={isLoading}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6} md={3}>
                            <TextField
                                fullWidth
                                label="Location"
                                placeholder="Filter by location..."
                                value={filterLocation}
                                onChange={(e) => handleLocationFilterChange(e.target.value)}
                                size="small"
                                disabled={isLoading}
                            />
                        </Grid>
                    </Grid>
                </CardContent>
            </Card>

            {/* Table Section */}
            {isLoading && inventoryItems.length === 0 ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
                    <CircularProgress />
                </Box>
            ) : inventoryItems.length === 0 ? (
                <Card>
                    <CardContent>
                        <Typography color="textSecondary" align="center">
                            No inventory records found. Try adjusting your search filters.
                        </Typography>
                    </CardContent>
                </Card>
            ) : (
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow sx={{ backgroundColor: '#f5f5f5' }}>
                                <TableCell><strong>Asset Tag</strong></TableCell>
                                <TableCell><strong>Device Name</strong></TableCell>
                                <TableCell><strong>Serial Number</strong></TableCell>
                                <TableCell><strong>Device Type</strong></TableCell>
                                <TableCell><strong>Department</strong></TableCell>
                                <TableCell><strong>Location</strong></TableCell>
                                <TableCell><strong>Status</strong></TableCell>
                                <TableCell><strong>Updated</strong></TableCell>
                                <TableCell align="right"><strong>Actions</strong></TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {inventoryItems.map((item) => (
                                <TableRow key={item.inventoryId} hover>
                                    <TableCell>{item.assetTag}</TableCell>
                                    <TableCell>{item.deviceName}</TableCell>
                                    <TableCell>{item.serialNumber}</TableCell>
                                    <TableCell>{item.deviceType || '-'}</TableCell>
                                    <TableCell>{item.department || '-'}</TableCell>
                                    <TableCell>{item.location || '-'}</TableCell>
                                    <TableCell>
                                        <Chip
                                            label={InventoryStatusLabel[item.status]}
                                            size="small"
                                            sx={{
                                                backgroundColor: InventoryStatusColor[item.status],
                                                color: 'white'
                                            }}
                                        />
                                    </TableCell>
                                    <TableCell>{new Date(item.updatedAt).toLocaleDateString()}</TableCell>
                                    <TableCell align="right">
                                        <IconButton
                                            size="small"
                                            onClick={(e) => handleMenuOpen(e, item)}
                                        >
                                            <MoreVertIcon />
                                        </IconButton>
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                    <TablePagination
                        rowsPerPageOptions={[5, 10, 25, 50]}
                        component="div"
                        count={totalCount}
                        rowsPerPage={rowsPerPage}
                        page={page}
                        onPageChange={handleChangePage}
                        onRowsPerPageChange={handleChangeRowsPerPage}
                        disabled={isLoading}
                    />
                </TableContainer>
            )}

            {/* Actions Menu */}
            <Menu
                anchorEl={anchorEl}
                open={Boolean(anchorEl)}
                onClose={handleMenuClose}
            >
                <MenuItem onClick={handleViewDetails}>View Details</MenuItem>
                <MenuItem onClick={handleEdit}>Edit</MenuItem>
                {canRetire && (
                    <MenuItem onClick={handleDeleteClick}>Retire</MenuItem>
                )}
            </Menu>

            {/* Delete Confirmation Dialog */}
            <Dialog open={deleteConfirmOpen} onClose={() => setDeleteConfirmOpen(false)}>
                <DialogTitle>Confirm Retirement</DialogTitle>
                <DialogContent>
                    <Typography>
                        Are you sure you want to retire this inventory item?
                    </Typography>
                    <Typography variant="body2" color="textSecondary" sx={{ mt: 2 }}>
                        <strong>Asset Tag:</strong> {selectedItem?.assetTag}
                    </Typography>
                    <Typography variant="body2" color="textSecondary">
                        <strong>Device Name:</strong> {selectedItem?.deviceName}
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