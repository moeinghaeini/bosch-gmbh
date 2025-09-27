using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.API.Controllers;
using AutoFixture;
using AutoFixture.Xunit2;

namespace IndustrialAutomation.Tests.Unit.Controllers;

public class AutomationJobsControllerTests
{
    private readonly Mock<IAutomationJobRepository> _mockRepository;
    private readonly Mock<ILogger<AutomationJobsController>> _mockLogger;
    private readonly AutomationJobsController _controller;
    private readonly IFixture _fixture;

    public AutomationJobsControllerTests()
    {
        _mockRepository = new Mock<IAutomationJobRepository>();
        _mockLogger = new Mock<ILogger<AutomationJobsController>>();
        _controller = new AutomationJobsController(_mockRepository.Object, _mockLogger.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetAutomationJobs_ShouldReturnOkResult_WhenJobsExist()
    {
        // Arrange
        var jobs = _fixture.CreateMany<AutomationJob>(5).ToList();
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(jobs);

        // Act
        var result = await _controller.GetAutomationJobs();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<AutomationJob>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(jobs);
    }

    [Fact]
    public async Task GetAutomationJobs_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAutomationJobs();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<AutomationJob>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetAutomationJob_ShouldReturnOkResult_WhenJobExists(int jobId)
    {
        // Arrange
        var job = _fixture.Create<AutomationJob>();
        job.Id = jobId;
        _mockRepository.Setup(x => x.GetByIdAsync(jobId)).ReturnsAsync(job);

        // Act
        var result = await _controller.GetAutomationJob(jobId);

        // Assert
        result.Should().BeOfType<ActionResult<AutomationJob>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(job);
    }

    [Theory, AutoData]
    public async Task GetAutomationJob_ShouldReturnNotFound_WhenJobDoesNotExist(int jobId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(jobId)).ReturnsAsync((AutomationJob?)null);

        // Act
        var result = await _controller.GetAutomationJob(jobId);

        // Assert
        result.Should().BeOfType<ActionResult<AutomationJob>>();
        var notFoundResult = result.Result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAutomationJob_ShouldReturnCreatedAtAction_WhenJobIsValid()
    {
        // Arrange
        var job = _fixture.Create<AutomationJob>();
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<AutomationJob>())).ReturnsAsync(job);

        // Act
        var result = await _controller.CreateAutomationJob(job);

        // Assert
        result.Should().BeOfType<ActionResult<AutomationJob>>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.ActionName.Should().Be(nameof(AutomationJobsController.GetAutomationJob));
        createdResult.Value.Should().BeEquivalentTo(job);
    }

    [Fact]
    public async Task CreateAutomationJob_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var job = _fixture.Create<AutomationJob>();
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<AutomationJob>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateAutomationJob(job);

        // Assert
        result.Should().BeOfType<ActionResult<AutomationJob>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task UpdateAutomationJob_ShouldReturnNoContent_WhenJobIsUpdated(int jobId)
    {
        // Arrange
        var job = _fixture.Create<AutomationJob>();
        job.Id = jobId;
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<AutomationJob>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateAutomationJob(jobId, job);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Theory, AutoData]
    public async Task UpdateAutomationJob_ShouldReturnBadRequest_WhenIdsDoNotMatch(int jobId, int differentId)
    {
        // Arrange
        var job = _fixture.Create<AutomationJob>();
        job.Id = differentId;

        // Act
        var result = await _controller.UpdateAutomationJob(jobId, job);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Theory, AutoData]
    public async Task DeleteAutomationJob_ShouldReturnNoContent_WhenJobIsDeleted(int jobId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(jobId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteAutomationJob(jobId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Theory, AutoData]
    public async Task DeleteAutomationJob_ShouldReturnNotFound_WhenJobDoesNotExist(int jobId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(jobId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteAutomationJob(jobId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory, AutoData]
    public async Task GetAutomationJobsByStatus_ShouldReturnOkResult_WhenJobsExist(int statusId)
    {
        // Arrange
        var jobs = _fixture.CreateMany<AutomationJob>(3).ToList();
        _mockRepository.Setup(x => x.GetByStatusAsync(statusId)).ReturnsAsync(jobs);

        // Act
        var result = await _controller.GetAutomationJobsByStatus(statusId);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<AutomationJob>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(jobs);
    }

    [Theory, AutoData]
    public async Task GetAutomationJobsByType_ShouldReturnOkResult_WhenJobsExist(int jobTypeId)
    {
        // Arrange
        var jobs = _fixture.CreateMany<AutomationJob>(3).ToList();
        _mockRepository.Setup(x => x.GetByJobTypeAsync(jobTypeId)).ReturnsAsync(jobs);

        // Act
        var result = await _controller.GetAutomationJobsByType(jobTypeId);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<AutomationJob>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(jobs);
    }
}
