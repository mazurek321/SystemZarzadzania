using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using MySqlConnector;  
using Respawn;
using Xunit;
using Core.Models.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Api.Dto.AuthDtos;
using System.Net;
using System.Net.Http.Json;
using Shouldly;


namespace SystemZarzadzania.Setup;

public class TestFixture : IAsyncLifetime
{
    private Respawner? _respawner;
    private MySqlConnection _connection;
    private WebApplicationFactory<Program> _factory;
    public HttpClient Client { get; private set; }

    public async Task InitializeAsync()
    {

        var configuration = new ConfigurationBuilder()
                                    .AddJsonFile("appsettings.Test.json")
                                    .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _connection = new MySqlConnection(connectionString);
        await _connection.OpenAsync();
        using var cmd = new MySqlCommand("SELECT DATABASE()", _connection);
        var dbName = (string?)await cmd.ExecuteScalarAsync();

        Console.WriteLine($">>> Akt. baza: {dbName}");

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.MySql,
            SchemasToInclude = Array.Empty<string>()
        });

        await ResetDatabaseAsync();

        _factory = new WebApplicationFactory<Program>()
        .WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.Sources.Clear();
                config.AddJsonFile("appsettings.Test.json");
            });

            builder.ConfigureServices((context, services) =>
            {
                services.RemoveAll(typeof(IHostedService));

                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                var config = context.Configuration;
                var connStr = config.GetConnectionString("DefaultConnection");

                services.AddDbContext<AppDbContext>(options =>
                    options.UseMySql(connStr, new MySqlServerVersion(new Version(8, 0, 32))));
        });
    });


        Client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        if (_connection != null)
            await _connection.DisposeAsync();
        if (_factory != null)
            _factory.Dispose();
        
        Client.DefaultRequestHeaders.Authorization = null;
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_connection);
    }

    public async Task<HttpStatusCode> RegisterAndLoginAsync()
    {
        var registerRequest = new RegisterDto
        {
            Name = "TesterName",
            Lastname = "TesterLastname",
            Password = "TesterPassword",
            ConfirmPassword = "TesterPassword",
            Email = "TesterEmail@Email.com",
            Phone = "123456789"
        };
        var registerResponse = await Client.PostAsJsonAsync("api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginRequest = new LoginDto
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };

        var loginResponse = await Client.PostAsJsonAsync("api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var token = await loginResponse.Content.ReadAsStringAsync();
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return loginResponse.StatusCode;
    }
}

