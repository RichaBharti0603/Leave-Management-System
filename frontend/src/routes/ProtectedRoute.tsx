import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { Role } from '../types';

interface ProtectedRouteProps {
    allowedRoles?: Role[];
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ allowedRoles }) => {
    const { user, isLoading } = useAuth();

    if (isLoading) {
        return <div>Loading...</div>; // Could be a fancy spinner
    }

    if (!user) {
        return <Navigate to="/login" replace />;
    }

    if (allowedRoles && !allowedRoles.includes(user.role)) {
        // User's role is not authorized, redirect to their home based on role
        const redirectPath = user.role === 'Employee' ? '/employee' : 
                             user.role === 'Manager' ? '/manager' : 
                             user.role === 'HR' ? '/hr' : '/';
        return <Navigate to={redirectPath} replace />;
    }

    return <Outlet />;
};
