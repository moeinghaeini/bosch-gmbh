using Microsoft.AspNetCore.Mvc;
using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IndustrialAutomation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SimpleAutomationJobsController : ControllerBase
{
    private readonly IndustrialAutomationDbContext _context;

    public SimpleAutomationJobsController(IndustrialAutomationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AutomationJob>>> GetAutomationJobs()
    {
        try
        {
            var jobs = await _context.AutomationJobs
                .Where(j => !j.IsDeleted)
                .ToListAsync();
            
            return Ok(jobs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AutomationJob>> GetAutomationJob(int id)
    {
        try
        {
            var job = await _context.AutomationJobs
                .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);

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

    [HttpPost]
    public async Task<ActionResult<AutomationJob>> CreateAutomationJob(AutomationJob job)
    {
        try
        {
            job.CreatedAt = DateTime.UtcNow;
            _context.AutomationJobs.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAutomationJob), new { id = job.Id }, job);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAutomationJob(int id, AutomationJob job)
    {
        try
        {
            if (id != job.Id)
            {
                return BadRequest();
            }

            job.UpdatedAt = DateTime.UtcNow;
            _context.Entry(job).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AutomationJobExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAutomationJob(int id)
    {
        try
        {
            var job = await _context.AutomationJobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            job.IsDeleted = true;
            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    private bool AutomationJobExists(int id)
    {
        return _context.AutomationJobs.Any(e => e.Id == id);
    }
}
