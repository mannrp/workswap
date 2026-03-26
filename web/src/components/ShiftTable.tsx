'use client';

export interface Shift {
    id: number;
    startTime: string;
    endTime: string;
    role: string;
    status: 'Assigned' | 'Open' | 'PendingTransfer' | 'Claimed';
    user?: string;
}

interface ShiftTableProps {
    shifts: Shift[];
    onAction?: (shift: Shift) => void;
    actionLabel?: string;
    // New optional actions for multiple buttons
    onOffer?: (shift: Shift) => void;
    onSwap?: (shift: Shift) => void;
}

export default function ShiftTable({ shifts, onAction, actionLabel, onOffer, onSwap }: ShiftTableProps) {
    const formatDate = (dateStr: string) => {
        return new Date(dateStr).toLocaleDateString('en-US', {
            weekday: 'short',
            month: 'short',
            day: 'numeric'
        });
    };

    const formatTime = (dateStr: string) => {
        return new Date(dateStr).toLocaleTimeString('en-US', {
            hour: 'numeric',
            minute: '2-digit'
        });
    };

    const getStatusStyle = (status: string) => {
        switch (status) {
            case 'Assigned': return 'text-green-500 bg-green-500/10 border-green-500/20';
            case 'Open': return 'text-blue-500 bg-blue-500/10 border-blue-500/20';
            case 'PendingTransfer': return 'text-amber-500 bg-amber-500/10 border-amber-500/20';
            case 'Claimed': return 'text-purple-500 bg-purple-500/10 border-purple-500/20';
            default: return 'text-muted-foreground bg-muted border-border';
        }
    };

    return (
        <div className="overflow-x-auto border border-border bg-surface shadow-hard">
            <table className="w-full text-left text-sm">
                <thead className="bg-muted text-muted-foreground font-mono uppercase text-xs tracking-wider border-b border-border">
                    <tr>
                        <th className="px-6 py-4 font-medium">Date</th>
                        <th className="px-6 py-4 font-medium">Time</th>
                        <th className="px-6 py-4 font-medium">Role</th>
                        <th className="px-6 py-4 font-medium">{shifts.some(s => s.user) ? 'User' : 'Status'}</th>
                        {onAction && <th className="px-6 py-4 font-medium text-right">Action</th>}
                    </tr>
                </thead>
                <tbody className="divide-y divide-border">
                    {shifts.map((shift) => (
                        <tr key={shift.id} className="hover:bg-muted/50 transition-colors">
                            <td className="px-6 py-4 font-bold text-foreground">{formatDate(shift.startTime)}</td>
                            <td className="px-6 py-4 text-muted-foreground font-mono text-xs">
                                {formatTime(shift.startTime)} - {formatTime(shift.endTime)}
                            </td>
                            <td className="px-6 py-4">
                                <span className="px-2 py-1 text-xs font-bold uppercase tracking-wider bg-background border border-border text-foreground" style={{ borderRadius: 0 }}>
                                    {shift.role}
                                </span>
                            </td>
                            <td className="px-6 py-4">
                                {shift.user ? (
                                    <span className="font-bold text-foreground">{shift.user}</span>
                                ) : (
                                    <span className={`px-2 py-1 text-xs font-bold uppercase tracking-wider border ${getStatusStyle(shift.status)}`} style={{ borderRadius: 0 }}>
                                        {shift.status}
                                    </span>
                                )}
                            </td>
                            {(onAction || onOffer || onSwap) && (
                                <td className="px-6 py-4 text-right">
                                    <div className="flex justify-end gap-2">
                                        {onOffer && (
                                            <button
                                                onClick={() => onOffer(shift)}
                                                className="bg-amber-500 text-white text-[10px] font-bold uppercase tracking-wider py-1.5 px-3 hover:bg-amber-600 transition-all hover:shadow-[2px_2px_0px_0px_var(--foreground)] active:translate-y-[1px] active:shadow-none"
                                                style={{ borderRadius: 0 }}
                                            >
                                                Offer
                                            </button>
                                        )}
                                        {onSwap && (
                                            <button
                                                onClick={() => onSwap(shift)}
                                                className="bg-primary text-primary-foreground text-[10px] font-bold uppercase tracking-wider py-1.5 px-3 hover:bg-primary/90 transition-all hover:shadow-[2px_2px_0px_0px_var(--foreground)] active:translate-y-[1px] active:shadow-none"
                                                style={{ borderRadius: 0 }}
                                            >
                                                Swap
                                            </button>
                                        )}
                                        {onAction && !onOffer && !onSwap && (
                                            <button
                                                onClick={() => onAction(shift)}
                                                className="bg-primary text-primary-foreground text-xs font-bold uppercase tracking-wider py-2 px-4 hover:bg-primary/90 transition-all hover:shadow-[2px_2px_0px_0px_var(--foreground)] active:translate-y-[1px] active:shadow-none"
                                                style={{ borderRadius: 0 }}
                                            >
                                                {actionLabel || 'Action'}
                                            </button>
                                        )}
                                    </div>
                                </td>
                            )}
                        </tr>
                    ))}
                    {shifts.length === 0 && (
                        <tr>
                            <td colSpan={5} className="px-6 py-12 text-center text-muted-foreground font-mono">
                                NO SHIFTS FOUND
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
}
