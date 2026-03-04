export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  role: string;
  expiresAt: string;
}

export interface Customer {
  id: string;
  name: string;
  document: string;
  email: string;
  phone: string;
  createdAt: string;
  isActive: boolean;
}

export interface CreateCustomer {
  name: string;
  document: string;
  email: string;
  phone: string;
}

export interface UpdateCustomer extends CreateCustomer {
  isActive: boolean;
}

export enum OrderStatus {
  Draft = 'Draft',
  Confirmed = 'Confirmed',
  Canceled = 'Canceled',
}

export interface OrderItem {
  id: string;
  orderId: string;
  description: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface CreateOrderItem {
  description: string;
  quantity: number;
  unitPrice: number;
}

export interface Order {
  id: string;
  customerId: string;
  customerName: string;
  status: OrderStatus;
  statusName: string;
  createdAt: string;
  total: number;
  items: OrderItem[];
}

export interface CreateOrder {
  customerId: string;
  items: CreateOrderItem[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}
