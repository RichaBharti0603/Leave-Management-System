import React from 'react';

export const ManagerDashboard: React.FC = () => {
    return (
        <div className="container">
            <h1 style={{ marginBottom: '2rem' }}>Manager Dashboard</h1>
            <div className="glass-panel" style={{ padding: '2rem' }}>
                <h3>Approval Inbox</h3>
                <p style={{ marginTop: '1rem' }}>Pending requests from your direct reports will appear here.</p>
            </div>
        </div>
    );
};
