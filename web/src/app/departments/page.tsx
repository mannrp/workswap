'use client';

import AuthenticatedLayout from '@/components/AuthenticatedLayout';

export default function DepartmentsPage() {
    const departments = [
        { id: 1, name: 'Nursing', manager: 'Sarah Wilson', staff: 24, shifts: 156 },
        { id: 2, name: 'Emergency', manager: 'Dr. Mike Ross', staff: 18, shifts: 112 },
        { id: 3, name: 'Reception', manager: 'Janet Smith', staff: 6, shifts: 42 },
    ];

    return (
        <AuthenticatedLayout>
            <div className="space-y-8">
                <div className="border-b border-border pb-4 flex justify-between items-end">
                    <div>
                        <h1 className="text-3xl font-bold uppercase tracking-tight text-foreground">Departments</h1>
                        <p className="text-muted-foreground mt-1 font-mono text-xs">Overview of organizational structure.</p>
                    </div>
                </div>

                <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
                    {departments.map(dept => (
                        <div key={dept.id} className="group border border-border bg-surface p-6 shadow-sm hover:shadow-hard hover:-translate-y-0.5 transition-all">
                            <div className="flex justify-between items-start mb-6">
                                <h3 className="text-xl font-bold text-foreground uppercase tracking-tight">{dept.name}</h3>
                                <span className="p-2 bg-background border border-border text-primary" style={{ borderRadius: 0 }}>
                                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" /></svg>
                                </span>
                            </div>

                            <div className="space-y-3 mb-6">
                                <div className="flex justify-between text-sm border-b border-border/50 pb-2">
                                    <span className="text-muted-foreground font-mono text-xs uppercase">Manager</span>
                                    <span className="font-bold text-foreground">{dept.manager}</span>
                                </div>
                                <div className="flex justify-between text-sm border-b border-border/50 pb-2">
                                    <span className="text-muted-foreground font-mono text-xs uppercase">Staff Members</span>
                                    <span className="font-bold text-foreground">{dept.staff}</span>
                                </div>
                                <div className="flex justify-between text-sm border-b border-border/50 pb-2">
                                    <span className="text-muted-foreground font-mono text-xs uppercase">Active Shifts</span>
                                    <span className="font-bold text-foreground">{dept.shifts}</span>
                                </div>
                            </div>

                            <button className="w-full bg-background border border-border text-foreground hover:bg-muted text-xs font-bold uppercase tracking-wider py-3 transition-colors text-center" style={{ borderRadius: 0 }}>
                                View Details
                            </button>
                        </div>
                    ))}
                </div>
            </div>
        </AuthenticatedLayout>
    );
}
