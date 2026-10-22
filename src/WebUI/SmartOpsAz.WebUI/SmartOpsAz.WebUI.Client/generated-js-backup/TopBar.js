"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.TopBar = void 0;
var jsx_runtime_1 = require("react/jsx-runtime");
var react_1 = __importDefault(require("react"));
var material_1 = require("@mui/material");
var react_router_dom_1 = require("react-router-dom");
var Menu_1 = __importDefault(require("@mui/icons-material/Menu"));
var AccountCircle_1 = __importDefault(require("@mui/icons-material/AccountCircle"));
var Logout_1 = __importDefault(require("@mui/icons-material/Logout"));
var AuthContext_1 = require("../../features/auth/context/AuthContext");
var TopBar = function (_a) {
    var onMenuClick = _a.onMenuClick, _b = _a.showMenuButton, showMenuButton = _b === void 0 ? false : _b;
    var navigate = (0, react_router_dom_1.useNavigate)();
    var _c = (0, AuthContext_1.useAuth)(), user = _c.user, logout = _c.logout;
    var _d = react_1.default.useState(null), anchorEl = _d[0], setAnchorEl = _d[1];
    var handleProfileClick = function (event) {
        setAnchorEl(event.currentTarget);
    };
    var handleProfileClose = function () {
        setAnchorEl(null);
    };
    var handleLogout = function () {
        handleProfileClose();
        logout();
        navigate('/login');
    };
    return ((0, jsx_runtime_1.jsx)(jsx_runtime_1.Fragment, { children: (0, jsx_runtime_1.jsx)(material_1.AppBar, { position: "static", children: (0, jsx_runtime_1.jsxs)(material_1.Toolbar, { children: [showMenuButton && ((0, jsx_runtime_1.jsx)(material_1.IconButton, { color: "inherit", "aria-label": "open drawer", edge: "start", onClick: onMenuClick, sx: { mr: 2 }, children: (0, jsx_runtime_1.jsx)(Menu_1.default, {}) })), (0, jsx_runtime_1.jsx)(material_1.Typography, { variant: "h6", component: "div", sx: { flexGrow: 1 }, children: "SmartOps \uFFFD Medical Asset Monitoring" }), user && ((0, jsx_runtime_1.jsxs)(material_1.Box, { sx: { display: 'flex', alignItems: 'center', gap: 2 }, children: [(0, jsx_runtime_1.jsx)(material_1.Chip, { avatar: (0, jsx_runtime_1.jsx)(AccountCircle_1.default, {}), label: user.username, onClick: handleProfileClick, clickable: true }), (0, jsx_runtime_1.jsxs)(material_1.Menu, { anchorEl: anchorEl, open: Boolean(anchorEl), onClose: handleProfileClose, children: [(0, jsx_runtime_1.jsx)(material_1.MenuItem, { disabled: true, children: (0, jsx_runtime_1.jsxs)(material_1.Typography, { variant: "caption", children: ["Role: ", user.role] }) }), (0, jsx_runtime_1.jsxs)(material_1.MenuItem, { onClick: handleLogout, children: [(0, jsx_runtime_1.jsx)(Logout_1.default, { sx: { mr: 1 } }), "Logout"] })] })] }))] }) }) }));
};
exports.TopBar = TopBar;
