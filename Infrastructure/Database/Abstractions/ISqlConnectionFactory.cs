using System.Data;              
using Microsoft.Extensions.Configuration;  
using MySqlConnector; 
namespace Infrastructure.Database.Abstractions
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateOpenConnection();
    }
}

