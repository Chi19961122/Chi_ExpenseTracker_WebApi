using Chi.ExpenseTracker.Common.Models.InfoModels;
using Chi.ExpenseTracker.Repositories.Categories;
using Chi.ExpenseTracker.Repositories.Entities;
using Chi.ExpenseTracker.Repositories.Transactions;
using Chi.ExpenseTracker.Services.Transactions;
using Chi.ExpenseTracker.Tests.TestHelpers;
using FluentAssertions;
using NSubstitute;

namespace Chi.ExpenseTracker.Tests.Transactions;

public class TransactionServiceTests
{
    private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();

    private TransactionService CreateService() =>
        new(_transactionRepository, _categoryRepository, MapperFactory.Create());

    private static CategoryEntity CreateCategory() => new()
    {
        CategoryId = 10,
        UserId = 1,
        Title = "餐飲",
        CategoryType = "Expense",
    };

    [Fact]
    public async Task GetTransactionsAsync_MapsCategoryInfoIntoDtos()
    {
        _transactionRepository.GetByUserAsync(1, Arg.Any<CancellationToken>())
            .Returns([
                new TransactionEntity
                {
                    TransactionId = 5,
                    UserId = 1,
                    CategoryId = 10,
                    Amount = 120m,
                    CreateDate = new DateTime(2026, 7, 1),
                    Description = "午餐",
                    Category = CreateCategory(),
                },
            ]);
        var service = CreateService();

        var result = await service.GetTransactionsAsync(1);

        result.Should().HaveCount(1);
        result[0].CategoryName.Should().Be("餐飲");
        result[0].CategoryType.Should().Be("Expense");
        result[0].Amount.Should().Be(120m);
    }

    [Fact]
    public async Task CreateTransactionAsync_ReturnsNull_WhenCategoryNotOwnedByUser()
    {
        _categoryRepository.GetByIdAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns((CategoryEntity?)null);
        var service = CreateService();

        var result = await service.CreateTransactionAsync(new SaveTransactionInfo
        {
            UserId = 1,
            CategoryId = 10,
            Amount = 100m,
            CreateDate = new DateTime(2026, 7, 1),
        });

        result.Should().BeNull();
        await _transactionRepository.DidNotReceive().AddAsync(Arg.Any<TransactionEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateTransactionAsync_CreatesTransaction_WhenCategoryValid()
    {
        var category = CreateCategory();
        _categoryRepository.GetByIdAsync(1, 10, Arg.Any<CancellationToken>()).Returns(category);
        _transactionRepository.AddAsync(Arg.Any<TransactionEntity>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var entity = callInfo.Arg<TransactionEntity>();
                entity.TransactionId = 5;
                entity.Category = category;
                return entity;
            });
        var service = CreateService();

        var result = await service.CreateTransactionAsync(new SaveTransactionInfo
        {
            UserId = 1,
            CategoryId = 10,
            Amount = 100m,
            CreateDate = new DateTime(2026, 7, 1),
            Description = "午餐",
        });

        result.Should().NotBeNull();
        result!.TransactionId.Should().Be(5);
        result.CategoryName.Should().Be("餐飲");
    }

    [Fact]
    public async Task UpdateTransactionAsync_ReturnsNull_WhenTransactionNotOwnedByUser()
    {
        _transactionRepository.GetByIdAsync(1, 99, Arg.Any<CancellationToken>())
            .Returns((TransactionEntity?)null);
        var service = CreateService();

        var result = await service.UpdateTransactionAsync(new SaveTransactionInfo
        {
            UserId = 1,
            TransactionId = 99,
            CategoryId = 10,
            Amount = 100m,
            CreateDate = new DateTime(2026, 7, 1),
        });

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateTransactionAsync_UpdatesFields_WhenTransactionExists()
    {
        var entity = new TransactionEntity
        {
            TransactionId = 5,
            UserId = 1,
            CategoryId = 10,
            Amount = 100m,
            CreateDate = new DateTime(2026, 7, 1),
            Category = CreateCategory(),
        };
        _transactionRepository.GetByIdAsync(1, 5, Arg.Any<CancellationToken>()).Returns(entity);
        var service = CreateService();

        var result = await service.UpdateTransactionAsync(new SaveTransactionInfo
        {
            UserId = 1,
            TransactionId = 5,
            CategoryId = 10,
            Amount = 250m,
            CreateDate = new DateTime(2026, 7, 2),
            Description = "晚餐",
        });

        result.Should().NotBeNull();
        result!.Amount.Should().Be(250m);
        result.CreateDate.Should().Be(new DateTime(2026, 7, 2));
        entity.Description.Should().Be("晚餐");
        await _transactionRepository.Received(1).UpdateAsync(entity, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateTransactionAsync_ReturnsNull_WhenNewCategoryNotOwnedByUser()
    {
        var entity = new TransactionEntity
        {
            TransactionId = 5,
            UserId = 1,
            CategoryId = 10,
            Amount = 100m,
            CreateDate = new DateTime(2026, 7, 1),
            Category = CreateCategory(),
        };
        _transactionRepository.GetByIdAsync(1, 5, Arg.Any<CancellationToken>()).Returns(entity);
        _categoryRepository.GetByIdAsync(1, 20, Arg.Any<CancellationToken>())
            .Returns((CategoryEntity?)null);
        var service = CreateService();

        var result = await service.UpdateTransactionAsync(new SaveTransactionInfo
        {
            UserId = 1,
            TransactionId = 5,
            CategoryId = 20,
            Amount = 100m,
            CreateDate = new DateTime(2026, 7, 1),
        });

        result.Should().BeNull();
        await _transactionRepository.DidNotReceive().UpdateAsync(Arg.Any<TransactionEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteTransactionAsync_ReturnsFalse_WhenTransactionNotFound()
    {
        _transactionRepository.GetByIdAsync(1, 99, Arg.Any<CancellationToken>())
            .Returns((TransactionEntity?)null);
        var service = CreateService();

        var result = await service.DeleteTransactionAsync(1, 99);

        result.Should().BeFalse();
    }
}
