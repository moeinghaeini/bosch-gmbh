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

public class WebAutomationsControllerTests
{
    private readonly Mock<IWebAutomationRepository> _mockRepository;
    private readonly Mock<IAIService> _mockAIService;
    private readonly Mock<ILogger<WebAutomationsController>> _mockLogger;
    private readonly WebAutomationsController _controller;
    private readonly IFixture _fixture;

    public WebAutomationsControllerTests()
    {
        _mockRepository = new Mock<IWebAutomationRepository>();
        _mockAIService = new Mock<IAIService>();
        _mockLogger = new Mock<ILogger<WebAutomationsController>>();
        _controller = new WebAutomationsController(_mockRepository.Object, _mockAIService.Object, _mockLogger.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetWebAutomations_ShouldReturnOkResult_WhenWebAutomationsExist()
    {
        // Arrange
        var webAutomations = _fixture.CreateMany<WebAutomation>(5).ToList();
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(webAutomations);

        // Act
        var result = await _controller.GetWebAutomations();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<WebAutomation>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(webAutomations);
    }

    [Fact]
    public async Task GetWebAutomations_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetWebAutomations();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<WebAutomation>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetWebAutomation_ShouldReturnOkResult_WhenWebAutomationExists(int webAutomationId)
    {
        // Arrange
        var webAutomation = _fixture.Create<WebAutomation>();
        webAutomation.Id = webAutomationId;
        _mockRepository.Setup(x => x.GetByIdAsync(webAutomationId)).ReturnsAsync(webAutomation);

        // Act
        var result = await _controller.GetWebAutomation(webAutomationId);

        // Assert
        result.Should().BeOfType<ActionResult<WebAutomation>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(webAutomation);
    }

    [Theory, AutoData]
    public async Task GetWebAutomation_ShouldReturnNotFound_WhenWebAutomationDoesNotExist(int webAutomationId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(webAutomationId)).ReturnsAsync((WebAutomation?)null);

        // Act
        var result = await _controller.GetWebAutomation(webAutomationId);

        // Assert
        result.Should().BeOfType<ActionResult<WebAutomation>>();
        var notFoundResult = result.Result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
    }

    [Theory, AutoData]
    public async Task GetWebAutomation_ShouldReturnInternalServerError_WhenExceptionOccurs(int webAutomationId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(webAutomationId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetWebAutomation(webAutomationId);

        // Assert
        result.Should().BeOfType<ActionResult<WebAutomation>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CreateWebAutomation_ShouldReturnCreatedAtAction_WhenWebAutomationIsValid()
    {
        // Arrange
        var webAutomation = _fixture.Create<WebAutomation>();
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<WebAutomation>())).ReturnsAsync(webAutomation);

        // Act
        var result = await _controller.CreateWebAutomation(webAutomation);

        // Assert
        result.Should().BeOfType<ActionResult<WebAutomation>>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.ActionName.Should().Be(nameof(WebAutomationsController.GetWebAutomation));
        createdResult.Value.Should().BeEquivalentTo(webAutomation);
    }

    [Fact]
    public async Task CreateWebAutomation_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var webAutomation = _fixture.Create<WebAutomation>();
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<WebAutomation>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateWebAutomation(webAutomation);

        // Assert
        result.Should().BeOfType<ActionResult<WebAutomation>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task UpdateWebAutomation_ShouldReturnNoContent_WhenWebAutomationIsUpdated(int webAutomationId)
    {
        // Arrange
        var webAutomation = _fixture.Create<WebAutomation>();
        webAutomation.Id = webAutomationId;
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<WebAutomation>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateWebAutomation(webAutomationId, webAutomation);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Theory, AutoData]
    public async Task UpdateWebAutomation_ShouldReturnBadRequest_WhenIdsDoNotMatch(int webAutomationId, int differentId)
    {
        // Arrange
        var webAutomation = _fixture.Create<WebAutomation>();
        webAutomation.Id = differentId;

        // Act
        var result = await _controller.UpdateWebAutomation(webAutomationId, webAutomation);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Theory, AutoData]
    public async Task UpdateWebAutomation_ShouldReturnInternalServerError_WhenExceptionOccurs(int webAutomationId)
    {
        // Arrange
        var webAutomation = _fixture.Create<WebAutomation>();
        webAutomation.Id = webAutomationId;
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<WebAutomation>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateWebAutomation(webAutomationId, webAutomation);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task DeleteWebAutomation_ShouldReturnNoContent_WhenWebAutomationIsDeleted(int webAutomationId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(webAutomationId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteWebAutomation(webAutomationId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Theory, AutoData]
    public async Task DeleteWebAutomation_ShouldReturnNotFound_WhenWebAutomationDoesNotExist(int webAutomationId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(webAutomationId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteWebAutomation(webAutomationId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory, AutoData]
    public async Task DeleteWebAutomation_ShouldReturnInternalServerError_WhenExceptionOccurs(int webAutomationId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(webAutomationId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteWebAutomation(webAutomationId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetWebAutomationsByStatus_ShouldReturnOkResult_WhenWebAutomationsExist(string status)
    {
        // Arrange
        var webAutomations = _fixture.CreateMany<WebAutomation>(3).ToList();
        _mockRepository.Setup(x => x.GetByStatusAsync(status)).ReturnsAsync(webAutomations);

        // Act
        var result = await _controller.GetWebAutomationsByStatus(status);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<WebAutomation>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(webAutomations);
    }

    [Theory, AutoData]
    public async Task GetWebAutomationsByStatus_ShouldReturnInternalServerError_WhenExceptionOccurs(string status)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByStatusAsync(status)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetWebAutomationsByStatus(status);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<WebAutomation>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetWebAutomationsByType_ShouldReturnOkResult_WhenWebAutomationsExist(string automationType)
    {
        // Arrange
        var webAutomations = _fixture.CreateMany<WebAutomation>(3).ToList();
        _mockRepository.Setup(x => x.GetByAutomationTypeAsync(automationType)).ReturnsAsync(webAutomations);

        // Act
        var result = await _controller.GetWebAutomationsByType(automationType);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<WebAutomation>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(webAutomations);
    }

    [Theory, AutoData]
    public async Task GetWebAutomationsByType_ShouldReturnInternalServerError_WhenExceptionOccurs(string automationType)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByAutomationTypeAsync(automationType)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetWebAutomationsByType(automationType);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<WebAutomation>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetWebAutomationsByWebsite_ShouldReturnOkResult_WhenWebAutomationsExist(string websiteUrl)
    {
        // Arrange
        var webAutomations = _fixture.CreateMany<WebAutomation>(3).ToList();
        _mockRepository.Setup(x => x.GetByWebsiteAsync(websiteUrl)).ReturnsAsync(webAutomations);

        // Act
        var result = await _controller.GetWebAutomationsByWebsite(websiteUrl);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<WebAutomation>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(webAutomations);
    }

    [Theory, AutoData]
    public async Task GetWebAutomationsByWebsite_ShouldReturnInternalServerError_WhenExceptionOccurs(string websiteUrl)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByWebsiteAsync(websiteUrl)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetWebAutomationsByWebsite(websiteUrl);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<WebAutomation>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetFailedAutomations_ShouldReturnOkResult_WhenFailedAutomationsExist()
    {
        // Arrange
        var failedAutomations = _fixture.CreateMany<WebAutomation>(3).ToList();
        _mockRepository.Setup(x => x.GetFailedAutomationsAsync()).ReturnsAsync(failedAutomations);

        // Act
        var result = await _controller.GetFailedAutomations();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<WebAutomation>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(failedAutomations);
    }

    [Fact]
    public async Task GetFailedAutomations_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetFailedAutomationsAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetFailedAutomations();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<WebAutomation>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task AnalyzeWebPage_ShouldReturnOkResult_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<AnalyzeWebPageRequest>();
        var analysis = "Web page analysis result";
        _mockAIService.Setup(x => x.AnalyzeWebPageAsync(request.Url, request.Prompt)).ReturnsAsync(analysis);

        // Act
        var result = await _controller.AnalyzeWebPage(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(analysis);
    }

    [Fact]
    public async Task AnalyzeWebPage_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<AnalyzeWebPageRequest>();
        _mockAIService.Setup(x => x.AnalyzeWebPageAsync(request.Url, request.Prompt)).ThrowsAsync(new Exception("AI service error"));

        // Act
        var result = await _controller.AnalyzeWebPage(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task IdentifyWebElement_ShouldReturnOkResult_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<IdentifyElementRequest>();
        var elementInfo = "Element identification result";
        _mockAIService.Setup(x => x.IdentifyWebElementAsync(request.PageContent, request.Description)).ReturnsAsync(elementInfo);

        // Act
        var result = await _controller.IdentifyWebElement(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(elementInfo);
    }

    [Fact]
    public async Task IdentifyWebElement_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<IdentifyElementRequest>();
        _mockAIService.Setup(x => x.IdentifyWebElementAsync(request.PageContent, request.Description)).ThrowsAsync(new Exception("AI service error"));

        // Act
        var result = await _controller.IdentifyWebElement(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GenerateWebSelector_ShouldReturnOkResult_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<GenerateSelectorRequest>();
        var selector = "Generated CSS selector";
        _mockAIService.Setup(x => x.GenerateWebSelectorAsync(request.ElementDescription, request.PageContent)).ReturnsAsync(selector);

        // Act
        var result = await _controller.GenerateWebSelector(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(selector);
    }

    [Fact]
    public async Task GenerateWebSelector_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<GenerateSelectorRequest>();
        _mockAIService.Setup(x => x.GenerateWebSelectorAsync(request.ElementDescription, request.PageContent)).ThrowsAsync(new Exception("AI service error"));

        // Act
        var result = await _controller.GenerateWebSelector(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ValidateWebAction_ShouldReturnOkResult_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<ValidateActionRequest>();
        var validation = "Action validation result";
        _mockAIService.Setup(x => x.ValidateWebActionAsync(request.Action, request.Element, request.PageContent)).ReturnsAsync(validation);

        // Act
        var result = await _controller.ValidateWebAction(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(validation);
    }

    [Fact]
    public async Task ValidateWebAction_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<ValidateActionRequest>();
        _mockAIService.Setup(x => x.ValidateWebActionAsync(request.Action, request.Element, request.PageContent)).ThrowsAsync(new Exception("AI service error"));

        // Act
        var result = await _controller.ValidateWebAction(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ExtractDataFromWeb_ShouldReturnOkResult_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<ExtractDataRequest>();
        var extractedData = "Extracted data result";
        _mockAIService.Setup(x => x.ExtractDataFromWebAsync(request.PageContent, request.ExtractionPrompt)).ReturnsAsync(extractedData);

        // Act
        var result = await _controller.ExtractDataFromWeb(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(extractedData);
    }

    [Fact]
    public async Task ExtractDataFromWeb_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<ExtractDataRequest>();
        _mockAIService.Setup(x => x.ExtractDataFromWebAsync(request.PageContent, request.ExtractionPrompt)).ThrowsAsync(new Exception("AI service error"));

        // Act
        var result = await _controller.ExtractDataFromWeb(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GenerateAutomationScript_ShouldReturnOkResult_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<GenerateScriptRequest>();
        var script = "Generated automation script";
        _mockAIService.Setup(x => x.GenerateAutomationScriptAsync(request.Requirements, request.TargetWebsite)).ReturnsAsync(script);

        // Act
        var result = await _controller.GenerateAutomationScript(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(script);
    }

    [Fact]
    public async Task GenerateAutomationScript_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<GenerateScriptRequest>();
        _mockAIService.Setup(x => x.GenerateAutomationScriptAsync(request.Requirements, request.TargetWebsite)).ThrowsAsync(new Exception("AI service error"));

        // Act
        var result = await _controller.GenerateAutomationScript(request);

        // Assert
        result.Should().BeOfType<ActionResult<string>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }
}
