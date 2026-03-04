import { useState, useEffect, FormEvent } from 'react';
import { getCustomers, createCustomer, updateCustomer, deleteCustomer } from '../api/customers';
import type { Customer, CreateCustomer, UpdateCustomer } from '../types';
import { useAuth } from '../context/AuthContext';

export default function CustomersPage() {
  const { isAdmin } = useAuth();
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [filterName, setFilterName] = useState('');
  const [loading, setLoading] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [editing, setEditing] = useState<Customer | null>(null);
  const [error, setError] = useState('');

  const [form, setForm] = useState<CreateCustomer & { isActive: boolean }>({
    name: '', document: '', email: '', phone: '', isActive: true,
  });

  const fetchData = async () => {
    setLoading(true);
    try {
      const res = await getCustomers({ name: filterName || undefined, page, pageSize: 10 });
      setCustomers(res.items);
      setTotalCount(res.totalCount);
    } catch {
      setError('Erro ao carregar clientes');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchData(); }, [page, filterName]);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    try {
      if (editing) {
        const dto: UpdateCustomer = { ...form };
        await updateCustomer(editing.id, dto);
      } else {
        const dto: CreateCustomer = { name: form.name, document: form.document, email: form.email, phone: form.phone };
        await createCustomer(dto);
      }
      resetForm();
      fetchData();
    } catch (err: any) {
      setError(err.response?.data?.detail || err.response?.data?.title || 'Erro ao salvar');
    }
  };

  const handleEdit = (c: Customer) => {
    setEditing(c);
    setForm({ name: c.name, document: c.document, email: c.email, phone: c.phone, isActive: c.isActive });
    setShowForm(true);
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Deseja excluir este cliente?')) return;
    try {
      await deleteCustomer(id);
      fetchData();
    } catch (err: any) {
      setError(err.response?.data?.detail || 'Erro ao excluir');
    }
  };

  const resetForm = () => {
    setShowForm(false);
    setEditing(null);
    setForm({ name: '', document: '', email: '', phone: '', isActive: true });
  };

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <h2 style={{ margin: 0, color: '#1a1a2e' }}>Clientes</h2>
        <button onClick={() => { resetForm(); setShowForm(true); }} style={primaryBtn}>Novo Cliente</button>
      </div>

      <div style={{ marginBottom: 16 }}>
        <input
          placeholder="Buscar por nome..."
          value={filterName}
          onChange={(e) => { setFilterName(e.target.value); setPage(1); }}
          style={{ ...inputStyle, width: 300 }}
        />
      </div>

      {error && <div style={errorBox}>{error}</div>}

      {showForm && (
        <div style={formCard}>
          <h3 style={{ margin: '0 0 16px' }}>{editing ? 'Editar Cliente' : 'Novo Cliente'}</h3>
          <form onSubmit={handleSubmit}>
            <div style={formGrid}>
              <div>
                <label style={labelStyle}>Nome *</label>
                <input value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} style={inputStyle} required />
              </div>
              <div>
                <label style={labelStyle}>Documento (CPF/CNPJ) *</label>
                <input value={form.document} onChange={e => setForm({ ...form, document: e.target.value })} style={inputStyle} required />
              </div>
              <div>
                <label style={labelStyle}>Email *</label>
                <input type="email" value={form.email} onChange={e => setForm({ ...form, email: e.target.value })} style={inputStyle} required />
              </div>
              <div>
                <label style={labelStyle}>Telefone</label>
                <input value={form.phone} onChange={e => setForm({ ...form, phone: e.target.value })} style={inputStyle} />
              </div>
              {editing && (
                <div>
                  <label style={labelStyle}>
                    <input type="checkbox" checked={form.isActive} onChange={e => setForm({ ...form, isActive: e.target.checked })} /> Ativo
                  </label>
                </div>
              )}
            </div>
            <div style={{ display: 'flex', gap: 8, marginTop: 16 }}>
              <button type="submit" style={primaryBtn}>Salvar</button>
              <button type="button" onClick={resetForm} style={secondaryBtn}>Cancelar</button>
            </div>
          </form>
        </div>
      )}

      {loading ? <p>Carregando...</p> : (
        <table style={tableStyle}>
          <thead>
            <tr>
              <th style={th}>Nome</th>
              <th style={th}>Documento</th>
              <th style={th}>Email</th>
              <th style={th}>Telefone</th>
              <th style={th}>Status</th>
              <th style={th}>Ações</th>
            </tr>
          </thead>
          <tbody>
            {customers.map(c => (
              <tr key={c.id}>
                <td style={td}>{c.name}</td>
                <td style={td}>{c.document}</td>
                <td style={td}>{c.email}</td>
                <td style={td}>{c.phone}</td>
                <td style={td}>
                  <span style={{ ...badge, background: c.isActive ? '#d4edda' : '#f8d7da', color: c.isActive ? '#155724' : '#721c24' }}>
                    {c.isActive ? 'Ativo' : 'Inativo'}
                  </span>
                </td>
                <td style={td}>
                  <button onClick={() => handleEdit(c)} style={linkBtn}>Editar</button>
                  {isAdmin && <button onClick={() => handleDelete(c.id)} style={{ ...linkBtn, color: '#c00' }}>Excluir</button>}
                </td>
              </tr>
            ))}
            {customers.length === 0 && (
              <tr><td colSpan={6} style={{ ...td, textAlign: 'center', color: '#999' }}>Nenhum cliente encontrado</td></tr>
            )}
          </tbody>
        </table>
      )}

      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: 16, fontSize: 13 }}>
        <span>{totalCount} registro(s)</span>
        <div style={{ display: 'flex', gap: 8 }}>
          <button disabled={page <= 1} onClick={() => setPage(p => p - 1)} style={secondaryBtn}>Anterior</button>
          <span style={{ padding: '6px 0' }}>Página {page}</span>
          <button disabled={customers.length < 10} onClick={() => setPage(p => p + 1)} style={secondaryBtn}>Próxima</button>
        </div>
      </div>
    </div>
  );
}

