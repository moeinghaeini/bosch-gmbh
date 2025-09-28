using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IndustrialAutomation.Infrastructure.Repositories;

public class WebAutomationRepository : IWebAutomationRepository
{
    private readonly IndustrialAutomationDbContext _context;

    public WebAutomationRepository(IndustrialAutomationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WebAutomation>> GetAllAsync()
    {
        return await _context.WebAutomations
            .Where(w => !w.IsDeleted)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<WebAutomation?> GetByIdAsync(int id)
    {
        return await _context.WebAutomations
            .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);
    }

    public async Task<WebAutomation> AddAsync(WebAutomation webAutomation)
    {
        _context.WebAutomations.Add(webAutomation);
        await _context.SaveChangesAsync();
        return webAutomation;
    }

    public async Task<WebAutomation> UpdateAsync(WebAutomation webAutomation)
    {
        webAutomation.UpdatedAt = DateTime.UtcNow;
        _context.WebAutomations.Update(webAutomation);
        await _context.SaveChangesAsync();
        return webAutomation;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var webAutomation = await _context.WebAutomations.FindAsync(id);
        if (webAutomation == null) return false;

        webAutomation.IsDeleted = true;
        webAutomation.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<WebAutomation>> GetByStatusAsync(int statusId)
    {
        return await _context.WebAutomations
            .Where(w => !w.IsDeleted && w.StatusId == statusId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WebAutomation>> GetByJobTypeAsync(int jobTypeId)
    {
        return await _context.WebAutomations
            .Where(w => !w.IsDeleted && w.JobTypeId == jobTypeId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WebAutomation>> GetByWebsiteAsync(string websiteUrl)
    {
        return await _context.WebAutomations
            .Where(w => !w.IsDeleted && w.WebsiteUrl == websiteUrl)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WebAutomation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.WebAutomations
            .Where(w => !w.IsDeleted && w.CreatedAt >= startDate && w.CreatedAt <= endDate)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WebAutomation>> GetFailedAutomationsAsync()
    {
        return await _context.WebAutomations
            .Where(w => !w.IsDeleted && w.StatusId == 4) // StatusId 4 = "Failed"
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<WebAutomation?> GetLatestByAutomationNameAsync(string automationName)
    {
        return await _context.WebAutomations
            .Where(w => !w.IsDeleted && w.AutomationName == automationName)
            .OrderByDescending(w => w.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
