using Chi.ExpenseTracker.Repositories.Data;
using Chi.ExpenseTracker.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chi.ExpenseTracker.Repositories.Transactions;

/// <summary>
/// 收支紀錄資料存取實作（EF Core）。
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly ExpenseDbContext _dbContext;

    /// <summary>
    /// 建立收支紀錄 Repository。
    /// </summary>
    public TransactionRepository(ExpenseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TransactionEntity>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Transactions
            .AsNoTracking()
            .Include(transaction => transaction.Category)
            .Where(transaction => transaction.UserId == userId)
            .OrderByDescending(transaction => transaction.CreateDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TransactionEntity?> GetByIdAsync(int userId, int transactionId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Transactions
            .Include(transaction => transaction.Category)
            .SingleOrDefaultAsync(transaction => transaction.UserId == userId && transaction.TransactionId == transactionId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TransactionEntity> AddAsync(TransactionEntity transaction, CancellationToken cancellationToken = default)
    {
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 載入所屬類別，讓呼叫端能直接映射出含類別資訊的 DTO。
        await _dbContext.Entry(transaction).Reference(t => t.Category).LoadAsync(cancellationToken);
        return transaction;
    }

    /// <inheritdoc />
    public Task UpdateAsync(TransactionEntity transaction, CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteAsync(TransactionEntity transaction, CancellationToken cancellationToken = default)
    {
        _dbContext.Transactions.Remove(transaction);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
