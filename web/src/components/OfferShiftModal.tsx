'use client';

import React, { useState } from 'react';
import { api } from '@/lib/api';

/**
 * OfferShiftModal: A simple modal that lets a user put their shift 
 * on the marketplace so others can claim it.
 */
interface OfferShiftModalProps {
    shiftId: number;
    shiftDate: string;
    onClose: () => void;
    onSuccess: () => void;
}

export default function OfferShiftModal({ shiftId, shiftDate, onClose, onSuccess }: OfferShiftModalProps) {
    const [expiresAt, setExpiresAt] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const expiryParam = expiresAt ? new Date(expiresAt).toISOString() : undefined;
            await api.createShiftOffer(shiftId, expiryParam);

            onSuccess();
            onClose();
        } catch (err) {
            const error = err as Error;
            setError(error.message || 'Failed to create offer');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
            <div className="bg-surface border border-border p-6 w-full max-w-md shadow-hard animate-in fade-in zoom-in duration-200" style={{ borderRadius: 0 }}>
                <h2 className="text-xl font-bold uppercase tracking-tight mb-2">Offer Shift</h2>
                <p className="text-sm text-muted-foreground mb-6 font-mono">
                    You are offering your shift on <span className="text-foreground font-bold">{shiftDate}</span> to the marketplace.
                </p>

                <form onSubmit={handleSubmit} className="space-y-4">
                    <div>
                        <label className="block text-xs font-bold uppercase tracking-wider mb-1">
                            Offer Expiration (Optional)
                        </label>
                        <input
                            type="datetime-local"
                            value={expiresAt}
                            onChange={(e) => setExpiresAt(e.target.value)}
                            className="w-full bg-background border border-border p-2 focus:border-primary outline-none font-mono text-sm"
                            style={{ borderRadius: 0 }}
                        />
                        <p className="text-[10px] text-muted-foreground mt-1 uppercase">
                            If left blank, the offer will expire in 7 days.
                        </p>
                    </div>

                    {error && (
                        <div className="p-2 bg-red-500/10 border border-red-500/20 text-red-500 text-xs font-bold">
                            {error.toUpperCase()}
                        </div>
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
                            disabled={loading}
                            className="px-6 py-2 bg-primary text-primary-foreground text-xs font-bold uppercase tracking-widest hover:shadow-[3px_3px_0px_0px_var(--foreground)] active:translate-y-[1px] active:shadow-none transition-all disabled:opacity-50"
                            style={{ borderRadius: 0 }}
                        >
                            {loading ? 'OFFERING...' : 'CONFIRM OFFER'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
