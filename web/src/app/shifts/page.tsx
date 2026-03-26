'use client';

import React, { useEffect, useState } from 'react';
import AuthenticatedLayout from '@/components/AuthenticatedLayout';
import ShiftTable, { Shift } from '@/components/ShiftTable';
import { getMyShifts } from '@/lib/api';
import OfferShiftModal from '@/components/OfferShiftModal';
import CreateSwapModal from '@/components/CreateSwapModal';

/**
 * MyShiftsPage: Displays the user's personal schedule.
 * Users can offer their shifts to the marketplace or request a swap with a colleague.
 */
export default function MyShiftsPage() {
    // Junior-level state: explicit loading, error, and data states
    const [shifts, setShifts] = useState<Shift[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    // Modal control states
    const [showOfferModal, setShowOfferModal] = useState(false);
    const [showSwapModal, setShowSwapModal] = useState(false);
    const [selectedShift, setSelectedShift] = useState<Shift | null>(null);

    // Initial load: fetch my shifts from the backend
    const fetchShifts = async () => {
        setLoading(true);
        try {
            const data = await getMyShifts();
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

    // Handlers for the "Offer Shift" flow
    const handleOfferClick = (shift: Shift) => {
        setSelectedShift(shift);
        setShowOfferModal(true);
    };

    // Handlers for the "Request Swap" flow
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

                {/* Show different UI states: Loading -> Error -> Table */}
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

                        {/* 
                          Since ShiftTable usually only takes one action, let's render 
                          the list manually or update ShiftTable to be more flexible. 
                          I will update the page to show buttons next to each row if 
                          ShiftTable is too rigid, but for the "Fastest Path", 
                          let's just modify the table rows directly if possible.
                        */}
                    </div>
                )}

                {/* Manual fallback if table is empty */}
                {shifts.length === 0 && !loading && !error && (
                    <div className="text-center py-12 border border-border bg-surface border-dashed">
                        <p className="text-muted-foreground font-mono">YOU HAVE NO UPCOMING SHIFTS ASSIGNED</p>
                    </div>
                )}

                {/* Modals are rendered conditionally based on state */}
                {showOfferModal && selectedShift && (
                    <OfferShiftModal
                        shiftId={selectedShift.id}
                        shiftDate={new Date(selectedShift.startTime).toLocaleDateString()}
                        onClose={() => setShowOfferModal(false)}
                        onSuccess={() => fetchShifts()} // Refresh list on success
                    />
                )}

                {showSwapModal && selectedShift && (
                    <CreateSwapModal
                        shiftId={selectedShift.id}
                        shiftDate={new Date(selectedShift.startTime).toLocaleDateString()}
                        onClose={() => setShowSwapModal(false)}
                        onSuccess={() => fetchShifts()} // Refresh list on success
                    />
                )}
            </div>
        </AuthenticatedLayout>
    );
}
