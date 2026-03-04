import { useState, useEffect, FormEvent } from 'react';
import { getOrders, createOrder, updateOrderStatus, addOrderItem, deleteOrderItem, deleteOrder, getOrder } from '../api/orders';
import { getCustomers } from '../api/customers';
import type { Order, CreateOrderItem, Customer } from '../types';
import { OrderStatus } from '../types';
import { useAuth } from '../context/AuthContext';

export default function OrdersPage() {
  const { isAdmin } = useAuth();
  const [orders, setOrders] = useState<Order[]>([]);
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [filterStatus, setFilterStatus] = useState<string>('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const [showCreate, setShowCreate] = useState(false);
  const [selectedCustomerId, setSelectedCustomerId] = useState('');
  const [newItems, setNewItems] = useState<CreateOrderItem[]>([{ description: '', quantity: 1, unitPrice: 0 }]);

  const [detail, setDetail] = useState<Order | null>(null);
  const [newItem, setNewItem] = useState<CreateOrderItem>({ description: '', quantity: 1, unitPrice: 0 });

  const fetchOrders = async () => {
    setLoading(true);
    try {
      const statusParam = filterStatus ? filterStatus as OrderStatus : undefined;
      const res = await getOrders({ status: statusParam, page, pageSize: 10 });
      setOrders(res.items);
      setTotalCount(res.totalCount);
    } catch {
      setError('Erro ao carregar pedidos');
    } finally {
      setLoading(false);
    }
  };

  const fetchCustomers = async () => {
    try {
      const res = await getCustomers({ pageSize: 100, isActive: true });
      setCustomers(res.items);
    } catch { /* ignore */ }
  };

  useEffect(() => { fetchOrders(); }, [page, filterStatus]);
  useEffect(() => { fetchCustomers(); }, []);

  const handleCreateOrder = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    try {
      await createOrder({ customerId: selectedCustomerId, items: newItems });
      setShowCreate(false);
      setNewItems([{ description: '', quantity: 1, unitPrice: 0 }]);
      fetchOrders();
    } catch (err: any) {
      setError(err.response?.data?.detail || 'Erro ao criar pedido');
    }
  };

  const handleStatusChange = async (id: string, status: OrderStatus) => {
    try {
      await updateOrderStatus(id, status);
      fetchOrders();
      if (detail?.id === id) {
        const updated = await getOrder(id);
        setDetail(updated);
      }
    } catch (err: any) {
      setError(err.response?.data?.detail || 'Erro ao alterar status');
    }
  };

  const handleAddItem = async () => {
    if (!detail) return;
    try {
      await addOrderItem(detail.id, newItem);
      const updated = await getOrder(detail.id);
      setDetail(updated);
      setNewItem({ description: '', quantity: 1, unitPrice: 0 });
      fetchOrders();
    } catch (err: any) {
      setError(err.response?.data?.detail || 'Erro ao adicionar item');
    }
  };

  const handleRemoveItem = async (itemId: string) => {
    if (!detail) return;
    try {
      await deleteOrderItem(detail.id, itemId);
      const updated = await getOrder(detail.id);
      setDetail(updated);
      fetchOrders();
    } catch (err: any) {
      setError(err.response?.data?.detail || 'Erro ao remover item');
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Deseja excluir este pedido?')) return;
    try {
      await deleteOrder(id);
      if (detail?.id === id) setDetail(null);
      fetchOrders();
    } catch (err: any) {
      setError(err.response?.data?.detail || 'Erro ao excluir');
    }
  };

  const openDetail = async (id: string) => {
    try {
      const o = await getOrder(id);
      setDetail(o);
    } catch {
      setError('Erro ao carregar pedido');
    }
  };

  const updateNewItem = (idx: number, field: string, value: any) => {
    setNewItems(prev => prev.map((item, i) => i === idx ? { ...item, [field]: value } : item));
  };

  const statusColor = (s: string) => {
    if (s === 'Draft') return { bg: '#fff3cd', fg: '#856404' };
    if (s === 'Confirmed') return { bg: '#d4edda', fg: '#155724' };
    return { bg: '#f8d7da', fg: '#721c24' };
  };

  const statusLabel = (s: string) => {
    if (s === 'Draft') return 'Rascunho';
    if (s === 'Confirmed') return 'Confirmado';
    return 'Cancelado';
  };

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <h2 style={{ margin: 0, color: '#1a1a2e' }}>Pedidos</h2>
        <button onClick={() => setShowCreate(true)} style={primaryBtn}>Novo Pedido</button>
      </div>

      <div style={{ marginBottom: 16, display: 'flex', gap: 8 }}>
        <select value={filterStatus} onChange={e => { setFilterStatus(e.target.value); setPage(1); }} style={{ ...inputStyle, width: 200 }}>
          <option value="">Todos os status</option>
          <option value="Draft">Rascunho</option>
          <option value="Confirmed">Confirmado</option>
          <option value="Canceled">Cancelado</option>
        </select>
      </div>

      {error && <div style={errorBox}>{error}</div>}

      {showCreate && (
        <div style={formCard}>
          <h3 style={{ margin: '0 0 16px' }}>Novo Pedido</h3>
          <form onSubmit={handleCreateOrder}>
            <label style={labelStyle}>Cliente *</label>
            <select value={selectedCustomerId} onChange={e => setSelectedCustomerId(e.target.value)} style={{ ...inputStyle, width: 400, marginBottom: 16 }} required>
              <option value="">Selecione...</option>
              {customers.map(c => <option key={c.id} value={c.id}>{c.name} ({c.document})</option>)}
            </select>

            <h4 style={{ margin: '0 0 8px', fontSize: 14 }}>Itens</h4>
            {newItems.map((item, idx) => (
              <div key={idx} style={{ display: 'flex', gap: 8, marginBottom: 8, alignItems: 'end' }}>
                <div style={{ flex: 3 }}>
                  <label style={labelStyle}>Descrição</label>
                  <input value={item.description} onChange={e => updateNewItem(idx, 'description', e.target.value)} style={inputStyle} required />
                </div>
                <div style={{ flex: 1 }}>
                  <label style={labelStyle}>Qtd</label>
                  <input type="number" min={1} value={item.quantity} onChange={e => updateNewItem(idx, 'quantity', +e.target.value)} style={inputStyle} required />
                </div>
                <div style={{ flex: 1 }}>
                  <label style={labelStyle}>Preço Unit.</label>
                  <input type="number" min={0.01} step={0.01} value={item.unitPrice} onChange={e => updateNewItem(idx, 'unitPrice', +e.target.value)} style={inputStyle} required />
                </div>
                {newItems.length > 1 && (
                  <button type="button" onClick={() => setNewItems(prev => prev.filter((_, i) => i !== idx))} style={{ ...linkBtn, color: '#c00' }}>X</button>
                )}
              </div>
            ))}
            <button type="button" onClick={() => setNewItems(prev => [...prev, { description: '', quantity: 1, unitPrice: 0 }])} style={secondaryBtn}>+ Item</button>

            <div style={{ display: 'flex', gap: 8, marginTop: 16 }}>
              <button type="submit" style={primaryBtn}>Criar Pedido</button>
              <button type="button" onClick={() => setShowCreate(false)} style={secondaryBtn}>Cancelar</button>
            </div>
          </form>
        </div>
      )}

      {detail && (
        <div style={{ ...formCard, marginBottom: 16 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <h3 style={{ margin: 0 }}>Pedido - {detail.customerName}</h3>
            <button onClick={() => setDetail(null)} style={secondaryBtn}>Fechar</button>
          </div>
          <div style={{ marginTop: 12, display: 'flex', gap: 24, fontSize: 13, color: '#555' }}>
            <span>Status: <strong style={{ color: statusColor(detail.statusName).fg }}>{statusLabel(detail.statusName)}</strong></span>
            <span>Total: <strong>R$ {detail.total.toFixed(2)}</strong></span>
            <span>Criado: {new Date(detail.createdAt).toLocaleDateString('pt-BR')}</span>
          </div>

          {detail.statusName === 'Draft' && (
            <div style={{ marginTop: 12, display: 'flex', gap: 8 }}>
              <button onClick={() => handleStatusChange(detail.id, OrderStatus.Confirmed)} style={{ ...primaryBtn, background: '#155724' }}>Confirmar</button>
              <button onClick={() => handleStatusChange(detail.id, OrderStatus.Canceled)} style={{ ...primaryBtn, background: '#721c24' }}>Cancelar Pedido</button>
            </div>
          )}

          <table style={{ ...tableStyle, marginTop: 16 }}>
            <thead>
              <tr>
                <th style={th}>Descrição</th>
                <th style={th}>Qtd</th>
                <th style={th}>Preço Unit.</th>
                <th style={th}>Subtotal</th>
                {detail.statusName === 'Draft' && <th style={th}>Ação</th>}
              </tr>
            </thead>
            <tbody>
              {detail.items.map(item => (
                <tr key={item.id}>
                  <td style={td}>{item.description}</td>
                  <td style={td}>{item.quantity}</td>
                  <td style={td}>R$ {item.unitPrice.toFixed(2)}</td>
                  <td style={td}>R$ {item.lineTotal.toFixed(2)}</td>
                  {detail.statusName === 'Draft' && (
                    <td style={td}><button onClick={() => handleRemoveItem(item.id)} style={{ ...linkBtn, color: '#c00' }}>Remover</button></td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>

          {detail.statusName === 'Draft' && (
            <div style={{ marginTop: 12, display: 'flex', gap: 8, alignItems: 'end' }}>
              <div style={{ flex: 3 }}>
                <label style={labelStyle}>Descrição</label>
                <input value={newItem.description} onChange={e => setNewItem({ ...newItem, description: e.target.value })} style={inputStyle} />
              </div>
              <div style={{ flex: 1 }}>
                <label style={labelStyle}>Qtd</label>
                <input type="number" min={1} value={newItem.quantity} onChange={e => setNewItem({ ...newItem, quantity: +e.target.value })} style={inputStyle} />
              </div>
              <div style={{ flex: 1 }}>
                <label style={labelStyle}>Preço</label>
                <input type="number" min={0.01} step={0.01} value={newItem.unitPrice} onChange={e => setNewItem({ ...newItem, unitPrice: +e.target.value })} style={inputStyle} />
              </div>
              <button onClick={handleAddItem} style={primaryBtn}>Adicionar</button>
            </div>
          )}
        </div>
      )}

      {loading ? <p>Carregando...</p> : (
        <table style={tableStyle}>
          <thead>
            <tr>
              <th style={th}>Cliente</th>
              <th style={th}>Status</th>
              <th style={th}>Total</th>
              <th style={th}>Data</th>
              <th style={th}>Ações</th>
            </tr>
          </thead>
          <tbody>
            {orders.map(o => (
              <tr key={o.id}>
                <td style={td}>{o.customerName}</td>
                <td style={td}>
                  <span style={{ ...badge, background: statusColor(o.statusName).bg, color: statusColor(o.statusName).fg }}>
                    {statusLabel(o.statusName)}
                  </span>
                </td>
                <td style={td}>R$ {o.total.toFixed(2)}</td>
                <td style={td}>{new Date(o.createdAt).toLocaleDateString('pt-BR')}</td>
                <td style={td}>
                  <button onClick={() => openDetail(o.id)} style={linkBtn}>Detalhes</button>
                  {isAdmin && o.statusName !== 'Confirmed' && (
                    <button onClick={() => handleDelete(o.id)} style={{ ...linkBtn, color: '#c00' }}>Excluir</button>
                  )}
                </td>
              </tr>
            ))}
            {orders.length === 0 && (
              <tr><td colSpan={5} style={{ ...td, textAlign: 'center', color: '#999' }}>Nenhum pedido encontrado</td></tr>
            )}
          </tbody>
        </table>
      )}

      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: 16, fontSize: 13 }}>
        <span>{totalCount} registro(s)</span>
        <div style={{ display: 'flex', gap: 8 }}>
          <button disabled={page <= 1} onClick={() => setPage(p => p - 1)} style={secondaryBtn}>Anterior</button>
          <span style={{ padding: '6px 0' }}>Página {page}</span>
          <button disabled={orders.length < 10} onClick={() => setPage(p => p + 1)} style={secondaryBtn}>Próxima</button>
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
const tableStyle: React.CSSProperties = { width: '100%', borderCollapse: 'collapse', background: '#fff', borderRadius: 8, overflow: 'hidden' };
const th: React.CSSProperties = { textAlign: 'left', padding: '10px 12px', background: '#f8f8f8', borderBottom: '2px solid #e0e0e0', fontSize: 12, fontWeight: 700, color: '#555', textTransform: 'uppercase' };
const td: React.CSSProperties = { padding: '10px 12px', borderBottom: '1px solid #f0f0f0', fontSize: 13 };
const badge: React.CSSProperties = { padding: '2px 8px', borderRadius: 12, fontSize: 11, fontWeight: 600 };
const errorBox: React.CSSProperties = { background: '#fee', color: '#c00', padding: '8px 12px', borderRadius: 4, marginBottom: 12, fontSize: 13 };
