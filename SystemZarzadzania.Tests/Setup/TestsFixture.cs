using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using MySqlConnector;  
using Respawn;
using Xunit;
using Core.Models.Users;


namespace SystemZarzadzania.Setup;

public class TestFixture : IAsyncLifetime
{
    private Respawner? _respawner;
    private MySqlConnection _connection;
    private WebApplicationFactory<Program> _factory;
    public HttpClient Client { get; private set; }
    public async Task InitializeAsync()
    {
        var connectionString = "server=localhost;port=3306;database=systemzarzadzania;user=root;password=Bartek123456";

        _connection = new MySqlConnection(connectionString);
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.MySql,
            SchemasToInclude = Array.Empty<string>()
        });

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                { });
            });

        Client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        _factory.Dispose();
    }

    public async Task ResetDatabaseAsync()
    {
        if (_respawner != null)
            await _respawner.ResetAsync(_connection);
    }

}
