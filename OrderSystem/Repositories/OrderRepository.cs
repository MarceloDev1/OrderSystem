using Dapper;
using OrderSystem.Data;
using OrderSystem.Models;

namespace OrderSystem.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public OrderRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(Order order)
    {
        const string sql = @"
                                INSERT INTO Orders (Id, ProductId, Quantity, Status, CreatedAt)
                                VALUES (@Id, @ProductId, @Quantity, @Status, @CreatedAt);";

        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(sql, order);
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        const string sql = @"
                                SELECT Id, ProductId, Quantity, Status, CreatedAt
                                FROM Orders
                                WHERE Id = @Id;";

        using var conn = _connectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Order>(sql, new { Id = id });
    }

    public async Task UpdateStatusAsync(Guid id, string status)
    {
        const string sql = "UPDATE Orders SET Status = @Status WHERE Id = @Id;";
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id, Status = status });
    }
}