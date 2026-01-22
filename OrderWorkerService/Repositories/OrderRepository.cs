using Dapper;
using OrderWorkerService.Data;

namespace OrderWorkerService.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public OrderRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task UpdateStatusAsync(Guid orderId, string status, CancellationToken ct = default)
    {
        const string sql = @"UPDATE Orders SET Status = @Status WHERE Id = @Id;";

        using var conn = _connectionFactory.CreateConnection();
        // Dapper não usa CancellationToken diretamente no ExecuteAsync em todas versões;
        // se sua versão suportar, ótimo. Se não, isso já funciona no MVP.
        await conn.ExecuteAsync(sql, new { Id = orderId, Status = status });
    }
}