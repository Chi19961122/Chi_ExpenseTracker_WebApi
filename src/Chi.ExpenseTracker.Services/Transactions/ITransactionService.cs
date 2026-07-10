using Chi.ExpenseTracker.Common.Models.Dtos;
using Chi.ExpenseTracker.Common.Models.InfoModels;

namespace Chi.ExpenseTracker.Services.Transactions;

/// <summary>
/// 收支紀錄 Service 介面。
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// 取得使用者的全部收支紀錄（含類別資訊），依交易日期由新到舊排序。
    /// </summary>
    Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取得單筆收支紀錄；不存在或不屬於該使用者時回傳 <c>null</c>。
    /// </summary>
    Task<TransactionDto?> GetTransactionAsync(int userId, int transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增收支紀錄；類別不存在或不屬於該使用者時回傳 <c>null</c>。
    /// </summary>
    Task<TransactionDto?> CreateTransactionAsync(SaveTransactionInfo info, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新收支紀錄；紀錄或類別不存在、不屬於該使用者時回傳 <c>null</c>。
    /// </summary>
    Task<TransactionDto?> UpdateTransactionAsync(SaveTransactionInfo info, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除收支紀錄；成功回傳 <c>true</c>，不存在或不屬於該使用者時回傳 <c>false</c>。
    /// </summary>
    Task<bool> DeleteTransactionAsync(int userId, int transactionId, CancellationToken cancellationToken = default);
}
