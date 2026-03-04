import { Outlet, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Layout() {
  const { user, logout, isAdmin } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div style={{ minHeight: '100vh', background: '#f5f5f5' }}>
      <header
        style={{
          background: '#1a1a2e',
          color: '#fff',
          padding: '0 24px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          height: 56,
        }}
      >
        <div style={{ display: 'flex', alignItems: 'center', gap: 32 }}>
          <h1 style={{ fontSize: 18, margin: 0, fontWeight: 700 }}>ERP Demo</h1>
          <nav style={{ display: 'flex', gap: 16 }}>
            <Link to="/customers" style={navLink}>Clientes</Link>
            <Link to="/orders" style={navLink}>Pedidos</Link>
          </nav>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: 16, fontSize: 14 }}>
          <span>{user?.email} ({isAdmin ? 'Admin' : 'Operador'})</span>
          <button onClick={handleLogout} style={logoutBtn}>Sair</button>
        </div>
      </header>
      <main style={{ maxWidth: 1100, margin: '0 auto', padding: 24 }}>
        <Outlet />
      </main>
    </div>
  );
}

const navLink: React.CSSProperties = {
  color: '#e0e0e0',
  textDecoration: 'none',
  fontSize: 14,
  fontWeight: 500,
};

const logoutBtn: React.CSSProperties = {
  background: 'transparent',
  border: '1px solid #555',
  color: '#ccc',
  padding: '4px 12px',
  borderRadius: 4,
  cursor: 'pointer',
  fontSize: 13,
};
