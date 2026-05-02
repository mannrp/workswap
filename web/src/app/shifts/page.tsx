'use client';

import React, { useEffect, useState } from 'react';
import AuthenticatedLayout from '@/components/AuthenticatedLayout';
import ShiftTable from '@/components/ShiftTable';
import { api } from '@/lib/api';
import { Shift } from '@/types';
import OfferShiftModal from '@/components/OfferShiftModal';
import CreateSwapModal from '@/components/CreateSwapModal';

/**
 * MyShiftsPage: Displays the user's personal schedule.
 */
export default function MyShiftsPage() {
    const [shifts, setShifts] = useState<Shift[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const [showOfferModal, setShowOfferModal] = useState(false);
    const [showSwapModal, setShowSwapModal] = useState(false);
    const [selectedShift, setSelectedShift] = useState<Shift | null>(null);

    const fetchShifts = async () => {
        setLoading(true);
        try {
            const data = await api.getMyShifts();
            setShifts(data);
        } catch (err: any) {
            setError('Failed to load your shifts. Please try again later.');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchShifts();
    }, []);

    const handleOfferClick = (shift: Shift) => {
        setSelectedShift(shift);
        setShowOfferModal(true);
    };

    const handleSwapClick = (shift: Shift) => {
        setSelectedShift(shift);
        setShowSwapModal(true);
    };

    return (
        <AuthenticatedLayout>
            <div className="space-y-6">
                <div className="flex justify-between items-end border-b border-border pb-4">
                    <div>
                        <h1 className="text-3xl font-bold uppercase tracking-tight text-foreground">My Schedule</h1>
                        <p className="text-muted-foreground mt-1 font-mono text-xs">Manage your assigned shifts and requests.</p>
                    </div>
                </div>

                {loading ? (
                    <div className="text-center py-12 border border-border border-dashed font-mono animate-pulse">
                        LOADING YOUR SCHEDULE...
                    </div>
                ) : error ? (
                    <div className="p-6 border border-red-500 bg-red-500/5 text-red-500 font-bold uppercase text-center">
                        {error}
                    </div>
                ) : (
                    <div className="border border-border bg-surface p-0 shadow-hard">
                        <ShiftTable
                            shifts={shifts}
                            onOffer={handleOfferClick}
                            onSwap={handleSwapClick}
                        />
                    </div>
                )}

                {shifts.length === 0 && !loading && !error && (
                    <div className="text-center py-12 border border-border bg-surface border-dashed">
                        <p className="text-muted-foreground font-mono">YOU HAVE NO UPCOMING SHIFTS ASSIGNED</p>
                    </div>
                )}

                {showOfferModal && selectedShift && (
                    <OfferShiftModal
                        shiftId={selectedShift.id}
                        shiftDate={selectedShift.date}
                        onClose={() => setShowOfferModal(false)}
                        onSuccess={() => fetchShifts()}
                    />
                )}

                {showSwapModal && selectedShift && (
                    <CreateSwapModal
                        shiftId={selectedShift.id}
                        shiftDate={selectedShift.date}
                        onClose={() => setShowSwapModal(false)}
                        onSuccess={() => fetchShifts()}
                    />
                )}
            </div>
        </AuthenticatedLayout>
    );
}
