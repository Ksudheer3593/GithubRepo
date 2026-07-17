import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { OrderCreateRequest, Order, OrderService, OrderSummary } from './order.service';

@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule],
  template: `
    <div class="dashboard">
      <header class="hero">
        <div>
          <p class="eyebrow">Food Delivery Order Management</p>
          <h1>{{ title() }}</h1>
          <p class="subtitle">Track orders, filter by customer or status, and create new deliveries.</p>
        </div>
      </header>

      @if (summary(); as summaryData) {
        <section class="summary-grid">
          <article class="card">
            <span class="label">Total Orders</span>
            <strong>{{ summaryData.totalOrders }}</strong>
          </article>
          <article class="card">
            <span class="label">Pending</span>
            <strong>{{ summaryData.pendingOrders }}</strong>
          </article>
          <article class="card">
            <span class="label">Delivered</span>
            <strong>{{ summaryData.deliveredOrders }}</strong>
          </article>
          <article class="card">
            <span class="label">Revenue</span>
            <strong>{{ summaryData.revenue | currency }}</strong>
          </article>
        </section>
      }

      <section class="panel">
        <div class="panel-header">
          <h2>Search and filter</h2>
          <div class="controls">
            <input [(ngModel)]="searchTerm" name="searchTerm" placeholder="Customer name" />
            <select [(ngModel)]="statusFilter" name="statusFilter">
              <option value="">All statuses</option>
              @for (status of statusOptions; track status) {
                <option [value]="status">{{ status }}</option>
              }
            </select>
            <button type="button" (click)="applyFilters()">Refresh</button>
          </div>
        </div>

        <div class="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>Order ID</th>
                <th>Customer</th>
                <th>Address</th>
                <th>Items</th>
                <th>Total</th>
                <th>Status</th>
                <th>Estimated</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (order of orders(); track order.id) {
                <tr>
                  <td>#{{ order.id }}</td>
                  <td>{{ order.customerName }}</td>
                  <td>{{ order.customerAddress }}</td>
                  <td>{{ order.items }}</td>
                  <td>{{ order.totalAmount | currency }}</td>
                  <td>
                    <select [ngModel]="order.status" (ngModelChange)="updateStatus(order, $event)">
                      @for (status of statusOptions; track status) {
                        <option [value]="status">{{ status }}</option>
                      }
                    </select>
                  </td>
                  <td>{{ order.estimatedDelivery | date:'short' }}</td>
                  <td>
                    <button class="danger" type="button" (click)="deleteOrder(order.id)">Delete</button>
                  </td>
                </tr>
              } @empty {
                <tr>
                  <td colspan="8">No orders available yet.</td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      </section>

      <section class="panel">
        <div class="panel-header">
          <h2>Create a new order</h2>
        </div>
        <form (ngSubmit)="createOrder()" class="form-grid">
          <input [(ngModel)]="form.customerName" name="customerName" placeholder="Customer name" required />
          <input [(ngModel)]="form.customerAddress" name="customerAddress" placeholder="Delivery address" required />
          <input [(ngModel)]="form.phoneNumber" name="phoneNumber" placeholder="Phone number" />
          <input [(ngModel)]="form.items" name="items" placeholder="Items" required />
          <input type="number" step="0.01" [(ngModel)]="form.totalAmount" name="totalAmount" placeholder="Total amount" />
          <select [(ngModel)]="form.status" name="status">
            @for (status of statusOptions; track status) {
              <option [value]="status">{{ status }}</option>
            }
          </select>
          <input type="datetime-local" [(ngModel)]="form.estimatedDelivery" name="estimatedDelivery" />
          <textarea [(ngModel)]="form.notes" name="notes" placeholder="Notes"></textarea>
          <button type="submit" [disabled]="isSaving">{{ isSaving ? 'Creating...' : 'Create order' }}</button>
        </form>
      </section>
    </div>
  `,
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('Food Delivery Orders');
  protected readonly orders = signal<Order[]>([]);
  protected readonly summary = signal<OrderSummary | null>(null);
  protected readonly statusOptions = ['Pending', 'Preparing', 'OutForDelivery', 'Delivered', 'Cancelled'];
  protected searchTerm = '';
  protected statusFilter = '';
  protected form: OrderCreateRequest = this.createEmptyOrder();
  protected isSaving = false;

  constructor(private readonly orderService: OrderService) {}

  ngOnInit(): void {
    this.refresh();
  }

  refresh(): void {
    this.orderService.getOrders(this.searchTerm || undefined, this.statusFilter || undefined).subscribe({
      next: (orders) => this.orders.set(orders),
      error: () => alert('Unable to load orders')
    });

    this.orderService.getSummary().subscribe({
      next: (summary) => this.summary.set(summary),
      error: () => alert('Unable to load summary')
    });
  }

  applyFilters(): void {
    this.refresh();
  }

  createOrder(): void {
    this.isSaving = true;
    const request: OrderCreateRequest = {
      ...this.form,
      totalAmount: Number(this.form.totalAmount || 0),
      estimatedDelivery: this.form.estimatedDelivery ? this.form.estimatedDelivery : undefined
    };

    this.orderService.createOrder(request).subscribe({
      next: () => {
        this.isSaving = false;
        this.form = this.createEmptyOrder();
        this.refresh();
      },
      error: () => {
        this.isSaving = false;
        alert('Unable to create order');
      }
    });
  }

  updateStatus(order: Order, status: string): void {
    this.orderService.updateStatus(order.id, status).subscribe({
      next: () => this.refresh(),
      error: (error: HttpErrorResponse) => {
        if (error.status === 404) {
          this.refresh();
          return;
        }

        alert('Unable to update status');
      }
    });
  }

  deleteOrder(id: number): void {
    this.orderService.deleteOrder(id).subscribe({
      next: () => this.refresh(),
      error: (error: HttpErrorResponse) => {
        if (error.status === 404) {
          this.refresh();
          return;
        }

        alert('Unable to delete order');
      }
    });
  }

  private createEmptyOrder(): OrderCreateRequest {
    return {
      customerName: '',
      customerAddress: '',
      phoneNumber: '',
      items: '',
      totalAmount: 0,
      status: 'Pending',
      estimatedDelivery: '',
      notes: ''
    };
  }
}
