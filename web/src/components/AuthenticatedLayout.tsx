'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Sidebar from '@/components/Sidebar';
import Navbar from '@/components/Navbar';
import { useAuth } from '@/hooks/useAuth';

interface AuthenticatedLayoutProps {
    children: React.ReactNode;
}

export default function AuthenticatedLayout({ children }: AuthenticatedLayoutProps) {
    const { user, loading, isAuthenticated } = useAuth();
    const router = useRouter();

    useEffect(() => {
        if (!loading && !isAuthenticated) {
            router.push('/');
        }
    }, [loading, isAuthenticated, router]);

    if (loading) {
        return (
            <div className="flex min-h-screen items-center justify-center bg-background text-muted-foreground">
                <div className="flex flex-col items-center gap-6">
                    {/* Square spinner */}
                    <div className="h-12 w-12 animate-spin border-4 border-border border-t-primary" style={{ borderRadius: 0 }}></div>
                    <p className="font-mono text-sm uppercase tracking-widest">Loading Workspace...</p>
                </div>
            </div>
        );
    }

    if (!isAuthenticated) return null;

    return (
        <div className="flex min-h-screen bg-background">
            <Sidebar />
            <div className="flex-1 flex flex-col">
                <Navbar user={user} />
                <main className="flex-1 p-8 overflow-auto">
                    <div className="container mx-auto">
                        {children}
                    </div>
                </main>
            </div>
        </div>
    );
}
