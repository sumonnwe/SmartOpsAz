"use strict";
/**
 * Backend API DTOs - Mirror of .NET service contracts
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.DeviceConnectivity = exports.DeviceStatus = exports.InventoryStatus = void 0;
// ============================================
// Inventory Types
// ============================================
var InventoryStatus;
(function (InventoryStatus) {
    InventoryStatus[InventoryStatus["Available"] = 0] = "Available";
    InventoryStatus[InventoryStatus["InUse"] = 1] = "InUse";
    InventoryStatus[InventoryStatus["Maintenance"] = 2] = "Maintenance";
    InventoryStatus[InventoryStatus["Offline"] = 3] = "Offline";
    InventoryStatus[InventoryStatus["Retired"] = 4] = "Retired";
})(InventoryStatus || (exports.InventoryStatus = InventoryStatus = {}));
// ============================================
// Device Types
// ============================================
var DeviceStatus;
(function (DeviceStatus) {
    DeviceStatus[DeviceStatus["Online"] = 0] = "Online";
    DeviceStatus[DeviceStatus["Offline"] = 1] = "Offline";
    DeviceStatus[DeviceStatus["Warning"] = 2] = "Warning";
    DeviceStatus[DeviceStatus["Critical"] = 3] = "Critical";
    DeviceStatus[DeviceStatus["Maintenance"] = 4] = "Maintenance";
})(DeviceStatus || (exports.DeviceStatus = DeviceStatus = {}));
var DeviceConnectivity;
(function (DeviceConnectivity) {
    DeviceConnectivity["Cellular"] = "Cellular";
    DeviceConnectivity["WiFi"] = "WiFi";
    DeviceConnectivity["Ethernet"] = "Ethernet";
    DeviceConnectivity["Bluetooth"] = "Bluetooth";
})(DeviceConnectivity || (exports.DeviceConnectivity = DeviceConnectivity = {}));
