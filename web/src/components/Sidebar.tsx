'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';

const navItems = [
    { name: 'Dashboard', href: '/dashboard', icon: 'M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6' },
    { name: 'My Shifts', href: '/shifts', icon: 'M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z' },
    { name: 'Open Shifts', href: '/shifts/open', icon: 'M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z' },
    { name: 'Swap Requests', href: '/swaps', icon: 'M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4' },
    { name: 'Departments', href: '/departments', icon: 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4' },
];

export default function Sidebar() {
    const pathname = usePathname();

    return (
        <aside className="w-64 border-r border-border bg-surface flex flex-col h-screen sticky top-0">
            <div className="p-6 border-b border-border">
                <Link href="/dashboard" className="flex items-center gap-3 group">
                    <div className="w-8 h-8 bg-primary flex items-center justify-center font-bold text-primary-foreground transition-transform group-hover:rotate-90" style={{ borderRadius: 0 }}>
                        W
                    </div>
                    <span className="text-lg font-bold tracking-tight text-foreground uppercase">WorkSwap</span>
                </Link>
            </div>

            <nav className="flex-1 px-4 py-6 space-y-2">
                {navItems.map((item) => {
                    const isActive = pathname === item.href;
                    return (
                        <Link
                            key={item.name}
                            href={item.href}
                            className={`flex items-center gap-3 px-4 py-3 transition-all border border-transparent ${isActive
                                ? 'bg-primary text-primary-foreground shadow-hard border-foreground'
                                : 'text-muted-foreground hover:text-foreground hover:bg-background hover:border-border'
                                }`}
                            style={{ borderRadius: 0 }}
                        >
                            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="square" strokeLinejoin="miter" strokeWidth={2} d={item.icon} />
                            </svg>
                            <span className="font-medium text-sm tracking-wide">{item.name}</span>
                        </Link>
                    );
                })}
            </nav>

            <div className="p-4 border-t border-border bg-background">
                <div className="p-3 text-xs text-muted-foreground border border-border bg-surface">
                    <p className="font-bold text-foreground mb-2 uppercase tracking-wider">System Status</p>
                    <div className="flex items-center gap-2">
                        <div className="w-2 h-2 bg-green-500" style={{ borderRadius: 0 }}></div>
                        <span className="font-mono">OPERATIONAL</span>
                    </div>
                </div>
            </div>
        </aside>
    );
}
