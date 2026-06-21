import { Routes, Route, Navigate } from 'react-router-dom';
import { ProtectedRoute } from './routes/ProtectedRoute';
import { MainLayout } from './pages/Layouts/MainLayout';
import { Login } from './pages/Auth/Login';
import { EmployeeDashboard } from './pages/Employee/EmployeeDashboard';
import { ManagerDashboard } from './pages/Manager/ManagerDashboard';
import { HRDashboard } from './pages/HR/HRDashboard';

function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      
      {/* Protected Routes */}
      <Route element={<ProtectedRoute />}>
        <Route element={<MainLayout />}>
          <Route path="/" element={<Navigate to="/employee" replace />} />
          
          <Route element={<ProtectedRoute allowedRoles={['Employee']} />}>
            <Route path="/employee" element={<EmployeeDashboard />} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={['Manager']} />}>
            <Route path="/manager" element={<ManagerDashboard />} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={['HR', 'Admin']} />}>
            <Route path="/hr" element={<HRDashboard />} />
          </Route>
        </Route>
      </Route>
      
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;
