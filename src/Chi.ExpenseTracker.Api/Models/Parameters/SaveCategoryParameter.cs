namespace Chi.ExpenseTracker.Api.Models.Parameters;

/// <summary>
/// 新增／更新收支類別參數。
/// </summary>
public class SaveCategoryParameter
{
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
