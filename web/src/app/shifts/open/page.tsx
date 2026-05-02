'use client';

import React, { useEffect, useState } from 'react';
import AuthenticatedLayout from '@/components/AuthenticatedLayout';
import ShiftTable from '@/components/ShiftTable';
import { api } from '@/lib/api';
import { Shift } from '@/types';

export default function OpenShiftsPage() {
    const [openShifts, setOpenShifts] = useState<Shift[]>([]);
    const [loading, setLoading] = useState(true);

    const fetchOpenShifts = async () => {
        setLoading(true);
        try {
            const data = await api.getOpenShifts();
            setOpenShifts(data);
        } catch {
            setOpenShifts([]);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchOpenShifts();
    }, []);

    const handleClaimShift = async (shift: Shift) => {
        try {
            await api.claimOpenShift(shift.id);
            await fetchOpenShifts();
            alert('Shift successfully claimed');
        } catch (err) {
            const error = err as Error;
            alert(error.message);
        }
    };

    return (
        <AuthenticatedLayout>
            <div className="space-y-6">
                <div className="border-b border-border pb-4">
                    <h1 className="text-3xl font-bold uppercase tracking-tight text-foreground">Open Shifts</h1>
                    <p className="text-muted-foreground mt-1 font-mono text-xs">Pick up extra shifts available in your department.</p>
                </div>

                <div className="border border-border bg-surface p-0 shadow-hard">
                    {loading ? (
                        <div className="text-center py-12 font-mono animate-pulse">LOADING OPEN SHIFTS...</div>
                    ) : (
                        <ShiftTable
                            shifts={openShifts}
                            actionLabel="CLAIM SHIFT"
                            onAction={handleClaimShift}
                        />
                    )}
                </div>

                {!loading && openShifts.length === 0 && (
                    <div className="text-center py-12 border border-border bg-surface border-dashed">
                        <p className="text-muted-foreground font-mono">NO OPEN SHIFTS AVAILABLE AT THIS TIME</p>
                    </div>
                )}
            </div>
        </AuthenticatedLayout>
    );
}