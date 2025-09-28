using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IndustrialAutomation.Infrastructure.Repositories;

public class JobScheduleRepository : IJobScheduleRepository
{
    private readonly IndustrialAutomationDbContext _context;

    public JobScheduleRepository(IndustrialAutomationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JobSchedule>> GetAllAsync()
    {
        return await _context.JobSchedules
            .Where(j => !j.IsDeleted)
            .OrderBy(j => j.NextRunTime)
            .ToListAsync();
    }

    public async Task<JobSchedule?> GetByIdAsync(int id)
    {
        return await _context.JobSchedules
            .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);
    }

    public async Task<JobSchedule> AddAsync(JobSchedule jobSchedule)
    {
        _context.JobSchedules.Add(jobSchedule);
        await _context.SaveChangesAsync();
        return jobSchedule;
    }

    public async Task<JobSchedule> UpdateAsync(JobSchedule jobSchedule)
    {
        jobSchedule.UpdatedAt = DateTime.UtcNow;
        _context.JobSchedules.Update(jobSchedule);
        await _context.SaveChangesAsync();
        return jobSchedule;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var jobSchedule = await _context.JobSchedules.FindAsync(id);
        if (jobSchedule == null) return false;

        jobSchedule.IsDeleted = true;
        jobSchedule.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<JobSchedule>> GetByStatusAsync(string status)
    {
        return await _context.JobSchedules
            .Where(j => !j.IsDeleted && j.Status == status)
            .OrderBy(j => j.NextRunTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobSchedule>> GetByJobTypeAsync(string jobType)
    {
        return await _context.JobSchedules
            .Where(j => !j.IsDeleted && j.JobType == jobType)
            .OrderBy(j => j.NextRunTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobSchedule>> GetEnabledJobsAsync()
    {
        return await _context.JobSchedules
            .Where(j => !j.IsDeleted && j.IsEnabled)
            .OrderBy(j => j.NextRunTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobSchedule>> GetJobsToRunAsync(DateTime currentTime)
    {
        return await _context.JobSchedules
            .Where(j => !j.IsDeleted && j.IsEnabled && j.NextRunTime <= currentTime)
            .OrderBy(j => j.Priority)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobSchedule>> GetByPriorityAsync(string priority)
    {
        return await _context.JobSchedules
            .Where(j => !j.IsDeleted && j.Priority == priority)
            .OrderBy(j => j.NextRunTime)
            .ToListAsync();
    }

    public async Task<JobSchedule?> GetLatestByJobNameAsync(string jobName)
    {
        return await _context.JobSchedules
            .Where(j => !j.IsDeleted && j.JobName == jobName)
            .OrderByDescending(j => j.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<JobSchedule>> GetDependentJobsAsync(int jobId)
    {
        var job = await _context.JobSchedules.FindAsync(jobId);
        if (job == null) return new List<JobSchedule>();

        // This is a simplified implementation
        // In a real scenario, you'd parse the Dependencies JSON field
        return await _context.JobSchedules
            .Where(j => !j.IsDeleted && j.Dependencies.Contains(jobId.ToString()))
            .ToListAsync();
    }
}
