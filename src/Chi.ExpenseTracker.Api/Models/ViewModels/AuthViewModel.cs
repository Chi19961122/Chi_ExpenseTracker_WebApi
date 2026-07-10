namespace Chi.ExpenseTracker.Api.Models.ViewModels;

/// <summary>
/// 認證結果 ViewModel（登入／換發權杖）。
/// </summary>
public class AuthViewModel
{
    /// <summary>
    /// 簽發的 JWT。
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 使用者資訊。
    /// </summary>
    public UserViewModel User { get; set; } = new();
}
