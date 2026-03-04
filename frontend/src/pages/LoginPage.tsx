import { useState, FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { login as apiLogin } from '../api/auth';
import { useAuth } from '../context/AuthContext';

export default function LoginPage() {
  const [email, setEmail] = useState('admin@demo.com');
  const [password, setPassword] = useState('123456');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await apiLogin({ email, password });
      login(res.token, res.email, res.role);
      navigate('/customers');
    } catch {
      setError('Email ou senha inválidos');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={container}>
      <form onSubmit={handleSubmit} style={card}>
        <h2 style={{ margin: '0 0 8px', color: '#1a1a2e' }}>ERP Demo</h2>
        <p style={{ margin: '0 0 24px', color: '#666', fontSize: 14 }}>
          Faça login para acessar o sistema
        </p>

        {error && <div style={errorStyle}>{error}</div>}

        <label style={label}>Email</label>
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          style={input}
          required
        />

        <label style={label}>Senha</label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          style={input}
          required
        />

        <button type="submit" disabled={loading} style={btn}>
          {loading ? 'Entrando...' : 'Entrar'}
        </button>

        <p style={{ margin: '16px 0 0', color: '#999', fontSize: 12, textAlign: 'center' }}>
          admin@demo.com / 123456 &nbsp;|&nbsp; operator@demo.com / 123456
        </p>
      </form>
    </div>
  );
}

const container: React.CSSProperties = {
  minHeight: '100vh',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  background: '#f0f0f0',
};

const card: React.CSSProperties = {
  background: '#fff',
  padding: 32,
  borderRadius: 8,
  boxShadow: '0 2px 12px rgba(0,0,0,0.1)',
  width: 360,
};

const label: React.CSSProperties = {
  display: 'block',
  fontSize: 13,
  fontWeight: 600,
  marginBottom: 4,
  color: '#333',
};

const input: React.CSSProperties = {
  width: '100%',
  padding: '8px 12px',
  marginBottom: 16,
  border: '1px solid #ddd',
  borderRadius: 4,
  fontSize: 14,
  boxSizing: 'border-box',
};

const btn: React.CSSProperties = {
  width: '100%',
  padding: '10px 0',
  background: '#1a1a2e',
  color: '#fff',
  border: 'none',
  borderRadius: 4,
  fontSize: 14,
  fontWeight: 600,
  cursor: 'pointer',
};

const errorStyle: React.CSSProperties = {
  background: '#fee',
  color: '#c00',
  padding: '8px 12px',
  borderRadius: 4,
  marginBottom: 16,
  fontSize: 13,
};
