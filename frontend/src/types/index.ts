export type Role = 'Employee' | 'Manager' | 'HR' | 'Admin';

export interface UserProfile {
    employeeCode: string;
    firstName: string;
    lastName: string;
}

export interface User {
    id: number;
    email: string;
    role: Role;
    profile: UserProfile | null;
}

export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
}

export interface LeaveType {
    id: number;
    name: string;
    unit: string;
    paid: boolean;
}

export interface LeaveRequestResponseDto {
    id: number;
    leaveTypeId: number;
    leaveTypeName: string;
    startDateTime: string;
    endDateTime: string;
    duration: number;
    status: string;
    createdAt: string;
}

export interface CreateLeaveRequestDto {
    leaveTypeId: number;
    startDateTime: string;
    endDateTime: string;
    reason?: string;
}
