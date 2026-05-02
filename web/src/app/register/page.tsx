'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/hooks/useAuth';

export default function RegisterPage() {
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        confirmPassword: '',
        firstName: '',
        lastName: '',
    });
    
    const { register, error: authError, loading, isAuthenticated } = useAuth();
    const [localError, setLocalError] = useState('');
    const router = useRouter();

    useEffect(() => {
        if (isAuthenticated) {
            router.push('/dashboard');
        }
    }, [isAuthenticated, router]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLocalError('');

        if (formData.password !== formData.confirmPassword) {
            setLocalError('Passwords do not match');
            return;
        }

        await register(
            formData.email,
            formData.password,
            formData.firstName,
            formData.lastName
        );
    };

    const displayError = localError || authError;

    return (
        <div className="flex min-h-screen items-center justify-center bg-background p-4">
            <div className="w-full max-w-md space-y-8 border border-border bg-surface p-8 shadow-hard">
                <div className="text-center">
                    <div className="mx-auto mb-6 flex h-16 w-16 items-center justify-center bg-primary text-3xl font-bold text-primary-foreground" style={{ borderRadius: 0 }}>
                        W
                    </div>
                    <h1 className="text-2xl font-bold tracking-tight text-foreground uppercase">Create Account</h1>
                    <p className="mt-2 text-sm text-muted-foreground font-mono">Join WorkSwap system.</p>
                </div>

                <form className="mt-8 space-y-4" onSubmit={handleSubmit}>
                    <div className="grid grid-cols-2 gap-4">
                        <div className="space-y-1">
                            <label className="text-xs font-mono font-medium uppercase tracking-wider text-muted-foreground">First Name</label>
                            <input
                                name="firstName"
                                type="text"
                                required
                                placeholder="JANE"
                                value={formData.firstName}
                                onChange={handleChange}
                                className="w-full border border-border bg-background px-3 py-3 text-sm text-foreground focus:border-primary focus:ring-1 focus:ring-primary focus:outline-none transition-all"
                                style={{ borderRadius: 0 }}
                            />
                        </div>
                        <div className="space-y-1">
                            <label className="text-xs font-mono font-medium uppercase tracking-wider text-muted-foreground">Last Name</label>
                            <input
                                name="lastName"
                                type="text"
                                required
                                placeholder="DOE"
                                value={formData.lastName}
                                onChange={handleChange}
                                className="w-full border border-border bg-background px-3 py-3 text-sm text-foreground focus:border-primary focus:ring-1 focus:ring-primary focus:outline-none transition-all"
                                style={{ borderRadius: 0 }}
                            />
                        </div>
                    </div>

                    <div className="space-y-1">
                        <label className="text-xs font-mono font-medium uppercase tracking-wider text-muted-foreground">Email</label>
                        <input
                            name="email"
                            type="email"
                            required
                            placeholder="user@example.com"
                            value={formData.email}
                            onChange={handleChange}
                            className="w-full border border-border bg-background px-3 py-3 text-sm text-foreground focus:border-primary focus:ring-1 focus:ring-primary focus:outline-none transition-all"
                            style={{ borderRadius: 0 }}
                        />
                    </div>

                    <div className="space-y-1">
                        <label className="text-xs font-mono font-medium uppercase tracking-wider text-muted-foreground">Password</label>
                        <input
                            name="password"
                            type="password"
                            required
                            placeholder="••••••••"
                            value={formData.password}
                            onChange={handleChange}
                            className="w-full border border-border bg-background px-3 py-3 text-sm text-foreground focus:border-primary focus:ring-1 focus:ring-primary focus:outline-none transition-all"
                            style={{ borderRadius: 0 }}
                        />
                    </div>

                    <div className="space-y-1">
                        <label className="text-xs font-mono font-medium uppercase tracking-wider text-muted-foreground">Confirm Password</label>
                        <input
                            name="confirmPassword"
                            type="password"
                            required
                            placeholder="••••••••"
                            value={formData.confirmPassword}
                            onChange={handleChange}
                            className="w-full border border-border bg-background px-3 py-3 text-sm text-foreground focus:border-primary focus:ring-1 focus:ring-primary focus:outline-none transition-all"
                            style={{ borderRadius: 0 }}
                        />
                    </div>

                    {displayError && (
                        <div className="border border-destructive/50 bg-destructive/10 p-3 text-xs text-destructive font-mono">
                            [ERROR]: {displayError}
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={loading}
                        className="group relative flex w-full justify-center border border-transparent bg-primary px-4 py-3 text-sm font-bold text-primary-foreground hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 disabled:opacity-50 transition-all hover:-translate-y-0.5 hover:shadow-[2px_2px_0px_0px_var(--foreground)] active:translate-y-0 active:shadow-none mt-4"
                        style={{ borderRadius: 0 }}
                    >
                        {loading ? 'INITIALIZING...' : 'CREATE ACCOUNT'}
                    </button>

                    <div className="text-center mt-6">
                        <p className="text-xs text-muted-foreground">
                            ALREADY HAVE AN ACCOUNT?{' '}
                            <Link href="/" className="font-medium text-primary hover:underline hover:text-primary/80 transition-colors">
                                SIGN IN
                            </Link>
                        </p>
                    </div>
                </form>
            </div>
        </div>
    );
}
