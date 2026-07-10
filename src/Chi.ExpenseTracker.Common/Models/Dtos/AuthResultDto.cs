namespace Chi.ExpenseTracker.Common.Models.Dtos;

/// <summary>
/// 認證結果 DTO — 登入或換發權杖成功後回傳。
/// </summary>
public class AuthResultDto
{
    /// <summary>
    /// 簽發的 JWT。
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 使用者資訊。
    /// </summary>
    public UserDto User { get; set; } = new();
}
