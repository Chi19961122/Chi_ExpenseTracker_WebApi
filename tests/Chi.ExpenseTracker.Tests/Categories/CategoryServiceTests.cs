using Chi.ExpenseTracker.Common.Models.InfoModels;
using Chi.ExpenseTracker.Repositories.Categories;
using Chi.ExpenseTracker.Repositories.Entities;
using Chi.ExpenseTracker.Services.Categories;
using Chi.ExpenseTracker.Tests.TestHelpers;
using FluentAssertions;
using NSubstitute;

namespace Chi.ExpenseTracker.Tests.Categories;

public class CategoryServiceTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();

    private CategoryService CreateService() => new(_categoryRepository, MapperFactory.Create());

    [Fact]
    public async Task GetCategoriesAsync_MapsEntitiesToDtos()
    {
        _categoryRepository.GetByUserAsync(1, "Expense", Arg.Any<CancellationToken>())
            .Returns([
                new CategoryEntity { CategoryId = 10, UserId = 1, Title = "餐飲", CategoryType = "Expense", Icon = "🍜" },
            ]);
        var service = CreateService();

        var result = await service.GetCategoriesAsync(1, "Expense");

        result.Should().HaveCount(1);
        result[0].CategoryId.Should().Be(10);
        result[0].Title.Should().Be("餐飲");
        result[0].Icon.Should().Be("🍜");
    }

    [Fact]
    public async Task UpdateCategoryAsync_ReturnsNull_WhenCategoryNotOwnedByUser()
    {
        _categoryRepository.GetByIdAsync(1, 99, Arg.Any<CancellationToken>())
            .Returns((CategoryEntity?)null);
        var service = CreateService();

        var result = await service.UpdateCategoryAsync(new SaveCategoryInfo
        {
            UserId = 1,
            CategoryId = 99,
            Title = "改名",
        });

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateCategoryAsync_UpdatesFields_WhenCategoryExists()
    {
        var entity = new CategoryEntity { CategoryId = 10, UserId = 1, Title = "舊名", CategoryType = "Expense" };
        _categoryRepository.GetByIdAsync(1, 10, Arg.Any<CancellationToken>()).Returns(entity);
        var service = CreateService();

        var result = await service.UpdateCategoryAsync(new SaveCategoryInfo
        {
            UserId = 1,
            CategoryId = 10,
            Title = "新名",
            CategoryType = "Income",
            Icon = "💰",
        });

        result.Should().NotBeNull();
        result!.Title.Should().Be("新名");
        entity.CategoryType.Should().Be("Income");
        await _categoryRepository.Received(1).UpdateAsync(entity, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCategoryAsync_ReturnsFalse_WhenCategoryNotFound()
    {
        _categoryRepository.GetByIdAsync(1, 99, Arg.Any<CancellationToken>())
            .Returns((CategoryEntity?)null);
        var service = CreateService();

        var result = await service.DeleteCategoryAsync(1, 99);

        result.Should().BeFalse();
        await _categoryRepository.DidNotReceive().DeleteAsync(Arg.Any<CategoryEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCategoryAsync_Deletes_WhenCategoryExists()
    {
        var entity = new CategoryEntity { CategoryId = 10, UserId = 1, Title = "餐飲" };
        _categoryRepository.GetByIdAsync(1, 10, Arg.Any<CancellationToken>()).Returns(entity);
        var service = CreateService();

        var result = await service.DeleteCategoryAsync(1, 10);

        result.Should().BeTrue();
        await _categoryRepository.Received(1).DeleteAsync(entity, Arg.Any<CancellationToken>());
    }
}
