using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Core.Models;
using IndustrialAutomation.API.Controllers;
using AutoFixture;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace IndustrialAutomation.Tests.Unit.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;
    private readonly IFixture _fixture;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
        _fixture = new Fixture();
        
        // Setup controller context
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();
        var response = _fixture.Create<AuthResponse>();
        _mockAuthService.Setup(x => x.LoginAsync(request)).ReturnsAsync(response);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<ActionResult<AuthResponse>>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();
        _controller.ModelState.AddModelError("Email", "Email is required");

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<ActionResult<AuthResponse>>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUnauthorizedAccessException()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();
        _mockAuthService.Setup(x => x.LoginAsync(request)).ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<ActionResult<AuthResponse>>();
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();
        _mockAuthService.Setup(x => x.LoginAsync(request)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Should().BeOfType<ActionResult<AuthResponse>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Register_ShouldReturnCreated_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequest>();
        var response = _fixture.Create<AuthResponse>();
        _mockAuthService.Setup(x => x.RegisterAsync(request)).ReturnsAsync(response);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<ActionResult<AuthResponse>>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.ActionName.Should().Be(nameof(AuthController.GetUserInfo));
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequest>();
        _controller.ModelState.AddModelError("Email", "Email is required");

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<ActionResult<AuthResponse>>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenInvalidOperationException()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequest>();
        _mockAuthService.Setup(x => x.RegisterAsync(request)).ThrowsAsync(new InvalidOperationException("User already exists"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<ActionResult<AuthResponse>>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequest>();
        _mockAuthService.Setup(x => x.RegisterAsync(request)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Should().BeOfType<ActionResult<AuthResponse>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<RefreshTokenRequest>();
        var response = _fixture.Create<AuthResponse>();
        _mockAuthService.Setup(x => x.RefreshTokenAsync(request)).ReturnsAsync(response);

        // Act
        var result = await _controller.RefreshToken(request);

        // Assert
        result.Should().BeOfType<ActionResult<AuthResponse>>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var request = _fixture.Create<RefreshTokenRequest>();
        _controller.ModelState.AddModelError("RefreshToken", "Refresh token is required");

        // Act
        var result = await _controller.RefreshToken(request);

        // Assert
        result.Should().BeOfType<ActionResult<AuthResponse>>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnUnauthorized_WhenUnauthorizedAccessException()
    {
        // Arrange
        var request = _fixture.Create<RefreshTokenRequest>();
        _mockAuthService.Setup(x => x.RefreshTokenAsync(request)).ThrowsAsync(new UnauthorizedAccessException("Invalid refresh token"));

        // Act
        var result = await _controller.RefreshToken(request);

        // Assert
        result.Should().BeOfType<ActionResult<AuthResponse>>();
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<RefreshTokenRequest>();
        _mockAuthService.Setup(x => x.RefreshTokenAsync(request)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.RefreshToken(request);

        // Assert
        result.Should().BeOfType<ActionResult<AuthResponse>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Logout_ShouldReturnOk_WhenValidToken()
    {
        // Arrange
        var token = "valid-token";
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.LogoutAsync(token)).ReturnsAsync(true);

        // Act
        var result = await _controller.Logout();

        // Assert
        result.Should().BeOfType<ActionResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Logout_ShouldReturnBadRequest_WhenTokenNotFound()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "";

        // Act
        var result = await _controller.Logout();

        // Assert
        result.Should().BeOfType<ActionResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Logout_ShouldReturnBadRequest_WhenLogoutFails()
    {
        // Arrange
        var token = "invalid-token";
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.LogoutAsync(token)).ReturnsAsync(false);

        // Act
        var result = await _controller.Logout();

        // Assert
        result.Should().BeOfType<ActionResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Logout_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var token = "valid-token";
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.LogoutAsync(token)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Logout();

        // Assert
        result.Should().BeOfType<ActionResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetUserInfo_ShouldReturnOk_WhenValidToken()
    {
        // Arrange
        var token = "valid-token";
        var userInfo = _fixture.Create<UserInfo>();
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.GetUserInfoAsync(token)).ReturnsAsync(userInfo);

        // Act
        var result = await _controller.GetUserInfo();

        // Assert
        result.Should().BeOfType<ActionResult<UserInfo>>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(userInfo);
    }

    [Fact]
    public async Task GetUserInfo_ShouldReturnBadRequest_WhenTokenNotFound()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "";

        // Act
        var result = await _controller.GetUserInfo();

        // Assert
        result.Should().BeOfType<ActionResult<UserInfo>>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserInfo_ShouldReturnUnauthorized_WhenUnauthorizedAccessException()
    {
        // Arrange
        var token = "invalid-token";
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.GetUserInfoAsync(token)).ThrowsAsync(new UnauthorizedAccessException("Invalid token"));

        // Act
        var result = await _controller.GetUserInfo();

        // Assert
        result.Should().BeOfType<ActionResult<UserInfo>>();
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserInfo_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var token = "valid-token";
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.GetUserInfoAsync(token)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetUserInfo();

        // Assert
        result.Should().BeOfType<ActionResult<UserInfo>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<ChangePasswordRequest>();
        var token = CreateValidJwtToken(1);
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.ChangePasswordAsync(1, request)).ReturnsAsync(true);

        // Act
        var result = await _controller.ChangePassword(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var request = _fixture.Create<ChangePasswordRequest>();
        _controller.ModelState.AddModelError("CurrentPassword", "Current password is required");

        // Act
        var result = await _controller.ChangePassword(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnUnauthorized_WhenInvalidToken()
    {
        // Arrange
        var request = _fixture.Create<ChangePasswordRequest>();
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "";

        // Act
        var result = await _controller.ChangePassword(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnBadRequest_WhenPasswordChangeFails()
    {
        // Arrange
        var request = _fixture.Create<ChangePasswordRequest>();
        var token = CreateValidJwtToken(1);
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.ChangePasswordAsync(1, request)).ReturnsAsync(false);

        // Act
        var result = await _controller.ChangePassword(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<ChangePasswordRequest>();
        var token = CreateValidJwtToken(1);
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.ChangePasswordAsync(1, request)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.ChangePassword(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ForgotPassword_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<ForgotPasswordRequest>();
        _mockAuthService.Setup(x => x.ForgotPasswordAsync(request)).ReturnsAsync(true);

        // Act
        var result = await _controller.ForgotPassword(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ForgotPassword_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var request = _fixture.Create<ForgotPasswordRequest>();
        _controller.ModelState.AddModelError("Email", "Email is required");

        // Act
        var result = await _controller.ForgotPassword(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ForgotPassword_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<ForgotPasswordRequest>();
        _mockAuthService.Setup(x => x.ForgotPasswordAsync(request)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.ForgotPassword(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<ResetPasswordRequest>();
        _mockAuthService.Setup(x => x.ResetPasswordAsync(request)).ReturnsAsync(true);

        // Act
        var result = await _controller.ResetPassword(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var request = _fixture.Create<ResetPasswordRequest>();
        _controller.ModelState.AddModelError("Token", "Token is required");

        // Act
        var result = await _controller.ResetPassword(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnBadRequest_WhenResetFails()
    {
        // Arrange
        var request = _fixture.Create<ResetPasswordRequest>();
        _mockAuthService.Setup(x => x.ResetPasswordAsync(request)).ReturnsAsync(false);

        // Act
        var result = await _controller.ResetPassword(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<ResetPasswordRequest>();
        _mockAuthService.Setup(x => x.ResetPasswordAsync(request)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.ResetPassword(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ValidateToken_ShouldReturnOk_WhenValidToken()
    {
        // Arrange
        var token = "valid-token";
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.ValidateTokenAsync(token)).ReturnsAsync(true);

        // Act
        var result = await _controller.ValidateToken();

        // Assert
        result.Should().BeOfType<ActionResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateToken_ShouldReturnBadRequest_WhenTokenNotFound()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "";

        // Act
        var result = await _controller.ValidateToken();

        // Assert
        result.Should().BeOfType<ActionResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateToken_ShouldReturnUnauthorized_WhenTokenInvalid()
    {
        // Arrange
        var token = "invalid-token";
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.ValidateTokenAsync(token)).ReturnsAsync(false);

        // Act
        var result = await _controller.ValidateToken();

        // Assert
        result.Should().BeOfType<ActionResult>();
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateToken_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var token = "valid-token";
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.ValidateTokenAsync(token)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.ValidateToken();

        // Assert
        result.Should().BeOfType<ActionResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnOk_WhenValidToken()
    {
        // Arrange
        var token = CreateValidJwtToken(1);
        var permissions = _fixture.CreateMany<string>(3).ToList();
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.GetUserPermissionsAsync(1)).ReturnsAsync(permissions);

        // Act
        var result = await _controller.GetPermissions();

        // Assert
        result.Should().BeOfType<ActionResult<List<string>>>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(permissions);
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnUnauthorized_WhenInvalidToken()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "";

        // Act
        var result = await _controller.GetPermissions();

        // Assert
        result.Should().BeOfType<ActionResult<List<string>>>();
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var token = CreateValidJwtToken(1);
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.GetUserPermissionsAsync(1)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetPermissions();

        // Assert
        result.Should().BeOfType<ActionResult<List<string>>>();
        var statusResult = result.Result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CheckPermission_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        var request = _fixture.Create<CheckPermissionRequest>();
        var token = CreateValidJwtToken(1);
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.HasPermissionAsync(1, request.Permission)).ReturnsAsync(true);

        // Act
        var result = await _controller.CheckPermission(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckPermission_ShouldReturnUnauthorized_WhenInvalidToken()
    {
        // Arrange
        var request = _fixture.Create<CheckPermissionRequest>();
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "";

        // Act
        var result = await _controller.CheckPermission(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckPermission_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = _fixture.Create<CheckPermissionRequest>();
        var token = CreateValidJwtToken(1);
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
        _mockAuthService.Setup(x => x.HasPermissionAsync(1, request.Permission)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CheckPermission(request);

        // Assert
        result.Should().BeOfType<ActionResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public void GetTokenFromHeader_ShouldReturnToken_WhenBearerTokenExists()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer test-token";

        // Act
        var result = _controller.GetType().GetMethod("GetTokenFromHeader", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(_controller, null) as string;

        // Assert
        result.Should().Be("test-token");
    }

    [Fact]
    public void GetTokenFromHeader_ShouldReturnEmpty_WhenNoAuthorizationHeader()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "";

        // Act
        var result = _controller.GetType().GetMethod("GetTokenFromHeader", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(_controller, null) as string;

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetTokenFromHeader_ShouldReturnEmpty_WhenInvalidFormat()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "InvalidFormat";

        // Act
        var result = _controller.GetType().GetMethod("GetTokenFromHeader", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(_controller, null) as string;

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetUserIdFromToken_ShouldReturnUserId_WhenValidToken()
    {
        // Arrange
        var token = CreateValidJwtToken(123);
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

        // Act
        var result = _controller.GetType().GetMethod("GetUserIdFromToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(_controller, null) as int?;

        // Assert
        result.Should().Be(123);
    }

    [Fact]
    public void GetUserIdFromToken_ShouldReturnZero_WhenNoToken()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "";

        // Act
        var result = _controller.GetType().GetMethod("GetUserIdFromToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(_controller, null) as int?;

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void GetUserIdFromToken_ShouldReturnZero_WhenInvalidToken()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer invalid-token";

        // Act
        var result = _controller.GetType().GetMethod("GetUserIdFromToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .Invoke(_controller, null) as int?;

        // Assert
        result.Should().Be(0);
    }

    private string CreateValidJwtToken(int userId)
    {
        var token = new JwtSecurityToken(
            issuer: "test",
            audience: "test",
            claims: new[] { new Claim("user_id", userId.ToString()) },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: null
        );
        
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(token);
    }
}
