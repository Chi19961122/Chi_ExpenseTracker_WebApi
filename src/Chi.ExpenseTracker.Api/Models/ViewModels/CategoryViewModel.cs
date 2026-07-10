namespace Chi.ExpenseTracker.Api.Models.ViewModels;

/// <summary>
/// 收支類別 ViewModel。
/// </summary>
public class CategoryViewModel
{
    /// <summary>
    /// 類別 ID。
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
