'use client';

import React, { useState, useEffect } from 'react';
import { getDepartmentEmployees, createSwapRequest, getMe } from '@/lib/api';

/**
 * CreateSwapModal: Allows a user to select a colleague and request 
 * a direct shift swap.
 */
interface CreateSwapModalProps {
    shiftId: number;
    shiftDate: string;
    onClose: () => void;
    onSuccess: () => void;
}

export default function CreateSwapModal({ shiftId, shiftDate, onClose, onSuccess }: CreateSwapModalProps) {
    // Junior-level approach: simple states for everything
    const [colleagues, setColleagues] = useState<any[]>([]);
    const [selectedReceiverId, setSelectedReceiverId] = useState<string>('');
    const [loading, setLoading] = useState(true);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => {
        // We need to know our department to fetch colleagues
        const fetchData = async () => {
            try {
                const me = await getMe();
                if (me?.departmentId) {
                    const employees = await getDepartmentEmployees(me.departmentId);
                    // Filter out ourselves from the list
                    const otherEmployees = employees.filter((e: any) => e.id !== me.id);
                    setColleagues(otherEmployees);
                } else {
                    setError('No department found for your user profile.');
                }
            } catch (err: any) {
                setError('Failed to load colleagues.');
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, []);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!selectedReceiverId) {
            setError('Please select a colleague.');
            return;
        }

        setSubmitting(true);
        setError('');

        try {
            // Note: createSwapRequest expects { senderShiftId, receiverId, receiverShiftId? }
            await createSwapRequest({
                senderShiftId: shiftId,
                receiverId: parseInt(selectedReceiverId),
                // receiverShiftId is optional and omitted for the "fastest path" general swap
            });

            onSuccess();
            onClose();
        } catch (err: any) {
            setError(err.message || 'Failed to send swap request.');
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
            <div className="bg-surface border border-border p-6 w-full max-w-md shadow-hard animate-in fade-in zoom-in duration-200" style={{ borderRadius: 0 }}>
                <h2 className="text-xl font-bold uppercase tracking-tight mb-2">Request Swap</h2>
                <p className="text-sm text-muted-foreground mb-6 font-mono">
                    Select a colleague to swap your shift on <span className="text-foreground font-bold">{shiftDate}</span>.
                </p>

                {loading ? (
                    <div className="text-center py-4 font-mono text-xs text-muted-foreground animate-pulse">
                        LOADING COLLEAGUES...
                    </div>
                ) : (
                    <form onSubmit={handleSubmit} className="space-y-4">
                        <div>
                            <label className="block text-xs font-bold uppercase tracking-wider mb-1">
                                Select Colleague
                            </label>
                            <select
                                value={selectedReceiverId}
                                onChange={(e) => setSelectedReceiverId(e.target.value)}
                                className="w-full bg-background border border-border p-2 focus:border-primary outline-none font-mono text-sm"
                                style={{ borderRadius: 0 }}
                                required
                            >
                                <option value="">-- Choose Colleague --</option>
                                {colleagues.map((c) => (
                                    <option key={c.id} value={c.id}>
                                        {c.fullName}
                                    </option>
                                ))}
                            </select>
                        </div>

                        {error && (
                            <div className="p-2 bg-red-500/10 border border-red-500/20 text-red-500 text-xs font-bold">
                                {error.toUpperCase()}
                            </div>
                        )}

                        {colleagues.length === 0 && !loading && !error && (
                            <p className="text-xs text-amber-500 font-bold uppercase">
                                No colleagues found in your department.
                            </p>
                        )}

                        <div className="flex justify-end gap-3 pt-2">
                            <button
                                type="button"
                                onClick={onClose}
                                className="px-4 py-2 border border-border text-xs font-bold uppercase tracking-widest hover:bg-muted transition-colors"
                                style={{ borderRadius: 0 }}
                            >
                                Cancel
                            </button>
                            <button
                                type="submit"
                                disabled={submitting || colleagues.length === 0}
                                className="px-6 py-2 bg-primary text-primary-foreground text-xs font-bold uppercase tracking-widest hover:shadow-[3px_3px_0px_0px_var(--foreground)] active:translate-y-[1px] active:shadow-none transition-all disabled:opacity-50"
                                style={{ borderRadius: 0 }}
                            >
                                {submitting ? 'SENDING...' : 'SEND REQUEST'}
                            </button>
                        </div>
                    </form>
                )}
            </div>
        </div>
    );
}
