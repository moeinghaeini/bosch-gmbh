using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using IndustrialAutomation.Infrastructure.Data;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Infrastructure.Repositories;
using IndustrialAutomation.Infrastructure.Services;

namespace IndustrialAutomation.Tests;

public static class TestConfiguration
{
    public static IServiceCollection AddTestServices(this IServiceCollection services)
    {
        // Add in-memory database
        services.AddDbContext<IndustrialAutomationDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDatabase");
        });

        // Add repositories
        services.AddScoped<IAutomationJobRepository, AutomationJobRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITestExecutionRepository, TestExecutionRepository>();
        services.AddScoped<IWebAutomationRepository, WebAutomationRepository>();
        services.AddScoped<IJobScheduleRepository, JobScheduleRepository>();

        // Add services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMonitoringService, MonitoringService>();
        services.AddScoped<IAIService, AIService>();

        // Add logging
        services.AddLogging(builder => builder.AddConsole());

        // Add memory cache
        services.AddMemoryCache();

        return services;
    }

    public static async Task SeedTestDataAsync(IndustrialAutomationDbContext context)
    {
        if (!context.AutomationJobs.Any())
        {
            var jobs = new List<AutomationJob>
            {
                new AutomationJob
                {
                    Id = 1,
                    Name = "Test Job 1",
                    Description = "Test Description 1",
                    StatusId = 1,
                    JobTypeId = 1,
                    Configuration = "{}",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new AutomationJob
                {
                    Id = 2,
                    Name = "Test Job 2",
                    Description = "Test Description 2",
                    StatusId = 2,
                    JobTypeId = 2,
                    Configuration = "{}",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            };

            context.AutomationJobs.AddRange(jobs);
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "testuser",
                    Email = "test@example.com",
                    PasswordHash = "hashedpassword",
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                },
                new User
                {
                    Id = 2,
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "hashedpassword",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }
    }
}
