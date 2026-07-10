namespace Chi.ExpenseTracker.Api.Models.Parameters;

/// <summary>
/// 註冊參數。
/// </summary>
public class RegisterParameter
{
    /// <summary>
    /// 顯示名稱。
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件（登入帳號）。
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 密碼。
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
