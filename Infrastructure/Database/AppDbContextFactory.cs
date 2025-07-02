using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace Infrastructure.Database;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "MainProgram"))
            .AddJsonFile("appsettings.Test.json") 
            .Build();

        optionsBuilder.UseMySql(
            configuration.GetConnectionString("DefaultConnection"),
            new MySqlServerVersion(new Version(8, 0, 32))
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}
