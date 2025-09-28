using Microsoft.AspNetCore.Mvc;
using IndustrialAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IndustrialAutomation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly IndustrialAutomationDbContext _context;

    public DebugController(IndustrialAutomationDbContext context)
    {
        _context = context;
    }

    [HttpGet("automationjobs")]
    public async Task<IActionResult> GetAutomationJobs()
    {
        try
        {
            // Test raw SQL query
            var rawJobs = await _context.Database
                .SqlQueryRaw<dynamic>("SELECT Id, Name, Description, StatusId, JobTypeId, CreatedAt, IsDeleted FROM AutomationJobs WHERE IsDeleted = 0")
                .ToListAsync();

            // Test Entity Framework query
            var efJobs = await _context.AutomationJobs
                .Where(j => !j.IsDeleted)
                .Select(j => new { j.Id, j.Name, j.Description, j.StatusId, j.JobTypeId, j.CreatedAt, j.IsDeleted })
                .ToListAsync();

            return Ok(new { 
                rawCount = rawJobs.Count,
                efCount = efJobs.Count,
                rawJobs = rawJobs,
                efJobs = efJobs
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    [HttpGet("test-connection")]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            var tableCount = await _context.Database
                .SqlQueryRaw<int>("SELECT COUNT(*) FROM AutomationJobs")
                .FirstOrDefaultAsync();

            return Ok(new { 
                canConnect = canConnect,
                tableCount = tableCount
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }
}
