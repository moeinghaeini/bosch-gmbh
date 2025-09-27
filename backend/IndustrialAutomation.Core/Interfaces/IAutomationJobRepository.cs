using IndustrialAutomation.Core.Entities;

namespace IndustrialAutomation.Core.Interfaces;

public interface IAutomationJobRepository
{
    Task<IEnumerable<AutomationJob>> GetAllAsync();
    Task<AutomationJob?> GetByIdAsync(int id);
    Task<AutomationJob> AddAsync(AutomationJob automationJob);
    Task<AutomationJob> UpdateAsync(AutomationJob automationJob);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<AutomationJob>> GetByStatusAsync(int statusId);
    Task<IEnumerable<AutomationJob>> GetByJobTypeAsync(int jobTypeId);
}
