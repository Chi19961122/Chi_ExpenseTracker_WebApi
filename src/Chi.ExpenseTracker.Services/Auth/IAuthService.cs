using Chi.ExpenseTracker.Common.Models.Dtos;
using Chi.ExpenseTracker.Common.Models.InfoModels;

namespace Chi.ExpenseTracker.Services.Auth;

/// <summary>
/// 認證 Service 介面（註冊、登入、換發權杖）。
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 註冊新使用者；電子郵件已存在時回傳 <c>null</c>。
    /// </summary>
    Task<UserDto?> RegisterAsync(RegisterInfo info, CancellationToken cancellationToken = default);

    /// <summary>
    /// 驗證帳號密碼並發行 JWT；憑證錯誤時回傳 <c>null</c>。
    /// </summary>
    Task<AuthResultDto?> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 為既有登入的使用者重新發行 JWT（延長工作階段）；使用者不存在時回傳 <c>null</c>。
    /// </summary>
    Task<AuthResultDto?> RefreshAsync(string email, CancellationToken cancellationToken = default);
}
