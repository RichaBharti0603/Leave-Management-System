import React from 'react';
import { useQuery } from '@tanstack/react-query';
import { getMyRequests } from '../../api/leaves';

export const EmployeeDashboard: React.FC = () => {
    const { data: requests, isLoading } = useQuery({
        queryKey: ['myRequests'],
        queryFn: getMyRequests
    });

    return (
        <div className="container">
            <h1 style={{ marginBottom: '2rem' }}>Employee Dashboard</h1>
            
            <div className="glass-panel" style={{ padding: '2rem', marginBottom: '2rem' }}>
                <h3>Apply for Leave</h3>
                <p>Form placeholder...</p>
                <button className="btn btn-primary" style={{ marginTop: '1rem' }}>Apply (Mock)</button>
            </div>

            <div className="glass-panel" style={{ padding: '2rem' }}>
                <h3>My Leave History</h3>
                {isLoading ? <p>Loading...</p> : (
                    <table style={{ width: '100%', textAlign: 'left', marginTop: '1rem', borderCollapse: 'collapse' }}>
                        <thead>
                            <tr style={{ borderBottom: '2px solid var(--border-color)' }}>
                                <th style={{ padding: '0.5rem' }}>Type</th>
                                <th style={{ padding: '0.5rem' }}>Dates</th>
                                <th style={{ padding: '0.5rem' }}>Duration</th>
                                <th style={{ padding: '0.5rem' }}>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            {requests?.map(req => (
                                <tr key={req.id} style={{ borderBottom: '1px solid var(--border-color)' }}>
                                    <td style={{ padding: '0.5rem' }}>{req.leaveTypeName}</td>
                                    <td style={{ padding: '0.5rem' }}>{new Date(req.startDateTime).toLocaleDateString()} - {new Date(req.endDateTime).toLocaleDateString()}</td>
                                    <td style={{ padding: '0.5rem' }}>{req.duration} days</td>
                                    <td style={{ padding: '0.5rem' }}>
                                        <span style={{
                                            padding: '0.25rem 0.5rem',
                                            borderRadius: '4px',
                                            fontSize: '0.75rem',
                                            fontWeight: 'bold',
                                            background: req.status === 'Approved' ? 'var(--success-color)' : req.status === 'Pending' ? 'var(--warning-color)' : 'var(--border-color)',
                                            color: req.status === 'Pending' ? '#000' : '#fff'
                                        }}>
                                            {req.status}
                                        </span>
                                    </td>
                                </tr>
                            ))}
                            {requests?.length === 0 && (
                                <tr>
                                    <td colSpan={4} style={{ padding: '1rem', textAlign: 'center' }}>No leave requests found.</td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                )}
            </div>
        </div>
    );
};
