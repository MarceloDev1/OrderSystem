using OrderSystem.Models;

namespace OrderSystem.Repositories;

public interface IOrderRepository
{
    Task CreateAsync(Order order);
    Task<Order?> GetByIdAsync(Guid id);
    Task UpdateStatusAsync(Guid id, string status);
}