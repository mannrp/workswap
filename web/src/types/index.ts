export interface User {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
    roles: string[];
}

export interface AuthResponse {
    success: boolean;
    token?: string;
    expiration?: string;
    error?: string;
}

export interface UserInfo {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
    roles: string[];
}

export interface Department {
    id: number;
    name: string;
    description?: string;
    employeeCount: number;
    createdAt: string;
}

export interface Shift {
    id: number;
    date: string;
    startTime: string;
    endTime: string;
    notes?: string;
    isAvailableForSwap: boolean;
    departmentId: number;
    departmentName: string;
    assignedUserId?: number;
    assignedUserName?: string;
    createdAt: string;
}

export interface ShiftOffer {
    id: number;
    shiftId: number;
    shift: Shift;
    offeredById: number;
    offeredByName: string;
    claimedById?: number;
    claimedByName?: string;
    createdAt: string;
    expiresAt: string;
    status: 'Active' | 'Claimed' | 'Expired' | 'Cancelled';
}

export interface SwapRequest {
    id: number;
    senderShiftId: number;
    senderShift: Shift;
    receiverShiftId?: number;
    receiverShift?: Shift;
    senderId: number;
    senderName: string;
    receiverId: number;
    receiverName: string;
    createdAt: string;
    status: 'Pending' | 'Completed' | 'Rejected' | 'Cancelled';
}

export interface Notification {
    id: number;
    message: string;
    isRead: boolean;
    createdAt: string;
    actionLink?: string;
}

export interface ApiError {
    message: string;
    errors?: Record<string, string[]>;
}
