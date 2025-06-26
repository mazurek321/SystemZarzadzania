using Infrastructure.Database.Abstractions;
using System.Data;              
using Microsoft.Extensions.Configuration;  
using MySqlConnector; 
namespace Infrastructure.Database;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IDbConnection CreateOpenConnection()
    {
        var connection = new MySqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}

