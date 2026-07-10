using Chi.ExpenseTracker.Services.Auth;
using Chi.ExpenseTracker.Services.Categories;
using Chi.ExpenseTracker.Services.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace Chi.ExpenseTracker.Services.DependencyInjection;

/// <summary>
/// 業務邏輯層的 DI 註冊。
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 註冊所有 Service。
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITransactionService, TransactionService>();
        return services;
    }
}
