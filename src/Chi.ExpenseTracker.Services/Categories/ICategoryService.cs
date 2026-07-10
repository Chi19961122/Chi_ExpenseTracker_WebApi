using Chi.ExpenseTracker.Common.Models.Dtos;
using Chi.ExpenseTracker.Common.Models.InfoModels;

namespace Chi.ExpenseTracker.Services.Categories;

/// <summary>
/// 收支類別 Service 介面。
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// 取得使用者的類別清單，可依類別型態篩選。
    /// </summary>
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(int userId, string? categoryType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增類別。
    /// </summary>
    Task<CategoryDto> CreateCategoryAsync(SaveCategoryInfo info, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新類別；不存在或不屬於該使用者時回傳 <c>null</c>。
    /// </summary>
    Task<CategoryDto?> UpdateCategoryAsync(SaveCategoryInfo info, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除類別；成功回傳 <c>true</c>，不存在或不屬於該使用者時回傳 <c>false</c>。
    /// </summary>
    Task<bool> DeleteCategoryAsync(int userId, int categoryId, CancellationToken cancellationToken = default);
}
