using AutoMapper;
using Chi.ExpenseTracker.Api.Models.Parameters;
using Chi.ExpenseTracker.Api.Models.ViewModels;
using Chi.ExpenseTracker.Common.Models.Dtos;
using Chi.ExpenseTracker.Common.Models.InfoModels;

namespace Chi.ExpenseTracker.Api.Mapping;

/// <summary>
/// API 層對應設定：Parameter → Info、DTO → ViewModel。
/// </summary>
public class ApiMappingProfile : Profile
{
    /// <summary>
    /// 建立對應設定。
    /// </summary>
    public ApiMappingProfile()
    {
        // Parameter → Info（UserId／路由 ID 由 Controller 自行填入）
        CreateMap<RegisterParameter, RegisterInfo>();
        CreateMap<SaveCategoryParameter, SaveCategoryInfo>();
        CreateMap<SaveTransactionParameter, SaveTransactionInfo>();

        // DTO → ViewModel
        CreateMap<UserDto, UserViewModel>();
        CreateMap<AuthResultDto, AuthViewModel>();
        CreateMap<CategoryDto, CategoryViewModel>();
        CreateMap<TransactionDto, TransactionViewModel>();
    }
}
