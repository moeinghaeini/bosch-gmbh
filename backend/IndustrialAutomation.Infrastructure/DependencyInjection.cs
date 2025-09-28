using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Infrastructure.Data;
using IndustrialAutomation.Infrastructure.Repositories;
using IndustrialAutomation.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IndustrialAutomation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<IndustrialAutomationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Add Repositories
        services.AddScoped<IAutomationJobRepository, AutomationJobRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITestExecutionRepository, TestExecutionRepository>();
        services.AddScoped<IWebAutomationRepository, WebAutomationRepository>();
        services.AddScoped<IJobScheduleRepository, JobScheduleRepository>();

        // Add Services
        services.AddScoped<IAIService, AIService>();
        services.AddHttpClient<IAIService, AIService>();
        services.AddScoped<IMonitoringService, MonitoringService>();

        return services;
    }
}
