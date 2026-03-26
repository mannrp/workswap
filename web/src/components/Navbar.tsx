'use client';

import { useRouter } from 'next/navigation';
import { removeToken, User } from '../lib/api';
import { ThemeToggle } from './ThemeToggle';

interface NavbarProps {
    user: User | null;
}

export default function Navbar({ user }: NavbarProps) {
    const router = useRouter();

    const handleLogout = () => {
        removeToken();
        router.push('/');
    };

    return (
        <nav className="flex items-center justify-end px-6 py-4 border-b border-border bg-background">
            <div className="flex items-center gap-4">
                <ThemeToggle />
                {user && (
                    <div className="flex items-center gap-6">
                        <div className="text-right hidden sm:block">
                            <p className="text-sm font-bold text-foreground">{user.firstName} {user.lastName}</p>
                            <p className="text-xs font-mono text-muted-foreground uppercase">{user.roles.join(', ')}</p>
                        </div>
                        <div className="w-10 h-10 border border-border bg-surface flex items-center justify-center text-sm font-bold text-primary shadow-sm" style={{ borderRadius: 0 }}>
                            {user.firstName[0]}{user.lastName[0]}
                        </div>
                        <button
                            onClick={handleLogout}
                            className="border border-border bg-surface hover:bg-muted text-foreground px-4 py-2 text-xs font-bold uppercase tracking-wider transition-all hover:shadow-[2px_2px_0px_0px_var(--foreground)] active:translate-y-[1px] active:shadow-none"
                            style={{ borderRadius: 0 }}
                        >
                            Sign out
                        </button>
                    </div>
                )}
            </div>
        </nav>
    );
}
