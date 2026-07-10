namespace Chi.ExpenseTracker.Api.Models.Parameters;

/// <summary>
/// 新增／更新收支紀錄參數。
/// </summary>
public class SaveTransactionParameter
{
    /// <summary>
    /// 類別 ID。
    /// </summary>
    public int CategoryId { get; set; }

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
