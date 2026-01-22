using System.Data;

namespace OrderSystem.Data
{
    public interface IConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}