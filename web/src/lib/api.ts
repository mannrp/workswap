import { 
    AuthResponse, 
    UserInfo, 
    Shift, 
    ShiftOffer, 
    SwapRequest, 
    CreateSwapRequest,
    Notification 
} from '../types';

export const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5156/api';

class ApiClient {
    private getToken(): string | null {
        if (typeof window !== 'undefined') {
            return localStorage.getItem('token');
        }
        return null;
    }

    private setToken(token: string): void {
        if (typeof window !== 'undefined') {
            localStorage.setItem('token', token);
        }
    }

    private removeToken(): void {
        if (typeof window !== 'undefined') {
            localStorage.removeItem('token');
        }
    }

    private async request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
        const token = this.getToken();
        const headers = {
            'Content-Type': 'application/json',
            ...options.headers,
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
        } as HeadersInit;

        const response = await fetch(`${API_URL}${endpoint}`, {
            ...options,
            headers,
        });

        if (response.status === 401) {
            this.removeToken();
            if (typeof window !== 'undefined' && window.location.pathname !== '/login') {
                window.location.href = '/login';
            }
        }

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            throw new Error(errorData.message || `API request failed with status ${response.status}`);
        }

        return response.json();
    }

    // --- Auth ---
    async login(email: string, password: string): Promise<AuthResponse> {
        const data = await this.request<AuthResponse>('/auth/login', {
            method: 'POST',
            body: JSON.stringify({ email, password }),
        });
        if (data.success && data.token) {
            this.setToken(data.token);
        }
        return data;
    }

    async register(email: string, password: string, firstName: string, lastName: string): Promise<AuthResponse> {
        const data = await this.request<AuthResponse>('/auth/register', {
            method: 'POST',
            body: JSON.stringify({ email, password, firstName, lastName }),
        });
        if (data.success && data.token) {
            this.setToken(data.token);
        }
        return data;
    }

    async getMe(): Promise<UserInfo> {
        return this.request<UserInfo>('/auth/me');
    }

    logout(): void {
        this.removeToken();
        if (typeof window !== 'undefined') {
            window.location.href = '/login';
        }
    }

    // --- Shifts ---
    async getMyShifts(): Promise<Shift[]> {
        const me = await this.getMe();
        return this.request<Shift[]>(`/shifts?userId=${me.id}`);
    }

    async getOpenShifts(departmentId?: number): Promise<Shift[]> {
        const params = departmentId ? `?departmentId=${departmentId}` : '';
        const shifts = await this.request<Shift[]>(`/shifts${params}`);
        return shifts.filter(s => s.assignedUserId == null);
    }

    async getShift(id: number): Promise<Shift> {
        return this.request<Shift>(`/shifts/${id}`);
    }

    async updateShift(id: number, data: Partial<Shift>): Promise<Shift> {
        return this.request<Shift>(`/shifts/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data),
        });
    }

    async claimOpenShift(shiftId: number): Promise<Shift> {
        const me = await this.getMe();
        const shift = await this.getShift(shiftId);

        const payload = {
            date: shift.date,
            startTime: shift.startTime,
            endTime: shift.endTime,
            departmentId: shift.departmentId,
            assignedUserId: me.id,
            notes: shift.notes,
            isAvailableForSwap: shift.isAvailableForSwap
        };

        return this.updateShift(shiftId, payload);
    }

    // --- Shift Offers ---
    async getShiftOffers(departmentId?: number): Promise<ShiftOffer[]> {
        const params = departmentId ? `?departmentId=${departmentId}` : '';
        return this.request<ShiftOffer[]>(`/shiftoffers${params}`);
    }

    async createShiftOffer(shiftId: number, expiresAt?: string): Promise<ShiftOffer> {
        return this.request<ShiftOffer>(`/shifts/${shiftId}/offer`, {
            method: 'POST',
            body: JSON.stringify({ expiresAt }),
        });
    }

    async claimShiftOffer(offerId: number): Promise<ShiftOffer> {
        return this.request<ShiftOffer>(`/shiftoffers/${offerId}/claim`, {
            method: 'POST',
        });
    }

    // --- Swaps ---
    async getMySwaps(): Promise<SwapRequest[]> {
        return this.request<SwapRequest[]>('/swaps');
    }

    async createSwapRequest(data: { senderShiftId: number; receiverShiftId?: number; receiverId: number }): Promise<SwapRequest> {
        return this.request<SwapRequest>('/swaps', {
            method: 'POST',
            body: JSON.stringify(data),
        });
    }

    async respondToSwap(swapId: number, accepted: boolean): Promise<{ message: string }> {
        return this.request<{ message: string }>(`/swaps/${swapId}/respond`, {
            method: 'PUT',
            body: JSON.stringify({ accepted }),
        });
    }

    // --- Departments ---
    async getDepartmentEmployees(departmentId: number): Promise<UserShort[]> {
        return this.request<UserShort[]>(`/departments/${departmentId}/employees`);
    }

    // --- Notifications ---
    async getNotifications(): Promise<Notification[]> {
        return this.request<Notification[]>('/notifications');
    }

    async markNotificationAsRead(id: number): Promise<void> {
        return this.request<void>(`/notifications/${id}/read`, { method: 'PUT' });
    }

    async markAllNotificationsAsRead(): Promise<void> {
        return this.request<void>('/notifications/read-all', { method: 'PUT' });
    }
}

interface UserShort {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
}

export const api = new ApiClient();

// Maintain backward compatibility for loose functions if needed, 
// but it's better to refactor callers to use 'api' instance.
// I'll provide these as wrappers to avoid breaking everything at once.
export const login = (e: string, p: string) => api.login(e, p);
export const register = (e: string, p: string, f: string, l: string) => api.register(e, p, f, l);
export const getMe = () => api.getMe();
export const getMyShifts = () => api.getMyShifts();
export const getOpenShifts = (d?: number) => api.getOpenShifts(d);
export const claimOpenShift = (id: number) => api.claimOpenShift(id);
export const getShiftOffers = (d?: number) => api.getShiftOffers(d);
export const createShiftOffer = (id: number, ex?: string) => api.createShiftOffer(id, ex);
export const claimShiftOffer = (id: number) => api.claimShiftOffer(id);
export const getMySwaps = () => api.getMySwaps();
export const createSwapRequest = (d: CreateSwapRequest) => api.createSwapRequest(d);
export const respondToSwap = (id: number, a: boolean) => api.respondToSwap(id, a);
export const getDepartmentEmployees = (id: number) => api.getDepartmentEmployees(id);
export const getNotifications = () => api.getNotifications();
export const markNotificationAsRead = (id: number) => api.markNotificationAsRead(id);
export const markAllNotificationsAsRead = () => api.markAllNotificationsAsRead();
export const getToken = () => typeof window !== 'undefined' ? localStorage.getItem('token') : null;
