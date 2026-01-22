using Dapper;
using OrderSystem.Data;
using OrderSystem.Models;

namespace OrderSystem.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly IConnectionFactory _connectionFactory;

    public ProductRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync()
    {
        const string sql = "SELECT Id, Name FROM Products ORDER BY Id;";
        using var conn = _connectionFactory.CreateConnection();
        var rows = await conn.QueryAsync<Product>(sql);
        return rows.ToList();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        const string sql = "SELECT Id, Name FROM Products WHERE Id = @Id;";
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
    }
}