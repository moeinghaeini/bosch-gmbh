using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BoschThesis.Infrastructure.Repositories;

public class TestExecutionRepository : ITestExecutionRepository
{
    private readonly BoschThesisDbContext _context;

    public TestExecutionRepository(BoschThesisDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TestExecution>> GetAllAsync()
    {
        return await _context.TestExecutions
            .Where(t => !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TestExecution?> GetByIdAsync(int id)
    {
        return await _context.TestExecutions
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task<TestExecution> AddAsync(TestExecution testExecution)
    {
        _context.TestExecutions.Add(testExecution);
        await _context.SaveChangesAsync();
        return testExecution;
    }

    public async Task<TestExecution> UpdateAsync(TestExecution testExecution)
    {
        testExecution.UpdatedAt = DateTime.UtcNow;
        _context.TestExecutions.Update(testExecution);
        await _context.SaveChangesAsync();
        return testExecution;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var testExecution = await _context.TestExecutions.FindAsync(id);
        if (testExecution == null) return false;

        testExecution.IsDeleted = true;
        testExecution.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TestExecution>> GetByStatusAsync(string status)
    {
        return await _context.TestExecutions
            .Where(t => !t.IsDeleted && t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TestExecution>> GetByTestTypeAsync(string testType)
    {
        return await _context.TestExecutions
            .Where(t => !t.IsDeleted && t.TestType == testType)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TestExecution>> GetByTestSuiteAsync(string testSuite)
    {
        return await _context.TestExecutions
            .Where(t => !t.IsDeleted && t.TestSuite == testSuite)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TestExecution>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.TestExecutions
            .Where(t => !t.IsDeleted && t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TestExecution>> GetFailedTestsAsync()
    {
        return await _context.TestExecutions
            .Where(t => !t.IsDeleted && t.Status == "Failed")
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TestExecution?> GetLatestByTestNameAsync(string testName)
    {
        return await _context.TestExecutions
            .Where(t => !t.IsDeleted && t.TestName == testName)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
