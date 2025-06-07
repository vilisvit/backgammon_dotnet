import axios from 'axios';

const gsAxios = axios.create({
    baseURL: 'http://localhost:3000/api/',
});

gsAxios.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    console.log(`Token in axios interceptor: ${token}`); // Debug the token value
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, (error) => {
    return Promise.reject(error);
});

export default gsAxios;