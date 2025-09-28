using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IndustrialAutomation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobSchedulesController : ControllerBase
{
    private readonly IJobScheduleRepository _jobScheduleRepository;
    private readonly ILogger<JobSchedulesController> _logger;

    public JobSchedulesController(
        IJobScheduleRepository jobScheduleRepository,
        ILogger<JobSchedulesController> logger)
    {
        _jobScheduleRepository = jobScheduleRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobSchedule>>> GetJobSchedules()
    {
        try
        {
            var jobSchedules = await _jobScheduleRepository.GetAllAsync();
            return Ok(jobSchedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job schedules");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobSchedule>> GetJobSchedule(int id)
    {
        try
        {
            var jobSchedule = await _jobScheduleRepository.GetByIdAsync(id);
            if (jobSchedule == null)
                return NotFound();

            return Ok(jobSchedule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job schedule {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<JobSchedule>> CreateJobSchedule(JobSchedule jobSchedule)
    {
        try
        {
            var createdJobSchedule = await _jobScheduleRepository.AddAsync(jobSchedule);
            return CreatedAtAction(nameof(GetJobSchedule), new { id = createdJobSchedule.Id }, createdJobSchedule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job schedule");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJobSchedule(int id, JobSchedule jobSchedule)
    {
        if (id != jobSchedule.Id)
            return BadRequest();

        try
        {
            await _jobScheduleRepository.UpdateAsync(jobSchedule);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job schedule {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobSchedule(int id)
    {
        try
        {
            var result = await _jobScheduleRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job schedule {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("status/{statusId}")]
    public async Task<ActionResult<IEnumerable<JobSchedule>>> GetJobSchedulesByStatus(int statusId)
    {
        try
        {
            var jobSchedules = await _jobScheduleRepository.GetByStatusAsync(statusId);
            return Ok(jobSchedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job schedules by status {StatusId}", statusId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("type/{jobTypeId}")]
    public async Task<ActionResult<IEnumerable<JobSchedule>>> GetJobSchedulesByType(int jobTypeId)
    {
        try
        {
            var jobSchedules = await _jobScheduleRepository.GetByJobTypeAsync(jobTypeId);
            return Ok(jobSchedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job schedules by type {JobTypeId}", jobTypeId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("enabled")]
    public async Task<ActionResult<IEnumerable<JobSchedule>>> GetEnabledJobSchedules()
    {
        try
        {
            var enabledJobs = await _jobScheduleRepository.GetEnabledJobsAsync();
            return Ok(enabledJobs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving enabled job schedules");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("ready-to-run")]
    public async Task<ActionResult<IEnumerable<JobSchedule>>> GetJobsToRun()
    {
        try
        {
            var jobsToRun = await _jobScheduleRepository.GetJobsToRunAsync(DateTime.UtcNow);
            return Ok(jobsToRun);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving jobs ready to run");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("priority/{priority}")]
    public async Task<ActionResult<IEnumerable<JobSchedule>>> GetJobSchedulesByPriority(string priority)
    {
        try
        {
            var jobSchedules = await _jobScheduleRepository.GetByPriorityAsync(priority);
            return Ok(jobSchedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job schedules by priority {Priority}", priority);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}/dependencies")]
    public async Task<ActionResult<IEnumerable<JobSchedule>>> GetDependentJobs(int id)
    {
        try
        {
            var dependentJobs = await _jobScheduleRepository.GetDependentJobsAsync(id);
            return Ok(dependentJobs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dependent jobs for {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/enable")]
    public async Task<IActionResult> EnableJobSchedule(int id)
    {
        try
        {
            var jobSchedule = await _jobScheduleRepository.GetByIdAsync(id);
            if (jobSchedule == null)
                return NotFound();

            jobSchedule.IsEnabled = true;
            await _jobScheduleRepository.UpdateAsync(jobSchedule);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling job schedule {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/disable")]
    public async Task<IActionResult> DisableJobSchedule(int id)
    {
        try
        {
            var jobSchedule = await _jobScheduleRepository.GetByIdAsync(id);
            if (jobSchedule == null)
                return NotFound();

            jobSchedule.IsEnabled = false;
            await _jobScheduleRepository.UpdateAsync(jobSchedule);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling job schedule {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/run-now")]
    public async Task<IActionResult> RunJobNow(int id)
    {
        try
        {
            var jobSchedule = await _jobScheduleRepository.GetByIdAsync(id);
            if (jobSchedule == null)
                return NotFound();

            // Update the job to run immediately
            jobSchedule.NextRunTime = DateTime.UtcNow;
            jobSchedule.StatusId = 1; // Scheduled (Pending)
            await _jobScheduleRepository.UpdateAsync(jobSchedule);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running job schedule {Id} now", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
