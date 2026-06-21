import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { User } from '../types';
import { getMe } from '../api/auth';
import { jwtDecode } from 'jwt-decode';

interface AuthContextType {
    user: User | null;
    isLoading: boolean;
    login: (token: string, refreshToken: string) => void;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const loadUser = async () => {
            const token = localStorage.getItem('accessToken');
            if (token) {
                try {
                    // Check if token is expired
                    const decoded = jwtDecode(token);
                    if (decoded.exp && decoded.exp * 1000 < Date.now()) {
                        throw new Error('Token expired');
                    }
                    
                    const userData = await getMe();
                    setUser(userData);
                } catch (error) {
                    console.error('Failed to load user', error);
                    localStorage.removeItem('accessToken');
                    localStorage.removeItem('refreshToken');
                }
            }
            setIsLoading(false);
        };

        loadUser();
    }, []);

    const login = (token: string, refreshToken: string) => {
        localStorage.setItem('accessToken', token);
        localStorage.setItem('refreshToken', refreshToken);
        // We could decode token to set initial user state or fetch full profile immediately
        getMe().then(setUser).catch(console.error);
    };

    const logout = () => {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, isLoading, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};
