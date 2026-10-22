"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.Sidebar = void 0;
var jsx_runtime_1 = require("react/jsx-runtime");
var material_1 = require("@mui/material");
var react_router_dom_1 = require("react-router-dom");
var Dashboard_1 = __importDefault(require("@mui/icons-material/Dashboard"));
var Inventory2_1 = __importDefault(require("@mui/icons-material/Inventory2"));
var Devices_1 = __importDefault(require("@mui/icons-material/Devices"));
var AuthContext_1 = require("../../features/auth/context/AuthContext");
var Sidebar = function (_a) {
    var open = _a.open, onClose = _a.onClose;
    var navigate = (0, react_router_dom_1.useNavigate)();
    var location = (0, react_router_dom_1.useLocation)();
    var theme = (0, material_1.useTheme)();
    var isMobile = (0, material_1.useMediaQuery)(theme.breakpoints.down('sm'));
    var hasRole = (0, AuthContext_1.useAuth)().hasRole;
    var menuItems = [
        {
            label: 'Dashboard',
            icon: (0, jsx_runtime_1.jsx)(Dashboard_1.default, {}),
            path: '/dashboard',
            roles: undefined
        },
        {
            label: 'Inventory',
            icon: (0, jsx_runtime_1.jsx)(Inventory2_1.default, {}),
            path: '/inventory',
            roles: undefined
        },
        {
            label: 'Devices',
            icon: (0, jsx_runtime_1.jsx)(Devices_1.default, {}),
            path: '/devices',
            roles: undefined
        }
    ];
    var handleNavigate = function (path) {
        navigate(path);
        if (isMobile && onClose) {
            onClose();
        }
    };
    var isActive = function (path) { return location.pathname.startsWith(path); };
    var drawer = ((0, jsx_runtime_1.jsxs)(material_1.Box, { sx: { width: 260 }, children: [(0, jsx_runtime_1.jsx)(material_1.Box, { sx: { p: 2 }, children: (0, jsx_runtime_1.jsx)(material_1.Typography, { variant: "h6", sx: { fontWeight: 'bold' }, children: "SmartOps" }) }), (0, jsx_runtime_1.jsx)(material_1.Divider, {}), (0, jsx_runtime_1.jsx)(material_1.List, { children: menuItems
                    .filter(function (item) { return !item.roles || hasRole(item.roles); })
                    .map(function (item) { return ((0, jsx_runtime_1.jsxs)(material_1.ListItem, { button: true, onClick: function () { return handleNavigate(item.path); }, selected: isActive(item.path), sx: {
                        backgroundColor: isActive(item.path)
                            ? 'action.selected'
                            : 'transparent',
                        '&:hover': {
                            backgroundColor: 'action.hover'
                        }
                    }, children: [(0, jsx_runtime_1.jsx)(material_1.ListItemIcon, { children: item.icon }), (0, jsx_runtime_1.jsx)(material_1.ListItemText, { primary: item.label })] }, item.path)); }) })] }));
    if (isMobile) {
        return ((0, jsx_runtime_1.jsx)(material_1.Drawer, { anchor: "left", open: open, onClose: onClose, children: drawer }));
    }
    return ((0, jsx_runtime_1.jsx)(material_1.Drawer, { variant: "permanent", sx: {
            width: 260,
            flexShrink: 0,
            '& .MuiDrawer-paper': {
                width: 260,
                boxSizing: 'border-box'
            }
        }, children: drawer }));
};
exports.Sidebar = Sidebar;
