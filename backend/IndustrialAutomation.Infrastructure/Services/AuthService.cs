using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Core.Models;
using IndustrialAutomation.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace IndustrialAutomation.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IndustrialAutomationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;

    public AuthService(
        IndustrialAutomationDbContext context,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _jwtSecret = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
        _jwtIssuer = _configuration["Jwt:Issuer"] ?? "IndustrialAutomation";
        _jwtAudience = _configuration["Jwt:Audience"] ?? "IndustrialAutomation";
        _jwtExpirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
        _refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive && !u.IsDeleted);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash, user.Salt))
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
            {
                _logger.LogWarning("Account locked for user: {UserId}", user.Id);
                throw new UnauthorizedAccessException("Account is locked");
            }

            // Reset failed login attempts on successful login
            user.FailedLoginAttempts = 0;
            user.LockedUntil = null;
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // Store refresh token
            await StoreRefreshTokenAsync(user.Id, refreshToken);

            var permissions = await GetUserPermissionsAsync(user.Id);

            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Permissions = permissions,
                    LastLoginAt = user.LastLoginAt ?? DateTime.UtcNow,
                    IsActive = user.IsActive
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username))
            {
                throw new InvalidOperationException("User with this email or username already exists");
            }

            var salt = GenerateSalt();
            var passwordHash = HashPassword(request.Password, salt);

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Salt = salt,
                Role = request.Role,
                IsActive = true,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow,
                PasswordChangedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token for new user
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            await StoreRefreshTokenAsync(user.Id, refreshToken);

            var permissions = await GetUserPermissionsAsync(user.Id);

            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Permissions = permissions,
                    LastLoginAt = DateTime.UtcNow,
                    IsActive = user.IsActive
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var refreshToken = await _context.UserRefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && 
                                         rt.ExpiresAt > DateTime.UtcNow && 
                                         !rt.IsRevoked);

            if (refreshToken == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            // Revoke old refresh token
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;

            // Generate new tokens
            var newToken = GenerateJwtToken(refreshToken.User);
            var newRefreshToken = GenerateRefreshToken();
            await StoreRefreshTokenAsync(refreshToken.User.Id, newRefreshToken);

            await _context.SaveChangesAsync();

            var permissions = await GetUserPermissionsAsync(refreshToken.User.Id);

            return new AuthResponse
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                User = new UserInfo
                {
                    Id = refreshToken.User.Id,
                    Username = refreshToken.User.Username,
                    Email = refreshToken.User.Email,
                    Role = refreshToken.User.Role,
                    Permissions = permissions,
                    LastLoginAt = refreshToken.User.LastLoginAt ?? DateTime.UtcNow,
                    IsActive = refreshToken.User.IsActive
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            throw;
        }
    }

    public async Task<bool> LogoutAsync(string token)
    {
        try
        {
            var jti = GetJtiFromToken(token);
            if (string.IsNullOrEmpty(jti))
                return false;

            // Add token to blacklist
            var blacklistedToken = new BlacklistedToken
            {
                Jti = jti,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                CreatedAt = DateTime.UtcNow
            };

            _context.BlacklistedTokens.Add(blacklistedToken);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            if (!VerifyPassword(request.CurrentPassword, user.PasswordHash, user.Salt))
                return false;

            var salt = GenerateSalt();
            user.PasswordHash = HashPassword(request.NewPassword, salt);
            user.Salt = salt;
            user.PasswordChangedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return true; // Don't reveal if user exists

            var resetToken = GenerateResetToken();
            var resetTokenEntity = new PasswordResetToken
            {
                UserId = user.Id,
                Token = resetToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow
            };

            _context.PasswordResetTokens.Add(resetTokenEntity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password reset token generated for user: {UserId}", user.Id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating password reset token for email: {Email}", request.Email);
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var resetToken = await _context.PasswordResetTokens
                .Include(prt => prt.User)
                .FirstOrDefaultAsync(prt => prt.Token == request.Token && 
                                          prt.User.Email == request.Email && 
                                          prt.ExpiresAt > DateTime.UtcNow && 
                                          !prt.IsUsed);

            if (resetToken == null)
                return false;

            var salt = GenerateSalt();
            resetToken.User.PasswordHash = HashPassword(request.NewPassword, salt);
            resetToken.User.Salt = salt;
            resetToken.User.PasswordChangedAt = DateTime.UtcNow;
            resetToken.IsUsed = true;
            resetToken.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return false;
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var jti = GetJtiFromToken(token);
            if (string.IsNullOrEmpty(jti))
                return false;

            // Check if token is blacklisted
            var isBlacklisted = await _context.BlacklistedTokens
                .AnyAsync(bt => bt.Jti == jti && bt.ExpiresAt > DateTime.UtcNow);

            return !isBlacklisted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return false;
        }
    }

    public async Task<UserInfo> GetUserInfoAsync(string token)
    {
        try
        {
            var userId = GetUserIdFromToken(token);
            if (userId == 0)
                throw new UnauthorizedAccessException("Invalid token");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive && !u.IsDeleted);

            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            var permissions = await GetUserPermissionsAsync(user.Id);

            return new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Permissions = permissions,
                LastLoginAt = user.LastLoginAt ?? DateTime.UtcNow,
                IsActive = user.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user info");
            throw;
        }
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        return await LogoutAsync(token);
    }

    public async Task<List<string>> GetUserPermissionsAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return new List<string>();

            var permissions = new List<string>();

            // Add role-based permissions
            foreach (var userRole in user.UserRoles)
            {
                var role = await _context.UserRoles.FindAsync(userRole.RoleId);
                if (role?.Permissions != null)
                {
                    // Parse JSON permissions
                    var rolePermissions = System.Text.Json.JsonSerializer.Deserialize<List<string>>(role.Permissions) ?? new List<string>();
                    permissions.AddRange(rolePermissions);
                }
            }

            // Add direct user permissions
            var directPermissions = await _context.UserPermissions
                .Where(up => up.UserId == userId && up.IsGranted && !up.IsDeleted)
                .Select(up => $"{up.PermissionType}:{up.Resource}")
                .ToListAsync();

            permissions.AddRange(directPermissions);

            return permissions.Distinct().ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user permissions for user: {UserId}", userId);
            return new List<string>();
        }
    }

    public async Task<bool> HasPermissionAsync(int userId, string permission)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        return permissions.Contains(permission) || permissions.Contains("*");
    }

    public async Task<bool> IsInRoleAsync(int userId, string role)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return false;

        return user.Role == role || user.UserRoles.Any(ur => ur.UserRole.RoleName == role);
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jti = Guid.NewGuid().ToString();
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("user_id", user.Id.ToString()),
            new Claim("username", user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private string GenerateResetToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private string GenerateSalt()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private string HashPassword(string password, string salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 100000, HashAlgorithmName.SHA256);
        return Convert.ToBase64String(pbkdf2.GetBytes(32));
    }

    private bool VerifyPassword(string password, string hash, string salt)
    {
        var hashedPassword = HashPassword(password, salt);
        return hashedPassword == hash;
    }

    private async Task StoreRefreshTokenAsync(int userId, string refreshToken)
    {
        var tokenEntity = new UserRefreshToken
        {
            UserId = userId,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow
        };

        _context.UserRefreshTokens.Add(tokenEntity);
        await _context.SaveChangesAsync();
    }

    private string GetJtiFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            return jsonToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private int GetUserIdFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
        catch
        {
            return 0;
        }
    }
}
