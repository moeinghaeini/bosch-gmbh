using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.API.Controllers;
using AutoFixture;

namespace IndustrialAutomation.Tests.Unit.Controllers;

public class MonitoringControllerTests
{
    private readonly Mock<IMonitoringService> _mockMonitoringService;
    private readonly Mock<ILogger<MonitoringController>> _mockLogger;
    private readonly MonitoringController _controller;
    private readonly IFixture _fixture;

    public MonitoringControllerTests()
    {
        _mockMonitoringService = new Mock<IMonitoringService>();
        _mockLogger = new Mock<ILogger<MonitoringController>>();
        _controller = new MonitoringController(_mockMonitoringService.Object, _mockLogger.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetSystemHealth_ShouldReturnOkResult_WhenHealthDataExists()
    {
        // Arrange
        var healthData = new Dictionary<string, object>
        {
            { "status", "healthy" },
            { "uptime", 99.9 },
            { "services", new[] { "database", "redis", "api" } }
        };
        _mockMonitoringService.Setup(x => x.GetSystemHealthAsync()).ReturnsAsync(healthData);

        // Act
        var result = await _controller.GetSystemHealth();

        // Assert
        result.Should().BeOfType<ActionResult<Dictionary<string, object>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(healthData);
    }

    [Fact]
    public async Task GetSystemHealth_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockMonitoringService.Setup(x => x.GetSystemHealthAsync()).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.GetSystemHealth();

        // Assert
        result.Should().BeOfType<ActionResult<Dictionary<string, object>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetPerformanceMetrics_ShouldReturnOkResult_WhenMetricsExist()
    {
        // Arrange
        var metrics = new Dictionary<string, object>
        {
            { "cpu_usage", 65.5 },
            { "memory_usage", 78.2 },
            { "response_time", 150.0 }
        };
        _mockMonitoringService.Setup(x => x.GetPerformanceMetricsAsync()).ReturnsAsync(metrics);

        // Act
        var result = await _controller.GetPerformanceMetrics();

        // Assert
        result.Should().BeOfType<ActionResult<Dictionary<string, object>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(metrics);
    }

    [Fact]
    public async Task GetPerformanceMetrics_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockMonitoringService.Setup(x => x.GetPerformanceMetricsAsync()).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.GetPerformanceMetrics();

        // Assert
        result.Should().BeOfType<ActionResult<Dictionary<string, object>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetBusinessMetrics_ShouldReturnOkResult_WhenMetricsExist()
    {
        // Arrange
        var metrics = new Dictionary<string, object>
        {
            { "total_jobs", 100 },
            { "successful_jobs", 95 },
            { "failed_jobs", 5 }
        };
        _mockMonitoringService.Setup(x => x.GetBusinessMetricsAsync()).ReturnsAsync(metrics);

        // Act
        var result = await _controller.GetBusinessMetrics();

        // Assert
        result.Should().BeOfType<ActionResult<Dictionary<string, object>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(metrics);
    }

    [Fact]
    public async Task GetBusinessMetrics_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockMonitoringService.Setup(x => x.GetBusinessMetricsAsync()).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.GetBusinessMetrics();

        // Assert
        result.Should().BeOfType<ActionResult<Dictionary<string, object>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetAlerts_ShouldReturnOkResult_WhenAlertsExist()
    {
        // Arrange
        var alerts = new List<Dictionary<string, object>>
        {
            new() { { "id", 1 }, { "message", "High CPU usage" }, { "severity", "warning" } },
            new() { { "id", 2 }, { "message", "Database connection failed" }, { "severity", "error" } }
        };
        _mockMonitoringService.Setup(x => x.GetAlertsAsync()).ReturnsAsync(alerts);

        // Act
        var result = await _controller.GetAlerts();

        // Assert
        result.Should().BeOfType<ActionResult<List<Dictionary<string, object>>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(alerts);
    }

    [Fact]
    public async Task GetAlerts_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockMonitoringService.Setup(x => x.GetAlertsAsync()).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.GetAlerts();

        // Assert
        result.Should().BeOfType<ActionResult<List<Dictionary<string, object>>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetDashboardData_ShouldReturnOkResult_WhenDataExists()
    {
        // Arrange
        var dashboardData = new Dictionary<string, object>
        {
            { "system_health", "good" },
            { "active_jobs", 5 },
            { "recent_errors", 0 }
        };
        _mockMonitoringService.Setup(x => x.GetDashboardDataAsync()).ReturnsAsync(dashboardData);

        // Act
        var result = await _controller.GetDashboardData();

        // Assert
        result.Should().BeOfType<ActionResult<Dictionary<string, object>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(dashboardData);
    }

    [Fact]
    public async Task GetDashboardData_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockMonitoringService.Setup(x => x.GetDashboardDataAsync()).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.GetDashboardData();

        // Assert
        result.Should().BeOfType<ActionResult<Dictionary<string, object>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetLogs_ShouldReturnOkResult_WhenLogsExist(int page, int pageSize, string level)
    {
        // Arrange
        var logs = new List<Dictionary<string, object>>
        {
            new() { { "id", 1 }, { "level", "info" }, { "message", "Test log" } },
            new() { { "id", 2 }, { "level", "error" }, { "message", "Error log" } }
        };
        _mockMonitoringService.Setup(x => x.GetLogsAsync(page, pageSize, level, null, null)).ReturnsAsync(logs);

        // Act
        var result = await _controller.GetLogs(page, pageSize, level);

        // Assert
        result.Should().BeOfType<ActionResult<List<Dictionary<string, object>>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(logs);
    }

    [Theory, AutoData]
    public async Task GetLogs_ShouldReturnInternalServerError_WhenExceptionOccurs(int page, int pageSize, string level)
    {
        // Arrange
        _mockMonitoringService.Setup(x => x.GetLogsAsync(page, pageSize, level, null, null)).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.GetLogs(page, pageSize, level);

        // Assert
        result.Should().BeOfType<ActionResult<List<Dictionary<string, object>>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetTrace_ShouldReturnOkResult_WhenTraceExists(string traceId)
    {
        // Arrange
        var trace = new Dictionary<string, object>
        {
            { "id", traceId },
            { "duration", 150.5 },
            { "spans", new[] { "span1", "span2" } }
        };
        _mockMonitoringService.Setup(x => x.GetTraceAsync(traceId)).ReturnsAsync(trace);

        // Act
        var result = await _controller.GetTrace(traceId);

        // Assert
        result.Should().BeOfType<ActionResult<Dictionary<string, object>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(trace);
    }

    [Theory, AutoData]
    public async Task GetTrace_ShouldReturnInternalServerError_WhenExceptionOccurs(string traceId)
    {
        // Arrange
        _mockMonitoringService.Setup(x => x.GetTraceAsync(traceId)).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.GetTrace(traceId);

        // Assert
        result.Should().BeOfType<ActionResult<Dictionary<string, object>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task RecordMetric_ShouldReturnOkResult_WhenMetricIsRecorded()
    {
        // Arrange
        var request = _fixture.Create<RecordMetricRequest>();
        _mockMonitoringService.Setup(x => x.RecordMetricAsync(request.MetricName, request.Value, request.Tags)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RecordMetric(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task RecordMetric_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<RecordMetricRequest>();
        _mockMonitoringService.Setup(x => x.RecordMetricAsync(request.MetricName, request.Value, request.Tags)).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.RecordMetric(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task RecordEvent_ShouldReturnOkResult_WhenEventIsRecorded()
    {
        // Arrange
        var request = _fixture.Create<RecordEventRequest>();
        _mockMonitoringService.Setup(x => x.RecordEventAsync(request.EventName, request.Properties)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RecordEvent(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task RecordEvent_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<RecordEventRequest>();
        _mockMonitoringService.Setup(x => x.RecordEventAsync(request.EventName, request.Properties)).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.RecordEvent(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task RecordError_ShouldReturnOkResult_WhenErrorIsRecorded()
    {
        // Arrange
        var request = _fixture.Create<RecordErrorRequest>();
        _mockMonitoringService.Setup(x => x.RecordErrorAsync(request.ErrorMessage, null, request.Context)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RecordError(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task RecordError_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<RecordErrorRequest>();
        _mockMonitoringService.Setup(x => x.RecordErrorAsync(request.ErrorMessage, null, request.Context)).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.RecordError(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CreateAlert_ShouldReturnOkResult_WhenAlertIsCreated()
    {
        // Arrange
        var request = _fixture.Create<CreateAlertRequest>();
        _mockMonitoringService.Setup(x => x.CreateAlertAsync(request.AlertName, request.Description, request.Severity, request.Conditions)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateAlert(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAlert_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<CreateAlertRequest>();
        _mockMonitoringService.Setup(x => x.CreateAlertAsync(request.AlertName, request.Description, request.Severity, request.Conditions)).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.CreateAlert(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task StartTrace_ShouldReturnOkResult_WhenTraceIsStarted()
    {
        // Arrange
        var request = _fixture.Create<StartTraceRequest>();
        _mockMonitoringService.Setup(x => x.StartTraceAsync(request.TraceName, request.Attributes)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.StartTrace(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task StartTrace_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<StartTraceRequest>();
        _mockMonitoringService.Setup(x => x.StartTraceAsync(request.TraceName, request.Attributes)).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.StartTrace(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }
}
