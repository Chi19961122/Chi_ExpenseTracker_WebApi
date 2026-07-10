using Chi.ExpenseTracker.Repositories.Data;
using Chi.ExpenseTracker.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chi.ExpenseTracker.Repositories.Categories;

/// <summary>
/// 收支類別資料存取實作（EF Core）。
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly ExpenseDbContext _dbContext;

    /// <summary>
    /// 建立類別 Repository。
    /// </summary>
    public CategoryRepository(ExpenseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CategoryEntity>> GetByUserAsync(int userId, string? categoryType = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Categories
            .AsNoTracking()
            .Where(category => category.UserId == userId);

        if (string.IsNullOrEmpty(categoryType) is false)
        {
            query = query.Where(category => category.CategoryType == categoryType);
        }

        return await query
            .OrderBy(category => category.CategoryId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<CategoryEntity?> GetByIdAsync(int userId, int categoryId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories
            .SingleOrDefaultAsync(category => category.UserId == userId && category.CategoryId == categoryId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CategoryEntity> AddAsync(CategoryEntity category, CancellationToken cancellationToken = default)
    {
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return category;
    }

    /// <inheritdoc />
    public Task UpdateAsync(CategoryEntity category, CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteAsync(CategoryEntity category, CancellationToken cancellationToken = default)
    {
        _dbContext.Categories.Remove(category);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
