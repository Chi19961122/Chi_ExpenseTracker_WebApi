using Chi.ExpenseTracker.Repositories.Entities;

namespace Chi.ExpenseTracker.Repositories.Users;

/// <summary>
/// 使用者資料存取介面。
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 以電子郵件取得使用者；不存在時回傳 <c>null</c>。
    /// </summary>
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增使用者。
    /// </summary>
    Task<UserEntity> AddAsync(UserEntity user, CancellationToken cancellationToken = default);
}
