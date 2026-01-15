import React from 'react';
import { useAuth } from '../../contexts/AuthContext';
import './Dashboard.css';

export default function Dashboard() {
  const { user, logout } = useAuth();

  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <h1 className="dashboard-title">
          Hi {user?.name}!
        </h1>
        <p className="dashboard-subtitle">
          Login as {user?.email}
        </p>
      </div>
      
      <button onClick={logout} className="logout-button">
        Wyloguj
      </button>
    </div>
  );
}
