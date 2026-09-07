import { useNavigate } from 'react-router-dom';

export function useAuth() {
  const user = JSON.parse(localStorage.getItem('user'));
  const navigate = useNavigate();

  const logout = () => {
    localStorage.removeItem('user');
    navigate('/');
  };

  return { user, logout };
}