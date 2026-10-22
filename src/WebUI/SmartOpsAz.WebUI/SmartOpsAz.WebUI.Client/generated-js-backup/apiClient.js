"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g = Object.create((typeof Iterator === "function" ? Iterator : Object).prototype);
    return g.next = verb(0), g["throw"] = verb(1), g["return"] = verb(2), typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (g && (g = 0, op[0] && (_ = 0)), _) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.apiClient = void 0;
var axios_1 = __importDefault(require("axios"));
var API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5101';
var ApiClient = /** @class */ (function () {
    function ApiClient() {
        var _this = this;
        this.client = axios_1.default.create({
            baseURL: API_BASE_URL,
            timeout: 30000,
            headers: {
                'Content-Type': 'application/json'
            }
        });
        // Request interceptor: attach JWT token
        this.client.interceptors.request.use(function (config) {
            var token = _this.getAccessToken();
            if (token) {
                config.headers.Authorization = "Bearer ".concat(token);
            }
            return config;
        }, function (error) { return Promise.reject(error); });
        // Response interceptor: handle errors and token expiry
        this.client.interceptors.response.use(function (response) { return response; }, function (error) {
            var _a;
            if (((_a = error.response) === null || _a === void 0 ? void 0 : _a.status) === 401) {
                // Token expired or invalid
                _this.clearAccessToken();
                window.location.href = '/login';
            }
            return Promise.reject(error);
        });
    }
    /**
     * Generic GET request
     */
    ApiClient.prototype.get = function (url, config) {
        return __awaiter(this, void 0, void 0, function () {
            var response, error_1;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 2, , 3]);
                        return [4 /*yield*/, this.client.get(url, config)];
                    case 1:
                        response = _a.sent();
                        return [2 /*return*/, {
                                data: response.data,
                                status: response.status
                            }];
                    case 2:
                        error_1 = _a.sent();
                        return [2 /*return*/, this.handleError(error_1)];
                    case 3: return [2 /*return*/];
                }
            });
        });
    };
    /**
     * Generic POST request
     */
    ApiClient.prototype.post = function (url, data, config) {
        return __awaiter(this, void 0, void 0, function () {
            var response, error_2;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 2, , 3]);
                        return [4 /*yield*/, this.client.post(url, data, config)];
                    case 1:
                        response = _a.sent();
                        return [2 /*return*/, {
                                data: response.data,
                                status: response.status
                            }];
                    case 2:
                        error_2 = _a.sent();
                        return [2 /*return*/, this.handleError(error_2)];
                    case 3: return [2 /*return*/];
                }
            });
        });
    };
    /**
     * Generic PUT request
     */
    ApiClient.prototype.put = function (url, data, config) {
        return __awaiter(this, void 0, void 0, function () {
            var response, error_3;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 2, , 3]);
                        return [4 /*yield*/, this.client.put(url, data, config)];
                    case 1:
                        response = _a.sent();
                        return [2 /*return*/, {
                                data: response.data,
                                status: response.status
                            }];
                    case 2:
                        error_3 = _a.sent();
                        return [2 /*return*/, this.handleError(error_3)];
                    case 3: return [2 /*return*/];
                }
            });
        });
    };
    /**
     * Generic DELETE request
     */
    ApiClient.prototype.delete = function (url, config) {
        return __awaiter(this, void 0, void 0, function () {
            var response, error_4;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        _a.trys.push([0, 2, , 3]);
                        return [4 /*yield*/, this.client.delete(url, config)];
                    case 1:
                        response = _a.sent();
                        return [2 /*return*/, {
                                data: response.data,
                                status: response.status
                            }];
                    case 2:
                        error_4 = _a.sent();
                        return [2 /*return*/, this.handleError(error_4)];
                    case 3: return [2 /*return*/];
                }
            });
        });
    };
    /**
     * Handle API errors consistently
     */
    ApiClient.prototype.handleError = function (error) {
        var _a, _b;
        if (axios_1.default.isAxiosError(error)) {
            var status_1 = ((_a = error.response) === null || _a === void 0 ? void 0 : _a.status) || 500;
            var data = (_b = error.response) === null || _b === void 0 ? void 0 : _b.data;
            var apiError = {
                status: status_1,
                message: typeof (data === null || data === void 0 ? void 0 : data.message) === 'string'
                    ? data.message
                    : error.message || 'An error occurred',
                errors: typeof (data === null || data === void 0 ? void 0 : data.errors) === 'object' && data.errors !== null
                    ? data.errors
                    : undefined,
                code: typeof (data === null || data === void 0 ? void 0 : data.code) === 'string'
                    ? data.code
                    : undefined
            };
            return { error: apiError, status: status_1 };
        }
        return {
            error: {
                status: 500,
                message: 'An unexpected error occurred'
            },
            status: 500
        };
    };
    /**
     * Token management
     */
    ApiClient.prototype.setAccessToken = function (token) {
        sessionStorage.setItem('accessToken', token);
    };
    ApiClient.prototype.getAccessToken = function () {
        return sessionStorage.getItem('accessToken');
    };
    ApiClient.prototype.clearAccessToken = function () {
        sessionStorage.removeItem('accessToken');
    };
    ApiClient.prototype.isTokenValid = function () {
        var token = this.getAccessToken();
        var expiresAt = sessionStorage.getItem('tokenExpiresAt');
        if (!token || !expiresAt)
            return false;
        return new Date(expiresAt) > new Date();
    };
    ApiClient.prototype.setTokenExpiry = function (expiresAtUtc) {
        sessionStorage.setItem('tokenExpiresAt', expiresAtUtc);
    };
    return ApiClient;
}());
exports.apiClient = new ApiClient();
