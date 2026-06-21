import React from 'react';
import { useAuth } from '../../hooks/useAuth';

export const Topbar: React.FC = () => {
    const { user, logout } = useAuth();

    return (
        <header style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            padding: '1rem 2rem',
            background: 'var(--surface-color)',
            borderBottom: '1px solid var(--border-color)',
            boxShadow: 'var(--shadow-sm)'
        }}>
            <h2>LMS Portal</h2>
            <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
                <div>
                    <strong>{user?.profile?.firstName} {user?.profile?.lastName}</strong>
                    <span style={{ marginLeft: '0.5rem', color: 'var(--text-secondary)', fontSize: '0.875rem' }}>
                        ({user?.role})
                    </span>
                </div>
                <button onClick={logout} className="btn btn-secondary">Logout</button>
            </div>
        </header>
    );
};
