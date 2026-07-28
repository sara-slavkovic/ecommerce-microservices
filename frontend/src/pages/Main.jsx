import { useNavigate } from 'react-router-dom';

function Main() {
  const navigate = useNavigate();

  return (
    <div style={{
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      height: '100vh',
      textAlign: 'center'
    }}>
      <h1 style={{ fontSize: '4.5rem', fontWeight: 700, margin: 0 }}>Beauty Store</h1>
      <div style={{ display: 'flex', gap: '1rem', marginTop: '2.5rem' }}>
        <button onClick={() => navigate('/login')} style={{ padding: '12px 35px', fontSize: '1.1rem' }}>
          Login
        </button>
        <button onClick={() => navigate('/register')} style={{ padding: '12px 35px', fontSize: '1.1rem' }}>
          Register
        </button>
      </div>
    </div>
  );
}

export default Main;