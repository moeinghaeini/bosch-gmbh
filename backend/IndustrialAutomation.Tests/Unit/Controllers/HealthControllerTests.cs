using Microsoft.AspNetCore.Mvc;
using Xunit;
using FluentAssertions;
using IndustrialAutomation.API.Controllers;

namespace IndustrialAutomation.Tests.Unit.Controllers;

public class HealthControllerTests
{
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _controller = new HealthController();
    }

    [Fact]
    public void Get_ShouldReturnOkResult_WithHealthStatus()
    {
        // Act
        var result = _controller.Get();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        
        var response = okResult!.Value;
        response.Should().NotBeNull();
        
        // Verify the response contains expected properties
        var responseType = response.GetType();
        responseType.GetProperty("status")!.GetValue(response).Should().Be("healthy");
        responseType.GetProperty("timestamp")!.GetValue(response).Should().NotBeNull();
        responseType.GetProperty("version")!.GetValue(response).Should().Be("1.0.0");
    }
}
