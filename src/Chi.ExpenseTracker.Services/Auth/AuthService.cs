using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Chi.ExpenseTracker.Common.Models.Dtos;
using Chi.ExpenseTracker.Common.Models.InfoModels;
using Chi.ExpenseTracker.Common.Options;
using Chi.ExpenseTracker.Repositories.Entities;
using Chi.ExpenseTracker.Repositories.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Chi.ExpenseTracker.Services.Auth;

/// <summary>
/// 認證 Service 實作。以 BCrypt 驗證密碼，成功後發行 JWT。
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtOptions _jwtOptions;

    /// <summary>
    /// 建立認證 Service。
    /// </summary>
    public AuthService(IUserRepository userRepository, IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _jwtOptions = jwtOptions.Value;
    }

    /// <inheritdoc />
    public async Task<UserDto?> RegisterAsync(RegisterInfo info, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(info.Email);

        var existing = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (existing is not null)
        {
            return null;
        }

        var user = new UserEntity
        {
            UserName = info.UserName,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(info.Password),
            Role = "User",
        };
        await _userRepository.AddAsync(user, cancellationToken);

        return ToUserDto(user);
    }

    /// <inheritdoc />
    public async Task<AuthResultDto?> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(NormalizeEmail(email), cancellationToken);
        if (user is null || BCrypt.Net.BCrypt.Verify(password, user.Password) is false)
        {
            return null;
        }

        return BuildResult(user);
    }

    /// <inheritdoc />
    public async Task<AuthResultDto?> RefreshAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(NormalizeEmail(email), cancellationToken);
        return user is null ? null : BuildResult(user);
    }

    /// <summary>
    /// 統一電子郵件格式（去空白、轉小寫），避免大小寫造成帳號比對不一致。
    /// </summary>
    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static UserDto ToUserDto(UserEntity user) => new()
    {
        UserId = user.UserId,
        UserName = user.UserName,
        Email = user.Email,
        Role = user.Role,
    };

    /// <summary>
    /// 由使用者資料組出登入結果（含新簽發的 JWT）。
    /// </summary>
    private AuthResultDto BuildResult(UserEntity user) => new()
    {
        Token = CreateToken(user),
        User = ToUserDto(user),
    };

    /// <summary>
    /// 依使用者資訊建立簽章後的 JWT。
    /// </summary>
    private string CreateToken(UserEntity user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.UserName),
            new Claim("role", user.Role ?? "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
