"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || (function () {
    var ownKeys = function(o) {
        ownKeys = Object.getOwnPropertyNames || function (o) {
            var ar = [];
            for (var k in o) if (Object.prototype.hasOwnProperty.call(o, k)) ar[ar.length] = k;
            return ar;
        };
        return ownKeys(o);
    };
    return function (mod) {
        if (mod && mod.__esModule) return mod;
        var result = {};
        if (mod != null) for (var k = ownKeys(mod), i = 0; i < k.length; i++) if (k[i] !== "default") __createBinding(result, mod, k[i]);
        __setModuleDefault(result, mod);
        return result;
    };
})();
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.router = void 0;
var jsx_runtime_1 = require("react/jsx-runtime");
var react_1 = __importDefault(require("react"));
var react_router_dom_1 = require("react-router-dom");
var ProtectedRoute_1 = require("../components/common/ProtectedRoute");
var MainLayout_1 = require("../components/layout/MainLayout");
var LoginPage_1 = require("../features/auth/pages/LoginPage");
// Lazy load page components for better performance
var DashboardPage = react_1.default.lazy(function () { return Promise.resolve().then(function () { return __importStar(require('../features/dashboard/pages/DashboardPage')); }).then(function (m) { return ({ default: m.DashboardPage }); }); });
var InventoryPage = react_1.default.lazy(function () { return Promise.resolve().then(function () { return __importStar(require('../features/inventory/pages/InventoryListPage')); }).then(function (m) { return ({ default: m.InventoryListPage }); }); });
var DevicesPage = react_1.default.lazy(function () { return Promise.resolve().then(function () { return __importStar(require('../features/devices/pages/DevicesListPage')); }).then(function (m) { return ({ default: m.DevicesListPage }); }); });
exports.router = (0, react_router_dom_1.createBrowserRouter)([
    {
        path: '/login',
        element: (0, jsx_runtime_1.jsx)(LoginPage_1.LoginPage, {})
    },
    {
        path: '/',
        element: ((0, jsx_runtime_1.jsx)(ProtectedRoute_1.ProtectedRoute, { children: (0, jsx_runtime_1.jsx)(MainLayout_1.MainLayout, { children: (0, jsx_runtime_1.jsx)(react_1.default.Suspense, { fallback: (0, jsx_runtime_1.jsx)("div", { children: "Loading..." }), children: (0, jsx_runtime_1.jsx)(DashboardPage, {}) }) }) }))
    },
    {
        path: '/dashboard',
        element: ((0, jsx_runtime_1.jsx)(ProtectedRoute_1.ProtectedRoute, { children: (0, jsx_runtime_1.jsx)(MainLayout_1.MainLayout, { children: (0, jsx_runtime_1.jsx)(react_1.default.Suspense, { fallback: (0, jsx_runtime_1.jsx)("div", { children: "Loading..." }), children: (0, jsx_runtime_1.jsx)(DashboardPage, {}) }) }) }))
    },
    {
        path: '/inventory',
        element: ((0, jsx_runtime_1.jsx)(ProtectedRoute_1.ProtectedRoute, { children: (0, jsx_runtime_1.jsx)(MainLayout_1.MainLayout, { children: (0, jsx_runtime_1.jsx)(react_1.default.Suspense, { fallback: (0, jsx_runtime_1.jsx)("div", { children: "Loading..." }), children: (0, jsx_runtime_1.jsx)(InventoryPage, {}) }) }) }))
    },
    {
        path: '/devices',
        element: ((0, jsx_runtime_1.jsx)(ProtectedRoute_1.ProtectedRoute, { children: (0, jsx_runtime_1.jsx)(MainLayout_1.MainLayout, { children: (0, jsx_runtime_1.jsx)(react_1.default.Suspense, { fallback: (0, jsx_runtime_1.jsx)("div", { children: "Loading..." }), children: (0, jsx_runtime_1.jsx)(DevicesPage, {}) }) }) }))
    }
]);
