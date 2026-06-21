import { apiClient } from './axios';
import { CreateLeaveRequestDto, LeaveRequestResponseDto } from '../types';

export const getMyRequests = async (): Promise<LeaveRequestResponseDto[]> => {
    const response = await apiClient.get<LeaveRequestResponseDto[]>('/leaves/requests?mine=true');
    return response.data;
};

export const createRequest = async (dto: CreateLeaveRequestDto): Promise<LeaveRequestResponseDto> => {
    const response = await apiClient.post<LeaveRequestResponseDto>('/leaves/requests', dto);
    return response.data;
};

export const submitRequest = async (id: number): Promise<void> => {
    await apiClient.post(`/leaves/requests/${id}/submit`);
};

export const cancelRequest = async (id: number): Promise<void> => {
    await apiClient.post(`/leaves/requests/${id}/cancel`);
};
