using Chi.ExpenseTracker.Repositories.Entities;

namespace Chi.ExpenseTracker.Repositories.Transactions;

/// <summary>
/// 收支紀錄資料存取介面。所有查詢一律以 UserId 隔離資料。
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// 取得使用者的收支紀錄清單（含所屬類別），依交易日期由新到舊排序。
    /// </summary>
    Task<IReadOnlyList<TransactionEntity>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取得使用者的單一收支紀錄（含所屬類別）；不存在或不屬於該使用者時回傳 <c>null</c>。
    /// </summary>
    Task<TransactionEntity?> GetByIdAsync(int userId, int transactionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增收支紀錄，並回傳含類別資訊的實體。
    /// </summary>
    Task<TransactionEntity> AddAsync(TransactionEntity transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新既有（追蹤中）的收支紀錄實體。
    /// </summary>
    Task UpdateAsync(TransactionEntity transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除既有（追蹤中）的收支紀錄實體。
    /// </summary>
    Task DeleteAsync(TransactionEntity transaction, CancellationToken cancellationToken = default);
}
