import React from 'react';

export const HRDashboard: React.FC = () => {
    return (
        <div className="container">
            <h1 style={{ marginBottom: '2rem' }}>HR Admin Dashboard</h1>
            <div className="glass-panel" style={{ padding: '2rem' }}>
                <h3>System Overview</h3>
                <p style={{ marginTop: '1rem' }}>Global leave requests, policy management, and analytics.</p>
            </div>
        </div>
    );
};
