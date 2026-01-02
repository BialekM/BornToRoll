import LoginForm from './components/Login/LoginForm';
import { useAuth } from './contexts/AuthContext';

function App() {
  const { user, loading } = useAuth();

  if (loading) return <div>Ładowanie...</div>;
  if (!user) return <LoginForm />;

  return (
    <div className="p-8">
      <h1>Cześć {user.name}! Zalogowany jako {user.email}</h1>
      <button onClick={() => localStorage.removeItem('token')}>Wyloguj</button>
    </div>
  );
}

export default App;