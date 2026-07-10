namespace Chi.ExpenseTracker.Common.Models.Dtos;

/// <summary>
/// 收支類別 DTO。
/// </summary>
public class CategoryDto
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
