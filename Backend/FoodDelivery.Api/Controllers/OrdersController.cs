using FoodDelivery.Api.Data;
using FoodDelivery.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public OrdersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders([FromQuery] string? customerName, [FromQuery] string? status)
    {
        IQueryable<Order> query = _dbContext.Orders.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(customerName))
        {
            query = query.Where(order => order.CustomerName.Contains(customerName));
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(order => order.Status == parsedStatus);
        }

        var orders = await query.OrderByDescending(order => order.OrderDate).ToListAsync();
        return Ok(orders);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Order>>> SearchOrders([FromQuery] string? customerName, [FromQuery] string? status)
    {
        return await GetOrders(customerName, status);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(OrderCreateRequest request)
    {
        var order = new Order
        {
            CustomerName = request.CustomerName,
            CustomerAddress = request.CustomerAddress,
            PhoneNumber = request.PhoneNumber,
            Items = request.Items,
            TotalAmount = request.TotalAmount,
            Status = request.Status,
            EstimatedDelivery = request.EstimatedDelivery,
            Notes = request.Notes
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateOrder(int id, OrderUpdateRequest request)
    {
        var order = await _dbContext.Orders.FindAsync(id);
        if (order is null)
        {
            return NotFound();
        }

        order.CustomerName = request.CustomerName;
        order.CustomerAddress = request.CustomerAddress;
        order.PhoneNumber = request.PhoneNumber;
        order.Items = request.Items;
        order.TotalAmount = request.TotalAmount;
        order.Status = request.Status;
        order.EstimatedDelivery = request.EstimatedDelivery;
        order.Notes = request.Notes;

        await _dbContext.SaveChangesAsync();
        return Ok(order);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, OrderStatusUpdateRequest request)
    {
        var order = await _dbContext.Orders.FindAsync(id);
        if (order is null)
        {
            return NotFound();
        }

        order.Status = request.Status;
        await _dbContext.SaveChangesAsync();
        return Ok(order);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _dbContext.Orders.FindAsync(id);
        if (order is null)
        {
            return NotFound();
        }

        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("summary")]
    public async Task<ActionResult<OrderSummaryDto>> GetSummary()
    {
        var orders = await _dbContext.Orders.AsNoTracking().ToListAsync();
        var summary = new OrderSummaryDto
        {
            TotalOrders = orders.Count,
            PendingOrders = orders.Count(order => order.Status == OrderStatus.Pending),
            DeliveredOrders = orders.Count(order => order.Status == OrderStatus.Delivered),
            Revenue = orders.Sum(order => order.TotalAmount),
            AverageOrderValue = orders.Count > 0 ? orders.Average(order => order.TotalAmount) : 0m
        };

        return Ok(summary);
    }
}
