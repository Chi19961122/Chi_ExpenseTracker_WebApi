namespace Chi.ExpenseTracker.Api.Models.Parameters;

/// <summary>
/// 登入參數。
/// </summary>
public class LoginParameter
{
    /// <summary>
    /// 電子郵件（登入帳號）。
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 密碼。
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
