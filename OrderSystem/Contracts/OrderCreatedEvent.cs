namespace OrderSystem.Contracts;

public sealed class OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public string Status { get; init; } = "Created";
    public string? CorrelationId { get; init; }
}
