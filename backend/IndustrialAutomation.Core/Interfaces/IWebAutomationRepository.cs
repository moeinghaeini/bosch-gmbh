using IndustrialAutomation.Core.Entities;

namespace IndustrialAutomation.Core.Interfaces;

public interface IWebAutomationRepository
{
    Task<IEnumerable<WebAutomation>> GetAllAsync();
    Task<WebAutomation?> GetByIdAsync(int id);
    Task<WebAutomation> AddAsync(WebAutomation webAutomation);
    Task<WebAutomation> UpdateAsync(WebAutomation webAutomation);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<WebAutomation>> GetByStatusAsync(int statusId);
    Task<IEnumerable<WebAutomation>> GetByJobTypeAsync(int jobTypeId);
    Task<IEnumerable<WebAutomation>> GetByWebsiteAsync(string websiteUrl);
    Task<IEnumerable<WebAutomation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<WebAutomation>> GetFailedAutomationsAsync();
    Task<WebAutomation?> GetLatestByAutomationNameAsync(string automationName);
}
