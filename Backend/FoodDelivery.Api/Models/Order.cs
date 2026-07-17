using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace FoodDelivery.Api.Models;

[SwaggerSchema(Description = "Represents a food delivery order")]
public class Order
{
    [SwaggerSchema(Description = "Unique order identifier")]
    public int Id { get; set; }

    [Required]
    [SwaggerSchema(Description = "Customer name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [SwaggerSchema(Description = "Delivery address")]
    public string CustomerAddress { get; set; } = string.Empty;

    [SwaggerSchema(Description = "Customer phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [SwaggerSchema(Description = "Ordered items")]
    public string Items { get; set; } = string.Empty;

    [SwaggerSchema(Description = "Total order amount")]
    public decimal TotalAmount { get; set; }

    [SwaggerSchema(Description = "Current order status")]
    public OrderStatus Status { get; set; }

    [SwaggerSchema(Description = "Order creation timestamp")]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [SwaggerSchema(Description = "Estimated delivery time")]
    public DateTime? EstimatedDelivery { get; set; }

    [SwaggerSchema(Description = "Customer notes")]
    public string Notes { get; set; } = string.Empty;
}

[SwaggerSchema(Description = "Possible order statuses")]
public enum OrderStatus
{
    Pending,
    Preparing,
    OutForDelivery,
    Delivered,
    Cancelled
}

[SwaggerSchema(Description = "Payload used to create a new order")]
public class OrderCreateRequest
{
    [Required]
    [SwaggerSchema(Description = "Customer name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [SwaggerSchema(Description = "Delivery address")]
    public string CustomerAddress { get; set; } = string.Empty;

    [SwaggerSchema(Description = "Contact phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [SwaggerSchema(Description = "Items in the order")]
    public string Items { get; set; } = string.Empty;

    [SwaggerSchema(Description = "Order total amount")]
    public decimal TotalAmount { get; set; }

    [SwaggerSchema(Description = "Initial status of the order")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [SwaggerSchema(Description = "Estimated delivery time")]
    public DateTime? EstimatedDelivery { get; set; }

    [SwaggerSchema(Description = "Customer or kitchen notes")]
    public string Notes { get; set; } = string.Empty;
}

[SwaggerSchema(Description = "Payload used to replace an existing order")]
public class OrderUpdateRequest
{
    [Required]
    [SwaggerSchema(Description = "Customer name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [SwaggerSchema(Description = "Delivery address")]
    public string CustomerAddress { get; set; } = string.Empty;

    [SwaggerSchema(Description = "Contact phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [SwaggerSchema(Description = "Items in the order")]
    public string Items { get; set; } = string.Empty;

    [SwaggerSchema(Description = "Order total amount")]
    public decimal TotalAmount { get; set; }

    [SwaggerSchema(Description = "Updated status")]
    public OrderStatus Status { get; set; }

    [SwaggerSchema(Description = "Estimated delivery time")]
    public DateTime? EstimatedDelivery { get; set; }

    [SwaggerSchema(Description = "Customer or kitchen notes")]
    public string Notes { get; set; } = string.Empty;
}

[SwaggerSchema(Description = "Payload used to change the order status")]
public class OrderStatusUpdateRequest
{
    [SwaggerSchema(Description = "New order status")]
    public OrderStatus Status { get; set; }
}

[SwaggerSchema(Description = "Dashboard summary of current orders")]
public class OrderSummaryDto
{
    [SwaggerSchema(Description = "Total number of orders")]
    public int TotalOrders { get; set; }

    [SwaggerSchema(Description = "Number of orders still pending")]
    public int PendingOrders { get; set; }

    [SwaggerSchema(Description = "Number of orders delivered")]
    public int DeliveredOrders { get; set; }

    [SwaggerSchema(Description = "Total revenue captured")]
    public decimal Revenue { get; set; }

    [SwaggerSchema(Description = "Average order value")]
    public decimal AverageOrderValue { get; set; }
}
