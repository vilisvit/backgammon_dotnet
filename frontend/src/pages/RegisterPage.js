import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from "axios";
import './LoginRegister.css';
import Header from "../components/Header/Header";

function RegisterPage() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = (e) => {
        e.preventDefault();
        axios.post(`${process.env.REACT_APP_API_URL}/auth/register`, { username, password })
            .then(() => {
                alert('Registration successful! You can now log in.');
                navigate('/login');
            })
            .catch(error => {
                if (error.response && error.response.status === 409) {
                    setError('Username already exists. Please choose a different one.');
                } else {
                    setError('Registration failed. Please try again later.');
                }
            });
    };

    return (
        <div className="login-register-container">
            <Header/>
            <h1>Register</h1>
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
                <button type="submit">Register</button>
            </form>
            <div className="error-message-container">
                {error && <p className="error-message">{error}</p>}
            </div>
            <div className="link-container">
                <p>Already have an account? <Link to="/login">Login here</Link></p>
            </div>
        </div>
    );
}

export default RegisterPage;