import { useAuth } from '../hooks/useAuth';
import { Link } from 'react-router-dom';

function Navbar() {
  const { user, logout } = useAuth();

  return (
    <nav style={{
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
      padding: '1rem 2rem',
      backgroundColor: 'var(--card-bg)',
      borderBottom: '1px solid var(--accent)'
    }}>
      <Link to="/home" style={{ textDecoration: 'none' }}>
        <h2 style={{ margin: 0 }}>Beauty Store</h2>
      </Link>
      <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
        <Link to="/cart">Cart</Link>
        <span style={{ fontWeight: 'bold' }}>{user?.fullName}</span>
        <button onClick={logout} style={{ padding: '6px 16px', fontSize: '0.9rem' }}>Logout</button>
      </div>
    </nav>
  );
}

export default Navbar;