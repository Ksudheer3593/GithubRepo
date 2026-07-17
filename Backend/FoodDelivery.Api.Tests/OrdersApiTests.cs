using FoodDelivery.Api.Controllers;
using FoodDelivery.Api.Data;
using FoodDelivery.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Api.Tests;

public class OrdersApiTests
{
    [Fact]
    public async Task GetOrders_ReturnsSeededOrders()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new AppDbContext(options);
        dbContext.Orders.Add(
            new Order
            {
                CustomerName = "Test Customer",
                CustomerAddress = "123 Main St",
                PhoneNumber = "555-0000",
                Items = "Burger",
                TotalAmount = 10.50m,
                Status = OrderStatus.Pending
            });
        await dbContext.SaveChangesAsync();

        var controller = new OrdersController(dbContext);
        var result = await controller.GetOrders(null, null);

        var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
        var orders = Assert.IsAssignableFrom<IEnumerable<Order>>(okObjectResult.Value);
        var firstOrder = Assert.Single(orders);
        Assert.Equal("Test Customer", firstOrder.CustomerName);
    }
}
