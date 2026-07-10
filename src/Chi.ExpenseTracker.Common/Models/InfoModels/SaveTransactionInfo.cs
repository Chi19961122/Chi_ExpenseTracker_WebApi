namespace Chi.ExpenseTracker.Common.Models.InfoModels;

/// <summary>
/// 新增／更新收支紀錄資訊。UserId 由 API 層自 JWT 填入，不接受外部傳入。
/// </summary>
public class SaveTransactionInfo
{
    /// <summary>
    /// 使用者 ID（來自 JWT）。
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 收支紀錄 ID（更新時使用）。
    /// </summary>
    public int TransactionId { get; set; }

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
