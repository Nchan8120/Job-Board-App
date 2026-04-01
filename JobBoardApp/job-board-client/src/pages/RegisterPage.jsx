import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';

export default function RegisterPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({ username: '', email: '', password: '', role: 'Viewer' });
  const [error, setError] = useState('');

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    if (form.password.length < 6) {
      setError('Password must be at least 6 characters.');
      return;
    }
    try {
      const res = await api.post('/auth/register', form);
      login(res.data);
      navigate('/');
    } catch {
      setError('Email is already registered.');
    }
  };

  return (
    <div style={{ maxWidth: '420px', margin: '4rem auto' }}>
      <div className="card">
        <h2 style={{ marginBottom: '1.5rem' }}>Create an account</h2>
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Username</label>
            <input
              type="text"
              name="username"
              value={form.username}
              onChange={handleChange}
              required
            />
          </div>
          <div className="form-group">
            <label>Email</label>
            <input
              type="email"
              name="email"
              value={form.email}
              onChange={handleChange}
              required
            />
          </div>
          <div className="form-group">
            <label>Password</label>
            <input
              type="password"
              name="password"
              value={form.password}
              onChange={handleChange}
              required
            />
          </div>
          <div className="form-group">
            <label>I want to</label>
            <select name="role" value={form.role} onChange={handleChange}>
              <option value="Viewer">Browse and apply for jobs</option>
              <option value="Poster">Post jobs</option>
            </select>
          </div>
          {error && <p className="error">{error}</p>}
          <button type="submit" className="btn-primary" style={{ width: '100%', marginTop: '0.5rem' }}>
            Create account
          </button>
        </form>
        <p style={{ marginTop: '1rem', fontSize: '0.9rem', textAlign: 'center' }}>
          Already have an account? <Link to="/login">Log in</Link>
        </p>
      </div>
    </div>
  );
}