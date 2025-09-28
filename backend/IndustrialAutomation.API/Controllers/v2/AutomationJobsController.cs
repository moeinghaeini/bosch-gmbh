using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace IndustrialAutomation.API.Controllers.v2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class AutomationJobsController : ControllerBase
{
    private readonly IAutomationJobRepository _automationJobRepository;
    private readonly ILogger<AutomationJobsController> _logger;
    private readonly IMonitoringService _monitoringService;

    public AutomationJobsController(
        IAutomationJobRepository automationJobRepository, 
        ILogger<AutomationJobsController> logger,
        IMonitoringService monitoringService)
    {
        _automationJobRepository = automationJobRepository;
        _logger = logger;
        _monitoringService = monitoringService;
    }

    /// <summary>
    /// Get all automation jobs with advanced filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<AutomationJob>>> GetAutomationJobs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? jobType = null,
        [FromQuery] string? search = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] string? sortOrder = "desc")
    {
        try
        {
            var startTime = DateTime.UtcNow;
            
            var jobs = await _automationJobRepository.GetAllAsync();
            
            // Apply filters
            if (!string.IsNullOrEmpty(status))
                jobs = jobs.Where(j => GetStatusName(j.StatusId) == status);
            
            if (!string.IsNullOrEmpty(jobType))
                jobs = jobs.Where(j => GetJobTypeName(j.JobTypeId) == jobType);
            
            if (!string.IsNullOrEmpty(search))
                jobs = jobs.Where(j => j.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                     j.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
            
            if (fromDate.HasValue)
                jobs = jobs.Where(j => j.CreatedAt >= fromDate.Value);
            
            if (toDate.HasValue)
                jobs = jobs.Where(j => j.CreatedAt <= toDate.Value);
            
            // Apply sorting
            jobs = sortBy.ToLower() switch
            {
                "name" => sortOrder.ToLower() == "asc" ? jobs.OrderBy(j => j.Name) : jobs.OrderByDescending(j => j.Name),
                "status" => sortOrder.ToLower() == "asc" ? jobs.OrderBy(j => GetStatusName(j.StatusId)) : jobs.OrderByDescending(j => GetStatusName(j.StatusId)),
                "jobtype" => sortOrder.ToLower() == "asc" ? jobs.OrderBy(j => GetJobTypeName(j.JobTypeId)) : jobs.OrderByDescending(j => GetJobTypeName(j.JobTypeId)),
                _ => sortOrder.ToLower() == "asc" ? jobs.OrderBy(j => j.CreatedAt) : jobs.OrderByDescending(j => j.CreatedAt)
            };
            
            var totalCount = jobs.Count();
            var pagedJobs = jobs.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            var result = new PagedResult<AutomationJob>
            {
                Data = pagedJobs,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
            
            // Record performance metric
            var duration = DateTime.UtcNow - startTime;
            await _monitoringService.RecordTimerAsync("automation_jobs_get_all", TimeSpan.FromMilliseconds(duration.TotalMilliseconds));
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving automation jobs");
            await _monitoringService.RecordErrorAsync("Error retrieving automation jobs", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get automation job by ID with detailed information
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AutomationJobDetail>> GetAutomationJob(int id)
    {
        try
        {
            var job = await _automationJobRepository.GetByIdAsync(id);
            if (job == null)
                return NotFound(new { message = $"Automation job with ID {id} not found" });

            var detail = new AutomationJobDetail
            {
                Id = job.Id,
                Name = job.Name,
                Description = job.Description,
                Status = GetStatusName(job.StatusId),
                JobTypeId = job.JobTypeId,
                Configuration = job.Configuration,
                ErrorMessage = job.ErrorMessage,
                ScheduledAt = job.ScheduledAt,
                StartedAt = job.StartedAt,
                CompletedAt = job.CompletedAt,
                CreatedAt = job.CreatedAt,
                UpdatedAt = job.UpdatedAt,
                ExecutionTime = job.CompletedAt.HasValue && job.StartedAt.HasValue 
                    ? job.CompletedAt.Value - job.StartedAt.Value 
                    : null,
                IsActive = !job.IsDeleted
            };

            return Ok(detail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving automation job {Id}", id);
            await _monitoringService.RecordErrorAsync($"Error retrieving automation job {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new automation job with validation
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AutomationJob>> CreateAutomationJob([FromBody] CreateAutomationJobRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var job = new AutomationJob
            {
                Name = request.Name,
                Description = request.Description,
                JobTypeId = request.JobTypeId,
                StatusId = 1, // Pending
                Configuration = request.Configuration,
                ScheduledAt = request.ScheduledAt,
                CreatedAt = DateTime.UtcNow
            };

            var createdJob = await _automationJobRepository.AddAsync(job);
            
            await _monitoringService.RecordEventAsync("automation_job_created", new Dictionary<string, object>
            {
                ["job_id"] = createdJob.Id,
                ["job_type"] = GetJobTypeName(createdJob.JobTypeId),
                ["user_id"] = GetCurrentUserId()
            });

            return CreatedAtAction(nameof(GetAutomationJob), new { id = createdJob.Id, version = "2.0" }, createdJob);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating automation job");
            await _monitoringService.RecordErrorAsync("Error creating automation job", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an automation job
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAutomationJob(int id, [FromBody] UpdateAutomationJobRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var job = await _automationJobRepository.GetByIdAsync(id);
            if (job == null)
                return NotFound(new { message = $"Automation job with ID {id} not found" });

            job.Name = request.Name;
            job.Description = request.Description;
            job.JobTypeId = request.JobTypeId;
            job.Configuration = request.Configuration;
            job.ScheduledAt = request.ScheduledAt;
            job.UpdatedAt = DateTime.UtcNow;

            await _automationJobRepository.UpdateAsync(job);
            
            await _monitoringService.RecordEventAsync("automation_job_updated", new Dictionary<string, object>
            {
                ["job_id"] = job.Id,
                ["user_id"] = GetCurrentUserId()
            });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating automation job {Id}", id);
            await _monitoringService.RecordErrorAsync($"Error updating automation job {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete an automation job (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAutomationJob(int id)
    {
        try
        {
            var job = await _automationJobRepository.GetByIdAsync(id);
            if (job == null)
                return NotFound(new { message = $"Automation job with ID {id} not found" });

            job.IsDeleted = true;
            job.UpdatedAt = DateTime.UtcNow;
            await _automationJobRepository.UpdateAsync(job);
            
            await _monitoringService.RecordEventAsync("automation_job_deleted", new Dictionary<string, object>
            {
                ["job_id"] = id,
                ["user_id"] = GetCurrentUserId()
            });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting automation job {Id}", id);
            await _monitoringService.RecordErrorAsync($"Error deleting automation job {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get automation job statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<AutomationJobStatistics>> GetStatistics()
    {
        try
        {
            var jobs = await _automationJobRepository.GetAllAsync();
            
            var statistics = new AutomationJobStatistics
            {
                TotalJobs = jobs.Count(),
                CompletedJobs = jobs.Count(j => j.StatusId == 3), // Completed
                FailedJobs = jobs.Count(j => j.StatusId == 4), // Failed
                RunningJobs = jobs.Count(j => j.StatusId == 2), // Running
                PendingJobs = jobs.Count(j => j.StatusId == 1), // Pending
                SuccessRate = jobs.Any() ? (double)jobs.Count(j => j.StatusId == 3) / jobs.Count() * 100 : 0,
                AverageExecutionTime = jobs.Where(j => j.CompletedAt.HasValue && j.StartedAt.HasValue)
                    .Select(j => (j.CompletedAt.Value - j.StartedAt.Value).TotalMinutes)
                    .DefaultIfEmpty(0)
                    .Average(),
                JobsByType = jobs.GroupBy(j => GetJobTypeName(j.JobTypeId))
                    .ToDictionary(g => g.Key, g => g.Count()),
                JobsByStatus = jobs.GroupBy(j => GetStatusName(j.StatusId))
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting automation job statistics");
            await _monitoringService.RecordErrorAsync("Error getting automation job statistics", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Execute an automation job
    /// </summary>
    [HttpPost("{id}/execute")]
    public async Task<ActionResult> ExecuteJob(int id)
    {
        try
        {
            var job = await _automationJobRepository.GetByIdAsync(id);
            if (job == null)
                return NotFound(new { message = $"Automation job with ID {id} not found" });

            if (job.StatusId != 1) // Not Pending
                return BadRequest(new { message = "Job can only be executed if it's in Pending status" });

            job.StatusId = 2; // Running
            job.StartedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            
            await _automationJobRepository.UpdateAsync(job);
            
            await _monitoringService.RecordEventAsync("automation_job_executed", new Dictionary<string, object>
            {
                ["job_id"] = id,
                ["user_id"] = GetCurrentUserId()
            });

            return Ok(new { message = "Job execution started" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing automation job {Id}", id);
            await _monitoringService.RecordErrorAsync($"Error executing automation job {id}", ex);
            return StatusCode(500, "Internal server error");
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    private string GetStatusName(int statusId)
    {
        return statusId switch
        {
            1 => "Pending",
            2 => "Running",
            3 => "Completed", 
            4 => "Failed",
            5 => "Cancelled",
            _ => "Unknown"
        };
    }

    private string GetJobTypeName(int jobTypeId)
    {
        return jobTypeId switch
        {
            1 => "Data Processing",
            2 => "Report Generation",
            3 => "System Maintenance",
            4 => "Backup",
            5 => "Web Scraping",
            6 => "API Testing",
            _ => "Unknown"
        };
    }
}

public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class AutomationJobDetail
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int JobTypeId { get; set; } = 1;
    public string Configuration { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public TimeSpan? ExecutionTime { get; set; }
    public bool IsActive { get; set; }
}

public class CreateAutomationJobRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int JobTypeId { get; set; } = 1;
    public string Configuration { get; set; } = string.Empty;
    public DateTime? ScheduledAt { get; set; }
}

public class UpdateAutomationJobRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int JobTypeId { get; set; } = 1;
    public string Configuration { get; set; } = string.Empty;
    public DateTime? ScheduledAt { get; set; }
}

public class AutomationJobStatistics
{
    public int TotalJobs { get; set; }
    public int CompletedJobs { get; set; }
    public int FailedJobs { get; set; }
    public int RunningJobs { get; set; }
    public int PendingJobs { get; set; }
    public double SuccessRate { get; set; }
    public double AverageExecutionTime { get; set; }
    public Dictionary<string, int> JobsByType { get; set; } = new();
    public Dictionary<string, int> JobsByStatus { get; set; } = new();
}
