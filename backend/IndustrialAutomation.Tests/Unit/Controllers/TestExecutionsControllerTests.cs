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

public class TestExecutionsControllerTests
{
    private readonly Mock<ITestExecutionRepository> _mockRepository;
    private readonly Mock<IAIService> _mockAIService;
    private readonly Mock<ILogger<TestExecutionsController>> _mockLogger;
    private readonly TestExecutionsController _controller;
    private readonly IFixture _fixture;

    public TestExecutionsControllerTests()
    {
        _mockRepository = new Mock<ITestExecutionRepository>();
        _mockAIService = new Mock<IAIService>();
        _mockLogger = new Mock<ILogger<TestExecutionsController>>();
        _controller = new TestExecutionsController(_mockRepository.Object, _mockAIService.Object, _mockLogger.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetTestExecutions_ShouldReturnOkResult_WhenTestExecutionsExist()
    {
        // Arrange
        var testExecutions = _fixture.CreateMany<TestExecution>(5).ToList();
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(testExecutions);

        // Act
        var result = await _controller.GetTestExecutions();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<TestExecution>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(testExecutions);
    }

    [Fact]
    public async Task GetTestExecutions_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetTestExecutions();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<TestExecution>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetTestExecution_ShouldReturnOkResult_WhenTestExecutionExists(int testExecutionId)
    {
        // Arrange
        var testExecution = _fixture.Create<TestExecution>();
        testExecution.Id = testExecutionId;
        _mockRepository.Setup(x => x.GetByIdAsync(testExecutionId)).ReturnsAsync(testExecution);

        // Act
        var result = await _controller.GetTestExecution(testExecutionId);

        // Assert
        result.Should().BeOfType<ActionResult<TestExecution>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(testExecution);
    }

    [Theory, AutoData]
    public async Task GetTestExecution_ShouldReturnNotFound_WhenTestExecutionDoesNotExist(int testExecutionId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(testExecutionId)).ReturnsAsync((TestExecution?)null);

        // Act
        var result = await _controller.GetTestExecution(testExecutionId);

        // Assert
        result.Should().BeOfType<ActionResult<TestExecution>>();
        var notFoundResult = result.Result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
    }

    [Theory, AutoData]
    public async Task GetTestExecution_ShouldReturnInternalServerError_WhenExceptionOccurs(int testExecutionId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(testExecutionId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetTestExecution(testExecutionId);

        // Assert
        result.Should().BeOfType<ActionResult<TestExecution>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CreateTestExecution_ShouldReturnCreatedAtAction_WhenTestExecutionIsValid()
    {
        // Arrange
        var testExecution = _fixture.Create<TestExecution>();
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<TestExecution>())).ReturnsAsync(testExecution);

        // Act
        var result = await _controller.CreateTestExecution(testExecution);

        // Assert
        result.Should().BeOfType<ActionResult<TestExecution>>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.ActionName.Should().Be(nameof(TestExecutionsController.GetTestExecution));
        createdResult.Value.Should().BeEquivalentTo(testExecution);
    }

    [Fact]
    public async Task CreateTestExecution_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var testExecution = _fixture.Create<TestExecution>();
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<TestExecution>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateTestExecution(testExecution);

        // Assert
        result.Should().BeOfType<ActionResult<TestExecution>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task UpdateTestExecution_ShouldReturnNoContent_WhenTestExecutionIsUpdated(int testExecutionId)
    {
        // Arrange
        var testExecution = _fixture.Create<TestExecution>();
        testExecution.Id = testExecutionId;
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<TestExecution>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateTestExecution(testExecutionId, testExecution);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Theory, AutoData]
    public async Task UpdateTestExecution_ShouldReturnBadRequest_WhenIdsDoNotMatch(int testExecutionId, int differentId)
    {
        // Arrange
        var testExecution = _fixture.Create<TestExecution>();
        testExecution.Id = differentId;

        // Act
        var result = await _controller.UpdateTestExecution(testExecutionId, testExecution);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Theory, AutoData]
    public async Task UpdateTestExecution_ShouldReturnInternalServerError_WhenExceptionOccurs(int testExecutionId)
    {
        // Arrange
        var testExecution = _fixture.Create<TestExecution>();
        testExecution.Id = testExecutionId;
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<TestExecution>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateTestExecution(testExecutionId, testExecution);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task DeleteTestExecution_ShouldReturnNoContent_WhenTestExecutionIsDeleted(int testExecutionId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(testExecutionId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteTestExecution(testExecutionId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Theory, AutoData]
    public async Task DeleteTestExecution_ShouldReturnNotFound_WhenTestExecutionDoesNotExist(int testExecutionId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(testExecutionId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteTestExecution(testExecutionId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory, AutoData]
    public async Task DeleteTestExecution_ShouldReturnInternalServerError_WhenExceptionOccurs(int testExecutionId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(testExecutionId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteTestExecution(testExecutionId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetTestExecutionsByStatus_ShouldReturnOkResult_WhenTestExecutionsExist(string status)
    {
        // Arrange
        var testExecutions = _fixture.CreateMany<TestExecution>(3).ToList();
        _mockRepository.Setup(x => x.GetByStatusAsync(status)).ReturnsAsync(testExecutions);

        // Act
        var result = await _controller.GetTestExecutionsByStatus(status);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<TestExecution>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(testExecutions);
    }

    [Theory, AutoData]
    public async Task GetTestExecutionsByStatus_ShouldReturnInternalServerError_WhenExceptionOccurs(string status)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByStatusAsync(status)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetTestExecutionsByStatus(status);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<TestExecution>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetTestExecutionsByType_ShouldReturnOkResult_WhenTestExecutionsExist(string testType)
    {
        // Arrange
        var testExecutions = _fixture.CreateMany<TestExecution>(3).ToList();
        _mockRepository.Setup(x => x.GetByTestTypeAsync(testType)).ReturnsAsync(testExecutions);

        // Act
        var result = await _controller.GetTestExecutionsByType(testType);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<TestExecution>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(testExecutions);
    }

    [Theory, AutoData]
    public async Task GetTestExecutionsByType_ShouldReturnInternalServerError_WhenExceptionOccurs(string testType)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByTestTypeAsync(testType)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetTestExecutionsByType(testType);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<TestExecution>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetTestExecutionsBySuite_ShouldReturnOkResult_WhenTestExecutionsExist(string testSuite)
    {
        // Arrange
        var testExecutions = _fixture.CreateMany<TestExecution>(3).ToList();
        _mockRepository.Setup(x => x.GetByTestSuiteAsync(testSuite)).ReturnsAsync(testExecutions);

        // Act
        var result = await _controller.GetTestExecutionsBySuite(testSuite);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<TestExecution>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(testExecutions);
    }

    [Theory, AutoData]
    public async Task GetTestExecutionsBySuite_ShouldReturnInternalServerError_WhenExceptionOccurs(string testSuite)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByTestSuiteAsync(testSuite)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetTestExecutionsBySuite(testSuite);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<TestExecution>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetFailedTests_ShouldReturnOkResult_WhenFailedTestsExist()
    {
        // Arrange
        var failedTests = _fixture.CreateMany<TestExecution>(3).ToList();
        _mockRepository.Setup(x => x.GetFailedTestsAsync()).ReturnsAsync(failedTests);

        // Act
        var result = await _controller.GetFailedTests();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<TestExecution>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(failedTests);
    }

    [Fact]
    public async Task GetFailedTests_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetFailedTestsAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetFailedTests();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<TestExecution>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task AnalyzeTestExecution_ShouldReturnOkResult_WhenTestExecutionExists(int testExecutionId)
    {
        // Arrange
        var testExecution = _fixture.Create<TestExecution>();
        testExecution.Id = testExecutionId;
        testExecution.ActualResult = "Test failed";
        testExecution.TestType = "Unit";
        var analysis = "Test failed due to assertion error";
        
        _mockRepository.Setup(x => x.GetByIdAsync(testExecutionId)).ReturnsAsync(testExecution);
        _mockAIService.Setup(x => x.AnalyzeTestResultsAsync(testExecution.ActualResult, testExecution.TestType))
            .ReturnsAsync(analysis);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<TestExecution>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AnalyzeTestExecution(testExecutionId);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(analysis);
    }

    [Theory, AutoData]
    public async Task AnalyzeTestExecution_ShouldReturnNotFound_WhenTestExecutionDoesNotExist(int testExecutionId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(testExecutionId)).ReturnsAsync((TestExecution?)null);

        // Act
        var result = await _controller.AnalyzeTestExecution(testExecutionId);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var notFoundResult = result.Result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
    }

    [Theory, AutoData]
    public async Task AnalyzeTestExecution_ShouldReturnInternalServerError_WhenExceptionOccurs(int testExecutionId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(testExecutionId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.AnalyzeTestExecution(testExecutionId);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GenerateTestCases_ShouldReturnOkResult_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<GenerateTestCasesRequest>();
        var testCases = "Generated test cases";
        _mockAIService.Setup(x => x.GenerateTestCasesAsync(request.Requirements, request.TestType))
            .ReturnsAsync(testCases);

        // Act
        var result = await _controller.GenerateTestCases(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(testCases);
    }

    [Fact]
    public async Task GenerateTestCases_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<GenerateTestCasesRequest>();
        _mockAIService.Setup(x => x.GenerateTestCasesAsync(request.Requirements, request.TestType))
            .ThrowsAsync(new Exception("AI service error"));

        // Act
        var result = await _controller.GenerateTestCases(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task OptimizeTestSuite_ShouldReturnOkResult_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<OptimizeTestSuiteRequest>();
        var optimization = "Test suite optimized";
        _mockAIService.Setup(x => x.OptimizeTestSuiteAsync(request.TestSuite))
            .ReturnsAsync(optimization);

        // Act
        var result = await _controller.OptimizeTestSuite(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(optimization);
    }

    [Fact]
    public async Task OptimizeTestSuite_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<OptimizeTestSuiteRequest>();
        _mockAIService.Setup(x => x.OptimizeTestSuiteAsync(request.TestSuite))
            .ThrowsAsync(new Exception("AI service error"));

        // Act
        var result = await _controller.OptimizeTestSuite(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }
}
