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

public class KPIControllerTests
{
    private readonly Mock<ITestExecutionRepository> _mockTestExecutionRepository;
    private readonly Mock<IWebAutomationRepository> _mockWebAutomationRepository;
    private readonly Mock<IJobScheduleRepository> _mockJobScheduleRepository;
    private readonly Mock<ILogger<KPIController>> _mockLogger;
    private readonly KPIController _controller;
    private readonly IFixture _fixture;

    public KPIControllerTests()
    {
        _mockTestExecutionRepository = new Mock<ITestExecutionRepository>();
        _mockWebAutomationRepository = new Mock<IWebAutomationRepository>();
        _mockJobScheduleRepository = new Mock<IJobScheduleRepository>();
        _mockLogger = new Mock<ILogger<KPIController>>();
        _controller = new KPIController(
            _mockTestExecutionRepository.Object,
            _mockWebAutomationRepository.Object,
            _mockJobScheduleRepository.Object,
            _mockLogger.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetTestExecutionKPIs_ShouldReturnOkResult_WhenDataExists()
    {
        // Arrange
        var testExecutions = _fixture.CreateMany<TestExecution>(10).ToList();
        testExecutions[0].Status = "Passed";
        testExecutions[1].Status = "Passed";
        testExecutions[2].Status = "Failed";
        testExecutions[3].Status = "Running";
        testExecutions[0].TestType = "Unit";
        testExecutions[1].TestType = "Integration";
        testExecutions[0].ExecutionTime = TimeSpan.FromMinutes(5);
        testExecutions[1].ExecutionTime = TimeSpan.FromMinutes(3);

        _mockTestExecutionRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(testExecutions);

        // Act
        var result = await _controller.GetTestExecutionKPIs();

        // Assert
        result.Should().BeOfType<ActionResult<TestExecutionKPIs>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var kpis = okResult!.Value as TestExecutionKPIs;
        kpis.Should().NotBeNull();
        kpis!.TotalTests.Should().Be(10);
        kpis.PassedTests.Should().Be(2);
        kpis.FailedTests.Should().Be(1);
        kpis.RunningTests.Should().Be(1);
    }

    [Fact]
    public async Task GetTestExecutionKPIs_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockTestExecutionRepository.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetTestExecutionKPIs();

        // Assert
        result.Should().BeOfType<ActionResult<TestExecutionKPIs>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetTestExecutionKPIs_ShouldCalculateSuccessRate_WhenNoTests()
    {
        // Arrange
        var testExecutions = new List<TestExecution>();
        _mockTestExecutionRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(testExecutions);

        // Act
        var result = await _controller.GetTestExecutionKPIs();

        // Assert
        result.Should().BeOfType<ActionResult<TestExecutionKPIs>>();
        var okResult = result.Result as OkObjectResult;
        var kpis = okResult!.Value as TestExecutionKPIs;
        kpis!.SuccessRate.Should().Be(0);
        kpis.FailureRate.Should().Be(0);
    }

    [Fact]
    public async Task GetWebAutomationKPIs_ShouldReturnOkResult_WhenDataExists()
    {
        // Arrange
        var webAutomations = _fixture.CreateMany<WebAutomation>(10).ToList();
        webAutomations[0].Status = "Completed";
        webAutomations[1].Status = "Completed";
        webAutomations[2].Status = "Failed";
        webAutomations[3].Status = "Running";
        webAutomations[0].AutomationType = "Login";
        webAutomations[1].AutomationType = "DataEntry";
        webAutomations[0].ExecutionTime = TimeSpan.FromMinutes(10);
        webAutomations[1].ExecutionTime = TimeSpan.FromMinutes(8);

        _mockWebAutomationRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(webAutomations);

        // Act
        var result = await _controller.GetWebAutomationKPIs();

        // Assert
        result.Should().BeOfType<ActionResult<WebAutomationKPIs>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var kpis = okResult!.Value as WebAutomationKPIs;
        kpis.Should().NotBeNull();
        kpis!.TotalAutomations.Should().Be(10);
        kpis.CompletedAutomations.Should().Be(2);
        kpis.FailedAutomations.Should().Be(1);
        kpis.RunningAutomations.Should().Be(1);
    }

    [Fact]
    public async Task GetWebAutomationKPIs_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockWebAutomationRepository.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetWebAutomationKPIs();

        // Assert
        result.Should().BeOfType<ActionResult<WebAutomationKPIs>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetJobSchedulingKPIs_ShouldReturnOkResult_WhenDataExists()
    {
        // Arrange
        var jobSchedules = _fixture.CreateMany<JobSchedule>(10).ToList();
        jobSchedules[0].IsEnabled = true;
        jobSchedules[1].IsEnabled = true;
        jobSchedules[2].IsEnabled = false;
        jobSchedules[0].Status = "Scheduled";
        jobSchedules[1].Status = "Running";
        jobSchedules[2].Status = "Completed";
        jobSchedules[0].JobType = "TestExecution";
        jobSchedules[1].JobType = "WebAutomation";
        jobSchedules[0].Priority = "High";
        jobSchedules[1].Priority = "Medium";

        _mockJobScheduleRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(jobSchedules);

        // Act
        var result = await _controller.GetJobSchedulingKPIs();

        // Assert
        result.Should().BeOfType<ActionResult<JobSchedulingKPIs>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var kpis = okResult!.Value as JobSchedulingKPIs;
        kpis.Should().NotBeNull();
        kpis!.TotalJobs.Should().Be(10);
        kpis.EnabledJobs.Should().Be(2);
        kpis.ScheduledJobs.Should().Be(1);
        kpis.RunningJobs.Should().Be(1);
        kpis.CompletedJobs.Should().Be(1);
    }

    [Fact]
    public async Task GetJobSchedulingKPIs_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockJobScheduleRepository.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetJobSchedulingKPIs();

        // Assert
        result.Should().BeOfType<ActionResult<JobSchedulingKPIs>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetOverallPerformanceKPIs_ShouldReturnOkResult_WhenDataExists()
    {
        // Arrange
        var testExecutions = _fixture.CreateMany<TestExecution>(5).ToList();
        testExecutions[0].Status = "Passed";
        testExecutions[1].Status = "Failed";
        var webAutomations = _fixture.CreateMany<WebAutomation>(5).ToList();
        webAutomations[0].Status = "Completed";
        webAutomations[1].Status = "Failed";
        var jobSchedules = _fixture.CreateMany<JobSchedule>(5).ToList();
        jobSchedules[0].Status = "Completed";
        jobSchedules[1].Status = "Failed";

        _mockTestExecutionRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(testExecutions);
        _mockWebAutomationRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(webAutomations);
        _mockJobScheduleRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(jobSchedules);

        // Act
        var result = await _controller.GetOverallPerformanceKPIs();

        // Assert
        result.Should().BeOfType<ActionResult<OverallPerformanceKPIs>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var kpis = okResult!.Value as OverallPerformanceKPIs;
        kpis.Should().NotBeNull();
        kpis!.SystemUptime.Should().Be(99.9);
        kpis.TotalAutomationTasks.Should().Be(15);
        kpis.SuccessfulTasks.Should().Be(3); // 1 passed test + 1 completed automation + 1 completed job
        kpis.FailedTasks.Should().Be(3); // 1 failed test + 1 failed automation + 1 failed job
    }

    [Fact]
    public async Task GetOverallPerformanceKPIs_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockTestExecutionRepository.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetOverallPerformanceKPIs();

        // Assert
        result.Should().BeOfType<ActionResult<OverallPerformanceKPIs>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }
}
