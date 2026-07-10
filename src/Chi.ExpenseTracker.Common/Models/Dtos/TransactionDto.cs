namespace Chi.ExpenseTracker.Common.Models.Dtos;

/// <summary>
/// 收支紀錄 DTO（含所屬類別資訊）。
/// </summary>
public class TransactionDto
{
    /// <summary>
    /// 收支紀錄 ID。
    /// </summary>
    public int TransactionId { get; set; }

    /// <summary>
    /// 類別 ID。
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 類別名稱。
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 類別型態（Expense／Income）。
    /// </summary>
    public string? CategoryType { get; set; }

    /// <summary>
    /// 金額。
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 交易日期。
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// 備註。
    /// </summary>
    public string? Description { get; set; }
}
