// placeholder
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav style={{
      background: 'white',
      padding: '1rem 2rem',
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
      boxShadow: '0 1px 3px rgba(0,0,0,0.1)'
    }}>
      <Link to="/" style={{ fontWeight: 700, fontSize: '1.2rem', textDecoration: 'none', color: '#2563eb' }}>
        JobBoard
      </Link>
      <div style={{ display: 'flex', gap: '1rem', alignItems: 'center' }}>
        {user ? (
          <>
            <span style={{ fontSize: '0.9rem', color: '#666' }}>
              {user.username} ({user.role})
            </span>
            {user.role === 'Poster' && (
              <>
                <Link to="/jobs/create"><button className="btn-primary">Post a Job</button></Link>
                <Link to="/my-jobs"><button className="btn-secondary">My Jobs</button></Link>
              </>
            )}
            <button className="btn-secondary" onClick={handleLogout}>Logout</button>
          </>
        ) : (
          <>
            <Link to="/login"><button className="btn-secondary">Login</button></Link>
            <Link to="/register"><button className="btn-primary">Register</button></Link>
          </>
        )}
      </div>
    </nav>
  );
}