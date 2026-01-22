using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using OrderSystem.Repositories;
using OrderSystem.Models;

namespace OrderSystem.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IProducer<string, string> _producer;
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _orders;

    public OrdersController(
        IProducer<string, string> producer,
        IConfiguration configuration,
        IOrderRepository orders)
    {
        _producer = producer;
        _configuration = configuration;
        _orders = orders;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var orderId = Guid.NewGuid();

        // 1) grava no banco
        var order = new Order
        {
            Id = orderId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Status = "Created",
            CreatedAt = DateTime.UtcNow
        };

        await _orders.CreateAsync(order);

        // 2) publica no kafka
        var orderEvent = new
        {
            OrderId = orderId,
            request.ProductId,
            request.Quantity,
            CreatedAt = order.CreatedAt
        };

        var topic = _configuration["Kafka:OrdersTopic"] ?? "orders.created";
        var message = new Message<string, string>
        {
            Key = orderEvent.OrderId.ToString(),
            Value = JsonSerializer.Serialize(orderEvent)
        };

        await _producer.ProduceAsync(topic, message, ct);

        return Created("", orderEvent);
    }
}

public class CreateOrderRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
