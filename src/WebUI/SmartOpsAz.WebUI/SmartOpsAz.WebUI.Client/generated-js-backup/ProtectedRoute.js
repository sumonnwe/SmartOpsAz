"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ProtectedRoute = void 0;
var jsx_runtime_1 = require("react/jsx-runtime");
var react_router_dom_1 = require("react-router-dom");
var AuthContext_1 = require("../../features/auth/context/AuthContext");
var ProtectedRoute = function (_a) {
    var children = _a.children, requiredRoles = _a.requiredRoles;
    var _b = (0, AuthContext_1.useAuth)(), isAuthenticated = _b.isAuthenticated, hasRole = _b.hasRole;
    if (!isAuthenticated) {
        return (0, jsx_runtime_1.jsx)(react_router_dom_1.Navigate, { to: "/login", replace: true });
    }
    if (requiredRoles && !hasRole(requiredRoles)) {
        return (0, jsx_runtime_1.jsx)(react_router_dom_1.Navigate, { to: "/dashboard", replace: true });
    }
    return (0, jsx_runtime_1.jsx)(jsx_runtime_1.Fragment, { children: children });
};
exports.ProtectedRoute = ProtectedRoute;
