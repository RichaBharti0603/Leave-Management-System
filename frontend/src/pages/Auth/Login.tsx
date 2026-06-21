import React, { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import { login as loginApi } from '../api/auth';
import { useNavigate } from 'react-router-dom';
import './Login.css';

export const Login: React.FC = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    
    const { login } = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setIsLoading(true);

        try {
            const { accessToken, refreshToken } = await loginApi(email, password);
            login(accessToken, refreshToken);
            // Redirection is handled by the ProtectedRoute or App based on state change
            // For immediate feedback, we can navigate to root
            navigate('/');
        } catch (err) {
            setError('Invalid credentials or server error.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="login-container">
            {/* The background hero image is handled via CSS in Login.css */}
            <div className="login-overlay">
                <div className="login-card glass-panel">
                    <div className="login-header">
                        <h1>LMS</h1>
                        <p>Smart Leave Management System</p>
                    </div>
                    
                    {error && <div className="alert-danger">{error}</div>}
                    
                    <form onSubmit={handleSubmit} className="login-form">
                        <div className="form-group">
                            <label className="form-label" htmlFor="email">Email</label>
                            <input
                                id="email"
                                type="email"
                                className="form-control"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                            />
                        </div>
                        <div className="form-group">
                            <label className="form-label" htmlFor="password">Password</label>
                            <input
                                id="password"
                                type="password"
                                className="form-control"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                        </div>
                        <button type="submit" className="btn btn-primary login-btn" disabled={isLoading}>
                            {isLoading ? 'Signing in...' : 'Login'}
                        </button>
                    </form>
                    
                    <div className="login-footer">
                        <p>Test accounts: admin@lms.com / hr@lms.com / manager@lms.com / employee@lms.com (password: hash)</p>
                    </div>
                </div>
            </div>
        </div>
    );
};
