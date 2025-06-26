using System.Data;

namespace Infrastructure.Database.Abstractions;

public interface ISqlConnectionFactory
{
    IDbConnection CreateOpenConnection();
}
