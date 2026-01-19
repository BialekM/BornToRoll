import React, { useState } from 'react';
import { Link, useSearchParams, useNavigate } from 'react-router-dom';
import { authAPI } from '../../services/api';
import './ResetPassword.css';

export default function ResetPassword() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const token = searchParams.get('token');

  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  // Show token status on mount
  React.useEffect(() => {
    if (!token) {
      setError('No reset token found in URL');
    } else {
      console.log('Token found:', token.substring(0, 20) + '...');
    }
  }, [token]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setMessage('');

    if (newPassword !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    if (newPassword.length < 8) {
      setError('Password must be at least 8 characters long and include uppercase, lowercase, digit, and special character');
      return;
    }

    if (!token) {
      setError('Invalid or missing reset token');
      return;
    }

    setLoading(true);

    try {
      const response = await authAPI.resetPassword(token, newPassword);
      setMessage(response.data.message);
      setTimeout(() => {
        navigate('/login');
      }, 2000);
    } catch (err: any) {
      setError(err.response?.data || 'Failed to reset password. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="reset-password-container">
      <div className="reset-password-card">
        <p className="reset-password-motto">
          black belt is a white belt that never quit
        </p>
        
        <div className="reset-password-header">
          <img 
            src="/BornToRoll.png" 
            alt="Born To Roll Logo" 
            className="reset-password-logo"
          />
        </div>

        <h2 className="reset-password-title">Reset Password</h2>
        <p className="reset-password-description">
          Enter your new password below.
        </p>

        {error && (
          <div className="error-message">
            {error}
          </div>
        )}

        {message && (
          <div className="success-message">
            {message}
          </div>
        )}

        <form onSubmit={handleSubmit} className="form-container">
          <div>
            <label className="form-label">
              New Password
            </label>
            <input
              type="password"
              className="form-input"
              placeholder="new password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              required
            />
            <p className="password-hint">
              At least 8 characters with uppercase, lowercase, digit, and special character
            </p>
          </div>

          <div>
            <label className="form-label">
              Confirm Password
            </label>
            <input
              type="password"
              className="form-input"
              placeholder="confirm password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="submit-button"
          >
            {loading ? 'Resetting...' : 'Reset Password'}
          </button>
        </form>

        <div className="footer-actions">
          <Link to="/login" className="switch-form-link">
            Back to Sign in
          </Link>
        </div>
      </div>
    </div>
  );
}
