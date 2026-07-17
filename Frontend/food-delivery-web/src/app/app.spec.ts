import { TestBed } from '@angular/core/testing';
import { HttpErrorResponse } from '@angular/common/http';
import { of, throwError } from 'rxjs';
import { vi } from 'vitest';
import { App } from './app';
import { OrderService } from './order.service';

describe('App', () => {
  const orderServiceStub = {
    getOrders: vi.fn().mockReturnValue(of([])),
    getSummary: vi.fn().mockReturnValue(of({ totalOrders: 0, pendingOrders: 0, deliveredOrders: 0, revenue: 0, averageOrderValue: 0 })),
    deleteOrder: vi.fn().mockReturnValue(of(void 0)),
    updateStatus: vi.fn().mockReturnValue(of(void 0)),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [{ provide: OrderService, useValue: orderServiceStub }],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render the dashboard title', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Food Delivery Orders');
  });

  it('refreshes the view when delete targets a missing order', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    const refreshSpy = vi.spyOn(app, 'refresh');

    orderServiceStub.deleteOrder.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 404, statusText: 'Not Found' }))
    );

    app.deleteOrder(999);

    expect(refreshSpy).toHaveBeenCalled();
  });
});
