namespace Chi.ExpenseTracker.Common.Models.InfoModels;

/// <summary>
/// 註冊使用者資訊。
/// </summary>
public class RegisterInfo
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
    /// 密碼（明文，僅於註冊流程中存在，落地前以 BCrypt 雜湊）。
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
