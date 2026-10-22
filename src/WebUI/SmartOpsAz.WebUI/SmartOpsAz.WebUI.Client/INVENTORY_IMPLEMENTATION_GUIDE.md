# Inventory Management - Implementation Guide

## Overview

The Inventory Management page now displays real data from PostgreSQL through the InventoryService backend. This guide explains the implementation and how to extend it.

## Architecture
┌─────────────────────────────────┐ │   InventoryListPage.tsx         │ │  (Material UI DataGrid Table)   │ └──────────────┬──────────────────┘ │ ▼ ┌─────────────────────────────────┐ │   inventoryService.ts           │ │  (Wraps apiClient for inventory)│ └──────────────┬──────────────────┘ │ ▼ ┌─────────────────────────────────┐ │   apiClient.ts                  │ │  (Multi-service Axios client)   │ └──────────────┬──────────────────┘ │ ▼ ┌─────────────────────────────────┐ │   Vite Dev Proxy                │ │  /inventory-api → :5102         │ └──────────────┬──────────────────┘ │ ▼ ┌─────────────────────────────────┐ │   InventoryService Backend      │ │  (Port 5102)                    │ └─────────────────────────────────┘

## Key Components

### 1. InventoryListPage.tsx

**Location**: `src/features/inventory/pages/InventoryListPage.tsx`

**Features**:
- Display paginated inventory table
- Search by asset tag, name, serial number
- Filter by status, department, location, device type
- Pagination (5, 10, 25, 50 rows per page)
- Actions menu (View, Edit, Retire)
- Role-based access (BiomedicalEngineer-only retire)
- Loading, error, and empty states

**Key Functions**:
- `loadInventory()` - Fetch data from backend
- `handleSearch()` - Update search filter
- `handleStatusFilterChange()` - Filter by inventory status
- `handleDepartmentFilterChange()` - Filter by department
- `handleLocationFilterChange()` - Filter by location
- `handleDeleteConfirm()` - Soft delete (retire) inventory

### 2. inventoryService.ts

**Location**: `src/services/inventoryService.ts`

**Methods**:
// Search inventory with filtering and pagination search(params: SearchInventoryRequest): Promise<SearchInventoryResponse>
// Get single inventory by ID getById(id: string): Promise<InventoryItem>
// Register new inventory register(data: RegisterInventoryRequest): Promise<RegisterInventoryResponse>
// Update inventory update(id: string, data: UpdateInventoryRequest): Promise<InventoryItem>
// Retire (soft delete) inventory retire(id: string): Promise<void>
// Assign to department assignDepartment(id: string, data: AssignDepartmentRequest): Promise<InventoryItem>
// Transfer to location transferLocation(id: string, data: TransferLocationRequest): Promise<InventoryItem>
// Get movement history getHistory(id: string): Promise<InventoryItem[]>
### 3. Type Definitions

**Location**: `src/types/api.types.ts`

**Key Types**:
- `InventoryStatus` - Enum (Available, InUse, Maintenance, Offline, Retired)
- `InventoryItem` - Single inventory record from backend
- `SearchInventoryResponse` - Paginated search results
- `SearchInventoryRequest` - Search parameters
- `InventoryStatusLabel` - Human-readable status names
- `InventoryStatusColor` - Status color codes

## API Integration

### Backend Endpoint

**Service**: InventoryService (Port 5102)  
**Endpoint**: `GET /api/inventory/search`  
**Auth**: Required (JWT Bearer)

**Query Parameters**:
search: string (optional)           - Text search status: int (optional)              - Status filter (0-4) department: string (optional)       - Department filter location: string (optional)         - Location filter deviceType: string (optional)       - Device type filter page: int (default: 1)             - Page number (1-based) pageSize: int (default: 20)        - Items per page


**Response**:

### Error Handling

**401 Unauthorized**: Token expired or invalid
- Handled automatically by apiClient interceptor
- User redirected to login page

**403 Forbidden**: User lacks permissions
- Caught in try/catch block
- Display "Access Denied" message
- Retry button available

**404 Not Found**: Resource not found
- Caught in try/catch block
- Display "Not found" message

**500 Server Error**: Backend error
- Caught in try/catch block
- Display error message from backend

## Usage Examples

### Load All Inventory
import { inventoryService } from '@/services/inventoryService'
const result = await inventoryService.search() // Default: page 1, pageSize 20

### Search with Filters
import { inventoryService } from '@/services/inventoryService' import { InventoryStatus } from '@/types/api.types'
const result = await inventoryService.search({ search: 'monitor', status: InventoryStatus.InUse, department: 'ICU', page: 1, pageSize: 10 })


### Get Single Item
import { inventoryService } from '@/services/inventoryService'
const item = await inventoryService.getById('123e4567-e89b-12d3-a456-426614174000') console.log(item.assetTag)


