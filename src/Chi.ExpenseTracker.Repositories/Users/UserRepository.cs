using Chi.ExpenseTracker.Repositories.Data;
using Chi.ExpenseTracker.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chi.ExpenseTracker.Repositories.Users;

/// <summary>
/// 使用者資料存取實作（EF Core）。
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ExpenseDbContext _dbContext;

    /// <summary>
    /// 建立使用者 Repository。
    /// </summary>
    public UserRepository(ExpenseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserEntity> AddAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }
}
