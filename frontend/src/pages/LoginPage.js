import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from "axios";
import './LoginRegister.css';
import Header from "../components/Header/Header";

function LoginPage() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = (e) => {
        e.preventDefault();
        axios.post('/api/auth/login', { username, password })
            .then(response => {
                if (response.data.accessToken) {
                    localStorage.setItem('token', response.data.accessToken	);
                    navigate('/');
                } else {
                    setError('Token not found in response.');
                }
            })
            .catch(() => {
                setError('Please check your username and password.');
            });
    };

    return (
        <div className="login-register-container">
            <Header/>
            <h1>Login</h1>
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    placeholder="Username"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                />
                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
                <button type="submit">Login</button>
            </form>
            <div className="error-message-container">
                {error && <p className="error-message">{error}</p>}
            </div>
            <div className="link-container">
                <p>Don't have an account? <Link to="/register">Register here</Link></p>
            </div>
        </div>
    );
}

export default LoginPage;