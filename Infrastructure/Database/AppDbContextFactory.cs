using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Database;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseMySql(
            "server=localhost;database=systemzarzadzania;user=root;password=Bartek123456",
            new MySqlServerVersion(new Version(8, 0, 32))
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}
