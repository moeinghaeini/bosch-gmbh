using IndustrialAutomation.Core.Entities;

namespace IndustrialAutomation.Core.Interfaces;

public interface IJobScheduleRepository
{
    Task<IEnumerable<JobSchedule>> GetAllAsync();
    Task<JobSchedule?> GetByIdAsync(int id);
    Task<JobSchedule> AddAsync(JobSchedule jobSchedule);
    Task<JobSchedule> UpdateAsync(JobSchedule jobSchedule);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<JobSchedule>> GetByStatusAsync(int statusId);
    Task<IEnumerable<JobSchedule>> GetByJobTypeAsync(int jobTypeId);
    Task<IEnumerable<JobSchedule>> GetEnabledJobsAsync();
    Task<IEnumerable<JobSchedule>> GetJobsToRunAsync(DateTime currentTime);
    Task<IEnumerable<JobSchedule>> GetByPriorityAsync(string priority);
    Task<JobSchedule?> GetLatestByJobNameAsync(string jobName);
    Task<IEnumerable<JobSchedule>> GetDependentJobsAsync(int jobId);
}
