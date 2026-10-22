"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.LoginPage = void 0;
var jsx_runtime_1 = require("react/jsx-runtime");
var react_1 = __importDefault(require("react"));
var react_router_dom_1 = require("react-router-dom");
var AuthContext_1 = require("../context/AuthContext");
var LoginForm_1 = require("../components/LoginForm");
var LoginPage = function () {
    var navigate = (0, react_router_dom_1.useNavigate)();
    var isAuthenticated = (0, AuthContext_1.useAuth)().isAuthenticated;
    react_1.default.useEffect(function () {
        if (isAuthenticated) {
            navigate('/dashboard');
        }
    }, [isAuthenticated, navigate]);
    return (0, jsx_runtime_1.jsx)(LoginForm_1.LoginForm, { onLoginSuccess: function () { return navigate('/dashboard'); } });
};
exports.LoginPage = LoginPage;
