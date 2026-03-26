'use client';

import { useEffect, useState } from 'react';
import { fetchWithAuth } from '@/lib/api';

interface Shift {
    id: number;
    date: string;
    startTime: string; // "HH:mm:ss"
    endTime: string;   // "HH:mm:ss"
    departmentId: number;
    departmentName: string;
    assignedUserName?: string;
    notes?: string;
    isAvailableForSwap: boolean;
}

interface Department {
    id: number;
    name: string;
}

export default function ShiftsPage() {
    const [shifts, setShifts] = useState<Shift[]>([]);
    const [departments, setDepartments] = useState<Department[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingShift, setEditingShift] = useState<Shift | null>(null);

    // Form state
    const [date, setDate] = useState('');
    const [startTime, setStartTime] = useState('');
    const [endTime, setEndTime] = useState('');
    const [departmentId, setDepartmentId] = useState('');
    const [notes, setNotes] = useState('');

    const fetchData = async () => {
        try {
            const [shiftsRes, deptsRes] = await Promise.all([
                fetchWithAuth('/shifts'),
                fetchWithAuth('/departments')
            ]);

            if (!shiftsRes.ok) throw new Error('Failed to fetch shifts');
            if (!deptsRes.ok) throw new Error('Failed to fetch departments');

            const shiftsData = await shiftsRes.json();
            const deptsData = await deptsRes.json();

            setShifts(shiftsData);
            setDepartments(deptsData);
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const headers = { 'Content-Type': 'application/json' };

        // Backend expects strings for DateOnly and TimeOnly usually, check format
        // DateOnly: "yyyy-MM-dd"
        // TimeOnly: "HH:mm:ss" or "HH:mm"

        const payload = {
            date,
            startTime: startTime + ':00', // Append seconds if needed by TimeOnly parser simple check
            endTime: endTime + ':00',
            departmentId: parseInt(departmentId),
            notes,
        };

        try {
            if (editingShift) {
                await fetchWithAuth(`/shifts/${editingShift.id}`, {
                    method: 'PUT',
                    headers,
                    body: JSON.stringify({ ...payload, isAvailableForSwap: editingShift.isAvailableForSwap }),
                });
            } else {
                await fetchWithAuth('/shifts', {
                    method: 'POST',
                    headers,
                    body: JSON.stringify(payload),
                });
            }
            closeModal();
            fetchData();
        } catch (err: any) {
            alert('Operation failed: ' + err.message);
        }
    };

    const handleDelete = async (id: number) => {
        if (!confirm('Are you sure?')) return;
        try {
            await fetchWithAuth(`/shifts/${id}`, { method: 'DELETE' });
            fetchData();
        } catch (err: any) {
            alert('Delete failed: ' + err.message);
        }
    };

    const openModal = (shift?: Shift) => {
        if (shift) {
            setEditingShift(shift);
            setDate(shift.date);
            setStartTime(shift.startTime.substring(0, 5)); // HH:mm
            setEndTime(shift.endTime.substring(0, 5));
            setDepartmentId(shift.departmentId.toString());
            setNotes(shift.notes || '');
        } else {
            setEditingShift(null);
            // default to today
            setDate(new Date().toISOString().split('T')[0]);
            setStartTime('09:00');
            setEndTime('17:00');
            setDepartmentId(departments[0]?.id.toString() || '');
            setNotes('');
        }
        setIsModalOpen(true);
    };

    const closeModal = () => {
        setIsModalOpen(false);
        setEditingShift(null);
    };

    if (loading) return <div>Loading...</div>;
    if (error) return <div className="text-red-500">Error: {error}</div>;

    return (
        <div>
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-3xl font-bold">Shifts</h1>
                <button onClick={() => openModal()} className="btn btn-primary">
                    + New Shift
                </button>
            </div>

            <div className="card overflow-hidden">
                <table>
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th>Time</th>
                            <th>Department</th>
                            <th>Assigned To</th>
                            <th>Notes</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {shifts.map((shift) => (
                            <tr key={shift.id}>
                                <td className="font-medium">{shift.date}</td>
                                <td>{shift.startTime.substring(0, 5)} - {shift.endTime.substring(0, 5)}</td>
                                <td>{shift.departmentName}</td>
                                <td>
                                    {shift.assignedUserName ? (
                                        <span className="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-blue-900 text-blue-100">
                                            {shift.assignedUserName}
                                        </span>
                                    ) : (
                                        <span className="text-[var(--muted)] text-sm">Unassigned</span>
                                    )}
                                </td>
                                <td className="text-[var(--muted)] text-sm">{shift.notes}</td>
                                <td>
                                    <div className="flex gap-2">
                                        <button
                                            onClick={() => openModal(shift)}
                                            className="btn btn-secondary text-xs"
                                        >
                                            Edit
                                        </button>
                                        <button
                                            onClick={() => handleDelete(shift.id)}
                                            className="btn btn-danger text-xs"
                                        >
                                            Delete
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                        {shifts.length === 0 && (
                            <tr>
                                <td colSpan={6} className="text-center py-8 text-[var(--muted)]">
                                    No shifts found. Schedule one to get started.
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            {isModalOpen && (
                <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4">
                    <div className="card w-full max-w-md max-h-[90vh] overflow-y-auto">
                        <h2 className="text-xl font-bold mb-4">
                            {editingShift ? 'Edit Shift' : 'New Shift'}
                        </h2>
                        <form onSubmit={handleSubmit} className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium mb-1">Department</label>
                                <select
                                    value={departmentId}
                                    onChange={(e) => setDepartmentId(e.target.value)}
                                    required
                                >
                                    <option value="">Select Department</option>
                                    {departments.map((d) => (
                                        <option key={d.id} value={d.id}>{d.name}</option>
                                    ))}
                                </select>
                            </div>

                            <div>
                                <label className="block text-sm font-medium mb-1">Date</label>
                                <input
                                    type="date"
                                    value={date}
                                    onChange={(e) => setDate(e.target.value)}
                                    required
                                />
                            </div>

                            <div className="grid grid-cols-2 gap-4">
                                <div>
                                    <label className="block text-sm font-medium mb-1">Start Time</label>
                                    <input
                                        type="time"
                                        value={startTime}
                                        onChange={(e) => setStartTime(e.target.value)}
                                        required
                                    />
                                </div>
                                <div>
                                    <label className="block text-sm font-medium mb-1">End Time</label>
                                    <input
                                        type="time"
                                        value={endTime}
                                        onChange={(e) => setEndTime(e.target.value)}
                                        required
                                    />
                                </div>
                            </div>

                            <div>
                                <label className="block text-sm font-medium mb-1">Notes</label>
                                <textarea
                                    value={notes}
                                    onChange={(e) => setNotes(e.target.value)}
                                    rows={2}
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
