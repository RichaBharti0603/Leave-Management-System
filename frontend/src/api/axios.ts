import axios from 'axios';

// Base instance
export const apiClient = axios.create({
    baseURL: 'http://localhost:5000/api', // Note: backend URL might be different
    headers: {
        'Content-Type': 'application/json',
    },
});

// Interceptor to attach token
apiClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('accessToken');
        if (token && config.headers) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Interceptor to handle 401 and refresh token (simplified)
apiClient.interceptors.response.use(
    (response) => response,
    async (error) => {
        // Implement auto-refresh logic here if refresh endpoint exists
        if (error.response?.status === 401) {
            localStorage.removeItem('accessToken');
            localStorage.removeItem('refreshToken');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);
