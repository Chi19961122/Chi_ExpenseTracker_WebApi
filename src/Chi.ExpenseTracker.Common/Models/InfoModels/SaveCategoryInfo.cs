namespace Chi.ExpenseTracker.Common.Models.InfoModels;

/// <summary>
/// 新增／更新收支類別資訊。UserId 由 API 層自 JWT 填入，不接受外部傳入。
/// </summary>
public class SaveCategoryInfo
{
    /// <summary>
    /// 使用者 ID（來自 JWT）。
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 類別 ID（更新時使用）。
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 類別名稱。
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 類別型態（Expense／Income）。
    /// </summary>
    public string? CategoryType { get; set; }

    /// <summary>
    /// 圖示。
    /// </summary>
    public string? Icon { get; set; }
}
