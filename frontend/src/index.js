import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import HomePage from './pages/HomePage';
import {createBrowserRouter, Navigate, RouterProvider} from "react-router-dom";
import LoginPage from "./pages/LoginPage";
import NotFoundPage from "./pages/NotFoundPage";
import RegisterPage from "./pages/RegisterPage";
import EndGameLabel from "./components/Board/EndGameLabel/EndGameLabel";
import PropTypes from "prop-types";

const ProtectedRoute = ({ children }) => {
    const token = localStorage.getItem('token');
    console.log(`token in index.js: ${token}`); // Debug the token value
    return token ? children : <Navigate to="/login" />;
};

ProtectedRoute.propTypes = {
    children: PropTypes.node.isRequired,
}

const router = createBrowserRouter([
    {
        path: "/",
        element: (
            <ProtectedRoute>
                <HomePage />
            </ProtectedRoute>
        ),
        errorElement: <NotFoundPage />,
    },
    {
        path: "/login",
        element: <LoginPage />,
    },
    {
        path: "/register",
        element: <RegisterPage />,
    }
]);

EndGameLabel.PropTypes = {
    onClick: PropTypes.func.isRequired,
    winner: PropTypes.string.isRequired,
};

const root = ReactDOM.createRoot(document.getElementById('root'));

root.render(
    <React.StrictMode>
        <RouterProvider router={router} />
    </React.StrictMode>
);
