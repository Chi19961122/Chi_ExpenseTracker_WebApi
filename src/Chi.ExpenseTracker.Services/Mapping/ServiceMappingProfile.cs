using AutoMapper;
using Chi.ExpenseTracker.Common.Models.Dtos;
using Chi.ExpenseTracker.Repositories.Entities;

namespace Chi.ExpenseTracker.Services.Mapping;

/// <summary>
/// Service 層對應設定：Entity → DTO。
/// </summary>
public class ServiceMappingProfile : Profile
{
    /// <summary>
    /// 建立對應設定。
    /// </summary>
    public ServiceMappingProfile()
    {
        CreateMap<CategoryEntity, CategoryDto>();

        CreateMap<TransactionEntity, TransactionDto>()
            .ForMember(dto => dto.CategoryName, options => options.MapFrom(entity => entity.Category.Title))
            .ForMember(dto => dto.CategoryType, options => options.MapFrom(entity => entity.Category.CategoryType));

        CreateMap<UserEntity, UserDto>();
    }
}