### Register New Inventory
import { inventoryService } from '@/services/inventoryService'
const result = await inventoryService.register({ assetTag: 'INV-002', serialNumber: 'SN-67890', deviceName: 'ECG Monitor', deviceType: 'Monitor' }) console.log(result.inventoryId) // New UUID


### Retire Inventory (BiomedicalEngineer only)
import { inventoryService } from '@/services/inventoryService'
try { await inventoryService.retire('123e4567-e89b-12d3-a456-426614174000') console.log('Inventory retired successfully') } catch (error) { if (error.status === 403) { console.log('Only BiomedicalEngineers can retire inventory') } else { console.log('Failed to retire:', error.message) } }


### Assign to Department
import { inventoryService } from '@/services/inventoryService'
const updated = await inventoryService.assignDepartment( '123e4567-e89b-12d3-a456-426614174000', { department: 'Emergency' } ) console.log(updated.department) // 'Emergency'

### Get Movement History
import { inventoryService } from '@/services/inventoryService'
const history = await inventoryService.getHistory('123e4567-e89b-12d3-a456-426614174000') console.log(history) // Array of inventory states over time


## Testing

### Run Tests
npm run test -- --run

### Test Coverage

- `inventoryService.test.ts`:
  - Search with default parameters ✓
  - Search with filters ✓
  - Error handling (401, 403, 500) ✓
  - Register new inventory ✓
  - Retire inventory ✓
  - Get by ID ✓
  - Get history ✓

## Verification Checklist

- [x] Backend search endpoint working
- [x] JWT authentication flows
- [x] Type definitions match backend DTOs
- [x] Pagination working correctly
- [x] Search/filter functionality
- [x] Error handling for all status codes
- [x] Loading states display
- [x] Empty state displays
- [x] Role-based UI (BiomedicalEngineer retire)
- [x] API routes correctly proxied
- [x] No hardcoded data
- [x] Tests passing

## Known Limitations

1. **Edit and View Details**: Menu items not yet implemented (TODO)
2. **Bulk Operations**: No multi-select or bulk actions
3. **Export**: No CSV/Excel export
4. **Real-time Updates**: No WebSocket updates, manual refresh only
5. **Advanced Filters**: Limited to simple dropdown filters

## Future Enhancements

### Short-term (Next Sprint)
- [ ] Implement View Details dialog
- [ ] Implement Edit dialog
- [ ] Add Register Asset modal
- [ ] Add Assign Department modal
- [ ] Add Transfer Location modal

### Medium-term (Next Quarter)
- [ ] Export to CSV/Excel
- [ ] Bulk operations (retire multiple, assign department)
- [ ] Advanced filters (date range, multiple values)
- [ ] Sort by columns
- [ ] Favorites/Starred items

### Long-term (Future)
- [ ] Real-time updates via WebSocket
- [ ] Barcode/QR code scanning
- [ ] Mobile app integration
- [ ] Asset condition monitoring
- [ ] Predictive maintenance alerts

## Troubleshooting

### No Data Displays

1. Verify backend service running: `http://localhost:5102/swagger`
2. Check browser DevTools Network tab
3. Verify JWT token in request headers
4. Check backend logs for errors

### CORS Errors

1. Verify vite.config.ts proxy configuration
2. Check backend CORS headers
3. Verify proxy rewrite rules

### 401 Errors

1. Login again to refresh token
2. Check sessionStorage for accessToken
3. Verify token not expired

### 403 Errors

1. Check user role
2. Verify backend authorization policies
3. Check backend logs for authorization failures

## Configuration

### Local Development

**Environment File**: `.env.development`
VITE_INVENTORY_API_URL=/inventory-api
**Vite Proxy**: `vite.config.ts`
/inventory-api → http://localhost:5102
### Production

**Environment File**: `.env.production`
VITE_INVENTORY_API_URL=http://inventory-service:5102

Or with API Gateway:
VITE_INVENTORY_API_URL=https://api.smartops.com/inventory


## Performance Considerations

- **Page Size**: Default 20 items, configurable to 50 max
- **Search Debounce**: Implement search debounce to reduce requests (TODO)
- **Pagination**: Use offset-based pagination (currently supported)
- **Caching**: Consider implementing request caching (TODO)

## Security

- ✓ JWT tokens not exposed in code
- ✓ 403 errors don't auto-logout
- ✓ Soft delete only (no hard delete)
- ✓ Role-based UI enforcement
- ✓ No sensitive data in URLs

## Support

- **Backend Docs**: http://localhost:5102/swagger
- **GitHub**: https://github.com/sumonnwe/SmartOpsAz
- **Issues**: Check GitHub Issues for known problems
