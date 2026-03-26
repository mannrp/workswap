export const API_URL = 'http://localhost:5156/api';

export interface User {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
    roles: string[];
}

export interface AuthResponse {
    token: string;
    expiration: string;
    user: User;
}

export const getToken = () => {
    if (typeof window !== 'undefined') {
        return localStorage.getItem('token');
    }
    return null;
};

export const setToken = (token: string) => {
    if (typeof window !== 'undefined') {
        localStorage.setItem('token', token);
    }
};

export const removeToken = () => {
    if (typeof window !== 'undefined') {
        localStorage.removeItem('token');
    }
};

export const fetchWithAuth = async (endpoint: string, options: RequestInit = {}) => {
    const token = getToken();
    const headers = {
        'Content-Type': 'application/json',
        ...options.headers,
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
    } as HeadersInit; // Explicit cast to HeadersInit

    const response = await fetch(`${API_URL}${endpoint}`, {
        ...options,
        headers,
    });

    if (response.status === 401) {
        removeToken();
        if (typeof window !== 'undefined') {
            window.location.href = '/';
        }
    }

    return response;
};

export const login = async (email: string, password: string): Promise<AuthResponse> => {
    const response = await fetch(`${API_URL}/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password }),
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Login failed');
    }

    return response.json();
};

export const register = async (email: string, password: string, firstName: string, lastName: string): Promise<AuthResponse> => {
    const response = await fetch(`${API_URL}/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password, firstName, lastName }),
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Registration failed');
    }

    return response.json();
};

export const getMe = async (): Promise<any> => {
    const response = await fetchWithAuth('/auth/me');
    if (!response.ok) throw new Error('Failed to fetch user');
    return response.json();
};

// --- DEPARTMENT UTILITIES ---

/**
 * Fetches a list of all employees in a specific department.
 * Used for selecting a colleague to swap shifts with.
 */
export const getDepartmentEmployees = async (departmentId: number) => {
    const response = await fetchWithAuth(`/departments/${departmentId}/employees`);

    if (!response.ok) {
        throw new Error('Failed to fetch department employees');
    }

    return response.json();
};

// Shift Offers API
export const getShiftOffers = async (departmentId?: number) => {
    const params = departmentId ? `?departmentId=${departmentId}` : '';
    const response = await fetchWithAuth(`/shiftoffers${params}`);
    if (!response.ok) throw new Error('Failed to fetch shift offers');
    return response.json();
};

export const createShiftOffer = async (shiftId: number, expiresAt?: string) => {
    const response = await fetchWithAuth(`/shifts/${shiftId}/offer`, {
        method: 'POST',
        body: JSON.stringify({ expiresAt }),
    });
    if (!response.ok) throw new Error('Failed to create shift offer');
    return response.json();
};

export const claimShiftOffer = async (offerId: number) => {
    const response = await fetchWithAuth(`/shiftoffers/${offerId}/claim`, {
        method: 'POST',
    });
    if (!response.ok) throw new Error('Failed to claim shift offer');
    return response.json();
};

// Swaps API
export const getMySwaps = async () => {
    const response = await fetchWithAuth('/swaps');
    if (!response.ok) throw new Error('Failed to fetch swaps');
    return response.json();
};

export const getMyShifts = async () => {
    const me = await getMe();
    const response = await fetchWithAuth(`/shifts?userId=${me.id}`);
    if (!response.ok) throw new Error('Failed to fetch my shifts');
    return response.json();
};

export const getOpenShifts = async (departmentId?: number) => {
    const params = departmentId ? `?departmentId=${departmentId}` : '';
    const response = await fetchWithAuth(`/shifts${params}`);
    if (!response.ok) throw new Error('Failed to fetch shifts');
    const shifts = await response.json();
    // Filter for unassigned shifts (open shifts)
    return shifts.filter((s: any) => s.assignedUserId == null);
};

export const claimOpenShift = async (shiftId: number) => {
    const me = await getMe();
    // Fetch the shift to get necessary fields
    const shiftRes = await fetchWithAuth(`/shifts/${shiftId}`);
    if (!shiftRes.ok) throw new Error('Failed to fetch shift');
    const shift = await shiftRes.json();

    const payload = {
        Date: shift.date,
        StartTime: shift.startTime,
        EndTime: shift.endTime,
        DepartmentId: shift.departmentId,
        AssignedUserId: me.id,
        Notes: shift.notes,
        IsAvailableForSwap: shift.isAvailableForSwap
    };

    const response = await fetchWithAuth(`/shifts/${shiftId}`, {
        method: 'PUT',
        body: JSON.stringify(payload),
    });

    if (!response.ok) throw new Error('Failed to claim shift');
    return response.json();
};

export const createSwapRequest = async (data: { senderShiftId: number; receiverShiftId?: number; receiverId: number }) => {
    const response = await fetchWithAuth('/swaps', {
        method: 'POST',
        body: JSON.stringify(data),
    });
    if (!response.ok) throw new Error('Failed to create swap request');
    return response.json();
};

export const respondToSwap = async (swapId: number, accepted: boolean) => {
    const response = await fetchWithAuth(`/swaps/${swapId}/respond`, {
        method: 'PUT',
        body: JSON.stringify({ accepted }),
    });
    if (!response.ok) throw new Error('Failed to respond to swap');
    return response.json();
};

// Notifications API
export const getNotifications = async () => {
    const response = await fetchWithAuth('/notifications');
    if (!response.ok) throw new Error('Failed to fetch notifications');
    return response.json();
};

export const markNotificationAsRead = async (notificationId: number) => {
    const response = await fetchWithAuth(`/notifications/${notificationId}/read`, {
        method: 'PUT',
    });
    if (!response.ok) throw new Error('Failed to mark notification as read');
    return response.json();
};

export const markAllNotificationsAsRead = async () => {
    const response = await fetchWithAuth('/notifications/read-all', {
        method: 'PUT',
    });
    if (!response.ok) throw new Error('Failed to mark all notifications as read');
    return response.json();
};