const primaryBtn: React.CSSProperties = { background: '#1a1a2e', color: '#fff', border: 'none', padding: '8px 16px', borderRadius: 4, cursor: 'pointer', fontSize: 13, fontWeight: 600 };
const secondaryBtn: React.CSSProperties = { background: '#fff', color: '#333', border: '1px solid #ccc', padding: '6px 14px', borderRadius: 4, cursor: 'pointer', fontSize: 13 };
const linkBtn: React.CSSProperties = { background: 'none', border: 'none', color: '#1a1a2e', cursor: 'pointer', fontSize: 13, textDecoration: 'underline', marginRight: 8 };
const inputStyle: React.CSSProperties = { width: '100%', padding: '7px 10px', border: '1px solid #ddd', borderRadius: 4, fontSize: 13, boxSizing: 'border-box' };
const labelStyle: React.CSSProperties = { display: 'block', fontSize: 12, fontWeight: 600, marginBottom: 4, color: '#555' };
const formCard: React.CSSProperties = { background: '#fff', padding: 20, borderRadius: 8, marginBottom: 16, border: '1px solid #e0e0e0' };
const formGrid: React.CSSProperties = { display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 };
const tableStyle: React.CSSProperties = { width: '100%', borderCollapse: 'collapse', background: '#fff', borderRadius: 8, overflow: 'hidden' };
const th: React.CSSProperties = { textAlign: 'left', padding: '10px 12px', background: '#f8f8f8', borderBottom: '2px solid #e0e0e0', fontSize: 12, fontWeight: 700, color: '#555', textTransform: 'uppercase' };
const td: React.CSSProperties = { padding: '10px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 };
const badge: React.CSSProperties = { padding: '2px 8px', borderRadius: 12, fontSize: 11, fontWeight: 600 };
const errorBox: React.CSSProperties = { background: '#fee', color: '#c00', padding: '8px 12px', borderRadius: 4, marginBottom: 12, fontSize: 13 };
