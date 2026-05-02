'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { api } from '@/lib/api';
import { UserInfo, Shift, SwapRequest, Notification } from '@/types';

import AuthenticatedLayout from '@/components/AuthenticatedLayout';
import StatCard from '@/components/StatCard';
import ShiftTable from '@/components/ShiftTable';

export default function DashboardPage() {
    const [user, setUser] = useState<UserInfo | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [recentShifts, setRecentShifts] = useState<Shift[]>([]);
    const [stats, setStats] = useState({
        totalShifts: 0,
        openShifts: 0,
        pendingSwaps: 0,
        totalHours: 0
    });

    useEffect(() => {
        let mounted = true;
        
        const fetchData = async () => {
            try {
                const [me, shifts, openShifts, swaps] = await Promise.all([
                    api.getMe(),
                    api.getMyShifts(),
                    api.getOpenShifts(),
                    api.getMySwaps()
                ]);

                if (!mounted) return;

                setUser(me);
                setRecentShifts(shifts.slice(0, 5));

                // Calculate stats
                const pendingSwaps = swaps.filter(s => s.status === 'Pending').length;
                
                // Calculate hours for shifts
                const totalHours = shifts.reduce((acc, shift) => {
                    const start = new Date(`2000-01-01T${shift.startTime}`);
                    const end = new Date(`2000-01-01T${shift.endTime}`);
                    let diff = (end.getTime() - start.getTime()) / (1000 * 60 * 60);
                    if (diff < 0) diff += 24; // Handle overnight shifts
                    return acc + diff;
                }, 0);

                setStats({
                    totalShifts: shifts.length,
                    openShifts: openShifts.length,
                    pendingSwaps,
                    totalHours: Math.round(totalHours * 10) / 10
                });

            } catch (err) {
                if (mounted) setError((err as Error).message);
            } finally {
                if (mounted) setLoading(false);
            }
        };

        fetchData();
        return () => { mounted = false; };
    }, []);

    if (loading) return null;

    return (
        <AuthenticatedLayout>
            <div className="space-y-8">
                <div>
                    <h1 className="text-3xl font-bold uppercase tracking-tight text-foreground">Welcome back, {user?.firstName}</h1>
                    <p className="text-muted-foreground mt-2 font-mono text-sm max-w-2xl">
                        Overview of your schedule, open shifts, and swap requests.
                    </p>
                </div>

                <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
                    <StatCard
                        label="My Shifts"
                        value={stats.totalShifts.toString()}
                        description="Assigned to you"
                        icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" /></svg>}
                    />
                    <StatCard
                        label="Open Shifts"
                        value={stats.openShifts.toString()}
                        description="Available in your department"
                        icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>}
                    />
                    <StatCard
                        label="Pending Swaps"
                        value={stats.pendingSwaps.toString()}
                        description="Awaiting response"
                        icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" /></svg>}
                    />
                    <StatCard
                        label="Hours Scheduled"
                        value={stats.totalHours.toString()}
                        description="Total hours this period"
                        icon={<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>}
                    />
                </div>

                <div className="grid grid-cols-1 gap-8 lg:grid-cols-3">
                    <div className="lg:col-span-2 space-y-6">
                        <div className="flex items-center justify-between border-b border-border pb-2">
                            <h2 className="text-xl font-bold uppercase tracking-tight">Upcoming Shifts</h2>
                            <Link href="/shifts" className="text-xs font-bold uppercase tracking-wider text-primary hover:text-primary/80 hover:underline">View all</Link>
                        </div>
                        <ShiftTable shifts={recentShifts} />
                    </div>

                    <div className="space-y-6">
                        <h2 className="text-xl font-bold uppercase tracking-tight border-b border-border pb-2">Quick Actions</h2>
                        <div className="grid grid-cols-1 gap-4">
                            <Link href="/shifts/open" className="group flex items-center gap-4 bg-surface border border-border p-4 hover:border-primary transition-all hover:shadow-hard hover:-translate-y-0.5" style={{ borderRadius: 0 }}>
                                <div className="p-2 bg-blue-500/10 text-blue-500 border border-blue-500/20" style={{ borderRadius: 0 }}>
                                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M12 4v16m8-8H4" /></svg>
                                </div>
                                <div>
                                    <span className="block font-bold text-sm uppercase">Browse Open Shifts</span>
                                    <span className="text-xs text-muted-foreground">Find extra coverage</span>
                                </div>
                            </Link>
                            <Link href="/offers" className="group flex items-center gap-4 bg-surface border border-border p-4 hover:border-amber-500 transition-all hover:shadow-hard hover:-translate-y-0.5" style={{ borderRadius: 0 }}>
                                <div className="p-2 bg-amber-500/10 text-amber-500 border border-amber-500/20" style={{ borderRadius: 0 }}>
                                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M4 7h16M4 12h8m-8 5h16" /></svg>
                                </div>
                                <div>
                                    <span className="block font-bold text-sm uppercase">Browse Offers</span>
                                    <span className="text-xs text-muted-foreground">Pick up offered shifts</span>
                                </div>
                            </Link>
                            <Link href="/swaps" className="group flex items-center gap-4 bg-surface border border-border p-4 hover:border-amber-500 transition-all hover:shadow-hard hover:-translate-y-0.5" style={{ borderRadius: 0 }}>
                                <div className="p-2 bg-amber-500/10 text-amber-500 border border-amber-500/20" style={{ borderRadius: 0 }}>
                                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" /></svg>
                                </div>
                                <div>
                                    <span className="block font-bold text-sm uppercase">Request Shift Swap</span>
                                    <span className="text-xs text-muted-foreground">Trade with a colleague</span>
                                </div>
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </AuthenticatedLayout>
    );
}
