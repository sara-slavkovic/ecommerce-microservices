import { useState } from 'react';
import { userApi } from '../api/axiosInstances';
import { useNavigate } from 'react-router-dom';

function Register() {
  const [formData, setFormData] = useState({ username: '', password: '', fullName: '', phone: '' });
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const navigate = useNavigate();

  const handleChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

  const handleRegister = async (e) => {
    e.preventDefault();
    setError('');
    try {
      await userApi.post('/users/register', formData);
      setMessage('Account created! Redirecting to login...');
      setTimeout(() => navigate('/login'), 1200);
    } catch (err) {
      setError('Registration failed. Please try again.');
    }
  };

  return (
    <div style={{ maxWidth: '400px', margin: '80px auto', padding: '30px', backgroundColor: 'var(--card-bg)', borderRadius: '8px', textAlign: 'center' }}>
      <h2>Register</h2>
      {error && <p style={{ color: '#b33' }}>{error}</p>}
      {message && <p style={{ fontWeight: 'bold' }}>{message}</p>}
      <form onSubmit={handleRegister}>
        <input type="text" name="fullName" placeholder="Full Name" value={formData.fullName} onChange={handleChange} required />
        <input type="text" name="username" placeholder="Username" value={formData.username} onChange={handleChange} required />
        <input type="text" name="phone" placeholder="Phone Number" value={formData.phone} onChange={handleChange} />
        <input type="password" name="password" placeholder="Password" value={formData.password} onChange={handleChange} required />
        <button type="submit" style={{ width: '100%' }}>Create Account</button>
      </form>
    </div>
  );
}

export default Register;