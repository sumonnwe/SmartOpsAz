"use strict";
/**
 * Form validation utilities
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.validators = void 0;
exports.getValidationError = getValidationError;
exports.validators = {
    /**
     * Validate asset tag format
     */
    assetTag: function (value) {
        if (!value)
            return 'Asset tag is required';
        if (value.length > 50)
            return 'Asset tag must be 50 characters or less';
        if (!/^[a-zA-Z0-9\-_]+$/.test(value))
            return 'Asset tag can only contain letters, numbers, hyphens, and underscores';
        return undefined;
    },
    /**
     * Validate serial number format
     */
    serialNumber: function (value) {
        if (!value)
            return 'Serial number is required';
        if (value.length > 50)
            return 'Serial number must be 50 characters or less';
        return undefined;
    },
    /**
     * Validate device name
     */
    deviceName: function (value) {
        if (!value)
            return 'Device name is required';
        if (value.length > 100)
            return 'Device name must be 100 characters or less';
        return undefined;
    },
    /**
     * Validate device code
     */
    deviceCode: function (value) {
        if (!value)
            return 'Device code is required';
        if (!/^[a-zA-Z0-9\-_]+$/.test(value))
            return 'Device code can only contain letters, numbers, hyphens, and underscores';
        return undefined;
    },
    /**
     * Validate battery percentage
     */
    batteryPercentage: function (value) {
        if (value < 0 || value > 100)
            return 'Battery must be between 0 and 100';
        return undefined;
    },
    /**
     * Validate signal strength (RSSI)
     */
    signalStrength: function (value) {
        if (value < -120 || value > 0)
            return 'Signal strength must be between -120 and 0';
        return undefined;
    },
    /**
     * Validate username
     */
    username: function (value) {
        if (!value)
            return 'Username is required';
        if (value.length < 3)
            return 'Username must be at least 3 characters';
        return undefined;
    },
    /**
     * Validate password
     */
    password: function (value) {
        if (!value)
            return 'Password is required';
        if (value.length < 6)
            return 'Password must be at least 6 characters';
        return undefined;
    }
};
function getValidationError(errors, fieldName) {
    if (!errors || !errors[fieldName])
        return undefined;
    return errors[fieldName][0];
}
