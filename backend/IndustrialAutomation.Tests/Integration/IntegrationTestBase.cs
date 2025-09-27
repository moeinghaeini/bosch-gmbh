using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IndustrialAutomation.Infrastructure.Data;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace IndustrialAutomation.Tests.Integration;

public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly MsSqlContainer SqlServerContainer;
    protected readonly RedisContainer RedisContainer;

    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory;
        SqlServerContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrong@Passw0rd")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("MSSQL_PID", "Express")
            .Build();

        RedisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .Build();

        Client = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<IndustrialAutomationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database for testing
                services.AddDbContext<IndustrialAutomationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Configure logging
                services.AddLogging(builder => builder.AddConsole());
            });
        }).CreateClient();
    }

    public async Task InitializeAsync()
    {
        await SqlServerContainer.StartAsync();
        await RedisContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await SqlServerContainer.StopAsync();
        await RedisContainer.StopAsync();
        Client?.Dispose();
    }

    protected string GetConnectionString()
    {
        return SqlServerContainer.GetConnectionString();
    }

    protected string GetRedisConnectionString()
    {
        return RedisContainer.GetConnectionString();
    }

    protected async Task SeedDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IndustrialAutomationDbContext>();
        
        // Add test data here
        await context.SaveChangesAsync();
    }

    protected async Task CleanDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IndustrialAutomationDbContext>();
        
        // Clean test data here
        await context.SaveChangesAsync();
    }
}
