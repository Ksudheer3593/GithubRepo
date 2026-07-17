using System.Text.Json.Serialization;
using FoodDelivery.Api.Data;
using FoodDelivery.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Food Delivery Order API",
        Version = "v1",
        Description = "Sample API for managing food delivery orders"
    });
});
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("FoodDeliveryOrdersDb"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:4200", "http://localhost:5000", "http://localhost:8080");
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("frontend");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.Orders.Any())
    {
        var customerNames = new[]
        {
            "Amelia Chen",
            "Marcus Lee",
            "Sofia Patel",
            "Jonah Rodriguez",
            "Liam Nguyen",
            "Rina Patel",
            "Ethan Brooks",
            "Nina Alvarez",
            "Owen Scott",
            "Priya Raman"
        };

        var addressSamples = new[]
        {
            "789 Pine St, Seattle",
            "12 River Rd, Bellevue",
            "55 Oak Ave, Tacoma",
            "200 1st Ave, Portland",
            "120 Lake St, Kirkland",
            "88 Cedar Ave, Redmond",
            "42 Pine St, Renton",
            "300 5th St, Spokane",
            "15 Harbor Dr, Olympia",
            "77 Sunset Blvd, Bellingham"
        };

        var itemSamples = new[]
        {
            "Margherita Pizza, Garlic Bread",
            "Chicken Burrito, Soda",
            "Veggie Wrap, Fries",
            "Spicy Ramen, Tea",
            "Chicken Caesar Wrap, Iced Tea",
            "Vegetable Noodles, Mango Smoothie",
            "BBQ Burger, Fries",
            "Salad Bowl, Lemon Water",
            "Taco Plate, Chips",
            "Sushi Combo, Green Tea"
        };

        var statusSamples = new[]
        {
            OrderStatus.Pending,
            OrderStatus.Preparing,
            OrderStatus.OutForDelivery,
            OrderStatus.Delivered,
            OrderStatus.Cancelled
        };

        var sampleOrders = Enumerable.Range(1, 100)
            .Select(index => new Order
            {
                CustomerName = $"{customerNames[index % customerNames.Length]} #{index}",
                CustomerAddress = $"{addressSamples[index % addressSamples.Length]} #{index}",
                PhoneNumber = $"{(100 + index) % 1000:000}-555-{(index % 100):00}",
                Items = itemSamples[index % itemSamples.Length],
                TotalAmount = 10m + (index % 20) * 2.5m + (index % 3) * 0.75m,
                Status = statusSamples[index % statusSamples.Length],
                EstimatedDelivery = DateTime.UtcNow.AddMinutes(10 + (index % 9) * 15),
                Notes = index % 2 == 0 ? "Priority delivery" : "Standard delivery"
            })
            .ToList();

        dbContext.Orders.AddRange(sampleOrders);
        dbContext.SaveChanges();
    }
}

app.Run();

public partial class Program
{
}
