namespace Chi.ExpenseTracker.Api.Models.ViewModels;

/// <summary>
/// 使用者資訊 ViewModel。
/// </summary>
public class UserViewModel
{
    /// <summary>
    /// 使用者 ID。
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 顯示名稱。
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件（登入帳號）。
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 角色。
    /// </summary>
    public string? Role { get; set; }
}
