using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IndustrialAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IndustrialAutomation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;
    private readonly IndustrialAutomationDbContext _context;

    public HealthController(ILogger<HealthController> logger, IndustrialAutomationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            _logger.LogInformation("Health check requested");
            
            // Test database connection
            await _context.Database.CanConnectAsync();
            
            return Ok(new { 
                status = "Healthy", 
                timestamp = DateTime.UtcNow,
                database = "Connected"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new { 
                status = "Unhealthy", 
                timestamp = DateTime.UtcNow,
                error = ex.Message 
            });
        }
    }
}