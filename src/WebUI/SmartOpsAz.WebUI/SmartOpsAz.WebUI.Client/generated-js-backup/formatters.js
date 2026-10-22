"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.formatInventoryStatus = formatInventoryStatus;
exports.getInventoryStatusColor = getInventoryStatusColor;
exports.formatDeviceStatus = formatDeviceStatus;
exports.getDeviceStatusColor = getDeviceStatusColor;
exports.formatDate = formatDate;
exports.formatDateTime = formatDateTime;
exports.formatTimeAgo = formatTimeAgo;
exports.formatConnectivity = formatConnectivity;
exports.formatSignalStrength = formatSignalStrength;
exports.getSignalStrengthColor = getSignalStrengthColor;
var api_types_1 = require("../types/api.types");
/**
 * Format inventory status to display string
 */
function formatInventoryStatus(status) {
    var _a;
    var statusMap = (_a = {},
        _a[api_types_1.InventoryStatus.Available] = 'Available',
        _a[api_types_1.InventoryStatus.InUse] = 'In Use',
        _a[api_types_1.InventoryStatus.Maintenance] = 'Maintenance',
        _a[api_types_1.InventoryStatus.Offline] = 'Offline',
        _a[api_types_1.InventoryStatus.Retired] = 'Retired',
        _a);
    return statusMap[status] || 'Unknown';
}
/**
 * Get color for inventory status badge
 */
function getInventoryStatusColor(status) {
    var _a;
    var colorMap = (_a = {},
        _a[api_types_1.InventoryStatus.Available] = 'success',
        _a[api_types_1.InventoryStatus.InUse] = 'info',
        _a[api_types_1.InventoryStatus.Maintenance] = 'warning',
        _a[api_types_1.InventoryStatus.Offline] = 'error',
        _a[api_types_1.InventoryStatus.Retired] = 'default',
        _a);
    return colorMap[status] || 'default';
}
/**
 * Format device status to display string
 */
function formatDeviceStatus(status) {
    var _a;
    var statusMap = (_a = {},
        _a[api_types_1.DeviceStatus.Online] = 'Online',
        _a[api_types_1.DeviceStatus.Offline] = 'Offline',
        _a[api_types_1.DeviceStatus.Warning] = 'Warning',
        _a[api_types_1.DeviceStatus.Critical] = 'Critical',
        _a[api_types_1.DeviceStatus.Maintenance] = 'Maintenance',
        _a);
    return statusMap[status] || 'Unknown';
}
/**
 * Get color for device status badge
 */
function getDeviceStatusColor(status) {
    var _a;
    var colorMap = (_a = {},
        _a[api_types_1.DeviceStatus.Online] = 'success',
        _a[api_types_1.DeviceStatus.Offline] = 'error',
        _a[api_types_1.DeviceStatus.Warning] = 'warning',
        _a[api_types_1.DeviceStatus.Critical] = 'error',
        _a[api_types_1.DeviceStatus.Maintenance] = 'info',
        _a);
    return colorMap[status] || 'default';
}
/**
 * Format date to readable string
 */
function formatDate(dateString) {
    var date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    });
}
/**
 * Format date and time
 */
function formatDateTime(dateString) {
    var date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
    });
}
/**
 * Format time ago (e.g., "2 minutes ago")
 */
function formatTimeAgo(dateString) {
    var date = new Date(dateString);
    var now = new Date();
    var seconds = Math.floor((now.getTime() - date.getTime()) / 1000);
    var interval = seconds / 31536000;
    if (interval > 1)
        return Math.floor(interval) + ' years ago';
    interval = seconds / 2592000;
    if (interval > 1)
        return Math.floor(interval) + ' months ago';
    interval = seconds / 86400;
    if (interval > 1)
        return Math.floor(interval) + ' days ago';
    interval = seconds / 3600;
    if (interval > 1)
        return Math.floor(interval) + ' hours ago';
    interval = seconds / 60;
    if (interval > 1)
        return Math.floor(interval) + ' minutes ago';
    return Math.floor(seconds) + ' seconds ago';
}
/**
 * Format connectivity type
 */
function formatConnectivity(connectivity) {
    if (typeof connectivity === 'string') {
        return connectivity.charAt(0).toUpperCase() + connectivity.slice(1);
    }
    return connectivity;
}
/**
 * Format RSSI signal strength
 */
function formatSignalStrength(rssi) {
    if (rssi >= -50)
        return 'Excellent';
    if (rssi >= -60)
        return 'Very Good';
    if (rssi >= -70)
        return 'Good';
    if (rssi >= -80)
        return 'Fair';
    return 'Poor';
}
/**
 * Get signal strength color
 */
function getSignalStrengthColor(rssi) {
    if (rssi >= -70)
        return 'success';
    if (rssi >= -80)
        return 'warning';
    return 'error';
}
