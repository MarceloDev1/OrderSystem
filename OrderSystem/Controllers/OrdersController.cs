using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace OrderSystem.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IProducer<string, string> _producer;
    private readonly IConfiguration _configuration;

    public OrdersController(
        IProducer<string, string> producer,
        IConfiguration configuration)
    {
        _producer = producer;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var orderEvent = new
        {
            OrderId = Guid.NewGuid(),
            request.ProductId,
            request.Quantity,
            CreatedAt = DateTime.UtcNow
        };

        var topic = _configuration["Kafka:OrdersTopic"];
        var message = new Message<string, string>
        {
            Key = orderEvent.OrderId.ToString(),
            Value = JsonSerializer.Serialize(orderEvent)
        };

        await _producer.ProduceAsync(topic, message);

        return Created("", orderEvent);
    }
}

public class CreateOrderRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}