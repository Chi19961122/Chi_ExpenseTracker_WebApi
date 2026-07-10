using AutoMapper;
using Chi.ExpenseTracker.Common.Models.Dtos;
using Chi.ExpenseTracker.Common.Models.InfoModels;
using Chi.ExpenseTracker.Repositories.Categories;
using Chi.ExpenseTracker.Repositories.Entities;

namespace Chi.ExpenseTracker.Services.Categories;

/// <summary>
/// 收支類別 Service 實作。
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// 建立類別 Service。
    /// </summary>
    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(int userId, string? categoryType = null, CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetByUserAsync(userId, categoryType, cancellationToken);
        return _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
    }

    /// <inheritdoc />
    public async Task<CategoryDto> CreateCategoryAsync(SaveCategoryInfo info, CancellationToken cancellationToken = default)
    {
        var category = new CategoryEntity
        {
            UserId = info.UserId,
            Title = info.Title,
            CategoryType = info.CategoryType,
            Icon = info.Icon,
        };
        await _categoryRepository.AddAsync(category, cancellationToken);

        return _mapper.Map<CategoryDto>(category);
    }

    /// <inheritdoc />
    public async Task<CategoryDto?> UpdateCategoryAsync(SaveCategoryInfo info, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(info.UserId, info.CategoryId, cancellationToken);
        if (category is null)
        {
            return null;
        }

        category.Title = info.Title;
        category.CategoryType = info.CategoryType;
        category.Icon = info.Icon;
        await _categoryRepository.UpdateAsync(category, cancellationToken);

        return _mapper.Map<CategoryDto>(category);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteCategoryAsync(int userId, int categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(userId, categoryId, cancellationToken);
        if (category is null)
        {
            return false;
        }

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        return true;
    }
}
