'use client';

import React from 'react';
import AuthenticatedLayout from '@/components/AuthenticatedLayout';
import { getShiftOffers, claimShiftOffer, getMe } from '@/lib/api';

export default function OffersPage() {
    const [offers, setOffers] = React.useState<any[]>([]);
    const [loading, setLoading] = React.useState(true);
    const [error, setError] = React.useState('');
    const [me, setMe] = React.useState<any | null>(null);
    const [claiming, setClaiming] = React.useState<number | null>(null);

    React.useEffect(() => {
        let mounted = true;
        Promise.all([getShiftOffers(), getMe()])
            .then(([data, user]) => {
                if (!mounted) return;
                setOffers(data);
                setMe(user);
            })
            .catch((err) => { if (mounted) setError((err as Error).message); })
            .finally(() => { if (mounted) setLoading(false); });
        return () => { mounted = false; };
    }, []);

    const handleClaim = async (offerId: number) => {
        try {
            setClaiming(offerId);
            await claimShiftOffer(offerId);
            const data = await getShiftOffers();
            setOffers(data);
            alert('Offer claimed successfully');
        } catch (err) {
            alert((err as Error).message);
        } finally {
            setClaiming(null);
        }
    };

    return (
        <AuthenticatedLayout>
            <div className="space-y-8">
                <div className="border-b border-border pb-4">
                    <h1 className="text-3xl font-bold uppercase tracking-tight text-foreground">Shift Offers</h1>
                    <p className="text-muted-foreground mt-1 font-mono text-xs">Browse active shift offers in your department and claim available shifts.</p>
                </div>

                {loading && <p className="text-muted-foreground">Loading offers...</p>}
                {error && <p className="text-red-500">{error}</p>}

                <div className="grid grid-cols-1 gap-4">
                    {offers.map((o) => (
                        <div key={o.id} className="border border-border bg-surface p-6 shadow-sm hover:shadow-hard flex items-center justify-between">
                            <div>
                                <p className="font-bold text-lg text-foreground">{o.shift.date} ({o.shift.startTime} - {o.shift.endTime})</p>
                                <p className="text-sm text-muted-foreground font-mono">Offered by: {o.offeredByName}</p>
                                <p className="text-xs text-muted-foreground">Expires: {new Date(o.expiresAt).toLocaleString()}</p>
                            </div>
                            <div className="flex items-center gap-4">
                                <span className={`text-xs font-bold uppercase tracking-wider px-3 py-1 border ${o.status === 'Active' ? 'text-amber-500 bg-amber-500/10 border-amber-500/20' : 'text-muted-foreground'}`} style={{ borderRadius: 0 }}>
                                    {o.status}
                                </span>
                                {o.status === 'Active' && o.offeredById !== me?.id && (
                                    <button
                                        onClick={() => handleClaim(o.id)}
                                        disabled={claiming === o.id}
                                        className="bg-primary text-primary-foreground text-xs font-bold uppercase tracking-wider py-2 px-4 hover:bg-primary/90 transition-all"
                                        style={{ borderRadius: 0 }}
                                    >
                                        {claiming === o.id ? 'Claiming...' : 'Claim'}
                                    </button>
                                )}
                            </div>
                        </div>
                    ))}

                    {offers.length === 0 && !loading && (
                        <div className="text-center py-12 border border-border bg-surface border-dashed">
                            <p className="text-muted-foreground font-mono">NO ACTIVE OFFERS</p>
                        </div>
                    )}
                </div>
            </div>
        </AuthenticatedLayout>
    );
}
