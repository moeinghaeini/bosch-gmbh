using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Infrastructure.Data;
using IndustrialAutomation.Infrastructure.Repositories;
using AutoFixture;

namespace IndustrialAutomation.Tests.Unit.Repositories;

public class AutomationJobRepositoryTests : IDisposable
{
    private readonly DbContextOptions<IndustrialAutomationDbContext> _options;
    private readonly IndustrialAutomationDbContext _context;
    private readonly AutomationJobRepository _repository;
    private readonly IFixture _fixture;

    public AutomationJobRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<IndustrialAutomationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new IndustrialAutomationDbContext(_options);
        _repository = new AutomationJobRepository(_context);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllJobs_WhenJobsExist()
    {
        // Arrange
        var jobs = _fixture.CreateMany<AutomationJob>(3).ToList();
        _context.AutomationJobs.AddRange(jobs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(jobs);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoJobsExist()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnJob_WhenJobExists()
    {
        // Arrange
        var job = _fixture.Create<AutomationJob>();
        _context.AutomationJobs.Add(job);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(job.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(job.Id);
        result.Name.Should().Be(job.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenJobDoesNotExist()
    {
        // Arrange
        var nonExistentId = 999;

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldAddJob_WhenValidJob()
    {
        // Arrange
        var job = _fixture.Create<AutomationJob>();

        // Act
        var result = await _repository.AddAsync(job);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be(job.Name);

        var savedJob = await _context.AutomationJobs.FindAsync(result.Id);
        savedJob.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateJob_WhenJobExists()
    {
        // Arrange
        var job = _fixture.Create<AutomationJob>();
        _context.AutomationJobs.Add(job);
        await _context.SaveChangesAsync();

        var updatedName = "Updated Job Name";
        job.Name = updatedName;

        // Act
        await _repository.UpdateAsync(job);

        // Assert
        var updatedJob = await _context.AutomationJobs.FindAsync(job.Id);
        updatedJob.Should().NotBeNull();
        updatedJob!.Name.Should().Be(updatedName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteJob_WhenJobExists()
    {
        // Arrange
        var job = _fixture.Create<AutomationJob>();
        _context.AutomationJobs.Add(job);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(job.Id);

        // Assert
        result.Should().BeTrue();
        var deletedJob = await _context.AutomationJobs.FindAsync(job.Id);
        deletedJob.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenJobDoesNotExist()
    {
        // Arrange
        var nonExistentId = 999;

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnJobs_WhenJobsWithStatusExist()
    {
        // Arrange
        var jobs = _fixture.CreateMany<AutomationJob>(3).ToList();
        jobs[0].StatusId = 1; // Pending
        jobs[1].StatusId = 1; // Pending
        jobs[2].StatusId = 2; // Running
        _context.AutomationJobs.AddRange(jobs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByStatusAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(j => j.StatusId == 1);
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnEmptyList_WhenNoJobsWithStatusExist()
    {
        // Arrange
        var jobs = _fixture.CreateMany<AutomationJob>(2).ToList();
        jobs[0].StatusId = 1;
        jobs[1].StatusId = 2;
        _context.AutomationJobs.AddRange(jobs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByStatusAsync(3);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByJobTypeAsync_ShouldReturnJobs_WhenJobsWithTypeExist()
    {
        // Arrange
        var jobs = _fixture.CreateMany<AutomationJob>(3).ToList();
        jobs[0].JobTypeId = 1; // WebAutomation
        jobs[1].JobTypeId = 1; // WebAutomation
        jobs[2].JobTypeId = 2; // TestExecution
        _context.AutomationJobs.AddRange(jobs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByJobTypeAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(j => j.JobTypeId == 1);
    }

    [Fact]
    public async Task GetByJobTypeAsync_ShouldReturnEmptyList_WhenNoJobsWithTypeExist()
    {
        // Arrange
        var jobs = _fixture.CreateMany<AutomationJob>(2).ToList();
        jobs[0].JobTypeId = 1;
        jobs[1].JobTypeId = 2;
        _context.AutomationJobs.AddRange(jobs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByJobTypeAsync(3);

        // Assert
        result.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
