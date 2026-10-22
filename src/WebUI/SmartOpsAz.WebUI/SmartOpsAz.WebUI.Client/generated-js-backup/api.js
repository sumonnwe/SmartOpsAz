"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.deviceStatus = exports.inventoryStatus = exports.api = void 0;
exports.errorText = errorText;
var axios_1 = __importDefault(require("axios"));
exports.api = axios_1.default.create({ baseURL: '/api', timeout: 15000 });
exports.api.interceptors.request.use(function (config) { var token = sessionStorage.getItem('smartops_token'); if (token)
    config.headers.Authorization = "Bearer ".concat(token); return config; });
function errorText(e) { var _a; if (axios_1.default.isAxiosError(e)) {
    var data = (_a = e.response) === null || _a === void 0 ? void 0 : _a.data;
    return (data === null || data === void 0 ? void 0 : data.detail) || (data === null || data === void 0 ? void 0 : data.message) || (data === null || data === void 0 ? void 0 : data.error) || (Array.isArray(data === null || data === void 0 ? void 0 : data.errors) ? data.errors.join(', ') : null) || e.message;
} return e instanceof Error ? e.message : 'Unexpected error'; }
exports.inventoryStatus = { 0: 'Available', 1: 'Available', 2: 'In Use', 3: 'Maintenance', 4: 'Offline', 5: 'Retired' };
exports.deviceStatus = { 1: 'Online', 2: 'Offline', 3: 'Warning', 4: 'Critical', 5: 'Maintenance' };
