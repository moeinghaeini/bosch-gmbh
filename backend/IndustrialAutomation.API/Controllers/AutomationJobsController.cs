using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IndustrialAutomation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutomationJobsController : ControllerBase
{
    private readonly IAutomationJobRepository _automationJobRepository;
    private readonly ILogger<AutomationJobsController> _logger;

    public AutomationJobsController(IAutomationJobRepository automationJobRepository, ILogger<AutomationJobsController> logger)
    {
        _automationJobRepository = automationJobRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AutomationJob>>> GetAutomationJobs()
    {
        try
        {
            var jobs = await _automationJobRepository.GetAllAsync();
            return Ok(jobs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving automation jobs");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AutomationJob>> GetAutomationJob(int id)
    {
        try
        {
            var job = await _automationJobRepository.GetByIdAsync(id);
            if (job == null)
                return NotFound();

            return Ok(job);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving automation job {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<AutomationJob>> CreateAutomationJob(AutomationJob automationJob)
    {
        try
        {
            var createdJob = await _automationJobRepository.AddAsync(automationJob);
            return CreatedAtAction(nameof(GetAutomationJob), new { id = createdJob.Id }, createdJob);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating automation job");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAutomationJob(int id, AutomationJob automationJob)
    {
        if (id != automationJob.Id)
            return BadRequest();

        try
        {
            await _automationJobRepository.UpdateAsync(automationJob);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating automation job {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAutomationJob(int id)
    {
        try
        {
            var result = await _automationJobRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting automation job {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("status/{statusId}")]
    public async Task<ActionResult<IEnumerable<AutomationJob>>> GetAutomationJobsByStatus(int statusId)
    {
        try
        {
            var jobs = await _automationJobRepository.GetByStatusAsync(statusId);
            return Ok(jobs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving automation jobs by status {StatusId}", statusId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("type/{jobTypeId}")]
    public async Task<ActionResult<IEnumerable<AutomationJob>>> GetAutomationJobsByType(int jobTypeId)
    {
        try
        {
            var jobs = await _automationJobRepository.GetByJobTypeAsync(jobTypeId);
            return Ok(jobs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving automation jobs by type {JobTypeId}", jobTypeId);
            return StatusCode(500, "Internal server error");
        }
    }
}
