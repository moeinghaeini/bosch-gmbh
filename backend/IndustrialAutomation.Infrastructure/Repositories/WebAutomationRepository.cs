using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BoschThesis.Infrastructure.Repositories;

public class WebAutomationRepository : IWebAutomationRepository
{
    private readonly BoschThesisDbContext _context;

    public WebAutomationRepository(BoschThesisDbContext context)
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

    public async Task<IEnumerable<WebAutomation>> GetByStatusAsync(string status)
    {
        return await _context.WebAutomations
            .Where(w => !w.IsDeleted && w.Status == status)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WebAutomation>> GetByAutomationTypeAsync(string automationType)
    {
        return await _context.WebAutomations
            .Where(w => !w.IsDeleted && w.AutomationType == automationType)
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
            .Where(w => !w.IsDeleted && w.Status == "Failed")
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
