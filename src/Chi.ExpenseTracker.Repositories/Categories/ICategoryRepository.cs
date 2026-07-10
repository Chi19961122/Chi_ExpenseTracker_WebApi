using Chi.ExpenseTracker.Repositories.Entities;

namespace Chi.ExpenseTracker.Repositories.Categories;

/// <summary>
/// 收支類別資料存取介面。所有查詢一律以 UserId 隔離資料。
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// 取得使用者的類別清單，可依類別型態篩選。
    /// </summary>
    Task<IReadOnlyList<CategoryEntity>> GetByUserAsync(int userId, string? categoryType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取得使用者的單一類別；不存在或不屬於該使用者時回傳 <c>null</c>。
    /// </summary>
    Task<CategoryEntity?> GetByIdAsync(int userId, int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增類別。
    /// </summary>
    Task<CategoryEntity> AddAsync(CategoryEntity category, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新既有（追蹤中）的類別實體。
    /// </summary>
    Task UpdateAsync(CategoryEntity category, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除既有（追蹤中）的類別實體。
    /// </summary>
    Task DeleteAsync(CategoryEntity category, CancellationToken cancellationToken = default);
}
