using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Turnament.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TurnamentDbContext>
{
    public TurnamentDbContext CreateDbContext(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory());
        if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")))
            builder.AddJsonFile("appsettings.json", optional: true);
        builder.AddEnvironmentVariables();
        var cfg = builder.Build();

        var optionsBuilder = new DbContextOptionsBuilder<TurnamentDbContext>();
        var cs = cfg.GetConnectionString("Default") ?? "Host=localhost;Port=5432;Database=turnament;Username=postgres;Password=postgres";
        optionsBuilder.UseNpgsql(cs);
        return new TurnamentDbContext(optionsBuilder.Options);
    }
}