using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.API.Controllers;
using AutoFixture;

namespace IndustrialAutomation.Tests.Unit.Controllers;

public class JobSchedulesControllerTests
{
    private readonly Mock<IJobScheduleRepository> _mockRepository;
    private readonly Mock<ILogger<JobSchedulesController>> _mockLogger;
    private readonly JobSchedulesController _controller;
    private readonly IFixture _fixture;

    public JobSchedulesControllerTests()
    {
        _mockRepository = new Mock<IJobScheduleRepository>();
        _mockLogger = new Mock<ILogger<JobSchedulesController>>();
        _controller = new JobSchedulesController(_mockRepository.Object, _mockLogger.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetJobSchedules_ShouldReturnOkResult_WhenJobSchedulesExist()
    {
        // Arrange
        var jobSchedules = _fixture.CreateMany<JobSchedule>(5).ToList();
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(jobSchedules);

        // Act
        var result = await _controller.GetJobSchedules();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(jobSchedules);
    }

    [Fact]
    public async Task GetJobSchedules_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetJobSchedules();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetJobSchedule_ShouldReturnOkResult_WhenJobScheduleExists(int jobScheduleId)
    {
        // Arrange
        var jobSchedule = _fixture.Create<JobSchedule>();
        jobSchedule.Id = jobScheduleId;
        _mockRepository.Setup(x => x.GetByIdAsync(jobScheduleId)).ReturnsAsync(jobSchedule);

        // Act
        var result = await _controller.GetJobSchedule(jobScheduleId);

        // Assert
        result.Should().BeOfType<ActionResult<JobSchedule>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(jobSchedule);
    }

    [Theory, AutoData]
    public async Task GetJobSchedule_ShouldReturnNotFound_WhenJobScheduleDoesNotExist(int jobScheduleId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(jobScheduleId)).ReturnsAsync((JobSchedule?)null);

        // Act
        var result = await _controller.GetJobSchedule(jobScheduleId);

        // Assert
        result.Should().BeOfType<ActionResult<JobSchedule>>();
        var notFoundResult = result.Result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
    }

    [Theory, AutoData]
    public async Task GetJobSchedule_ShouldReturnInternalServerError_WhenExceptionOccurs(int jobScheduleId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(jobScheduleId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetJobSchedule(jobScheduleId);

        // Assert
        result.Should().BeOfType<ActionResult<JobSchedule>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CreateJobSchedule_ShouldReturnCreatedAtAction_WhenJobScheduleIsValid()
    {
        // Arrange
        var jobSchedule = _fixture.Create<JobSchedule>();
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<JobSchedule>())).ReturnsAsync(jobSchedule);

        // Act
        var result = await _controller.CreateJobSchedule(jobSchedule);

        // Assert
        result.Should().BeOfType<ActionResult<JobSchedule>>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.ActionName.Should().Be(nameof(JobSchedulesController.GetJobSchedule));
        createdResult.Value.Should().BeEquivalentTo(jobSchedule);
    }

    [Fact]
    public async Task CreateJobSchedule_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var jobSchedule = _fixture.Create<JobSchedule>();
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<JobSchedule>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateJobSchedule(jobSchedule);

        // Assert
        result.Should().BeOfType<ActionResult<JobSchedule>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task UpdateJobSchedule_ShouldReturnNoContent_WhenJobScheduleIsUpdated(int jobScheduleId)
    {
        // Arrange
        var jobSchedule = _fixture.Create<JobSchedule>();
        jobSchedule.Id = jobScheduleId;
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<JobSchedule>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateJobSchedule(jobScheduleId, jobSchedule);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Theory, AutoData]
    public async Task UpdateJobSchedule_ShouldReturnBadRequest_WhenIdsDoNotMatch(int jobScheduleId, int differentId)
    {
        // Arrange
        var jobSchedule = _fixture.Create<JobSchedule>();
        jobSchedule.Id = differentId;

        // Act
        var result = await _controller.UpdateJobSchedule(jobScheduleId, jobSchedule);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Theory, AutoData]
    public async Task UpdateJobSchedule_ShouldReturnInternalServerError_WhenExceptionOccurs(int jobScheduleId)
    {
        // Arrange
        var jobSchedule = _fixture.Create<JobSchedule>();
        jobSchedule.Id = jobScheduleId;
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<JobSchedule>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateJobSchedule(jobScheduleId, jobSchedule);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task DeleteJobSchedule_ShouldReturnNoContent_WhenJobScheduleIsDeleted(int jobScheduleId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(jobScheduleId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteJobSchedule(jobScheduleId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Theory, AutoData]
    public async Task DeleteJobSchedule_ShouldReturnNotFound_WhenJobScheduleDoesNotExist(int jobScheduleId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(jobScheduleId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteJobSchedule(jobScheduleId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory, AutoData]
    public async Task DeleteJobSchedule_ShouldReturnInternalServerError_WhenExceptionOccurs(int jobScheduleId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(jobScheduleId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteJobSchedule(jobScheduleId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetJobSchedulesByStatus_ShouldReturnOkResult_WhenJobSchedulesExist(string status)
    {
        // Arrange
        var jobSchedules = _fixture.CreateMany<JobSchedule>(3).ToList();
        _mockRepository.Setup(x => x.GetByStatusAsync(status)).ReturnsAsync(jobSchedules);

        // Act
        var result = await _controller.GetJobSchedulesByStatus(status);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(jobSchedules);
    }

    [Theory, AutoData]
    public async Task GetJobSchedulesByStatus_ShouldReturnInternalServerError_WhenExceptionOccurs(string status)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByStatusAsync(status)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetJobSchedulesByStatus(status);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetJobSchedulesByType_ShouldReturnOkResult_WhenJobSchedulesExist(string jobType)
    {
        // Arrange
        var jobSchedules = _fixture.CreateMany<JobSchedule>(3).ToList();
        _mockRepository.Setup(x => x.GetByJobTypeAsync(jobType)).ReturnsAsync(jobSchedules);

        // Act
        var result = await _controller.GetJobSchedulesByType(jobType);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(jobSchedules);
    }

    [Theory, AutoData]
    public async Task GetJobSchedulesByType_ShouldReturnInternalServerError_WhenExceptionOccurs(string jobType)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByJobTypeAsync(jobType)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetJobSchedulesByType(jobType);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetEnabledJobSchedules_ShouldReturnOkResult_WhenEnabledJobsExist()
    {
        // Arrange
        var enabledJobs = _fixture.CreateMany<JobSchedule>(3).ToList();
        _mockRepository.Setup(x => x.GetEnabledJobsAsync()).ReturnsAsync(enabledJobs);

        // Act
        var result = await _controller.GetEnabledJobSchedules();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(enabledJobs);
    }

    [Fact]
    public async Task GetEnabledJobSchedules_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetEnabledJobsAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetEnabledJobSchedules();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetJobsToRun_ShouldReturnOkResult_WhenJobsToRunExist()
    {
        // Arrange
        var jobsToRun = _fixture.CreateMany<JobSchedule>(3).ToList();
        _mockRepository.Setup(x => x.GetJobsToRunAsync(It.IsAny<DateTime>())).ReturnsAsync(jobsToRun);

        // Act
        var result = await _controller.GetJobsToRun();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(jobsToRun);
    }

    [Fact]
    public async Task GetJobsToRun_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetJobsToRunAsync(It.IsAny<DateTime>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetJobsToRun();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetJobSchedulesByPriority_ShouldReturnOkResult_WhenJobSchedulesExist(string priority)
    {
        // Arrange
        var jobSchedules = _fixture.CreateMany<JobSchedule>(3).ToList();
        _mockRepository.Setup(x => x.GetByPriorityAsync(priority)).ReturnsAsync(jobSchedules);

        // Act
        var result = await _controller.GetJobSchedulesByPriority(priority);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(jobSchedules);
    }

    [Theory, AutoData]
    public async Task GetJobSchedulesByPriority_ShouldReturnInternalServerError_WhenExceptionOccurs(string priority)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByPriorityAsync(priority)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetJobSchedulesByPriority(priority);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetDependentJobs_ShouldReturnOkResult_WhenDependentJobsExist(int jobId)
    {
        // Arrange
        var dependentJobs = _fixture.CreateMany<JobSchedule>(3).ToList();
        _mockRepository.Setup(x => x.GetDependentJobsAsync(jobId)).ReturnsAsync(dependentJobs);

        // Act
        var result = await _controller.GetDependentJobs(jobId);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(dependentJobs);
    }

    [Theory, AutoData]
    public async Task GetDependentJobs_ShouldReturnInternalServerError_WhenExceptionOccurs(int jobId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetDependentJobsAsync(jobId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetDependentJobs(jobId);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<JobSchedule>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task EnableJobSchedule_ShouldReturnOk_WhenJobScheduleExists(int jobId)
    {
        // Arrange
        var jobSchedule = _fixture.Create<JobSchedule>();
        jobSchedule.Id = jobId;
        jobSchedule.IsEnabled = false;
        _mockRepository.Setup(x => x.GetByIdAsync(jobId)).ReturnsAsync(jobSchedule);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<JobSchedule>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.EnableJobSchedule(jobId);

        // Assert
        result.Should().BeOfType<OkResult>();
        _mockRepository.Verify(x => x.UpdateAsync(It.Is<JobSchedule>(j => j.IsEnabled == true)), Times.Once);
    }

    [Theory, AutoData]
    public async Task EnableJobSchedule_ShouldReturnNotFound_WhenJobScheduleDoesNotExist(int jobId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(jobId)).ReturnsAsync((JobSchedule?)null);

        // Act
        var result = await _controller.EnableJobSchedule(jobId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory, AutoData]
    public async Task EnableJobSchedule_ShouldReturnInternalServerError_WhenExceptionOccurs(int jobId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(jobId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.EnableJobSchedule(jobId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task DisableJobSchedule_ShouldReturnOk_WhenJobScheduleExists(int jobId)
    {
        // Arrange
        var jobSchedule = _fixture.Create<JobSchedule>();
        jobSchedule.Id = jobId;
        jobSchedule.IsEnabled = true;
        _mockRepository.Setup(x => x.GetByIdAsync(jobId)).ReturnsAsync(jobSchedule);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<JobSchedule>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DisableJobSchedule(jobId);

        // Assert
        result.Should().BeOfType<OkResult>();
        _mockRepository.Verify(x => x.UpdateAsync(It.Is<JobSchedule>(j => j.IsEnabled == false)), Times.Once);
    }

    [Theory, AutoData]
    public async Task DisableJobSchedule_ShouldReturnNotFound_WhenJobScheduleDoesNotExist(int jobId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(jobId)).ReturnsAsync((JobSchedule?)null);

        // Act
        var result = await _controller.DisableJobSchedule(jobId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory, AutoData]
    public async Task DisableJobSchedule_ShouldReturnInternalServerError_WhenExceptionOccurs(int jobId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(jobId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DisableJobSchedule(jobId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task RunJobNow_ShouldReturnOk_WhenJobScheduleExists(int jobId)
    {
        // Arrange
        var jobSchedule = _fixture.Create<JobSchedule>();
        jobSchedule.Id = jobId;
        _mockRepository.Setup(x => x.GetByIdAsync(jobId)).ReturnsAsync(jobSchedule);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<JobSchedule>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RunJobNow(jobId);

        // Assert
        result.Should().BeOfType<OkResult>();
        _mockRepository.Verify(x => x.UpdateAsync(It.Is<JobSchedule>(j => 
            j.NextRunTime <= DateTime.UtcNow && j.Status == "Scheduled")), Times.Once);
    }

    [Theory, AutoData]
    public async Task RunJobNow_ShouldReturnNotFound_WhenJobScheduleDoesNotExist(int jobId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(jobId)).ReturnsAsync((JobSchedule?)null);

        // Act
        var result = await _controller.RunJobNow(jobId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory, AutoData]
    public async Task RunJobNow_ShouldReturnInternalServerError_WhenExceptionOccurs(int jobId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(jobId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.RunJobNow(jobId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }
}
