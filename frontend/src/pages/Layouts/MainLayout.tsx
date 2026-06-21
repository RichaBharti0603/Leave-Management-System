import React from 'react';
import { Outlet } from 'react-router-dom';
import { Topbar } from '../components/Topbar';

export const MainLayout: React.FC = () => {
    return (
        <div style={{ display: 'flex', flexDirection: 'column', minHeight: '100vh', background: 'var(--bg-color)' }}>
            <Topbar />
            <main style={{ flex: 1, padding: '2rem' }}>
                <Outlet />
            </main>
        </div>
    );
};
