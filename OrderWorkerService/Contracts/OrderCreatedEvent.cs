namespace OrderWorkerService.Contracts;

public sealed record OrderCreatedEvent(
    Guid OrderId,
    int ProductId,
    int Quantity,
    DateTime CreatedAtUtc,
    string Status = "Created",
    string? CorrelationId = null
);
