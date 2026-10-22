"use strict";
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
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
Object.defineProperty(exports, "__esModule", { value: true });
exports.LoginForm = void 0;
var jsx_runtime_1 = require("react/jsx-runtime");
var react_1 = require("react");
var material_1 = require("@mui/material");
var AuthContext_1 = require("../context/AuthContext");
var validators_1 = require("../../../utils/validators");
var LoginForm = function (_a) {
    var onLoginSuccess = _a.onLoginSuccess;
    var _b = (0, AuthContext_1.useAuth)(), login = _b.login, isLoading = _b.isLoading;
    var _c = (0, react_1.useState)({ username: '', password: '' }), formData = _c[0], setFormData = _c[1];
    var _d = (0, react_1.useState)({}), errors = _d[0], setErrors = _d[1];
    var _e = (0, react_1.useState)(null), apiError = _e[0], setApiError = _e[1];
    var handleChange = function (e) {
        var _a = e.target, name = _a.name, value = _a.value;
        setFormData(function (prev) {
            var _a;
            return (__assign(__assign({}, prev), (_a = {}, _a[name] = value, _a)));
        });
        // Clear error when user starts typing
        if (errors[name]) {
            setErrors(function (prev) {
                var _a;
                return (__assign(__assign({}, prev), (_a = {}, _a[name] = '', _a)));
            });
        }
    };
    var validateForm = function () {
        var newErrors = {};
        var usernameError = validators_1.validators.username(formData.username);
        if (usernameError)
            newErrors.username = usernameError;
        var passwordError = validators_1.validators.password(formData.password);
        if (passwordError)
            newErrors.password = passwordError;
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };
    var handleSubmit = function (e) { return __awaiter(void 0, void 0, void 0, function () {
        var error_1;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    e.preventDefault();
                    setApiError(null);
                    if (!validateForm())
                        return [2 /*return*/];
                    _a.label = 1;
                case 1:
                    _a.trys.push([1, 3, , 4]);
                    return [4 /*yield*/, login(formData.username, formData.password)];
                case 2:
                    _a.sent();
                    setFormData({ username: '', password: '' });
                    onLoginSuccess === null || onLoginSuccess === void 0 ? void 0 : onLoginSuccess();
                    return [3 /*break*/, 4];
                case 3:
                    error_1 = _a.sent();
                    setApiError(error_1 instanceof Error ? error_1.message : 'Login failed');
                    return [3 /*break*/, 4];
                case 4: return [2 /*return*/];
            }
        });
    }); };
    return ((0, jsx_runtime_1.jsx)(material_1.Container, { maxWidth: "sm", children: (0, jsx_runtime_1.jsx)(material_1.Box, { sx: {
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center',
                minHeight: '100vh'
            }, children: (0, jsx_runtime_1.jsxs)(material_1.Paper, { elevation: 3, sx: { p: 4 }, children: [(0, jsx_runtime_1.jsx)(material_1.Typography, { variant: "h4", component: "h1", gutterBottom: true, sx: { textAlign: 'center', mb: 3 }, children: "SmartOps \uFFFD Login" }), apiError && ((0, jsx_runtime_1.jsx)(material_1.Alert, { severity: "error", sx: { mb: 2 }, children: apiError })), (0, jsx_runtime_1.jsxs)("form", { onSubmit: handleSubmit, children: [(0, jsx_runtime_1.jsx)(material_1.TextField, { fullWidth: true, label: "Username", name: "username", value: formData.username, onChange: handleChange, error: !!errors.username, helperText: errors.username, margin: "normal", disabled: isLoading, autoComplete: "username" }), (0, jsx_runtime_1.jsx)(material_1.TextField, { fullWidth: true, label: "Password", name: "password", type: "password", value: formData.password, onChange: handleChange, error: !!errors.password, helperText: errors.password, margin: "normal", disabled: isLoading, autoComplete: "current-password" }), (0, jsx_runtime_1.jsx)(material_1.Button, { fullWidth: true, type: "submit", variant: "contained", size: "large", sx: { mt: 3 }, disabled: isLoading, children: isLoading ? (0, jsx_runtime_1.jsx)(material_1.CircularProgress, { size: 24 }) : 'Login' })] }), (0, jsx_runtime_1.jsx)(material_1.Typography, { variant: "caption", sx: {
                            display: 'block',
                            textAlign: 'center',
                            mt: 3,
                            color: 'text.secondary'
                        }, children: "Use demo credentials for testing" })] }) }) }));
};
exports.LoginForm = LoginForm;
