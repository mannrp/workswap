'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { login, setToken } from '@/lib/api';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const router = useRouter();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const response = await login(email, password);
      setToken(response.token);
      router.push('/dashboard');
    } catch (err: any) {
      setError(err.message || 'Login failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-background p-4">
      <div className="w-full max-w-md space-y-8 border border-border bg-surface p-8 shadow-hard">
        <div className="text-center">
          <div className="mx-auto mb-6 flex h-16 w-16 items-center justify-center bg-primary text-3xl font-bold text-primary-foreground">
            W
          </div>
          <h1 className="text-2xl font-bold tracking-tight text-foreground">WORK SWAP</h1>
          <p className="mt-2 text-sm text-muted-foreground">SYSTEM ACCESS</p>
        </div>

        <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
          <div className="space-y-4">
            <div className="space-y-1">
              <label className="text-xs font-mono font-medium uppercase tracking-wider text-muted-foreground">Email address</label>
              <input
                id="email-address"
                name="email"
                type="email"
                autoComplete="email"
                required
                placeholder="user@example.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full border-b border-border bg-background px-3 py-3 text-sm text-foreground focus:border-primary focus:bg-surface focus:outline-none transition-colors"
                // Minimalist input style: Bottom border only, or full square border. Let's go with full square for better hit area but sharp.
                // Actually design system said "Bottom border only (Minimal) OR Full box with 1px border". 
                // Let's try full box for login to be safe.
                style={{ borderRadius: 0 }}
              />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-mono font-medium uppercase tracking-wider text-muted-foreground">Password</label>
              <input
                id="password"
                name="password"
                type="password"
                autoComplete="current-password"
                required
                placeholder="••••••••"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="w-full border border-border bg-background px-3 py-3 text-sm text-foreground placeholder:text-muted-foreground focus:border-primary focus:ring-1 focus:ring-primary focus:outline-none transition-all"
                style={{ borderRadius: 0 }}
              />
            </div>
          </div>

          {error && (
            <div className="border border-destructive/50 bg-destructive/10 p-3 text-xs text-destructive font-mono">
              [ERROR]: {error}
            </div>
          )}

          <div>
            <button
              type="submit"
              disabled={loading}
              className="group relative flex w-full justify-center border border-transparent bg-primary px-4 py-3 text-sm font-bold text-primary-foreground hover:bg-primary/90 focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 disabled:opacity-50 transition-all hover:-translate-y-0.5 hover:shadow-[2px_2px_0px_0px_var(--foreground)] active:translate-y-0 active:shadow-none"
              style={{ borderRadius: 0 }}
            >
              {loading ? 'AUTHENTICATING...' : 'ENTER SYSTEM'}
            </button>
          </div>

          <div className="text-center mt-6">
            <p className="text-xs text-muted-foreground">
              NEW USER?{' '}
              <Link href="/register" className="font-medium text-primary hover:underline hover:text-primary/80 transition-colors">
                INITIALIZE ACCOUNT
              </Link>
            </p>
          </div>
        </form>
      </div>
    </div>
  );
}
