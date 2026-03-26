'use client';

interface StatCardProps {
    label: string;
    value: string | number;
    description?: string;
    icon?: React.ReactNode;
    trend?: {
        value: string;
        positive: boolean;
    };
}

export default function StatCard({ label, value, description, icon, trend }: StatCardProps) {
    return (
        <div className="border border-border bg-surface p-6 shadow-hard transition-transform hover:-translate-y-0.5" style={{ borderRadius: 0 }}>
            <div className="flex justify-between items-start">
                <div>
                    <p className="text-xs font-mono uppercase tracking-wider text-muted-foreground">{label}</p>
                    <h3 className="text-3xl font-bold mt-2 text-foreground">{value}</h3>
                    {description && <p className="text-xs text-muted-foreground mt-1">{description}</p>}
                </div>
                {icon && (
                    <div className="p-2 border border-border bg-background text-primary" style={{ borderRadius: 0 }}>
                        {icon}
                    </div>
                )}
            </div>
            {trend && (
                <div className="mt-4 flex items-center gap-2">
                    <span className={`text-xs font-bold font-mono ${trend.positive ? 'text-green-500' : 'text-destructive'}`}>
                        {trend.positive ? '↑' : '↓'} {trend.value}
                    </span>
                    <span className="text-xs text-muted-foreground uppercase tracking-wide">vs last week</span>
                </div>
            )}
        </div>
    );
}
