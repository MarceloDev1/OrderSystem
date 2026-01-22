using System.Data;

namespace OrderWorkerService.Data;

public interface IConnectionFactory
{
    IDbConnection CreateConnection();
}