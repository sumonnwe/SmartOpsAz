"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.MainLayout = void 0;
var jsx_runtime_1 = require("react/jsx-runtime");
var react_1 = require("react");
var material_1 = require("@mui/material");
var Sidebar_1 = require("./Sidebar");
var TopBar_1 = require("./TopBar");
var MainLayout = function (_a) {
    var children = _a.children;
    var theme = (0, material_1.useTheme)();
    var isMobile = (0, material_1.useMediaQuery)(theme.breakpoints.down('sm'));
    var _b = (0, react_1.useState)(!isMobile), sidebarOpen = _b[0], setSidebarOpen = _b[1];
    return ((0, jsx_runtime_1.jsxs)(material_1.Box, { sx: { display: 'flex', minHeight: '100vh', backgroundColor: '#f5f5f5' }, children: [(0, jsx_runtime_1.jsx)(Sidebar_1.Sidebar, { open: sidebarOpen, onClose: function () { return setSidebarOpen(false); } }), (0, jsx_runtime_1.jsxs)(material_1.Box, { sx: { flex: 1, display: 'flex', flexDirection: 'column' }, children: [(0, jsx_runtime_1.jsx)(TopBar_1.TopBar, { onMenuClick: function () { return setSidebarOpen(!sidebarOpen); }, showMenuButton: isMobile }), (0, jsx_runtime_1.jsx)(material_1.Box, { component: "main", sx: { flex: 1, p: 3, overflow: 'auto' }, children: children })] })] }));
};
exports.MainLayout = MainLayout;
