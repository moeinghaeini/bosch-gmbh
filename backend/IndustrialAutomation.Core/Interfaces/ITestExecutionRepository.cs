using IndustrialAutomation.Core.Entities;

namespace IndustrialAutomation.Core.Interfaces;

public interface ITestExecutionRepository
{
    Task<IEnumerable<TestExecution>> GetAllAsync();
    Task<TestExecution?> GetByIdAsync(int id);
    Task<TestExecution> AddAsync(TestExecution testExecution);
    Task<TestExecution> UpdateAsync(TestExecution testExecution);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<TestExecution>> GetByStatusAsync(int statusId);
    Task<IEnumerable<TestExecution>> GetByTestTypeAsync(int testTypeId);
    Task<IEnumerable<TestExecution>> GetByTestSuiteAsync(string testSuite);
    Task<IEnumerable<TestExecution>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<TestExecution>> GetFailedTestsAsync();
    Task<TestExecution?> GetLatestByTestNameAsync(string testName);
}
