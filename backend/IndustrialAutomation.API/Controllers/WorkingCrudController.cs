using Microsoft.AspNetCore.Mvc;
using IndustrialAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace IndustrialAutomation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkingCrudController : ControllerBase
{
    private readonly IndustrialAutomationDbContext _context;

    public WorkingCrudController(IndustrialAutomationDbContext context)
    {
        _context = context;
    }

    [HttpGet("automationjobs")]
    public async Task<IActionResult> GetAutomationJobs()
    {
        try
        {
            // Use raw SQL to bypass Entity Framework issues
            var jobs = await _context.Database
                .SqlQueryRaw<dynamic>("SELECT Id, Name, Description, StatusId, JobTypeId, CreatedAt, IsDeleted FROM AutomationJobs WHERE IsDeleted = 0 ORDER BY CreatedAt DESC")
                .ToListAsync();

            return Ok(jobs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("automationjobs/{id}")]
    public async Task<IActionResult> GetAutomationJob(int id)
    {
        try
        {
            var job = await _context.Database
                .SqlQueryRaw<dynamic>("SELECT Id, Name, Description, StatusId, JobTypeId, CreatedAt, IsDeleted FROM AutomationJobs WHERE Id = {0} AND IsDeleted = 0", id)
                .FirstOrDefaultAsync();

            if (job == null)
            {
                return NotFound();
            }

            return Ok(job);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("automationjobs")]
    public async Task<IActionResult> CreateAutomationJob([FromBody] CreateJobRequest request)
    {
        try
        {
            var id = await _context.Database
                .SqlQueryRaw<int>("INSERT INTO AutomationJobs (Name, Description, StatusId, JobTypeId, CreatedAt, IsDeleted) OUTPUT INSERTED.Id VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                    request.Name, request.Description, request.StatusId, request.JobTypeId, DateTime.UtcNow, false)
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetAutomationJob), new { id = id }, new { id = id, message = "Job created successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("automationjobs/{id}")]
    public async Task<IActionResult> UpdateAutomationJob(int id, [FromBody] UpdateJobRequest request)
    {
        try
        {
            var rowsAffected = await _context.Database
                .ExecuteSqlRawAsync("UPDATE AutomationJobs SET Name = {0}, Description = {1}, StatusId = {2}, JobTypeId = {3}, UpdatedAt = {4} WHERE Id = {5} AND IsDeleted = 0",
                    request.Name, request.Description, request.StatusId, request.JobTypeId, DateTime.UtcNow, id);

            if (rowsAffected == 0)
            {
                return NotFound();
            }

            return Ok(new { message = "Job updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("automationjobs/{id}")]
    public async Task<IActionResult> DeleteAutomationJob(int id)
    {
        try
        {
            var rowsAffected = await _context.Database
                .ExecuteSqlRawAsync("UPDATE AutomationJobs SET IsDeleted = 1, UpdatedAt = {0} WHERE Id = {1} AND IsDeleted = 0",
                    DateTime.UtcNow, id);

            if (rowsAffected == 0)
            {
                return NotFound();
            }

            return Ok(new { message = "Job deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _context.Database
                .SqlQueryRaw<dynamic>("SELECT Id, Username, Email, Role, IsActive, CreatedAt FROM Users WHERE IsDeleted = 0 ORDER BY CreatedAt DESC")
                .ToListAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class CreateJobRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StatusId { get; set; } = 1;
    public int JobTypeId { get; set; } = 1;
}

public class UpdateJobRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StatusId { get; set; } = 1;
    public int JobTypeId { get; set; } = 1;
}
