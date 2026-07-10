namespace Chi.ExpenseTracker.Common.Options;

/// <summary>
/// JWT 設定物件（Options）— 綁定 appsettings 的 <c>Jwt</c> 區段。
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// 設定區段名稱。
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// 簽章金鑰（HS256，至少 32 bytes）。
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 發行者。
    /// </summary>
    public string Issuer { get; set; } = "Chi.ExpenseTracker";

    /// <summary>
    /// 目標對象。
    /// </summary>
    public string Audience { get; set; } = "Chi.ExpenseTracker";

    /// <summary>
    /// 權杖有效分鐘數。
    /// </summary>
    public int ExpiryMinutes { get; set; } = 60;
}
