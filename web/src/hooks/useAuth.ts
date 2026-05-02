'use client';

import { useState, useEffect, useCallback } from 'react';
import { api } from '@/lib/api';
import { UserInfo } from '@/types';
import { useRouter } from 'next/navigation';

export function useAuth() {
    const [user, setUser] = useState<UserInfo | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const router = useRouter();

    const fetchUser = useCallback(async () => {
        const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;
        if (!token) {
            setUser(null);
            setLoading(false);
            return;
        }

        try {
            const userInfo = await api.getMe();
            setUser(userInfo);
        } catch (err) {
            console.error('Failed to fetch user:', err);
            setUser(null);
            // If we're not on the login/register page, we might want to redirect
            // but let's keep it simple for now.
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchUser();
    }, [fetchUser]);

    const login = async (email: string, password: string) => {
        setLoading(true);
        setError(null);
        try {
            const res = await api.login(email, password);
            if (res.success) {
                await fetchUser();
                router.push('/dashboard');
            } else {
                setError(res.error || 'Login failed');
            }
        } catch (err) {
            const error = err as Error;
            setError(error.message || 'Login failed');
        } finally {
            setLoading(false);
        }
    };

    const register = async (email: string, password: string, firstName: string, lastName: string) => {
        setLoading(true);
        setError(null);
        try {
            const res = await api.register(email, password, firstName, lastName);
            if (res.success) {
                await fetchUser();
                router.push('/dashboard');
            } else {
                setError(res.error || 'Registration failed');
            }
        } catch (err) {
            const error = err as Error;
            setError(error.message || 'Registration failed');
        } finally {
            setLoading(false);
        }
    };

    const logout = () => {
        api.logout();
        setUser(null);
        router.push('/login');
    };

    return {
        user,
        loading,
        error,
        login,
        register,
        logout,
        isAuthenticated: !!user,
        refreshUser: fetchUser
    };
}
