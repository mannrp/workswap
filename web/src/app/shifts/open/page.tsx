'use client';

import React from 'react';
import AuthenticatedLayout from '@/components/AuthenticatedLayout';
import ShiftTable, { Shift } from '@/components/ShiftTable';
import { getOpenShifts, claimOpenShift } from '@/lib/api';

export default function OpenShiftsPage() {
    const [openShifts, setOpenShifts] = React.useState<Shift[]>([]);
    const [loading, setLoading] = React.useState(true);

    React.useEffect(() => {
        let mounted = true;
        getOpenShifts()
            .then((data) => { if (mounted) setOpenShifts(data); })
            .catch(() => { if (mounted) setOpenShifts([]); })
            .finally(() => { if (mounted) setLoading(false); });
        return () => { mounted = false; };
    }, []);

    const handleClaimShift = async (shift: Shift) => {
        try {
            await claimOpenShift(shift.id);
            const data = await getOpenShifts();
            setOpenShifts(data);
            alert('Shift claimed');
        } catch (err) {
            alert((err as Error).message);
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
                    {loading ? <p className="p-6">Loading...</p> : (
                        <ShiftTable
                            shifts={openShifts}
                            actionLabel="CLAIM SHIFT"
                            onAction={handleClaimShift}
                        />
                    )}
                </div>
            </div>
        </AuthenticatedLayout>
    );
}
