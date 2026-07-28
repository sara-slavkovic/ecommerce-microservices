import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

function Home() {
  const user = JSON.parse(localStorage.getItem('user'));
  const navigate = useNavigate();

  useEffect(() => {
    if (!user) navigate('/login');
  }, [user, navigate]);

  if (!user) return null;

  return (
    <div style={{ minHeight: '100vh' }}>
      <nav style={{
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        padding: '1rem 2rem',
        backgroundColor: 'var(--card-bg)',
        borderBottom: '1px solid var(--accent)'
      }}>
        <h2 style={{ margin: 0 }}>Beauty Store</h2>
        <span style={{ fontWeight: 'bold' }}>{user.fullName}</span>
      </nav>
    </div>
  );
}

export default Home;