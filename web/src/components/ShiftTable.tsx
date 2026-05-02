'use client';

import { Shift } from '@/types';

interface ShiftTableProps {
    shifts: Shift[];
    onAction?: (shift: Shift) => void;
    actionLabel?: string;
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

    const formatTime = (timeStr: string) => {
        const date = timeStr.includes('T')
            ? new Date(timeStr)
            : new Date(`2000-01-01T${timeStr}`);
        return date.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit' });
    };

    return (
        <div className="overflow-x-auto border border-border bg-surface shadow-hard">
            <table className="w-full text-left text-sm">
                <thead className="bg-muted text-muted-foreground font-mono uppercase text-xs tracking-wider border-b border-border">
                    <tr>
                        <th className="px-6 py-4 font-medium">Date</th>
                        <th className="px-6 py-4 font-medium">Time</th>
                        <th className="px-6 py-4 font-medium">Department</th>
                        <th className="px-6 py-4 font-medium">Assigned To</th>
                        {(onAction || onOffer || onSwap) && (
                            <th className="px-6 py-4 font-medium text-right">Action</th>
                        )}
                    </tr>
                </thead>
                <tbody className="divide-y divide-border">
                    {shifts.map((shift) => (
                        <tr key={shift.id} className="hover:bg-muted/50 transition-colors">
                            <td className="px-6 py-4 font-bold text-foreground">{formatDate(shift.date)}</td>
                            <td className="px-6 py-4 text-muted-foreground font-mono text-xs">
                                {formatTime(shift.startTime)} &ndash; {formatTime(shift.endTime)}
                            </td>
                            <td className="px-6 py-4">
                                <span className="px-2 py-1 text-xs font-bold uppercase tracking-wider bg-background border border-border text-foreground" style={{ borderRadius: 0 }}>
                                    {shift.departmentName}
                                </span>
                            </td>
                            <td className="px-6 py-4">
                                {shift.assignedUserName ? (
                                    <span className="font-bold text-foreground">{shift.assignedUserName}</span>
                                ) : (
                                    <span className="px-2 py-1 text-xs font-bold uppercase tracking-wider border text-blue-500 bg-blue-500/10 border-blue-500/20" style={{ borderRadius: 0 }}>
                                        OPEN
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
