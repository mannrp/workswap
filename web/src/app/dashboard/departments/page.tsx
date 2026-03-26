'use client';

import { useEffect, useState } from 'react';
import { fetchWithAuth } from '@/lib/api';

interface Department {
    id: number;
    name: string;
    description: string;
    employeeCount: number;
}

export default function DepartmentsPage() {
    const [departments, setDepartments] = useState<Department[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingDept, setEditingDept] = useState<Department | null>(null);

    // Form state
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');

    const fetchDepartments = async () => {
        try {
            const res = await fetchWithAuth('/departments');
            if (!res.ok) throw new Error('Failed to fetch departments');
            const data = await res.json();
            setDepartments(data);
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchDepartments();
    }, []);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const headers = { 'Content-Type': 'application/json' };
        const body = JSON.stringify({ name, description });

        try {
            if (editingDept) {
                await fetchWithAuth(`/departments/${editingDept.id}`, {
                    method: 'PUT',
                    headers,
                    body,
                });
            } else {
                await fetchWithAuth('/departments', {
                    method: 'POST',
                    headers,
                    body,
                });
            }
            closeModal();
            fetchDepartments();
        } catch (err: any) {
            alert('Operation failed: ' + err.message);
        }
    };

    const handleDelete = async (id: number) => {
        if (!confirm('Are you sure? This cannot be undone.')) return;
        try {
            await fetchWithAuth(`/departments/${id}`, { method: 'DELETE' });
            fetchDepartments();
        } catch (err: any) {
            alert('Delete failed: ' + err.message);
        }
    };

    const openModal = (dept?: Department) => {
        if (dept) {
            setEditingDept(dept);
            setName(dept.name);
            setDescription(dept.description || '');
        } else {
            setEditingDept(null);
            setName('');
            setDescription('');
        }
        setIsModalOpen(true);
    };

    const closeModal = () => {
        setIsModalOpen(false);
        setEditingDept(null);
    };

    if (loading) return <div>Loading...</div>;
    if (error) return <div className="text-red-500">Error: {error}</div>;

    return (
        <div>
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-3xl font-bold">Departments</h1>
                <button onClick={() => openModal()} className="btn btn-primary">
                    + New Department
                </button>
            </div>

            <div className="card overflow-hidden">
                <table>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Description</th>
                            <th>Employees</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {departments.map((dept) => (
                            <tr key={dept.id}>
                                <td className="font-medium">{dept.name}</td>
                                <td className="text-[var(--muted)]">{dept.description}</td>
                                <td>{dept.employeeCount}</td>
                                <td>
                                    <div className="flex gap-2">
                                        <button
                                            onClick={() => openModal(dept)}
                                            className="btn btn-secondary text-xs"
                                        >
                                            Edit
                                        </button>
                                        <button
                                            onClick={() => handleDelete(dept.id)}
                                            className="btn btn-danger text-xs"
                                        >
                                            Delete
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                        {departments.length === 0 && (
                            <tr>
                                <td colSpan={4} className="text-center py-8 text-[var(--muted)]">
                                    No departments found. Create one to get started.
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            {isModalOpen && (
                <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4">
                    <div className="card w-full max-w-md">
                        <h2 className="text-xl font-bold mb-4">
                            {editingDept ? 'Edit Department' : 'New Department'}
                        </h2>
                        <form onSubmit={handleSubmit} className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium mb-1">Name</label>
                                <input
                                    value={name}
                                    onChange={(e) => setName(e.target.value)}
                                    required
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium mb-1">Description</label>
                                <textarea
                                    value={description}
                                    onChange={(e) => setDescription(e.target.value)}
                                    rows={3}
                                />
                            </div>
                            <div className="flex justify-end gap-2 pt-4">
                                <button type="button" onClick={closeModal} className="btn btn-secondary">
                                    Cancel
                                </button>
                                <button type="submit" className="btn btn-primary">
                                    Save
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}
