import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginForm from './components/Login/LoginForm';
import RegisterForm from './components/Register/RegisterForm';
import Dashboard from './components/Dashboard/Dashboard';
import ForgotPassword from './components/ForgotPassword/ForgotPassword';
import ResetPassword from './components/ResetPassword/ResetPassword';
import { useAuth } from './contexts/AuthContext';

function App() {
  const { user, loading } = useAuth();

  if (loading) return <div>Ładowanie...</div>;

  return (
    <BrowserRouter>
      <Routes>
        {/* Public routes */}
        <Route path="/login" element={!user ? <LoginForm /> : <Navigate to="/dashboard" />} />
        <Route path="/register" element={!user ? <RegisterForm /> : <Navigate to="/dashboard" />} />
        <Route path="/forgot-password" element={<ForgotPassword />} />
        <Route path="/reset-password" element={<ResetPassword />} />
        
        {/* Protected routes */}
        <Route path="/dashboard" element={user ? <Dashboard /> : <Navigate to="/login" />} />
        
        {/* Default redirect */}
        <Route path="/" element={<Navigate to={user ? "/dashboard" : "/login"} />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;