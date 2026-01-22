namespace OrderWorkerService.Repositories;

public interface IOrderRepository
{
    Task UpdateStatusAsync(Guid orderId, string status, CancellationToken ct = default);
}