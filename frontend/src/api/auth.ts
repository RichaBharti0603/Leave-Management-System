import { apiClient } from './axios';
import { AuthResponse, User } from '../types';

export const login = async (email: string, password: string): Promise<AuthResponse> => {
    const response = await apiClient.post<AuthResponse>('/auth/login', { email, password });
    return response.data;
};

export const getMe = async (): Promise<User> => {
    const response = await apiClient.get<User>('/auth/me');
    return response.data;
};
