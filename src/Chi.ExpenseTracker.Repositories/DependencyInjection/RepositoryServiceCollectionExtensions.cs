using Chi.ExpenseTracker.Repositories.Categories;
using Chi.ExpenseTracker.Repositories.Data;
using Chi.ExpenseTracker.Repositories.Transactions;
using Chi.ExpenseTracker.Repositories.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Chi.ExpenseTracker.Repositories.DependencyInjection;

/// <summary>
/// 資料存取層的 DI 註冊。
/// </summary>
public static class RepositoryServiceCollectionExtensions
{
    /// <summary>
    /// 註冊 DbContext 與所有 Repository。
    /// </summary>
    public static IServiceCollection AddDatabaseInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ExpenseDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }
}
