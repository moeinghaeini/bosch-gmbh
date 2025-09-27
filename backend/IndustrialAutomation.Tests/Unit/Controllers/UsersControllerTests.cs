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

public class UsersControllerTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<ILogger<UsersController>> _mockLogger;
    private readonly UsersController _controller;
    private readonly IFixture _fixture;

    public UsersControllerTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UsersController>>();
        _controller = new UsersController(_mockRepository.Object, _mockLogger.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetUsers_ShouldReturnOkResult_WhenUsersExist()
    {
        // Arrange
        var users = _fixture.CreateMany<User>(5).ToList();
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetUsers();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<User>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetUsers();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<User>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task GetUser_ShouldReturnOkResult_WhenUserExists(int userId)
    {
        // Arrange
        var user = _fixture.Create<User>();
        user.Id = userId;
        _mockRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUser(userId);

        // Assert
        result.Should().BeOfType<ActionResult<User>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(user);
    }

    [Theory, AutoData]
    public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist(int userId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.GetUser(userId);

        // Assert
        result.Should().BeOfType<ActionResult<User>>();
        var notFoundResult = result.Result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
    }

    [Theory, AutoData]
    public async Task GetUser_ShouldReturnInternalServerError_WhenExceptionOccurs(int userId)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(userId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetUser(userId);

        // Assert
        result.Should().BeOfType<ActionResult<User>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnCreatedAtAction_WhenUserIsValid()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync(user);

        // Act
        var result = await _controller.CreateUser(user);

        // Assert
        result.Should().BeOfType<ActionResult<User>>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.ActionName.Should().Be(nameof(UsersController.GetUser));
        createdResult.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<User>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateUser(user);

        // Assert
        result.Should().BeOfType<ActionResult<User>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task UpdateUser_ShouldReturnNoContent_WhenUserIsUpdated(int userId)
    {
        // Arrange
        var user = _fixture.Create<User>();
        user.Id = userId;
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateUser(userId, user);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Theory, AutoData]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenIdsDoNotMatch(int userId, int differentId)
    {
        // Arrange
        var user = _fixture.Create<User>();
        user.Id = differentId;

        // Act
        var result = await _controller.UpdateUser(userId, user);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Theory, AutoData]
    public async Task UpdateUser_ShouldReturnInternalServerError_WhenExceptionOccurs(int userId)
    {
        // Arrange
        var user = _fixture.Create<User>();
        user.Id = userId;
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<User>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateUser(userId, user);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Theory, AutoData]
    public async Task DeleteUser_ShouldReturnNoContent_WhenUserIsDeleted(int userId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(userId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Theory, AutoData]
    public async Task DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist(int userId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(userId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Theory, AutoData]
    public async Task DeleteUser_ShouldReturnInternalServerError_WhenExceptionOccurs(int userId)
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(userId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }
}
