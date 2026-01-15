import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import './RegisterForm.css';

export default function RegisterForm() {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { register } = useAuth();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    if (password.length < 6) {
      setError('Password must be at least 6 characters');
      return;
    }

    setLoading(true);

    try {
      await register(email, password, name);
      alert('✅ Account created successfully');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Registration error');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="register-container">
      <div className="register-card">
        {/* Motto */}
        <p className="register-motto">
          black belt is a white belt that never quit
        </p>
        
        {/* Logo */}
        <div className="register-header">
          <img 
            src="/BornToRoll.png" 
            alt="Born To Roll Logo" 
            className="register-logo"
          />
        </div>

        {error && (
          <div className="error-message">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="form-container">
          <div>
            <label className="form-label">
              Name
            </label>
            <input
              type="text"
              className="form-input"
              placeholder="Your name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
            />
          </div>

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

          <div>
            <label className="form-label">
              Password
            </label>
            <input
              type="password"
              className="form-input"
              placeholder="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
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
            {loading ? 'Creating account…' : 'Sign up'}
          </button>
        </form>

        <div className="footer-actions">
          <span>Already have an account?</span>
          <Link to="/login" className="switch-form-link">
            Sign in
          </Link>
        </div>
      </div>
    </div>
  );
}
