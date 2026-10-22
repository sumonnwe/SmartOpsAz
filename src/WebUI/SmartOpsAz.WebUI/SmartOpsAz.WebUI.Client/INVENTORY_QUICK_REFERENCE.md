# Inventory Management - Quick Reference

## Starting the Application

### Backend Services
Terminal 1: IdentityService (5101)
cd src/IdentityService/IdentityService.Api dotnet run --urls="https://localhost:5101"
Terminal 2: InventoryService (5102)
cd src/InventoryService/InventoryService.Api dotnet run --urls="https://localhost:5102"
Terminal 3: DashboardService (5104) - optional
cd src/DashboardService/DashboardService.Api dotnet run --urls="https://localhost:5104"
Terminal 4: DeviceService (5103) - optional
cd src/DeviceService/DeviceService.Api dotnet run --urls="https://localhost:5103"

### Frontend
````````
cd src/WebUI/SmartOpsAz.WebUI npm install npm run dev


### Access Application
http://localhost:5173


## Using Inventory Management

### Login
1. Navigate to http://localhost:5173/login
2. Enter credentials (check backend seeded data)
3. Click Login

### View Inventory
1. Click "Inventory Management" in left sidebar
2. See table with all inventory items from PostgreSQL

### Search Inventory
1. Enter text in "Search" field (searches: asset tag, name, serial)
2. Results update automatically
3. Click Refresh to reload

### Filter Inventory
1. Use Status dropdown (Available, In Use, Maintenance, Offline, Retired)
2. Use Department text field
3. Use Location text field
4. Filters apply immediately

### Change Page Size
1. Click rows per page dropdown (5, 10, 25, 50)
2. Select desired size
3. Table updates automatically

### Pagination
1. Use arrow buttons at bottom to navigate pages
2. Click specific page number to jump
3. Current page shown in center

### Retire Inventory
1. Click three-dot menu (⋮) on inventory row
2. Click "Retire" (only visible for BiomedicalEngineer)
3. Confirm in dialog
4. Table updates automatically

## File Locations
Frontend:
•	src/features/inventory/pages/InventoryListPage.tsx     (Main page)
•	src/services/inventoryService.ts                       (API client)
•	src/types/api.types.ts                                 (Types)
•	src/services/inventoryService.test.ts                  (Tests)
Documentation:
•	INVENTORY_INTEGRATION_ANALYSIS.md                      (Architecture)
•	INVENTORY_IMPLEMENTATION_GUIDE.md                      (Detailed guide)
•	INVENTORY_IMPLEMENTATION_SUMMARY.md                    (Summary)
•	INVENTORY_QUICK_REFERENCE.md                           (This file)
Backend:
•	src/InventoryService/InventoryService.Api/Controllers/InventoryController.cs
•	src/InventoryService/InventoryService.Api/Program.cs
•	http://localhost:5102/swagger

## API Endpoint Reference

| Feature | Method | Endpoint | Frontend Method |
|---------|--------|----------|-----------------|
| List/Search | GET | `/api/inventory/search?...` | `inventoryService.search()` |
| Get by ID | GET | `/api/inventory/{id}` | `inventoryService.getById()` |
| Create | POST | `/api/inventory` | `inventoryService.register()` |
| Update | PUT | `/api/inventory/{id}` | `inventoryService.update()` |
| Retire | DELETE | `/api/inventory/{id}` | `inventoryService.retire()` |
| Assign Dept | PUT | `/api/inventory/{id}/department` | `inventoryService.assignDepartment()` |
| Transfer Loc | PUT | `/api/inventory/{id}/location` | `inventoryService.transferLocation()` |
| History | GET | `/api/inventory/{id}/history` | `inventoryService.getHistory()` |

## Common Issues & Solutions

### No Data Displays
1.	Check backend running: http://localhost:5102/swagger
2.	Check Network tab in DevTools for errors
3.	Verify JWT token in request headers
4.	Check browser console for JavaScript errors


### CORS Errors
1.	Verify vite.config.ts proxy configuration
2.	Verify backend CORS settings
3.	Check Network tab for actual request URL
4.	Ensure /inventory-api prefix in URL

### Can't Retire Inventory
1.	Check user role in DevTools (console: window.localStorage)
2.	Only BiomedicalEngineer role can retire
3.	Check backend authorization logs
4.	Verify 403 error from backend

### Search Not Working
1.	Type in Search field and wait ~300ms
2.	Check Network tab for search request
3.	Verify query parameters in URL
4.	Check backend search index/performance


### Table Not Loading
1.	Check browser console for errors
2.	Check Network tab for API responses
3.	Check backend logs for database errors
4.	Refresh page (F5)
5.	Check PostgreSQL connection


## Code Examples

### Import and Use Service
import { inventoryService } from '@/services/inventoryService'
const result = await inventoryService.search({ search: 'monitor', status: InventoryStatus.InUse, page: 1, pageSize: 10 })

### Handle Errors
try { await inventoryService.retire(id) } catch (error: any) { if (error.status === 403) { console.log('Only BiomedicalEngineers can retire') } else if (error.status === 404) { console.log('Inventory not found') } else { console.log('Error:', error.message) } }

### Display Status Color
import { InventoryStatusColor, InventoryStatusLabel } from '@/types/api.types'
const label = InventoryStatusLabel[item.status]  // "In Use" const color = InventoryStatusColor[item.status]  // "#2196f3"


## Testing

### Run All Tests
npm run test -- --run


### Run Only Inventory Tests
npm run test -- --run inventoryService

### Watch Mode (Continuous)
npm run test


### View Coverage
npm run test -- --coverage


## Debugging

### Enable Network Logging
// In browser console localStorage.setItem('debug', 'axios:*')
### Check JWT Token
// In browser console sessionStorage.getItem('accessToken') sessionStorage.getItem('user')
### Check Request Headers
// In DevTools Network tab // Select any API request // View "Request Headers" tab // Look for: Authorization: Bearer <token>
### Check API Response
// In DevTools Network tab // Select API request // View "Response" tab // Check data structure matches type


## Performance Tips

1. **Search Debounce**: Implement 300ms debounce on search input (TODO)
2. **Pagination**: Use smaller page sizes for large tables
3. **Filtering**: Combine multiple filters for faster results
4. **Caching**: Consider caching search results (TODO)
5. **Lazy Loading**: Images/large data loaded on demand (TODO)

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Keyboard Shortcuts (Planned)

- `Ctrl+K` / `Cmd+K` - Open search (TODO)
- `Ctrl+N` / `Cmd+N` - New asset (TODO)
- `Escape` - Close dialogs (TODO)
- `Enter` - Submit forms (TODO)

## Useful Links

- **Frontend Repo**: https://github.com/sumonnwe/SmartOpsAz/tree/main/src/WebUI/SmartOpsAz.WebUI
- **Backend API Docs**: http://localhost:5102/swagger (when running)
- **Material UI Docs**: https://mui.com/
- **Vite Docs**: https://vitejs.dev/
- **Axios Docs**: https://axios-http.com/

## Contact & Support

- **GitHub Issues**: https://github.com/sumonnwe/SmartOpsAz/issues
- **Backend Swagger**: http://localhost:5102/swagger
- **Frontend Logs**: Browser DevTools Console
- **Backend Logs**: Terminal where service is running

---

**Last Updated**: October 21, 2024
**Status**: ✅ Ready for Use


