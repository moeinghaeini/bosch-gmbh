using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BoschThesis.Infrastructure.Repositories;

public class AutomationJobRepository : IAutomationJobRepository
{
    private readonly BoschThesisDbContext _context;

    public AutomationJobRepository(BoschThesisDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AutomationJob>> GetAllAsync()
    {
        return await _context.AutomationJobs
            .Where(j => !j.IsDeleted)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<AutomationJob?> GetByIdAsync(int id)
    {
        return await _context.AutomationJobs
            .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);
    }

    public async Task<AutomationJob> AddAsync(AutomationJob automationJob)
    {
        _context.AutomationJobs.Add(automationJob);
        await _context.SaveChangesAsync();
        return automationJob;
    }

    public async Task<AutomationJob> UpdateAsync(AutomationJob automationJob)
    {
        automationJob.UpdatedAt = DateTime.UtcNow;
        _context.AutomationJobs.Update(automationJob);
        await _context.SaveChangesAsync();
        return automationJob;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var job = await _context.AutomationJobs.FindAsync(id);
        if (job == null) return false;

        job.IsDeleted = true;
        job.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<AutomationJob>> GetByStatusAsync(int statusId)
    {
        return await _context.AutomationJobs
            .Where(j => !j.IsDeleted && j.StatusId == statusId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AutomationJob>> GetByJobTypeAsync(int jobTypeId)
    {
        return await _context.AutomationJobs
            .Where(j => !j.IsDeleted && j.JobTypeId == jobTypeId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }
}
