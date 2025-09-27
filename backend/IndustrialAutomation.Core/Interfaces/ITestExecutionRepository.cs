using IndustrialAutomation.Core.Entities;

namespace IndustrialAutomation.Core.Interfaces;

public interface ITestExecutionRepository
{
    Task<IEnumerable<TestExecution>> GetAllAsync();
    Task<TestExecution?> GetByIdAsync(int id);
    Task<TestExecution> AddAsync(TestExecution testExecution);
    Task<TestExecution> UpdateAsync(TestExecution testExecution);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<TestExecution>> GetByStatusAsync(string status);
    Task<IEnumerable<TestExecution>> GetByTestTypeAsync(string testType);
    Task<IEnumerable<TestExecution>> GetByTestSuiteAsync(string testSuite);
    Task<IEnumerable<TestExecution>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<TestExecution>> GetFailedTestsAsync();
    Task<TestExecution?> GetLatestByTestNameAsync(string testName);
}
