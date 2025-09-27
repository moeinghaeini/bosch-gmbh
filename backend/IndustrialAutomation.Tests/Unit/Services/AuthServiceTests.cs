using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using IndustrialAutomation.Core.Entities;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Infrastructure.Services;
using AutoFixture;
using System.Security.Cryptography;
using System.Text;

namespace IndustrialAutomation.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly AuthService _authService;
    private readonly IFixture _fixture;
    private readonly string _jwtSecret = "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<AuthService>>();
        _authService = new AuthService(_mockUserRepository.Object, _mockLogger.Object, _jwtSecret);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var username = "testuser";
        var password = "testpassword";
        var user = _fixture.Create<User>();
        user.Username = username;
        user.PasswordHash = HashPassword(password);
        user.IsActive = true;

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(username)).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var username = "nonexistentuser";
        var password = "password";
        _mockUserRepository.Setup(x => x.GetByUsernameAsync(username)).ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        var username = "testuser";
        var password = "wrongpassword";
        var user = _fixture.Create<User>();
        user.Username = username;
        user.PasswordHash = HashPassword("correctpassword");
        user.IsActive = true;

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(username)).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserIsInactive()
    {
        // Arrange
        var username = "testuser";
        var password = "testpassword";
        var user = _fixture.Create<User>();
        user.Username = username;
        user.PasswordHash = HashPassword(password);
        user.IsActive = false;

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(username)).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Account is inactive");
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenUserDoesNotExist()
    {
        // Arrange
        var username = "newuser";
        var email = "newuser@example.com";
        var password = "newpassword";
        var user = _fixture.Create<User>();
        user.Username = username;
        user.Email = email;

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(username)).ReturnsAsync((User?)null);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync((User?)null);
        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync(user);

        // Act
        var result = await _authService.RegisterAsync(username, email, password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFailure_WhenUsernameExists()
    {
        // Arrange
        var username = "existinguser";
        var email = "newuser@example.com";
        var password = "newpassword";
        var existingUser = _fixture.Create<User>();
        existingUser.Username = username;

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(username)).ReturnsAsync(existingUser);

        // Act
        var result = await _authService.RegisterAsync(username, email, password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Username already exists");
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFailure_WhenEmailExists()
    {
        // Arrange
        var username = "newuser";
        var email = "existing@example.com";
        var password = "newpassword";
        var existingUser = _fixture.Create<User>();
        existingUser.Email = email;

        _mockUserRepository.Setup(x => x.GetByUsernameAsync(username)).ReturnsAsync((User?)null);
        _mockUserRepository.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(existingUser);

        // Act
        var result = await _authService.RegisterAsync(username, email, password);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Email already exists");
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNewToken_WhenRefreshTokenIsValid()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";
        var user = _fixture.Create<User>();
        user.IsActive = true;

        _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(refreshToken)).ReturnsAsync(user);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnFailure_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var refreshToken = "invalid-refresh-token";
        _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(refreshToken)).ReturnsAsync((User?)null);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Invalid refresh token");
    }

    [Fact]
    public async Task LogoutAsync_ShouldReturnSuccess_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        _mockUserRepository.Setup(x => x.RemoveRefreshTokenAsync(userId)).Returns(Task.CompletedTask);

        // Act
        var result = await _authService.LogoutAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
