namespace Chi.ExpenseTracker.Common.Models.Dtos;

/// <summary>
/// 使用者資訊 DTO（不含密碼等敏感欄位）。
/// </summary>
public class UserDto
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
