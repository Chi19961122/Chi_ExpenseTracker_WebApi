using AutoMapper;
using Chi.ExpenseTracker.Common.Models.Dtos;
using Chi.ExpenseTracker.Common.Models.InfoModels;
using Chi.ExpenseTracker.Repositories.Categories;
using Chi.ExpenseTracker.Repositories.Entities;
using Chi.ExpenseTracker.Repositories.Transactions;

namespace Chi.ExpenseTracker.Services.Transactions;

/// <summary>
/// 收支紀錄 Service 實作。
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// 建立收支紀錄 Service。
    /// </summary>
    public TransactionService(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TransactionDto>> GetTransactionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var transactions = await _transactionRepository.GetByUserAsync(userId, cancellationToken);
        return _mapper.Map<IReadOnlyList<TransactionDto>>(transactions);
    }

    /// <inheritdoc />
    public async Task<TransactionDto?> GetTransactionAsync(int userId, int transactionId, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(userId, transactionId, cancellationToken);
        return transaction is null ? null : _mapper.Map<TransactionDto>(transaction);
    }

    /// <inheritdoc />
    public async Task<TransactionDto?> CreateTransactionAsync(SaveTransactionInfo info, CancellationToken cancellationToken = default)
    {
        // 確認類別存在且屬於該使用者，避免把紀錄掛到別人的類別上。
        var category = await _categoryRepository.GetByIdAsync(info.UserId, info.CategoryId, cancellationToken);
        if (category is null)
        {
            return null;
        }

        var transaction = new TransactionEntity
        {
            UserId = info.UserId,
            CategoryId = info.CategoryId,
            Amount = info.Amount,
            CreateDate = info.CreateDate,
            Description = info.Description,
        };
        await _transactionRepository.AddAsync(transaction, cancellationToken);

        return _mapper.Map<TransactionDto>(transaction);
    }

    /// <inheritdoc />
    public async Task<TransactionDto?> UpdateTransactionAsync(SaveTransactionInfo info, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(info.UserId, info.TransactionId, cancellationToken);
        if (transaction is null)
        {
            return null;
        }

        if (transaction.CategoryId != info.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(info.UserId, info.CategoryId, cancellationToken);
            if (category is null)
            {
                return null;
            }
            transaction.Category = category;
            transaction.CategoryId = info.CategoryId;
        }

        transaction.Amount = info.Amount;
        transaction.CreateDate = info.CreateDate;
        transaction.Description = info.Description;
        await _transactionRepository.UpdateAsync(transaction, cancellationToken);

        return _mapper.Map<TransactionDto>(transaction);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteTransactionAsync(int userId, int transactionId, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(userId, transactionId, cancellationToken);
        if (transaction is null)
        {
            return false;
        }

        await _transactionRepository.DeleteAsync(transaction, cancellationToken);
        return true;
    }
}
