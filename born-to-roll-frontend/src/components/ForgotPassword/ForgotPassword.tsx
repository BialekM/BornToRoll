import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { authAPI } from '../../services/api';
import './ForgotPassword.css';

export default function ForgotPassword() {
  const [email, setEmail] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setMessage('');
    setLoading(true);

    try {
      const response = await authAPI.forgotPassword(email);
      setMessage(response.data.message);
      setEmail('');
    } catch (err: any) {
      setError(err.response?.data || 'Failed to send reset email. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="forgot-password-container">
      <div className="forgot-password-card">
        <p className="forgot-password-motto">
          black belt is a white belt that never quit
        </p>
        
        <div className="forgot-password-header">
          <img 
            src="/BornToRoll.png" 
            alt="Born To Roll Logo" 
            className="forgot-password-logo"
          />
        </div>

        <h2 className="forgot-password-title">Forgot Password?</h2>
        <p className="forgot-password-description">
          Enter your email address and we'll send you a link to reset your password.
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
              Email
            </label>
            <input
              type="email"
              className="form-input"
              placeholder="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="submit-button"
          >
            {loading ? 'Sending...' : 'Send Reset Link'}
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
