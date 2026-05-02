'use client';

import React, { useEffect, useState } from 'react';
import AuthenticatedLayout from '@/components/AuthenticatedLayout';
import { api } from '@/lib/api';
import { SwapRequest } from '@/types';
import { useAuth } from '@/hooks/useAuth';

export default function SwapRequestsPage() {
    const [swaps, setSwaps] = useState<SwapRequest[]>([]);
    const [loading, setLoading] = useState(true);
    const { user } = useAuth();

    const fetchSwaps = async () => {
        setLoading(true);
        try {
            const data = await api.getMySwaps();
            setSwaps(data);
        } catch {
            setSwaps([]);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchSwaps();
    }, []);

    const handleRespond = async (swapId: number, accepted: boolean) => {
        try {
            await api.respondToSwap(swapId, accepted);
            await fetchSwaps();
        } catch (err) {
            const error = err as Error;
            alert(error.message);
        }
    };

    return (
        <AuthenticatedLayout>
            <div className="space-y-8">
                <div className="border-b border-border pb-4">
                    <h1 className="text-3xl font-bold uppercase tracking-tight text-foreground">Swap Requests</h1>
                    <p className="text-muted-foreground mt-1 font-mono text-xs">Review and manage your shift exchange requests.</p>
                </div>

                <div className="grid grid-cols-1 gap-4">
                    {loading && (
                        <div className="text-center py-12 border border-border border-dashed font-mono animate-pulse">
                            LOADING REQUESTS...
                        </div>
                    )}

                    {!loading && swaps.map((swap) => (
                        <div key={swap.id} className="border border-border bg-surface p-6 shadow-sm hover:shadow-hard transition-all flex items-center justify-between group">
                            <div className="flex items-center gap-6">
                                <div className={`w-1 h-12 ${swap.status === 'Pending' ? 'bg-amber-500' : 'bg-green-500'}`} style={{ borderRadius: 0 }}></div>
                                <div>
                                    <p className="font-bold text-lg text-foreground">{swap.senderShift?.date} ({swap.senderShift?.startTime} - {swap.senderShift?.endTime})</p>
                                    <p className="text-sm text-muted-foreground font-mono">From: {swap.senderName} To: {swap.receiverName}</p>
                                </div>
                            </div>
                            <div className="flex items-center gap-4">
                                <span className={`text-xs font-bold uppercase tracking-wider px-3 py-1 border ${swap.status === 'Pending' ? 'text-amber-500 bg-amber-500/10 border-amber-500/20' : 'text-green-500 bg-green-500/10 border-green-500/20'}`} style={{ borderRadius: 0 }}>
                                    {swap.status}
                                </span>
                                {swap.status === 'Pending' && swap.receiverId === user?.id && (
                                    <div className="flex gap-2">
                                        <button onClick={() => handleRespond(swap.id, true)} className="bg-primary text-primary-foreground text-xs font-bold uppercase tracking-wider py-2 px-4 hover:bg-primary/90 transition-all hover:shadow-[2px_2px_0px_0px_var(--foreground)] active:translate-y-[1px] active:shadow-none" style={{ borderRadius: 0 }}>
                                            Accept
                                        </button>
                                        <button onClick={() => handleRespond(swap.id, false)} className="bg-background border border-border text-foreground text-xs font-bold uppercase tracking-wider py-2 px-4 hover:bg-muted transition-colors" style={{ borderRadius: 0 }}>
                                            Decline
                                        </button>
                                    </div>
                                )}
                            </div>
                        </div>
                    ))}

                    {swaps.length === 0 && !loading && (
                        <div className="text-center py-12 border border-border bg-surface border-dashed">
                            <p className="text-muted-foreground font-mono">NO PENDING SWAP REQUESTS</p>
                        </div>
                    )}
                </div>
            </div>
        </AuthenticatedLayout>
    );
}
