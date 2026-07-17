import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Order {
  id: number;
  customerName: string;
  customerAddress: string;
  phoneNumber: string;
  items: string;
  totalAmount: number;
  status: string;
  orderDate: string;
  estimatedDelivery?: string;
  notes: string;
}

export interface OrderSummary {
  totalOrders: number;
  pendingOrders: number;
  deliveredOrders: number;
  revenue: number;
  averageOrderValue: number;
}

export interface OrderCreateRequest {
  customerName: string;
  customerAddress: string;
  phoneNumber: string;
  items: string;
  totalAmount: number;
  status: string;
  estimatedDelivery?: string;
  notes: string;
}

export interface OrderStatusUpdateRequest {
  status: string;
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly apiUrl = '/api/orders';

  constructor(private http: HttpClient) {}

  getOrders(customerName?: string, status?: string): Observable<Order[]> {
    let params = new HttpParams();
    if (customerName) {
      params = params.set('customerName', customerName);
    }
    if (status) {
      params = params.set('status', status);
    }

    return this.http.get<Order[]>(this.apiUrl, { params });
  }

  getOrder(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.apiUrl}/${id}`);
  }

  getSummary(): Observable<OrderSummary> {
    return this.http.get<OrderSummary>(`${this.apiUrl}/summary`);
  }

  createOrder(request: OrderCreateRequest): Observable<Order> {
    return this.http.post<Order>(this.apiUrl, request);
  }

  updateOrder(id: number, request: OrderCreateRequest): Observable<Order> {
    return this.http.put<Order>(`${this.apiUrl}/${id}`, request);
  }

  updateStatus(id: number, status: string): Observable<Order> {
    return this.http.patch<Order>(`${this.apiUrl}/${id}/status`, { status } satisfies OrderStatusUpdateRequest);
  }

  deleteOrder(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
