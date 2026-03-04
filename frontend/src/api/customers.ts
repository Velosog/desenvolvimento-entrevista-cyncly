import api from './client';
import { Customer, CreateCustomer, UpdateCustomer, PagedResult } from '../types';

export const getCustomers = async (params?: {
  name?: string;
  document?: string;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
}): Promise<PagedResult<Customer>> => {
  const response = await api.get<PagedResult<Customer>>('/customers', { params });
  return response.data;
};

export const getCustomer = async (id: string): Promise<Customer> => {
  const response = await api.get<Customer>(`/customers/${id}`);
  return response.data;
};

export const createCustomer = async (data: CreateCustomer): Promise<Customer> => {
  const response = await api.post<Customer>('/customers', data);
  return response.data;
};

export const updateCustomer = async (id: string, data: UpdateCustomer): Promise<Customer> => {
  const response = await api.put<Customer>(`/customers/${id}`, data);
  return response.data;
};

export const deleteCustomer = async (id: string): Promise<void> => {
  await api.delete(`/customers/${id}`);
};
