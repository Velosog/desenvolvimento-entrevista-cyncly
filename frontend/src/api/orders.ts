import api from './client';
import { Order, CreateOrder, CreateOrderItem, OrderStatus, PagedResult } from '../types';

export const getOrders = async (params?: {
  customerId?: string;
  status?: OrderStatus;
  fromDate?: string;
  toDate?: string;
  page?: number;
  pageSize?: number;
}): Promise<PagedResult<Order>> => {
  const response = await api.get<PagedResult<Order>>('/orders', { params });
  return response.data;
};

export const getOrder = async (id: string): Promise<Order> => {
  const response = await api.get<Order>(`/orders/${id}`);
  return response.data;
};

export const createOrder = async (data: CreateOrder): Promise<Order> => {
  const response = await api.post<Order>('/orders', data);
  return response.data;
};

export const updateOrderStatus = async (id: string, status: OrderStatus): Promise<Order> => {
  const response = await api.patch<Order>(`/orders/${id}/status`, { status });
  return response.data;
};

export const addOrderItem = async (orderId: string, data: CreateOrderItem): Promise<void> => {
  await api.post(`/orders/${orderId}/items`, data);
};

export const deleteOrderItem = async (orderId: string, itemId: string): Promise<void> => {
  await api.delete(`/orders/${orderId}/items/${itemId}`);
};

export const deleteOrder = async (id: string): Promise<void> => {
  await api.delete(`/orders/${id}`);
};
